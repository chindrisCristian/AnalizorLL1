using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.CSharp;

namespace MiniInterpreterCore
{
	public class SourceCodeGenerator
	{
		List<CodeMemberMethod> codeMethods = new List<CodeMemberMethod>();

		#region Public Properties
		/// <summary>
		/// Metoda pentru generarea fisierului sursa care contine
		/// analizorul LL1 pentru gramatica data ca aplicatie consola.
		/// </summary>
		/// <param name="fileName">Numele fisierului.</param>
		/// <param name="tabel">Tabelul de analiza sintactica.</param>
		public void GenereazaCodSursa(string fileName, TAS tabel, List<string> terminale)
		{
			//Ne folosim de libraria CodeDOM.
			//Cream un CodeCompileUnit care sa contina graful programului.
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			//Declaram un namespace pentru aplicatie (numele fisierului).
			CodeNamespace codeNamespace = new CodeNamespace(Path.GetFileNameWithoutExtension(Path.GetFileName(fileName)));
			//Adaugam namespace-ul create unitatii de compilare.
			codeCompileUnit.Namespaces.Add(codeNamespace);
			//Adaugam namespace-ul System.
			codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
			//Adaugam clasa Program, pe care o adaugam si tipurilor de date din namespace-ul creat.
			CodeTypeDeclaration classProgram = new CodeTypeDeclaration("Program");
			codeNamespace.Types.Add(classProgram);
	
			//Generam toate metodele de care avem nevoie.
			GenereazaMetodeIndividuale(tabel, terminale);
			//Le adaugam in clasa noastra.
			classProgram.IsClass = true;
			classProgram.Members.AddRange(codeMethods.ToArray());
			//Adaugam campul pentru input.
			classProgram.Members.Add(new CodeMemberField(new CodeTypeReference(typeof(List<string>)), "input") { Attributes = MemberAttributes.Public | MemberAttributes.Static });
			//Adaugam campul pentru pozitie.
			classProgram.Members.Add(new CodeMemberField(new CodeTypeReference(typeof(System.Int32)), "pozitie") { Attributes = MemberAttributes.Public | MemberAttributes.Static });

			//Declaram un nou punct de intrare in program.
			CodeMemberMethod mainMethod = GenereazaMetodaMain(tabel.Linii[0].Linie);
			//O adaugam la clasa.
			classProgram.Members.Add(mainMethod);

			//Generam fisierul .cs.
			CSharpCodeProvider provider = new CSharpCodeProvider();
			using(StreamWriter sw =new StreamWriter(fileName, false))
			{
				IndentedTextWriter tw = new IndentedTextWriter(sw, "\t");
				provider.GenerateCodeFromCompileUnit(codeCompileUnit, tw, new CodeGeneratorOptions());
				tw.Close();
			}

		}

		/// <summary>
		/// Metoda folosita pentru a compila codul sursa dintr-un anumit fisier.
		/// </summary>
		/// <param name="sourceFile">Fisierul care contine codul sursa.</param>
		/// <returns>Erorile ce pot aparea pe parcursul procesului de compilare sau string.Empty daca totul functioneaza corect.</returns>
		public string CompileazaCodSursa(string sourceFile)
		{
			CompilerParameters cp = new CompilerParameters
			{

				// Generate an executable instead of 
				// a class library.
				GenerateExecutable = true,

				// Generate debug information.
				IncludeDebugInformation = true,

				// Save the assembly as a physical file.
				GenerateInMemory = false,

				// Set the level at which the compiler 
				// should start displaying warnings.
				WarningLevel = 4,

				// Set whether to treat all warnings as errors.
				TreatWarningsAsErrors = false,

				// Set compiler argument to optimize output.
				CompilerOptions = "/optimize",

				// Specify the assembly file name to generate.
				OutputAssembly = Path.GetFileNameWithoutExtension(sourceFile) + ".exe"
			};

			// Add an assembly reference.
			cp.ReferencedAssemblies.Add("System.dll");

			// Invoke compilation.
			CodeDomProvider provider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
			CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceFile);

			if (cr.Errors.Count > 0)
			{
				string errors = string.Format("Errors building {0} into {1}: \n",
					sourceFile, cr.PathToAssembly);
				foreach (CompilerError ce in cr.Errors)
				{
					errors += string.Format("  {0}\n", ce.ToString());
				}
				return errors;
			}
			return string.Empty;
		}

		/// <summary>
		/// Metoda care ruleaza un cod compilat.
		/// </summary>
		/// <param name="appName">Numele complet al aplicatiei.</param>
		/// <param name="args">Argumentele pentru main.</param>
		public static void RuleazaAplicatie(string appName, string args)
		{
			Process.Start(appName, args);
		}
		#endregion

		#region Private properties

		/// <summary>
		/// Generam toate metodele de care avem nevoie pe baza tabelului de analiza sintactica.
		/// </summary>
		/// <param name="tabel">Tabelul de analiza sintactica.</param>
		private void GenereazaMetodeIndividuale(TAS tabel, List<string> terminale)
		{
			//Pentur fiecare linie din tabel cream cate o metoda care sa rezolve cerintele analizorului.
			foreach (var linie in tabel.Linii.Distinct())
			{
				//Cream metoda curenta.
				CodeMemberMethod memberMethod = new CodeMemberMethod
				{
					//Setam numele metodei.
					Name = linie.Linie,
					//Setam ceea ce returneaza.
					ReturnType = new CodeTypeReference(typeof(void)),
					//Setam atributele metodei.
					Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static
				};
				//Adaugam codul din cadrul metodei.
				CodeSnippetExpression codeExpression = new CodeSnippetExpression();
				bool firstAttempt = true;
				foreach(var simbol in linie.Valori)
				{
					if (!firstAttempt)
						codeExpression.Value += "\t\t\t";
					codeExpression.Value += $"if(input[pozitie] == \"{simbol.Key}\") {{\n";
					//Analizam codul dupa urmatoarele posibilitati:
					foreach(var rightSideSymbol in simbol.Value.RightSide)
					{
						//Daca primul simbol din partea dreapta a regulii este terminal
						//ne deplasam mai departe in input.
						if (terminale.Contains(rightSideSymbol))
						{
							codeExpression.Value += "\t\t\t\tpozitie++;\n";
						}
						else
						{
							//Atlfel apelam metoda corespunzatoare urmatorului neterminal.
							codeExpression.Value += $"\t\t\t\t{rightSideSymbol}();\n";
						}
					}
					//Daca avem regula care in partea dreapta il contine pe eps
					//il vom trece direct la return;.
					codeExpression.Value += "\t\t\t\treturn;\n\t\t\t}\n";
					firstAttempt = false;
				}
				//Daca nu gasim ceea ce cautam aruncam o eroare.
				codeExpression.Value += $"\t\tthrow new Exception($\"Error at {memberMethod.Name}. Input given: {{input[pozitie]}}. Expected something else.\")";
				memberMethod.Statements.Add(new CodeExpressionStatement(codeExpression));
				codeMethods.Add(memberMethod);
			}
		}

		/// <summary>
		/// Generam metoda main.
		/// </summary>
		/// <param name="linie">Prima functie care trebuie apelata.</param>
		/// <returns>Un CodeEntryPoint care reprezinta functia main.</returns>
		private CodeMemberMethod GenereazaMetodaMain(string linie)
		{
			CodeMemberMethod result = new CodeMemberMethod
			{
				Name = "Main",
				ReturnType = new CodeTypeReference(typeof(void)),
				Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final
			};
			//Adaugam parametrii.
			result.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string[])), "args"));
			//Generam codul necesar rularii metodei main.
			CodeSnippetExpression mainValues = new CodeSnippetExpression("input = new List<string>(args);\n");
			mainValues.Value += $"\t\t\ttry\n" +
								$"\t\t\t{{\n" +
								$"\t\t\t\t{linie}();\n" +
								$"\t\t\t\tif(input[pozitie] == \"$\")\n" +
								$"\t\t\t\t\tConsole.WriteLine(\"Propozitie corecta!\");\n" +
								$"\t\t\t\telse\n" +
								$"\t\t\t\t\tConsole.WriteLine(\"Propozitie incorecta!\");\n" +
								$"\t\t\t}}\n" +
								$"\t\t\tcatch(Exception e)\n" +
								$"\t\t\t{{\n" +
								$"\t\t\t\tConsole.WriteLine(e.Message);\n" +
								$"\t\t\t\tConsole.WriteLine(\"Propozitie incorecta!\");\n" +
								$"\t\t\t}}\n" +
								$"\t\t\tConsole.ReadKey();";
			result.Statements.Add(new CodeExpressionStatement(mainValues));
			return result;
		}
		#endregion
	}

}
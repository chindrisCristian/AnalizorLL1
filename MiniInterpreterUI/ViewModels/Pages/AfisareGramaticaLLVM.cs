using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using MiniInterpreterCore;

namespace MiniInterpreterUI
{
	/// <summary>
	/// The view model for the main window.
	/// </summary>
	public class AfisareGramaticaLLVM : BaseViewModel
    {

		#region Public Properties
		
		/// <summary>
		/// Folosit pentru afisarea tabelului de analiza sintactica.
		/// </summary>
		public DataTable TableResults;

		/// <summary>
		/// Folosit pentru afisarea gramaticii LL(1).
		/// </summary>
		public string FormaGramaticaNoua { get; set; }

		/// <summary>
		/// Folosit pentru afisarea simbolilor directori.
		/// </summary>
		public string SimboliDirectori { get; set; }

		/// <summary>
		/// Folosit pentru afisarea tabelului de analiza sintactica.
		/// </summary>
		public DataView TabelAnalizaSintatica => TableResults.DefaultView;

		#endregion

		#region Private Properties

		/// <summary>
		/// Gramatica pe care vom lucra in aceasta pagina.
		/// </summary>
		private Gramatica gramatica1 { get; set; } = null;

		/// <summary>
		/// Tabelul de analiza sintatica folosit.
		/// </summary>
		private TAS tasCurent { get; set; }

		#endregion

		#region Commands

		/// <summary>
		/// Comanda pentru a genera codul sursa pornind de la tabelul de analiza sintactica.
		/// </summary>
		public ICommand InterpreteazaGramaticaCMD { get; set; }

		#endregion

		#region Constructor

		public AfisareGramaticaLLVM(Gramatica gramatica)
		{
			gramatica1 = gramatica;
			InterpreteazaGramaticaCMD = new RelayCommand(async () => await InterpreteazaGramatica());

			PrintingInformation();
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Metoda folosita pentru generarea codului sursa.
		/// </summary>
		/// <returns></returns>
		private async Task InterpreteazaGramatica()
		{
			SaveFileDialog saveFile = new SaveFileDialog
			{
				Filter = "C# Files|*.cs",
				Title = "Salveaza fisierul cu cod sursa pentru gramatica curenta:"
			};
			saveFile.ShowDialog();
			if (saveFile.FileName != "")
			{
				SourceCodeGenerator generator = new SourceCodeGenerator();
				generator.GenereazaCodSursa(saveFile.FileName, tasCurent, gramatica1.Terminale);
				generator.CompileazaCodSursa(saveFile.FileName);
			}

			AppViewModel.Instance.SetCurrentPage(AppPages.AfisareCodSursa, saveFile.FileName);

			await Task.Delay(1);
		}

		/// <summary>
		/// Metoda cu ajutorul careia executam partea de verificare a conditiilor necesare algoritmului LL(1)
		/// si facem si afisarile corespunzatoare.
		/// </summary>
		private void PrintingInformation()
		{
			AnalizorLL1 analizor = new AnalizorLL1 { GramaticaAnalizata = gramatica1.Clone() };
			try
			{
				analizor.AnalizaRecursivitateStanga();
				analizor.AnalizaInceputComun();
			}
			catch (Exception)
			{
				MessageBox.Show("Gramatica introdusa nu poate fi analizata cu ajutorul unui analizor LL(1)", "Gramatica incorecta!", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			FormaGramaticaNoua = analizor.GramaticaAnalizata.AfiseazaGramatica();

			//Calcul simboli directori.
			var simboliDirectori = analizor.CalculMultimiSimboliDirectori();
			SimboliDirectori = Utility.AfisareMultimeSimboliDirectori(simboliDirectori, analizor.GramaticaAnalizata.ReguliProductie.Count);

			if (analizor.AnalizaConditiiLL1(simboliDirectori))
			{
				tasCurent = new TAS();
				tasCurent.GenereazaTabel(analizor.GramaticaAnalizata, simboliDirectori);
				TableResults = tasCurent.AfiseazaTabel(analizor.GramaticaAnalizata.Terminale);
			}
			else
			{
				MessageBox.Show("Gramatica nu respecta conditiile LL(1).", "Gramatica nu corespunde!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				AppViewModel.Instance.SetCurrentPage(AppPages.Introducere, null);
			}
		}

		#endregion
	}
}

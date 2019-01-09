using System;
using System.Collections.Generic;

namespace MiniInterpreterCore
{
	public static class Utility
	{
		/// <summary>
		/// Afisare lista sub forma unei multimi { el1, el2 ... }.
		/// </summary>
		/// <param name="multime">Multimea de unde luam elementele pe care sa le afisam.</param>
		/// <returns></returns>
		public static string AfisareMultime(List<string> multime)
		{
			string result = "{ ";
			for (int i = 0; i < multime.Count; i++)
			{
				result += (i == multime.Count - 1) ? string.Format($"{multime[i]}") : string.Format($"{multime[i]}, ");
			}
			result += " }";
			return result;
		}

		/// <summary>
		/// Afisarea simbolilor directori pe baza gramaticii.
		/// </summary>
		/// <param name="simboliDirectori">Simbolii directori pe care trebuie sa ii afisam.</param>
		/// <param name="nrReguli">Numarul de reguli pentru care am calculat acele multimi.</param>
		public static string AfisareMultimeSimboliDirectori(List<string>[] simboliDirectori, int nrReguli)
		{
			string result = "";
			for (int i = 0; i < nrReguli; i++)
			{
				string rezultat = string.Format("D[{0}] = {1}\n", i + 1, AfisareMultime(simboliDirectori[i]));
				result += rezultat;
			}
			return result;
		}

		/// <summary>
		/// Generarea unei gramatici pe baza input-ului dat.
		/// </summary>
		/// <returns></returns>
		public static Gramatica GenereazaGramaticaFromUI(string simbolStart, string neterminale, string terminale, string reguliProductie)
		{
			#region Verificari initiale
			if (simbolStart == null || neterminale == null || terminale == null || reguliProductie == null)
				throw new Exception("Nu s-au introdus datele necesare generarii unei gramatici!");
			#endregion

			#region Neterminale
			//Construim lista de neterminale.
			List<string> neterminaleList = new List<string>();
			foreach (var neterminal in neterminale.Split(' '))
			{
				//Daca vreun neterminal contine caractere interzise aruncam o eroare.
				if (neterminal.Contains("-") || neterminal.Contains("<") || neterminal.Contains(">") || neterminal.Contains(".") || neterminal.Contains("+"))
					throw new Exception("Terminalele nu pot contine caractere interzise.");
				neterminaleList.Add(neterminal);
			}
			//Daca simbolul de start nu apartine acestei liste aruncam o eroare.
			if (!neterminaleList.Contains(simbolStart))
				throw new Exception("Multimea terminalelor este incorecta sau simbolul de start nu apartine acesteia!");
			#endregion

			#region Terminale
			//Construim lista de terminale.
			List<string> terminaleList = new List<string>();
			foreach (var terminal in terminale.Split(' '))
				terminaleList.Add(terminal);
			#endregion

			#region Reguli de productie
			//Construim lista cu regulile de productie.
			List<RegulaProductie> reguliProductieList = new List<RegulaProductie>();
			foreach(var regula in reguliProductie.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				RegulaProductie regulaProductie = new RegulaProductie();
				regulaProductie.LeftSide = regula.Split(':' )[0];
				foreach (var rightSideTerminal in regula.Split(':')[1].Split(' '))
					if(rightSideTerminal != "")
						regulaProductie.RightSide.Add(rightSideTerminal);
				reguliProductieList.Add(regulaProductie);
			}
			#endregion

			return new Gramatica
			{
				SimbolStart = simbolStart,
				Neterminale = neterminaleList,
				Terminale = terminaleList,
				ReguliProductie = reguliProductieList
			};
		}
	}
}
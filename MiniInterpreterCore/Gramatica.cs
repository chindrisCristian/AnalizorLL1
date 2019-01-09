using System.Collections.Generic;

namespace MiniInterpreterCore
{
	public class Gramatica
	{
		#region Public properties

		public string SimbolStart { get; set; }

		public List<string> Neterminale { get; set; }

		public List<string> Terminale { get; set; }

		public List<RegulaProductie> ReguliProductie { get; set; }

		#endregion

		#region Public Methods

		public string AfiseazaGramatica()
		{
			//Afisare simbol de start.
			string result = "Gr = < Σ, Vn, S, P >\n";
				
			result += $"S = {SimbolStart}\n";

			//Afisarea multimimii neterminalelor.
			result += $"Vn = {Utility.AfisareMultime(Neterminale)}\n";

			//Afisarea multimii neterminalelor.
			result += $"Terminale = {Utility.AfisareMultime(Terminale)}\n";

			//Afisarea multimii regulilor de productie.
			result += "P = {\n";
			for (int i = 0; i < ReguliProductie.Count; i++)
			{
				result += $"\t{i + 1}. {ReguliProductie[i].PrintRule()}\n";
			}
			result += "\t}";
			return result;
		}

		public Gramatica Clone()
		{
			var reguliProductie = new List<RegulaProductie>();
			foreach (var regula in ReguliProductie)
				reguliProductie.Add(regula.Clone());

			return new Gramatica
			{
				SimbolStart = (string)this.SimbolStart.Clone(),
				Neterminale = new List<string>(this.Neterminale),
				Terminale = new List<string>(this.Terminale),
				ReguliProductie = reguliProductie
			};
		}

		#endregion
	}
}

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MiniInterpreterCore
{
	public class TASLine
	{
		#region Public properties

		public string Linie { get; set; }

		public Dictionary<string, RegulaProductie> Valori { get; set; }

		

		#endregion
	}



	public class TAS
	{
		#region Public properties

		public List<TASLine> Linii { get; set; } = new List<TASLine>();

		#endregion

		#region Public Methods

		/// <summary>
		/// Folosit pentru generarea unei tabele de analiza sintactica.
		/// </summary>
		/// <param name="gramaticaAnalizata">Gramatica pentru care se construieste TAS.</param>
		/// <param name="simboliDirectori">Multimile de simboli directori cu ajutorul carora se construieste TAS.</param>
		public void GenereazaTabel(Gramatica gramaticaAnalizata, List<string>[] simboliDirectori)
		{
			for(int i=0; i<gramaticaAnalizata.ReguliProductie.Count; i++)
			{
				Dictionary<string, RegulaProductie> tmp = new Dictionary<string, RegulaProductie>();
				foreach (var simbol in simboliDirectori[i])
				{
					tmp.Add(simbol, gramaticaAnalizata.ReguliProductie[i]);
				}
				if (gramaticaAnalizata.ReguliProductie[i].RightSide.Count == 0)
				{
					tmp.Add("$", gramaticaAnalizata.ReguliProductie[i]);
				}
				if (Linii.Where(x => x.Linie.Equals(gramaticaAnalizata.ReguliProductie[i].LeftSide)).Count() > 0)
				{
					foreach (var element in tmp)
					{
						Linii[Linii.FindIndex(x => x.Linie.Equals(gramaticaAnalizata.ReguliProductie[i].LeftSide))].Valori.Add(element.Key, element.Value);
					}
				}
				else
				{
					Linii.Add(new TASLine
					{
						Linie = gramaticaAnalizata.ReguliProductie[i].LeftSide,
						Valori = tmp
					});
				}
			}
		}

		/// <summary>
		/// Metoda folosita pentru afisarea tabelului de analiza sintactica sub forma tabelara. 
		/// </summary>
		/// <param name="teminale">Lista de terminale aferenta tabelului.</param>
		/// <returns>Tabelul rezultat.</returns>
		public DataTable AfiseazaTabel(List<string> terminale)
		{
			DataTable result = new DataTable("TAS");
			DataColumn column;
			DataRow row;

			#region Definirea coloanelor
			column = new DataColumn
			{
				DataType = typeof(string),
				ColumnName = "FirstCol",
				Caption = ""
			};
			result.Columns.Add(column);
			int i;
			for (i = 0; i< terminale.Count;i++)
			{
				var terminal = terminale[i];
				column = new DataColumn
				{
					DataType = typeof(string),
					ColumnName = string.Format("{0}", i),
					Caption = string.Format("{0}", terminal)
				};
				result.Columns.Add(column);
			}
			column = new DataColumn
			{
				DataType = typeof(string),
				ColumnName = string.Format("{0}", i),
				Caption = string.Format("{0}", "$")
			};
			result.Columns.Add(column);
			#endregion

			#region Definirea randurilor pentur fiecare neterminal
			foreach(var line in Linii)
			{
				row = result.NewRow();
				row["FirstCol"] = line.Linie;
				foreach (var value in line.Valori)
				{
					row[GetColumnName(result, value.Key)] = value.Value;
				}
				result.Rows.Add(row);
			}
			#endregion

			#region Definirea randurilor pentru fiecare terminal
			foreach(var terminal in terminale)
			{
				row = result.NewRow();
				row["FirstCol"] = terminal;
				row[GetColumnName(result, terminal)] = "Pop";
				result.Rows.Add(row);
			}
			row = result.NewRow();
			row["FirstCol"] = "$";
			row[GetColumnName(result, "$")] = "Accept";
			result.Rows.Add(row);
			#endregion

			return result;
		}


		/// <summary>
		/// Metoda creata pentru a gasi numele coloanei pentru care Caption este key.
		/// </summary>
		/// <param name="result">DataTable-ul unde cautam</param>
		/// <param name="key">Cheia pentru care ii cautam numele coloanei.</param>
		/// <returns>Numele coloanei pe care o cautam.</returns>
		private string GetColumnName(DataTable result, string key)
		{
			foreach(DataColumn col in result.Columns)
			{
				if (col.Caption == key)
					return col.ColumnName;
			}
			return string.Empty;
		}
		#endregion
	}
}

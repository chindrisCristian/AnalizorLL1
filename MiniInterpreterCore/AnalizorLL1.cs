using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniInterpreterCore
{
	public class AnalizorLL1
	{
		#region Private properties

		List<string> neterminaleVizitate = new List<string>();

		#endregion

		#region Public properties

		public Gramatica GramaticaAnalizata { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Rezolvam problemele recursivitatii stanga, daca exista.
		/// </summary>
		public void AnalizaRecursivitateStanga()
		{

			//Construim o lista cu toate regulile care au recursivitate stanga.
			var reguliRecursiveStanga = GramaticaAnalizata.ReguliProductie.Where(x => x.RightSide.Count > 0).Where(x => x.LeftSide.Equals(x.RightSide[0])).ToList();

			//Aplicam algoritmul de schimbare pentru fiecare regula cu recursivitate stanga.
			foreach(var regula in reguliRecursiveStanga)
			{
				//Preluam regula de forma A->b.
				var regulaSecundara = GramaticaAnalizata.ReguliProductie.
					Where(x => x.LeftSide == regula.LeftSide && !x.RightSide[0].Equals(regula.RightSide[0])).First();
				//Preluam indexul primei reguli.
				int index = GramaticaAnalizata.ReguliProductie.FindIndex(x => x == regula);
				//Stergem cele 2 reguli din gramatica.
				GramaticaAnalizata.ReguliProductie.Remove(regula);
				GramaticaAnalizata.ReguliProductie.Remove(regulaSecundara);
				//Adaugam regulile noi formate.
				GramaticaAnalizata.ReguliProductie.InsertRange((index > GramaticaAnalizata.ReguliProductie.Count) ? (GramaticaAnalizata.ReguliProductie.Count) : (index), InlocuireRecursivitateStanga(regula, regulaSecundara));
			}
		}

		/// <summary>
		/// Rezolvam probleme regulilor cu acelasi inceput.
		/// </summary>
		public void AnalizaInceputComun()
		{
			//Generare lista noua de reguli.
			List<RegulaProductie> reguliNoi = new List<RegulaProductie>();

			//Verificam pentru fiecare regula daca exista o alta regula care sa inceapa la fel.
			//Pentru regulile pe care le-am verificat le omitem.
			List<int> reguliVerificate = new List<int>();
			for (int i = 0; i < GramaticaAnalizata.ReguliProductie.Count; i++)
			{
				//Daca am verificat-o deja mergem mai departe.
				if (reguliVerificate.Contains(i))
					continue;

				var regula = GramaticaAnalizata.ReguliProductie[i];
				//Cautam o alta regula cu acelasi inceput ca si al celei de mai sus.
				var tmp = GasesteAcelasiInceput(i);
				//Daca am gasit o astfel de regula...
				if (tmp.Item1 > 0)
				{
					//O selectam.
					var regulaSecundara = GramaticaAnalizata.ReguliProductie[tmp.Item1];
					reguliVerificate.Add(tmp.Item1);
					//Si aplicam algoritmul de inlocuire al inceputului comun.
					reguliNoi.AddRange(InlocuireInceputComun(regula, regulaSecundara, tmp));
				}
				else
				{
					//Luam regula ca atare.
					reguliNoi.Add(regula);
				}
			}
			GramaticaAnalizata.ReguliProductie = reguliNoi;
		}

		/// <summary>
		/// Calcularea multimilor de simboli directori.
		/// </summary>
		/// <returns>Returneaza lista cu acele multimi.</returns>
		public List<string>[] CalculMultimiSimboliDirectori()
		{
			//Generarea multimilor de simboli directori.
			List<string>[] simboliDirectori = new List<string>[GramaticaAnalizata.ReguliProductie.Count];
			for (int i = 0; i < GramaticaAnalizata.ReguliProductie.Count; i++)
			{
				simboliDirectori[i] = new List<string>();
				//Daca pentur regula curenta avem in partea dreapta elemente => vom calcula multimea FIRST+...
				if (GramaticaAnalizata.ReguliProductie[i].RightSide.Count > 0)
					simboliDirectori[i].AddRange(First(GramaticaAnalizata.ReguliProductie[i].RightSide[0]));
				else
					//Altfel calculam multimea FOLLOW+ pentru aceasta regula.
					simboliDirectori[i].AddRange(Follow(GramaticaAnalizata.ReguliProductie[i].LeftSide));

				neterminaleVizitate.Clear();
			}
			return simboliDirectori;
		}

		/// <summary>
		/// Verificarea conditiilor pe baza multimilor de simboli directori.
		/// </summary>
		/// <param name="simboliDirectori">Simboli directori cu ajutorul carora verificam conditia.</param>
		/// <returns>Returnam daca putem sau nu sa aplicam analiza LL1 pe aceasta gramatica.</returns>
		public bool AnalizaConditiiLL1(List<string>[] simboliDirectori)
		{
			List<string> elementeComune = new List<string>();

			//Verificam conditia pentru fiecare neterminal.
			foreach(var neterminal in GramaticaAnalizata.Neterminale)
			{
				//Identificarea indecsilori pentru toate regulile care au in partea stanga neterminalul curent.
				var indecsi = GramaticaAnalizata.ReguliProductie.
					Select((x, i) => x.LeftSide.Equals(neterminal) ? i : -1).
					Where(i => i >= 0).
					ToList();
				//Facem intersectia multimilor 2 cate 2.
				for(int i = 0; i < indecsi.Count - 1; i++)
				{
					elementeComune = simboliDirectori[indecsi[i]];
					elementeComune = elementeComune.Intersect(simboliDirectori[indecsi[i + 1]]).ToList();
				}
				if (elementeComune.Count > 0 && indecsi.Count > 1)
					return false;
			}
			return true;
		}

		#endregion

		#region Private Helpers

		/// <summary>
		/// Algoritm pentru inlocuirea recursivitatii stanga.
		/// </summary>
		/// <param name="regula">Regula de forma A->Aa.</param>
		/// <param name="regulaSecundara">Regula de forma A->b.</param>
		/// <param name="neterminaleNoi">Pentru a putea adauga noul neterminal aparut.</param>
		/// <returns></returns>
		private List<RegulaProductie> InlocuireRecursivitateStanga(RegulaProductie regula, RegulaProductie regulaSecundara)
		{
			string prefixNeterminale = "1";
			List<RegulaProductie> rezultat = new List<RegulaProductie>();

			//Generam neterminalul A^
			string neterminalNou = string.Format($"{regula.LeftSide}{prefixNeterminale}");
			GramaticaAnalizata.Neterminale.Add(neterminalNou);
			
			//Adaugam regula A->bA^
			rezultat.Add(new RegulaProductie
			{
				LeftSide = regula.LeftSide,
				RightSide = regulaSecundara.RightSide
			});
			rezultat.Last().RightSide.Add(neterminalNou);

			//Adaugam regula A^->aA^
			rezultat.Add(new RegulaProductie
			{
				LeftSide = neterminalNou,
				RightSide = regula.RightSide
			});
			rezultat.Last().RightSide.RemoveAt(0);
			rezultat.Last().RightSide.Add(neterminalNou);

			//Adaugam regula A^->eps
			rezultat.Add(new RegulaProductie
			{
				LeftSide = neterminalNou,
				RightSide = new List<string>()
			});

			return rezultat;
		}

		/// <summary>
		/// Algoritm pentru inlocuirea inceputului comun.
		/// </summary>
		/// <param name="regula">Prima regula de forma A->aC.</param>
		/// <param name="regulaSecundara">A doua regula de forma A->aB</param>
		/// <returns></returns>
		private List<RegulaProductie> InlocuireInceputComun(RegulaProductie regula, RegulaProductie regulaSecundara, Tuple<int, int> info)
		{
			string prefixNeterminale = "2";
			List<RegulaProductie> rezultat = new List<RegulaProductie>();

			//Generam neterminalul A*.
			string neterminalNou = string.Format($"{regula.LeftSide}{prefixNeterminale}");
			GramaticaAnalizata.Neterminale.Add(neterminalNou);

			//Preluam partea comuna.
			var aData = regula.RightSide.GetRange(0, info.Item2);
			aData.Add(neterminalNou);
			//Preluam partea ulterioara partii comune din prima regula.
			var bData = regula.RightSide.GetRange(info.Item2, regula.RightSide.Count - info.Item2);
			//Preluam partea ulterioara partii comune din a doua regula.
			var cData = regulaSecundara.RightSide.GetRange(info.Item2, regulaSecundara.RightSide.Count - info.Item2);

			//Adaugam regula A->aA*.
			rezultat.Add(new RegulaProductie
			{
				LeftSide = regula.LeftSide,
				RightSide = new List<string>(aData)
			});
			//Adaugam regula A*->b.
			rezultat.Add(new RegulaProductie
			{
				LeftSide = neterminalNou,
				RightSide = bData
			});
			//Adaugam regula A*->c.
			rezultat.Add(new RegulaProductie
			{
				LeftSide = neterminalNou,
				RightSide = cData
			});

			return rezultat;
		}

		/// <summary>
		/// Cautam o regula care sa aiba acelasi inceput ca si regula precedenta regulii start.
		/// </summary>
		/// <param name="start">De unde sa incepem cautarea in lista de reguli.</param>
		/// <returns></returns>
		private Tuple<int, int> GasesteAcelasiInceput(int start)
		{
			int index = -1, length = 0;
			//Regula pentru care facem verificarea.
			var regula = GramaticaAnalizata.ReguliProductie[start].RightSide;

			for (int i = 0; i < GramaticaAnalizata.ReguliProductie.Count; i++)
			{
				//Daca nu au aceeasi parte stanga mergem mai departe.
				if (GramaticaAnalizata.ReguliProductie[i].LeftSide != GramaticaAnalizata.ReguliProductie[start].LeftSide)
					continue;
				//Sarim peste aceeasi regula.
				if (i == start)
					continue;
				//Cand am gasit-o ii calculam lungimea comuna a partii de inceput.
				var regulaCurenta = GramaticaAnalizata.ReguliProductie[i].RightSide;
				for (int j = 0; j < ((regula.Count >= regulaCurenta.Count) ? regulaCurenta.Count : regula.Count); j++)
				{
					if (regula[j].Equals(regulaCurenta[j]))
						length++;
					else
						break;
				}
				if (length > 0)
				{
					index = i;
					break;
				}
			}

			Tuple<int, int> tuple = Tuple.Create(index, length);
			return tuple;
		}

		/// <summary>
		/// Calculam multimea FOLLOW+ pentu simbolul curent.
		/// </summary>
		/// <param name="neterminal">Simbolul pentru care calculam aceasta multime.</param>
		/// <returns>O lista de terminale reprezentand valorile multimii FOLLOW+</returns>
		private List<string> Follow(string neterminal)
		{
			//Marcam neterminalul ca fiind verificat.
			neterminaleVizitate.Add(neterminal);

			List<string> rezultat = new List<string>();

			//Verificam fiecare regula unde neterminalul apare in partea dreapta a acesteia.
			foreach(var regula in GramaticaAnalizata.ReguliProductie.Where(x => x.RightSide.Contains(neterminal)))
			{
				//Preluam pozitia lui in cadrul regulii.
				int pozitieNeterminal = regula.RightSide.IndexOf(neterminal);
				//Daca neterminalul apare in mijlocul formei propozitionale...
				if (pozitieNeterminal < regula.RightSide.Count - 1)
					rezultat.AddRange(First(regula.RightSide[pozitieNeterminal + 1]));
				else
				{
					//Nu mai calculam FOLLOW+ pentru acelasi neterminal.
					if (neterminaleVizitate.Contains(regula.LeftSide))
						continue;
					//Altfel calculam follow pentru partea stanga a acelei reguli.
					rezultat.AddRange(Follow(regula.LeftSide));
				}
			}
			return rezultat.Distinct().ToList();
		}

		/// <summary>
		/// Calculam multimea FIRST+ pentu simbolul curent.
		/// </summary>
		/// <param name="simbol">Simbolul pentru care calculam aceasta multime.</param>
		/// <returns>O lista de terminale reprezentand valorile multimii FIRST+</returns>
		private List<string> First(string simbol)
		{
			List<string> rezultat = new List<string>();

			//Daca simbolul este un terminal atunci ne oprim.
			if (GramaticaAnalizata.Terminale.Contains(simbol))
				rezultat.Add(simbol);
			else
			{
				//Altfel calculam FIRST pentru toate aparitiile sale unde partea dreapta contine cel putin un element.
				foreach(var regula in GramaticaAnalizata.ReguliProductie.Where(x => x.LeftSide == simbol))
				{
					if (regula.RightSide.Count > 0)
						rezultat.AddRange(First(regula.RightSide[0]));
					else
						rezultat.AddRange(Follow(regula.LeftSide));
				}
			}
			return rezultat.Distinct().ToList();
		}
		#endregion
	}
}
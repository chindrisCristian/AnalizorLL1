using System;
using System.IO;
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
	public class IntroducereGramaticaVM : BaseViewModel
	{
		#region Public properties

		/// <summary>
		/// Titlul aplicatiei.
		/// </summary>
		public string MainWindowTitle { get; set; } = "Mini Interpreter";

		/// <summary>
		/// Descriem aplicatia.
		/// </summary>
		public string MainWindowDescription { get; set; } = "It's not such a real interpreter, but something smaller...";

		public string SimbolStart { get; set; }
		public string Neterminale { get; set; }
		public string Terminale { get; set; }
		public string ReguliProductie { get; set; }

		#endregion

		#region Commands

		/// <summary>
		/// Comanda pentru a trece mai departe la a verifica conditiile LL1.
		/// </summary>
		public ICommand VerifyLLConditionsCMD { get; set; }

		/// <summary>
		/// Comanda pentru a prelua o configuratie dintr-un fisier.
		/// </summary>
		public ICommand LoadFromFileCMD { get; set; }

		/// <summary>
		/// Comanda pentru a salva o configuratie.
		/// </summary>
		public ICommand SaveConfigCMD { get; set; }

		/// <summary>
		/// Comanda pentru a afisa fereastra de ajutor.
		/// </summary>
		public ICommand HelpCMD { get; set; }

		#endregion

		#region Constructor

		public IntroducereGramaticaVM()
		{
			//Cream comenzile.
			VerifyLLConditionsCMD = new RelayCommand(async () => await VerifyLLConditions());
			LoadFromFileCMD = new RelayCommand(async () => await LoadConfigFromFile());
			SaveConfigCMD = new RelayCommand(async () => await SaveConfig());
			HelpCMD = new RelayCommand(async () => await ShowHelpMsg());
		}

		#endregion

		#region Helper methods   

		/// <summary>
		/// Verificam daca inputul introdus de utilizator este corect,
		/// iar in caz afirmativ aplicam algoritmul pentru analizorul LL1.
		/// </summary>
		/// <returns></returns>
		private async Task VerifyLLConditions()
		{
			try
			{
				Gramatica gramatica = Utility.GenereazaGramaticaFromUI(SimbolStart, Neterminale, Terminale, ReguliProductie);
				AppViewModel.Instance.SetCurrentPage(AppPages.AfisareLL, gramatica);				
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message, "Eroare intampinata in generarea gramaticii!", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			await Task.Delay(1);
		}

		/// <summary>
		/// Ne incarcam configuratia dintr-un fisier.
		/// </summary>
		/// <returns></returns>
		private async Task LoadConfigFromFile()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			string fileName;
			if (openFileDialog.ShowDialog() == true)
			{
				fileName = openFileDialog.FileName;
				StreamReader file = new StreamReader(fileName);
				//Prima linie care reprezinta simbolul de start.
				SimbolStart = file.ReadLine();
				//A doua linie care reprezinta multimea neterminalelor.
				Neterminale = file.ReadLine();
				//A treia linie care reprezinta multimea terminalelor.
				Terminale = file.ReadLine();
				//A 4a linie care reprezinta numarul de reguli de productie.
				int nrReguli = int.Parse(file.ReadLine());
				for(int i=0; i < nrReguli; i++)
				{
					ReguliProductie += file.ReadLine();
					ReguliProductie += "\r\n";
				}
			}
			await Task.Delay(1);
		}

		/// <summary>
		/// Salvam configuratia curenta.
		/// </summary>
		/// <returns></returns>
		private async Task SaveConfig()
		{
			string fileName = "input" + DateTime.Now.ToShortTimeString() + ".txt";
			File.WriteAllText(fileName, SimbolStart);
			File.WriteAllText(fileName, Neterminale);
			File.WriteAllText(fileName, Terminale);
			File.WriteAllText(fileName, GetNrOfRules());
			File.WriteAllText(fileName, ReguliProductie);

			await Task.Delay(1);
		}

		/// <summary>
		/// Numaram reguli de productie.
		/// </summary>
		/// <returns></returns>
		private string GetNrOfRules()
		{
			int result = 0;
			foreach (var str in ReguliProductie.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
				result++;
			return result.ToString();
		}

		/// <summary>
		/// Deschidem o noua pagina care contine cateva idei pentru buna functionare a acestei aplicatii.
		/// </summary>
		/// <returns></returns>
		private async Task ShowHelpMsg()
		{
			HelpWindow helpWindow = new HelpWindow();
			helpWindow.Show();

			await Task.Delay(1);
		}

		#endregion
	}
}

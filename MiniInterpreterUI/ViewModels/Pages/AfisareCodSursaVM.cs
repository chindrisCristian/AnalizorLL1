using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using MiniInterpreterCore;

namespace MiniInterpreterUI
{
	/// <summary>
	/// The view model for the main window.
	/// </summary>
	public class AfisareCodSursaVM : BaseViewModel
    {

		#region Public Properties

		/// <summary>
		/// Folosit pentru afisarea codului sursa generat anterior.
		/// </summary>
		public string CodSursa { get; set; }

		/// <summary>
		/// Folosit pentru preluarea propozitiilor care urmeaza a fi verificate.
		/// </summary>
		public string InputString { get; set; }

		#endregion

		#region Private Properties

		/// <summary>
		/// Numele fisierului care contine codul sursa.
		/// </summary>
		private string _fileName;

		#endregion

		#region Commands

		/// <summary>
		/// Comanda pentru a genera codul sursa pornind de la tabelul de analiza sintactica.
		/// </summary>
		public ICommand ExecutaCMD { get; set; }

		/// <summary>
		/// Comanda pentru a afisa fereastra de ajutor.
		/// </summary>
		public ICommand HelpCMD { get; set; }

		/// <summary>
		/// Comanda pentru a ne intoarce la prima pagina.
		/// </summary>
		public ICommand FirstPageCMD { get; set; }

		#endregion

		#region Constructor

		public AfisareCodSursaVM(string fileName)
		{
			_fileName = fileName;

			ExecutaCMD = new RelayCommand(async () => await ExecutaAplicatie());
			HelpCMD = new RelayCommand(async () => await AfiseazaFereastraHelp());
			FirstPageCMD = new RelayCommand(async () => await LaPrimaPagina());

			CodSursa = File.ReadAllText(_fileName);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Metoda folosita pentru generarea codului sursa.
		/// </summary>
		/// <returns></returns>
		private async Task ExecutaAplicatie()
		{
			if (string.IsNullOrEmpty(InputString))
			{
				MessageBox.Show("Trebuie trimit un input pentru a putea executa programul!");
				return;
			}
			SourceCodeGenerator.RuleazaAplicatie(Path.GetFileNameWithoutExtension(_fileName) + ".exe", InputString);

			await Task.Delay(1);
		}

		/// <summary>
		/// Metoda folosita pentru a afisa fereastra de ajutor.
		/// </summary>
		/// <returns></returns>
		private async Task AfiseazaFereastraHelp()
		{
			HelpWindow helpWindow = new HelpWindow();
			helpWindow.Show();

			await Task.Delay(1);
		}
	
		/// <summary>
		/// Metoda folosita pentru a ne intoarce la prima pagina.
		/// </summary>
		/// <returns></returns>
		private async Task LaPrimaPagina()
		{
			AppViewModel.Instance.SetCurrentPage(AppPages.Introducere, null);

			await Task.Delay(1);
		}
		#endregion
	}
}

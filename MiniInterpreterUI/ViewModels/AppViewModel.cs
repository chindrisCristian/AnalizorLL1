namespace MiniInterpreterUI
{

	public enum AppPages
	{
		Introducere,
		AfisareLL,
		AfisareCodSursa
	}

	/// <summary>
	/// View Model pentru fereastra principala.
	/// Contine modul de prelucrare al datelor pentru pagini.
	/// </summary>
	public class AppViewModel : BaseViewModel
    {
		/// <summary>
		/// Instanta propriu zisa cu care va lucra fiecare ViewModel al fiecarei pagini.
		/// </summary>
		public static AppViewModel Instance = new AppViewModel();

		#region Private properties

		/// <summary>
		/// Builder-ul care va prelua informatiile necesare pentru trecerea la alte pagini.
		/// </summary>
		private ViewModelBuilder _builder = new ViewModelBuilder();

		#endregion

		#region Public properties

		/// <summary>
		/// Pagina curenta, cea care este afisata in user control-ul din fereastra principala.
		/// </summary>
		public AppPages CurrentPage { get; set; } = AppPages.Introducere;

		/// <summary>
		/// Builder-ul care va prelua informatiile necesare pentru trecerea la alte pagini.
		/// </summary>
		public ViewModelBuilder ViewModelBuilder => _builder;

		#endregion

		#region Constructor

		private AppViewModel() { }

		#endregion

		#region Public Methods

		public void SetCurrentPage(AppPages appPage, object pageData)
		{
			_builder.PageToBuild = appPage;
			_builder.ViewModelData = pageData;
			CurrentPage = appPage;
		}

		#endregion
	}
}

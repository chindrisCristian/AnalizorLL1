using MiniInterpreterCore;

namespace MiniInterpreterUI
{
	/// <summary>
	/// The view model for the main window.
	/// </summary>
	public class ViewModelBuilder
	{
		public AppPages PageToBuild;

		public object ViewModelData;

		public BaseViewModel Build()
		{
			switch (PageToBuild)
			{
				case AppPages.Introducere:
					return new IntroducereGramaticaVM();
				case AppPages.AfisareLL:
					return new AfisareGramaticaLLVM(ViewModelData as Gramatica);
				case AppPages.AfisareCodSursa:
					return new AfisareCodSursaVM(ViewModelData as string);
				default:
					return null;
			}
		}


	}
}

using System;
using System.Globalization;

namespace MiniInterpreterUI
{
	/// <summary>
	/// Convert the <see cref="ApplicationPage"/> to an actual view/page
	/// </summary>
	public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//Find the appropriate page
			switch ((AppPages)value)
			{
				case AppPages.Introducere:
					return new IntroducereGramatica(AppViewModel.Instance.ViewModelBuilder.Build());
				case AppPages.AfisareLL:
					return new AfisareGramaticaLL(AppViewModel.Instance.ViewModelBuilder.Build());
				case AppPages.AfisareCodSursa:
					return new AfisareCodSursa(AppViewModel.Instance.ViewModelBuilder.Build());
				default:
					//Debugger.Break();
					return null;
			}
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

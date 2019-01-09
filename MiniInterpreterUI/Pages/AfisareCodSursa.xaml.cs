using System.Windows.Controls;

namespace MiniInterpreterUI
{
	/// <summary>
	/// Interaction logic for AfisareCodSursa.xaml
	/// </summary>
	public partial class AfisareCodSursa : UserControl
	{
		public string FileName { get; set; }

		public AfisareCodSursa(BaseViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel as AfisareCodSursaVM;
		}
	}
}

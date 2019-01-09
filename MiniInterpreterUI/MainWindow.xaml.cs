using System.Windows;
using System.Windows.Controls;

namespace MiniInterpreterUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = AppViewModel.Instance;
		//	Switcher.pageSwitcher = this;
		//	Switcher.Switch(new IntroducereGramatica());
		}

		public void Navigate(UserControl nextPage)
		{
			Content = nextPage;
		}
	}
}

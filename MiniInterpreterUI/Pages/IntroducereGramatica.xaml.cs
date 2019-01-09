using System.Windows.Controls;

namespace MiniInterpreterUI
{
	/// <summary>
	/// Interaction logic for IntroducereGramatica.xaml
	/// </summary>
	public partial class IntroducereGramatica : UserControl
    {
        public IntroducereGramatica(BaseViewModel viewModel)
        {
            InitializeComponent();
			DataContext = viewModel as IntroducereGramaticaVM;
        }
    }
}

using System.Windows;
using AgnesAIImageEdit.ViewModels;

namespace AgnesAIImageEdit.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            if (DataContext is MainViewModel vm)
            {
                vm.RequestPromptFocus += () => PromptTextBox.Focus();
            }
        }

        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            Drawer.Visibility = Drawer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

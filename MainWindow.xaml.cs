using System.Windows;

namespace CBLiteApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainWindowViewModel();
            DataContext = _vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.CreateProducts();
        }

        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
           await _vm.LoadDataFromCache();
        }
    }
}

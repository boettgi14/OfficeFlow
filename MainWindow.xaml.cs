using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OfficeFlow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EndProgram_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des MainWindows
            this.Close();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des LoginWindows
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            // Schließen des MainWindows
            this.Close();
        }

        private void OpenGitHub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/boettgi14/OfficeFlow",
                UseShellExecute = true
            });
        }
    }
}
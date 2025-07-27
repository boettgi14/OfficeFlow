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
        public MainWindow(User user)
        {
            if (user.AdminStatus)
            {
                // Benutzer ist Admin
                InitializeComponent();
                // Setzen der Adminfunktionen
                UserManagement.IsEnabled = true;
                AppointmentChange.IsEnabled = true;
                AppointmentDelete.IsEnabled = true;
                TaskChange.IsEnabled = true;
                TaskDelete.IsEnabled = true;
            }
            else
            {
                // Benutzer ist User
                InitializeComponent();
            }
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

        private void OpenUserManagement_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des UserManagementWindows
            UserManagementWindow userManagementWindow = new UserManagementWindow();
            userManagementWindow.Owner = this; // Besitzer auf MainWindow setzen
            userManagementWindow.ShowDialog();
        }

        private void OpenAbout_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des AboutWindows
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this; // Besitzer auf MainWindow setzen
            aboutWindow.ShowDialog();
        }
    }
}
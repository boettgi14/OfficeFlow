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
            InitializeComponent();

            // Initialisieren der Datenbank für Aufgaben
            TaskDatabaseHelper.InitializeDatabase();

            // Setzen der UI auf Adminstatus des Benutzers
            setAdminStatus(user);
        }

        private void setAdminStatus(User user)
        {
            if (user.AdminStatus)
            {
                // Nutzer ist Admin
                // Admin Funktionen aktivieren
                UserManagementMenuItem.IsEnabled = true;
                EditAppointmentMenuItem.IsEnabled = true;
                DeleteAppointmentMenuItem.IsEnabled = true;
                EditTaskMenuItem.IsEnabled = true;
                DeleteTaskMenuItem.IsEnabled = true;
            }
            else
            {
                // Nutzer ist kein Admin
                // Admin Funktionen deaktivieren
                UserManagementMenuItem.IsEnabled = true;
                EditAppointmentMenuItem.IsEnabled = true;
                DeleteAppointmentMenuItem.IsEnabled = true;
                EditTaskMenuItem.IsEnabled = true;
                DeleteTaskMenuItem.IsEnabled = true;
            }
        }

        private void EndProgramMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des MainWindows
            this.Close();
        }

        private void LogOutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des LoginWindows
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            // Schließen des MainWindows
            this.Close();
        }

        private void GitHubMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Öffnen des Browsers mit der GitHub Seite des Projekts
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/boettgi14/OfficeFlow",
                UseShellExecute = true
            });
        }

        private void UserManagementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des UserManagementWindows
            UserManagementWindow userManagementWindow = new UserManagementWindow();
            userManagementWindow.Owner = this; // Besitzer auf MainWindow setzen
            userManagementWindow.ShowDialog();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des AboutWindows
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this; // Besitzer auf MainWindow setzen
            aboutWindow.ShowDialog();
        }

        private void InstructionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des InstructionsWindows
            InstructionsWindow instructionsWindow = new InstructionsWindow();
            instructionsWindow.Owner = this; // Besitzer auf MainWindow setzen
            instructionsWindow.ShowDialog();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des SettingsWindows
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this; // Besitzer auf MainWindow setzen
            settingsWindow.ShowDialog();
        }
    }
}
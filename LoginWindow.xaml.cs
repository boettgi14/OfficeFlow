using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OfficeFlow
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void OpenMainWindow(User user)
        {
            // Erstellen des MainWindows
            MainWindow mainWindow = new MainWindow(user);
            mainWindow.Show();
            // Schließen des LoginWindows
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Setzen der Nutzerdaten aus den Textfeldern
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Sicherstellen, dass die Datenbank initialisiert ist
            UserDatabaseHelper.InitializeDatabase();

            if (UserDatabaseHelper.VerifyLogin(username, password))
            {
                // Login erfolgreich
                // Benutzer aus der Datenbank abrufen
                User user = UserDatabaseHelper.GetUser(username);
                // Öffnen des MainWindows mit dem Benutzer
                OpenMainWindow(user);
            }
            else
            {
                // Login fehlgeschlagen
                MessageBox.Show("Ungültiger Nutzername oder ungültiges Passwort!", "Login Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

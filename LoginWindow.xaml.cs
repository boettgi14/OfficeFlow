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

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            DataBaseHelper.InitializeDatabase(); // Sicherstellen, dass die Datenbank initialisiert ist

            if (DataBaseHelper.VerifyLogin(username, password))
            {
                // Login erfolgreich
                User user = setUser(username, password);
                OpenMainWindow(user);
            }
            else
            {
                // Login fehlgeschlagen
                MessageBox.Show("Ungültiger Benutzername oder Passwort.", "Login Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private User setUser(string username, string password)
        {
            // Benutzer aus der Datenbank abrufen
            User user = DataBaseHelper.GetUser(username);
            return user;
        }

        private void OpenMainWindow(User user)
        {
            // Erstellen des MainWindows
            MainWindow mainWindow = new MainWindow(user);
            mainWindow.Show();
            // Schließen des LoginWindows
            this.Close();
        }
    }
}

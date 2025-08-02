using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaktionslogik für AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
        }

        private void SafeButton_Click(object sender, RoutedEventArgs e)
        {
            // Eingabefelder auslesen
            string username = UsernameTextBox.Text.Trim();
            string password1 = PasswordBox1.Password;
            string password2 = PasswordBox2.Password;
            bool adminStatus = AdminCheckBox.IsChecked == true;

            if (username == "" || password1 == "" || password1.Trim() == "" || password2 == "" || password2.Trim() == "")
            {
                // Eingabefelder sind leer
                MessageBox.Show("Bitte füllen Sie alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (password1 != password2)
            {
                // Passwörter stimmen nicht überein
                MessageBox.Show("Die Passwörter stimmen nicht überein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (UserDatabaseHelper.GetUser(username) != null)
            {
                // Benutzername bereits vorhanden
                MessageBox.Show("Der Benutzername ist bereits vergeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Nutzer in Datenbanken hinzufügen
                int userResult = UserDatabaseHelper.AddUser(username, password1, adminStatus);
                User? newUser = UserDatabaseHelper.GetUser(username);
                int settingsResult = 0;
                if (newUser != null)
                {
                    settingsResult = SettingsDatabaseHelper.AddUser(newUser.Id);
                }

                if (userResult == 1 && settingsResult == 1)
                {
                    // Schließen des AddUserWindows
                    this.Close();
                }
                else
                {
                    // Fehler beim Hinzufügen
                    MessageBox.Show("Fehler beim Hinzufügen des Benutzers! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des AddUserWindows
            this.Close();
        }
    }
}

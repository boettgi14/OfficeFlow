using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaktionslogik für EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditUserWindow"/> class with the specified user data.
        /// </summary>
        /// <remarks>The constructor initializes the window with the provided user's data, setting the
        /// username label and the admin status checkbox to reflect the user's current information.</remarks>
        /// <param name="user">The user whose data will be displayed and edited in the window. Cannot be null.</param>
        public EditUserWindow(User user)
        {
            InitializeComponent();
            // Setzen der Nutzerdaten
            UsernameLabel.Content = user.Username;
            AdminCheckBox.IsChecked = user.AdminStatus;
        }

        private void SafeButton_Click(object sender, RoutedEventArgs e)
        {
            // Alte Nutzerdaten setzen
            User? oldUser = UserDatabaseHelper.GetUser(UsernameLabel.Content.ToString());

            if (oldUser == null)
            {
                // Benutzer nicht gefunden
                MessageBox.Show("Der Benutzer konnte nicht gefunden werden! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                // Schließen des EditUserWindows
                this.Close();
            }
            else
            {
                // Benutzer gefunden
                // Alte Nutzerdaten setzen
                string oldUsername = oldUser.Username;
                bool oldAdminStatus = oldUser.AdminStatus;

                // Neue Nutzerdaten setzen
                string newUsername = UsernameTextBox.Text.Trim();
                string newPassword1 = PasswordBox1.Password;
                string newPassword2 = PasswordBox2.Password;
                bool newAdminStatus = AdminCheckBox.IsChecked == true;

                // Ergebniswerte initialisieren
                int resultPassword = 1;
                int resultUsername = 1;
                int resultAdminStatus = 1;

                if (newPassword1 != "" || newPassword1.Trim() != "" || newPassword2 != "" || newPassword2.Trim() != "")
                {
                    // Passwort soll geändert werden
                    if (newPassword1 != newPassword2)
                    {
                        // Passwörter stimmen nicht überein
                        MessageBox.Show("Die Passwörter stimmen nicht überein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        // Passwörter stimmen überein, Passwort wird geändert
                        resultPassword = UserDatabaseHelper.EditPassword(oldUsername, newPassword1);
                    }
                }

                if (newUsername != "")
                {
                    // Nutzername soll geändert werden
                    if (UserDatabaseHelper.GetUser(newUsername) != null)
                    {
                        // Benutzername bereits vorhanden
                        MessageBox.Show("Der Benutzername ist bereits vergeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        // Benutzername ist frei, Nutzername wird geändert
                        resultUsername = UserDatabaseHelper.EditUsername(oldUsername, newUsername);
                    }
                }

                if (oldAdminStatus != newAdminStatus)
                {
                    // Admin Status soll geändert werden
                    resultAdminStatus = UserDatabaseHelper.EditAdminStatus(oldUsername, newAdminStatus);
                }

                if (resultPassword == 1 && resultUsername == 1 && resultAdminStatus == 1)
                {
                    // Alle Daten wurden erfolgreich geändert
                    //Schließen des EditUserWindows
                    this.Close();
                }
                else
                {
                    // Fehler beim Speichern der Daten
                    MessageBox.Show("Fehler beim Speichern einiger Daten! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    //Schließen des EditUserWindows
                    this.Close();
                }
            }

        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des EditUserWindows
            this.Close();
        }
    }
}

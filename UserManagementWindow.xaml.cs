using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
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
    /// Interaktionslogik für UserManagementWindow.xaml
    /// </summary>
    public partial class UserManagementWindow : Window
    {
        /// <summary>
        /// Gets or sets the currently authenticated user.
        /// </summary>
        User CurrentUser { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagementWindow"/> class.
        /// </summary>
        /// <remarks>This constructor sets up the user management window by initializing its components
        /// and populating the user list display.</remarks>
        public UserManagementWindow(User user)
        {
            InitializeComponent();
            CurrentUser = user;

            // Anzeigen der Nutzerliste
            UpdateUsersListBox();

            // Setzen des Button Status
            SetButtonStatus();
        }

        /// <summary>
        /// Updates the <see cref="UsersListBox"/> to display the list of all users, including their usernames and
        /// administrative status.
        /// </summary>
        /// <remarks>This method retrieves all users from the database and populates the <see
        /// cref="UsersListBox"/> with their usernames. If a user has administrative privileges, "(Admin)" is appended
        /// to their username in the list.</remarks>
        private void UpdateUsersListBox()
        {
            // Leeren der ListBox
            UsersListBox.Items.Clear();

            List<User> users = UserDatabaseHelper.GetAllUsers();
            foreach (User user in users)
            {
                // Setzen der ListBox Items
                int index = UsersListBox.Items.Add(user.Username);

                // Hinzufügen des Admin Status
                if (user.AdminStatus)
                {
                    UsersListBox.Items[index] += " (Admin)";
                }
            }
        }

        /// <summary>
        /// Updates the enabled state of the Edit and Delete buttons based on the current selection in the user list.
        /// </summary>
        /// <remarks>If a user is selected in the <see cref="UsersListBox"/>, the Edit and Delete buttons
        /// are enabled.  Otherwise, the buttons are disabled. This method ensures that the buttons are only active when
        /// a valid user selection is present.</remarks>
        private void SetButtonStatus()
        {
            if (UsersListBox.SelectedItem != null)
            {
                // Nuter ausgewählt
                // Aktivieren der Buttons
                EditUserButton.IsEnabled = true;
                DeleteUserButton.IsEnabled = true;
            }
            else
            {
                // Kein Nutzer ausgewählt
                // Deaktivieren der Buttons
                EditUserButton.IsEnabled = false;
                DeleteUserButton.IsEnabled = false;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des UserManagementWindows
            this.Close();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des AddUserWindows
            AddUserWindow addUserWindow = new AddUserWindow();
            addUserWindow.Owner = this; // Besitzer auf UserManagementWindow setzen
            addUserWindow.ShowDialog();

            // Updaten der Nutzerliste nach Schließen des AddUserWindows
            UpdateUsersListBox();
        }

        private void UsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Überprüfen ob ein Nutzer ausgewählt wurde
            SetButtonStatus();
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            User? user = null;
            if (UsersListBox.SelectedItem.ToString() != null && UsersListBox.SelectedItem.ToString() != "")
            {
                // Löschen des Admins am Ende des Nutzernamens
                string username = UsersListBox.SelectedItem.ToString().Replace(" (Admin)", "");
                // Ausgewählten Nutzer aus der Datenbank abrufen
                user = UserDatabaseHelper.GetUser(username);
            }

            if (user == null)
            {
                // Benutzer nicht gefunden
                MessageBox.Show("Der Benutzer konnte nicht gefunden werden! Bitte versúche Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Erstellen des EdiUserWindows
                EditUserWindow editUserWindow = new EditUserWindow(user);
                editUserWindow.Owner = this; // Besitzer auf UserManagementWindow setzen
                editUserWindow.ShowDialog();

                // Updaten der Nutzerliste nach Schließen des EditUserWindows
                UpdateUsersListBox();
            }
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            User? user = null;
            if (UsersListBox.SelectedItem.ToString() != null && UsersListBox.SelectedItem.ToString() != "")
            {
                // Löschen des Admins am Ende des Nutzernamens
                string username = UsersListBox.SelectedItem.ToString().Replace(" (Admin)", "");
                // Ausgewählten Nutzer aus der Datenbank abrufen
                user = UserDatabaseHelper.GetUser(username);
            }

            // Fehlerbehandlung für Holen des Nutzers
            if (user == null)
            {
                MessageBox.Show("Fehler beim Finden des Nutzers! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verhindern vom Löschen des aktuell angemeldeten Nutzers
            if (CurrentUser.Username == user.Username)
            {
                MessageBox.Show("Sie können Ihren eigenen Nutzer nicht löschen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Löschen des ausgewählten Nutzers
            int userResult = UserDatabaseHelper.DeleteUser(user.Username);

            // Löschen der ausgewählten Nutzereinstellungen
            int settingsResult = SettingsDatabaseHelper.DeleteUser(user.Id);

            // Löschen aller Aufgaben des ausgewählten Nutzers
            int taskResult = TaskDatabaseHelper.DeleteAllTasks(user.Id);

            // Löschen aller Zeiterfassungen des Nuters
            int timeResult = TimeDatabaseHelper.DeleteAllTimes(user.Id);

            if (userResult == 1 && settingsResult == 1 && taskResult == 1 && timeResult == 1)
            {
                // Nutzer erfolgreich gelöscht
                // Updaten der Nutzerliste nach dem Löschen
                UpdateUsersListBox();
            }
            else
            {
                // Fehler beim Löschen des Nutzers oder der Einstellungen
                MessageBox.Show("Fehler beim Löschen des Nutzers! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsersListBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Überprüfen, ob die Entf Taste gedrückt wurde
            if (e.Key == Key.Delete)
            {
                // Überprüfen ob ein Nutzer ausgewählt ist
                var selectedUser = UsersListBox.SelectedItem as User;
                if (selectedUser != null)
                {
                    // Löschen des Admins am Ende des Nutzernamens
                    string username = UsersListBox.SelectedItem.ToString().Replace(" (Admin)", "");

                    // Löschen des ausgewählten Nutzers
                    int userResult = UserDatabaseHelper.DeleteUser(selectedUser.Username);

                    // Löschen der ausgewählten Nutzereinstellungen
                    int settingsResult = SettingsDatabaseHelper.DeleteUser(selectedUser.Id);

                    // Löschen aller Aufgaben des ausgewählten Nutzers
                    int taskResult = TaskDatabaseHelper.DeleteAllTasks(selectedUser.Id);

                    // Löschen aller Zeiterfassungen des Nutzers
                    int timeResult = TimeDatabaseHelper.DeleteAllTimes(selectedUser.Id);

                    // Löschen aller exportierten Aufgaben
                    OutlookHelper.DeleteAllExportedTasks();

                    if (userResult == 1 && settingsResult == 1 && taskResult == 1 && timeResult == 1)
                    {
                        // Nutzer erfolgreich gelöscht
                        // Updaten der Nutzerliste nach dem Löschen
                        UpdateUsersListBox();
                    }
                    else
                    {
                        // Fehler beim Löschen des Nutzers
                        MessageBox.Show("Fehler beim Löschen des Nutzers! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Kein Nutzer ausgewählt
                    MessageBox.Show("Bitte wählen Sie einen Nutzer aus, um ihn zu löschen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteAllUsersButton_Click(object sender, RoutedEventArgs e)
        {
            // Bestätigungsdialog anzeigen
            var result = MessageBox.Show("Sind Sie sicher, dass Sie alle Nutzer, deren Einstellungen und deren Aufgaben löschen möchten?\n" +
                "Daraufhin wird OfficeFlow geschlossen und der Standardnutzer wiederhergestellt.", "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                // Abbrechen des Löschvorgangs
                return;
            }

            // Löschen aller Nutzer
            int settingsResult = SettingsDatabaseHelper.DeleteDatabase();

            // Löschen aller Nutzereinstellungen
            int userResult = UserDatabaseHelper.DeleteDatabase();

            // Löschen aller Aufgaben
            int taskResult = TaskDatabaseHelper.DeleteDatabase();

            // Löschen aller Zeiterfassungen
            int timeResult = TimeDatabaseHelper.DeleteDatabase();

            // Löschen aller exportierten Aufgaben
            OutlookHelper.DeleteAllExportedTasks();

            if (settingsResult == 1 && userResult == 1 && taskResult == 1 && timeResult == 1)
            {
                // Nutzer und Einstellungen erfolgreich gelöscht
                // Schließen der Anwendung
                Application.Current.Shutdown();
            }
            else
            {
                // Fehler beim Löschen der Nutzer und Einstellungen
                MessageBox.Show("Fehler beim Löschen der Nutzer! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

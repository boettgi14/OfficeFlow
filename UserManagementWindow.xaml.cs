using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public UserManagementWindow()
        {
            InitializeComponent();
            /// Anzeigen der Nutzerliste
            UpdateUsersListBox();
        }

        public void UpdateUsersListBox()
        {
            // Leeren der ListBox
            UsersListBox.Items.Clear();

            List<User> users = UserDatabaseHelper.GetAllUsers();
            foreach (User user in users)
            {
                // Setzen der ListBox Items
                int index = UsersListBox.Items.Add(user.Id + " " + user.Username);

                // Hinzufügen des Admin Status
                if (user.AdminStatus)
                {
                    UsersListBox.Items[index] += " (Admin)";
                }
            }
        }

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
            // Überprüfen, ob ein Nutzer ausgewählt wurde
            SetButtonStatus();
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Bearbeiten des ausgewählten Nutzers
            string username = UsersListBox.SelectedItem.ToString().Split(" ")[1];
            User user = UserDatabaseHelper.GetUser(username);

            // Erstellen des EdiUserWindows
            EditUserWindow editUserWindow = new EditUserWindow(user);
            editUserWindow.Owner = this; // Besitzer auf UserManagementWindow setzen
            editUserWindow.ShowDialog();

            // Updaten der Nutzerliste nach Schließen des EditUserWindows
            UpdateUsersListBox();
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Löschen des ausgewählten Nutzers
            string username = UsersListBox.SelectedItem.ToString().Split(" ")[1];
            UserDatabaseHelper.DeleteUser(username);

            // Updaten der Nutzerliste nach dem Löschen
            UpdateUsersListBox();
        }
    }
}

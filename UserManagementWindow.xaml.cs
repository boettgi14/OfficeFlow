using Microsoft.Data.Sqlite;
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
    /// Interaktionslogik für UserManagementWindow.xaml
    /// </summary>
    public partial class UserManagementWindow : Window
    {
        public UserManagementWindow()
        {
            InitializeComponent();
            updateUsers();
        }

        private void updateUsers()
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Select Befehl vorbereiten
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users";

            // Select Befehl ausführen und Ergebnis lesen
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string userName = reader.GetString(1);
                string passwordHash = reader.GetString(2);
                bool isAdmin = reader.GetBoolean(3);

                // Setzen der ListBox Items
                int index = ListBox.Items.Add(id + " " + userName);

                // Hinzufügen des Admin Status
                if (isAdmin)
                {
                    ListBox.Items[index] += " (Admin)";
                }
            }

            // Schließen der Verbindung
            connection.Close();
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des UserManagementWindows
            this.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des UserManagementWindows
            this.Close();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des AddUserWindow
            AddUserWindow addUserWindow = new AddUserWindow();
            addUserWindow.Owner = this; // Besitzer auf UserManagementWindow setzen
            addUserWindow.ShowDialog();
        }
    }
}

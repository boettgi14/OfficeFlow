using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;
using System.Collections;

namespace OfficeFlow
{
    public class UserDatabaseHelper
    {
        public static int InitializeDatabase()
        {
            if (!File.Exists("users.db"))
            {
                // Datenbankdatei erstellen und Verbindung öffnen
                using var connection = new SqliteConnection("Data Source=users.db");
                connection.Open();

                // Create Befehl vorbereiten
                var createCommand = connection.CreateCommand();
                createCommand.CommandText = @"
                    CREATE TABLE users (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT NOT NULL UNIQUE,
                    password_hash TEXT NOT NULL,
                    admin_status BOOLEAN NOT NULL);";

                // Create Befehl ausführen
                int result = createCommand.ExecuteNonQuery();

                // Erstellen von Standardbenutzer
                AddUser("admin", "password", true); // Admin Benutzer
                AddUser("user", "password", false); // Normaler Benutzer

                // Schließen der Verbindung
                connection.Close();

                // Rückgabe des Ergebnisses
                return result;
            }
            return -1; // Datenbank existiert bereits
        }

        public static int AddUser(string username, string password, bool adminStatus)
        {
            // Passwort Hash generieren
            string passwordHash = PasswordHelper.HashMD5(password);

            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Insert Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO users (username, password_hash, admin_status)
                VALUES ($username, $password_hash, $admin_status);";
            insertCommand.Parameters.AddWithValue("$username", username);
            insertCommand.Parameters.AddWithValue("$password_hash", passwordHash);
            insertCommand.Parameters.AddWithValue("$admin_status", adminStatus);

            // Insert Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Schließen der Verbindung
            connection.Close();

            // Rückgabe des Ergebnisses
            return result;
        }

        public static int DeleteUser(string username)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Delete Befehl vorbereiten
            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = @"
                DELETE FROM users
                WHERE username = $username;";
            deleteCommand.Parameters.AddWithValue("$username", username);

            // Delete Befehl ausführen
            int result = deleteCommand.ExecuteNonQuery();

            // Schließen der Verbindung
            connection.Close();

            // Rückgabe des Ergebnisses
            return result;
        }

        public static int EditUsername(string oldUsername, string newUsername)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Update Befehl vorbereiten
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE users
                SET username = $newUsername
                WHERE username = $oldUsername;";
            updateCommand.Parameters.AddWithValue("$newUsername", newUsername);
            updateCommand.Parameters.AddWithValue("$oldUsername", oldUsername);

            // Update Befehl ausführen
            int result = updateCommand.ExecuteNonQuery();

            // Schließen der Verbindung
            connection.Close();

            // Rückgabe des Ergebnisses
            return result;
        }

        public static int EditPassword(string username, string newPassword)
        {
            // Passwort Hash generieren
            string passwordHash = PasswordHelper.HashMD5(newPassword);

            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Update Befehl vorbereiten
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE users
                SET password_hash = $password_hash
                WHERE username = $username;";
            updateCommand.Parameters.AddWithValue("$password_hash", passwordHash);
            updateCommand.Parameters.AddWithValue("$username", username);

            // Update Befehl ausführen
            int result = updateCommand.ExecuteNonQuery();

            // Schließen der Verbindung
            connection.Close();

            // Rückgabe des Ergebnisses
            return result;
        }

        public static int EditAdminStatus(string username, bool newAdminStatus)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Update Befehl vorbereiten
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE users
                SET admin_status = $newAdminStatus
                WHERE username = $username;";
            updateCommand.Parameters.AddWithValue("$newAdminStatus", newAdminStatus);
            updateCommand.Parameters.AddWithValue("$username", username);

            // Update Befehl ausführen
            int result = updateCommand.ExecuteNonQuery();

            // Schließen der Verbindung
            connection.Close();

            // Rückgabe des Ergebnisses
            return result;
        }

        public static User GetUser(string inputUsername)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Select Befehl vorbereiten
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT id, username, password_hash, admin_status FROM users
                WHERE username = $username;";
            command.Parameters.AddWithValue("$username", inputUsername);

            // Select Befehl ausführen
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                // Ergebnisse auslesen
                int id = reader.GetInt32(0);
                string username = reader.GetString(1);
                string passwordHash = reader.GetString(2);
                bool AdminStatus = reader.GetBoolean(3);

                // Schließen der Verbindung
                connection.Close();

                // Rückgabe des Users aus Ergebnissen
                return new User(id, username, passwordHash, AdminStatus);
            }

            // Schließen der Verbindung
            connection.Close();

            // Fehlerfall gibt null zurück
            return null;
        }

        public static List<User> GetAllUsers()
        {
            // Initalisieren der Nutzerliste
            List<User> users = new List<User>();

            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Select Befehl vorbereiten
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users;";

            // Select Befehl ausführen
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Auslesen der Ergebnisse
                int id = reader.GetInt32(0);
                string username = reader.GetString(1);
                string passwordHash = reader.GetString(2);
                bool adminStatus = reader.GetBoolean(3);

                User user = new User(id, username, passwordHash, adminStatus);
                users.Add(user);
            }

            // Schließen der Verbindung
            connection.Close();

            // Rückgabe der Nutzerliste
            return users;
        }

        public static bool VerifyLogin(string username, string password)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Select Befehl vorbereiten
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT password_hash FROM users
                WHERE username = $username;";
            command.Parameters.AddWithValue("$username", username);

            // Select Befehl ausführen
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                // Ergebnis auslesen
                string storedHash = reader.GetString(0);

                // Schließen der Verbindung
                connection.Close();

                // Rückgabe des Vergleichs des Passworts mit dem gespeicherten Hash
                return PasswordHelper.VerifyMD5(password, storedHash);
            }

            // Schließen der Verbindung
            connection.Close();

            // Fehlerfall gibt false zurück
            return false;
        }
    }
}

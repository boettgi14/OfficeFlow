using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    public class DataBaseHelper
    {
        public static void InitializeDatabase()
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
                createCommand.ExecuteNonQuery();

                // Erstellen von Standardbenutzer
                AddUser("admin", "password", true); // Admin Benutzer
                AddUser("user", "password", false); // Normaler Benutzer

                // SChließen der Verbindung
                connection.Close();
            }
        }

        public static void AddUser(string username, string password, bool adminStatus)
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
            insertCommand.ExecuteNonQuery();

            // SChließen der Verbindung
            connection.Close();
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

            // Select Befehl ausführen und Ergebnis lesen
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                string storedHash = reader.GetString(0);

                // SChließen der Verbindung
                connection.Close();

                return PasswordHelper.VerifyMD5(password, storedHash);
            }

            // SChließen der Verbindung
            connection.Close();

            // Fehlerfall gibt false zurück
            return false;
        }

        public static User GetUser(string username)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Select Befehl vorbereiten
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT id, username, password_hash, admin_status FROM users
                WHERE username = $username;";
            command.Parameters.AddWithValue("$username", username);

            // Select Befehl ausführen und Ergebnis lesen
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                int id = reader.GetInt32(0);
                string userName = reader.GetString(1);
                string passwordHash = reader.GetString(2);
                bool isAdmin = reader.GetBoolean(3);

                // Scließen der Verbindung
                connection.Close();

                return new User(id, userName, passwordHash, isAdmin);
            }

            // SChließen der Verbindung
            connection.Close();

            // Fehlerfall gibt null zurück
            return null;
        }
    }
}

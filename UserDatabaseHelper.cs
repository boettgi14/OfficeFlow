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
    /// <summary>
    /// Provides helper methods for managing user data in a SQLite database.
    /// </summary>
    /// <remarks>The <see cref="UserDatabaseHelper"/> class includes methods for initializing the database, 
    /// adding, deleting, and updating user records, as well as retrieving user information.  It operates on a SQLite
    /// database file named "users.db" and assumes the presence of a "users" table  with the following schema: <code>
    /// CREATE TABLE users (     id INTEGER PRIMARY KEY AUTOINCREMENT,     username TEXT NOT NULL UNIQUE,    
    /// password_hash TEXT NOT NULL,     admin_status BOOLEAN NOT NULL ); </code> Note that the class uses MD5 hashing
    /// for password storage, which is not recommended for secure  applications. For production systems, consider using
    /// a more secure hashing algorithm such as  bcrypt or Argon2.</remarks>
    public class UserDatabaseHelper
    {
        /// <summary>
        /// The file path to the database used for storing user data.
        /// </summary>
        /// <remarks>This field is a constant and represents the default location of the database file. It
        /// is used internally by the application to access user-related data.</remarks>
        private static readonly string _dbFilePath = "users.db";
        /// <summary>
        /// Represents the connection string used to connect to the database.
        /// </summary>
        /// <remarks>The connection string is constructed using the database file path. Ensure that the
        /// <c>_dbFilePath</c> variable is correctly set to a valid file path before using this connection
        /// string.</remarks>
        private static readonly string _connectionString = $"Data Source={_dbFilePath};";

        /// <summary>
        /// Initializes the database by creating a new SQLite database file and a "users" table if it does not already
        /// exist.
        /// </summary>
        /// <remarks>If the database file "users.db" does not exist, this method creates it, sets up the
        /// "users" table,  and adds default user entries. If the database file already exists, the method does nothing
        /// and returns -1.</remarks>
        /// <returns>The number of rows affected by the creation of the "users" table, or -1 if the database already exists.</returns>
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

        /// <summary>
        /// Adds a new user to the database with the specified username, password, and admin status.
        /// </summary>
        /// <remarks>The password is hashed using MD5 before being stored in the database. Note that MD5
        /// is considered cryptographically weak and should not be used for secure password storage in production
        /// systems.</remarks>
        /// <param name="username">The username of the new user. Must not be null or empty.</param>
        /// <param name="password">The password for the new user. Must not be null or empty.</param>
        /// <param name="adminStatus">A value indicating whether the new user should have administrative privileges.</param>
        /// <returns>The number of rows affected by the operation. Typically, this will be 1 if the user was successfully added.</returns>
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

        /// <summary>
        /// Deletes a user from the database based on the specified username.
        /// </summary>
        /// <remarks>This method connects to a SQLite database named "users.db" and attempts to delete a
        /// user with the specified username from the "users" table. Ensure that the database and table exist and that
        /// the application has the necessary permissions to access and modify the database.</remarks>
        /// <param name="username">The username of the user to be deleted. This value cannot be null or empty.</param>
        /// <returns>The number of rows affected by the delete operation. Returns 0 if no user with the specified username
        /// exists.</returns>
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

        /// <summary>
        /// Updates a user's username in the database.
        /// </summary>
        /// <remarks>This method updates the username of a user in the database. Ensure that the database
        /// connection string is correctly configured          and that the database schema includes a table named
        /// "users" with a column "username".</remarks>
        /// <param name="oldUsername">The current username of the user to be updated. This value must match an existing username in the database.</param>
        /// <param name="newUsername">The new username to assign to the user. This value must not be null or empty.</param>
        /// <returns>The number of rows affected by the update operation. Returns 0 if no rows were updated, indicating that the
        /// specified <paramref name="oldUsername"/> was not found.</returns>
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

        /// <summary>
        /// Updates the password for the specified user in the database.
        /// </summary>
        /// <remarks>The password is hashed using MD5 before being stored in the database. Note that MD5
        /// is not recommended for secure password storage. Ensure the database connection string is properly configured
        /// to access the "users.db" database.</remarks>
        /// <param name="username">The username of the user whose password is to be updated. Cannot be null or empty.</param>
        /// <param name="password">The new password to set for the user. Cannot be null or empty.</param>
        /// <returns>The number of rows affected by the update operation. Returns 0 if no user with the specified username
        /// exists.</returns>
        public static int EditPassword(string username, string password)
        {
            // Passwort Hash generieren
            string passwordHash = PasswordHelper.HashMD5(password);

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

        /// <summary>
        /// Updates the administrative status of a user in the database.
        /// </summary>
        /// <remarks>This method updates the <c>admin_status</c> field in the <c>users</c> table for the
        /// specified user. Ensure that the database connection string is correctly configured and that the <c>users</c>
        /// table exists with the appropriate schema.</remarks>
        /// <param name="username">The username of the user whose administrative status is to be updated. Cannot be null or empty.</param>
        /// <param name="adminStatus">A boolean value indicating the new administrative status. <see langword="true"/> to grant admin privileges;
        /// otherwise, <see langword="false"/>.</param>
        /// <returns>The number of rows affected by the update operation. Returns 0 if no matching user was found.</returns>
        public static int EditAdminStatus(string username, bool adminStatus)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=users.db");
            connection.Open();

            // Update Befehl vorbereiten
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE users
                SET admin_status = $adminStatus
                WHERE username = $username;";
            updateCommand.Parameters.AddWithValue("$adminStatus", adminStatus);
            updateCommand.Parameters.AddWithValue("$username", username);

            // Update Befehl ausführen
            int result = updateCommand.ExecuteNonQuery();

            // Schließen der Verbindung
            connection.Close();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Retrieves a user from the database based on the specified username.
        /// </summary>
        /// <remarks>This method queries a SQLite database to retrieve user information, including the
        /// user's ID,  username, password hash, and admin status. Ensure that the database connection string and schema
        /// are properly configured before calling this method.</remarks>
        /// <param name="inputUsername">The username of the user to retrieve. This value cannot be null or empty.</param>
        /// <returns>A <see cref="User"/> object representing the user with the specified username,  or <see langword="null"/> if
        /// no user with the given username exists in the database.</returns>
        public static User? GetUser(string inputUsername)
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

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <remarks>This method queries the database for all user records and returns them as a list of
        /// <see cref="User"/> objects. Each user record includes the user's ID, username, password hash, and admin
        /// status.</remarks>
        /// <returns>A list of <see cref="User"/> objects representing all users in the database.  The list will be empty if no
        /// users are found.</returns>
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

        /// <summary>
        /// Verifies the provided username and password against stored credentials.
        /// </summary>
        /// <remarks>This method checks the provided credentials against a SQLite database containing user
        /// information. The password is verified using an MD5 hash comparison. Ensure that the database connection
        /// string and schema are properly configured before calling this method.</remarks>
        /// <param name="username">The username to authenticate. Cannot be null or empty.</param>
        /// <param name="password">The password to authenticate. Cannot be null or empty.</param>
        /// <returns><see langword="true"/> if the username exists and the provided password matches the stored password hash;
        /// otherwise, <see langword="false"/>.</returns>
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

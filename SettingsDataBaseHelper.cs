using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    /// <summary>
    /// Provides utility methods for managing the application's settings database.
    /// </summary>
    /// <remarks>The <see cref="SettingsDatabaseHelper"/> class includes methods for initializing, deleting, 
    /// and interacting with the settings database. It is designed to handle common operations such as  adding users,
    /// retrieving user preferences, and updating settings. This class assumes that the  database is stored locally and
    /// uses SQLite as the underlying database engine.</remarks>
    class SettingsDatabaseHelper
    {
        /// <summary>
        /// The file path to the database used for storing application settings.
        /// </summary>
        /// <remarks>This field is a constant and represents the default location of the settings
        /// database. It is used internally by the application and is not intended for external modification.</remarks>
        private static readonly string _dbFilePath = "settings.db";
        /// <summary>
        /// Represents the connection string used to connect to the database.
        /// </summary>
        /// <remarks>The connection string is constructed using the database file path. This field is
        /// read-only and cannot be modified at runtime.</remarks>
        private static readonly string _connectionString = $"Data Source={_dbFilePath};";

        /// <summary>
        /// Initializes the database by creating the required tables if the database file does not already exist.
        /// </summary>
        /// <remarks>This method checks for the existence of the database file at the specified path. If
        /// the file does not exist, it creates the database file and initializes it with the necessary schema. If the
        /// database file already exists, the method does nothing and returns -1.</remarks>
        /// <returns>An integer indicating the result of the database initialization. Returns the number of rows affected by the
        /// schema creation if the database was successfully initialized, or -1 if the database already exists.</returns>
        public static int InitializeDatabase()
        {
            if (!File.Exists(_dbFilePath))
            {
                // Datenbankdatei erstellen und Verbindung öffnen
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                // Create Befehl vorbereiten
                var createCommand = connection.CreateCommand();
                createCommand.CommandText = @"
                    CREATE TABLE settings (
                    user_id INTEGER PRIMARY KEY,
                    order_tasks_by STRING NOT NULL DEFAULT 'id',
                    export_tasks_to_outlook BOOLEAN NOT NULL DEFAULT false,
                    automatic_time_tracking BOOLEAN NOT NULL DEFAULT false);";

                // Create Befehl ausführen
                int result = createCommand.ExecuteNonQuery();

                // Erstellen von Standardbenutzer
                AddUser(1);

                // Rückgabe des Ergebnisses
                return result;
            }
            return -1; // Datenbank existiert bereits
        }

        /// <summary>
        /// Deletes the database file if it exists.
        /// </summary>
        /// <remarks>This method ensures that all resources are released by triggering garbage collection 
        /// and clearing all SQLite connection pools before attempting to delete the database file.  If the file does
        /// not exist, no action is taken.</remarks>
        /// <returns>An integer indicating the result of the operation:  <see langword="1"/> if the database file was
        /// successfully deleted;  <see langword="0"/> if the file does not exist.</returns>
        public static int DeleteDatabase()
        {
            // Garbage Collection auslösen um sicherzustellen dass alle Ressourcen freigegeben sind
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Schließen aller offenen Verbindungen und Pools
            SqliteConnection.ClearAllPools();

            if (File.Exists(_dbFilePath))
            {
                // Löschen der Datenbankdatei
                File.Delete(_dbFilePath);
                return 1; // Erfolgreich gelöscht
            }
            return 0; // Datei existiert nicht
        }

        /// <summary>
        /// Adds a new user to the database with the specified user ID.
        /// </summary>
        /// <remarks>This method establishes a connection to the database, inserts a new record into the
        /// "settings" table with the provided user ID,  and then closes the connection. Ensure that the database is
        /// properly configured and accessible before calling this method.</remarks>
        /// <param name="userId">The unique identifier of the user to be added. Must be a valid integer.</param>
        /// <returns>The number of rows affected by the operation. Typically, this will be 1 if the user was successfully added.</returns>
        public static int AddUser(int userId)
        {
            // Verbindung zur Datenbank herstellen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Insert Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO settings (user_id) VALUES ($userId);";
            insertCommand.Parameters.AddWithValue("$userId", userId);

            // Insert Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Deletes a user and their associated settings from the database.
        /// </summary>
        /// <remarks>This method removes all settings associated with the specified user from the
        /// database. Ensure that the provided <paramref name="userId"/> corresponds to an existing user.</remarks>
        /// <param name="userId">The unique identifier of the user to delete. Must be a valid user ID.</param>
        /// <returns>The number of rows affected by the delete operation. Returns 0 if no matching user was found.</returns>
        public static int DeleteUser(int userId)
        {
            // Verbindung zur Datenbank herstellen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Delete Befehl vorbereiten
            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = @"
                DELETE FROM settings WHERE user_id = $userId;";
            deleteCommand.Parameters.AddWithValue("$userId", userId);

            // Delete Befehl ausführen
            int result = deleteCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Resets a user by deleting and re-adding the user with the specified ID.
        /// </summary>
        /// <remarks>This method performs a reset operation by first deleting the user and then re-adding
        /// them.  Ensure that the user ID provided is valid and that the underlying operations for deleting  and adding
        /// a user are implemented correctly.</remarks>
        /// <param name="userId">The unique identifier of the user to reset.</param>
        /// <returns>An integer indicating the result of the operation.  Returns <see langword="1"/> if the user was successfully
        /// reset; otherwise, <see langword="0"/>.</returns>
        public static int ResetUser(int userId)
        {
            // Löschen des Nutzers
            int deleteResult = DeleteUser(userId);
            // Neues Hinzufügen des Nutzers
            int addResult = AddUser(userId);

            if (deleteResult == 1 && addResult == 1)
            {
                // Rückgabe bei Erfolg
                return 1;
            }
            else
            {
                // Rückgabe bei Fehler
                return 0;
            }
        }

        /// <summary>
        /// Retrieves the order preference for tasks associated with the specified user.
        /// </summary>
        /// <remarks>This method queries the database to retrieve the task order preference stored in the
        /// settings table  for the given user. Ensure that the database connection string is properly configured before
        /// calling this method.</remarks>
        /// <param name="userId">The unique identifier of the user whose task order preference is being retrieved.</param>
        /// <returns>A string representing the user's task order preference.  Returns an empty string if no preference is found
        /// for the specified user.</returns>
        public static string GetOrderTasksBy(int userId)
        {
            // Verbindung zur Datenbank herstellen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Select Befehl vorbereiten
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = @"
                SELECT order_tasks_by FROM settings WHERE user_id = $userId;";
            selectCommand.Parameters.AddWithValue("$userId", userId);

            // Select Befehl ausführen und Ergebnis abrufen
            var result = selectCommand.ExecuteScalar();

            // Rückgabe des Ergebnisses oder Standardwert
            return (string)(result != null ? result : "");
        }

        /// <summary>
        /// Updates the task ordering preference for a specific user in the database.
        /// </summary>
        /// <remarks>This method updates the `order_tasks_by` column in the `settings` table for the
        /// specified user.  Ensure that the provided <paramref name="userId"/> exists in the database and that
        /// <paramref name="orderTasksBy"/>  contains a valid ordering criterion.</remarks>
        /// <param name="userId">The unique identifier of the user whose task ordering preference is being updated.</param>
        /// <param name="orderTasksBy">The new task ordering preference to be set. This value is typically a string representing the desired order,
        /// such as "date", "priority", or another valid ordering criterion.</param>
        /// <returns>The number of rows affected by the update operation. A return value of 1 indicates that the update was
        /// successful,  while 0 indicates that no matching user was found.</returns>
        public static int SetOrderTasksBy(int userId, string orderTasksBy)
        {
            // Verbindung zur Datenbank herstellen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Insert Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                UPDATE settings SET order_tasks_by = $orderTasksBy
                WHERE user_id = $userId;";
            insertCommand.Parameters.AddWithValue("$userId", userId);
            insertCommand.Parameters.AddWithValue("$orderTasksBy", orderTasksBy);

            // Insert Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Determines whether the user has enabled the option to export tasks to Outlook.
        /// </summary>
        /// <remarks>This method retrieves the user's preference for exporting tasks to Outlook from the
        /// database. If no preference is found for the specified user, the method returns <see
        /// langword="false"/>.</remarks>
        /// <param name="userId">The unique identifier of the user whose settings are being queried.</param>
        /// <returns><see langword="true"/> if the user has enabled the export to Outlook option;  otherwise, <see
        /// langword="false"/>.</returns>
        public static bool GetExportTasksToOutlook(int userId)
        {
            // Verbindung zur Datenbank herstellen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Select Befehl vorbereiten
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = @"
                SELECT export_tasks_to_outlook FROM settings WHERE user_id = $userId;";
            selectCommand.Parameters.AddWithValue("$userId", userId);

            // Select Befehl ausführen und Ergebnis abrufen
            var result = selectCommand.ExecuteScalar();

            // Rückgabe des Ergebnisses oder Standardwert
            return result is long longValue && longValue != 0;
        }

        /// <summary>
        /// Updates the user's preference for exporting tasks to Outlook in the database.
        /// </summary>
        /// <remarks>This method updates the `export_tasks_to_outlook` field in the `settings` table for
        /// the specified user. Ensure that the database connection string is properly configured before calling this
        /// method.</remarks>
        /// <param name="userId">The unique identifier of the user whose preference is being updated.</param>
        /// <param name="exportToOutlook">A boolean value indicating whether the user wants to enable (<see langword="true"/>) or disable (<see
        /// langword="false"/>) task export to Outlook.</param>
        /// <returns>The number of rows affected by the update operation. A value of 1 indicates that the update was successful,
        /// while 0 indicates that no matching user was found.</returns>
        public static int SetExportTasksToOutlook(int userId, bool exportToOutlook)
        {
            // Verbindung zur Datenbank herstellen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Insert Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                UPDATE settings SET export_tasks_to_outlook = $exportToOutlook
                WHERE user_id = $userId;";
            insertCommand.Parameters.AddWithValue("$userId", userId);
            insertCommand.Parameters.AddWithValue("$exportToOutlook", exportToOutlook);

            // Insert Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Retrieves the automatic time tracking setting for the specified user.
        /// </summary>
        /// <remarks>This method queries the database to determine whether the automatic time tracking 
        /// feature is enabled for the specified user. If the setting is not found, the method  returns <see
        /// langword="false"/> as the default value.</remarks>
        /// <param name="userId">The unique identifier of the user whose setting is being retrieved.</param>
        /// <returns><see langword="true"/> if automatic time tracking is enabled for the user;  otherwise, <see
        /// langword="false"/>.</returns>
        public static bool GetAutomaticTimeTracking(int userId)
        {
            // Verbindung zur Datenbank herstellen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Select Befehl vorbereiten
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = @"
                SELECT automatic_time_tracking FROM settings WHERE user_id = $userId;";
            selectCommand.Parameters.AddWithValue("$userId", userId);

            // Select Befehl ausführen und Ergebnis abrufen
            var result = selectCommand.ExecuteScalar();

            // Rückgabe des Ergebnisses oder Standardwert
            return result is long longValue && longValue != 0;
        }

        /// <summary>
        /// Updates the automatic time tracking setting for a specific user in the database.
        /// </summary>
        /// <remarks>This method updates the "automatic_time_tracking" setting in the "settings" table for
        /// the specified user. Ensure that the database connection string is properly configured before calling this
        /// method.</remarks>
        /// <param name="userId">The unique identifier of the user whose setting is being updated. Must be a valid user ID.</param>
        /// <param name="automaticTimeTracking">A boolean value indicating whether automatic time tracking should be enabled. <see langword="true"/> to
        /// enable automatic time tracking; otherwise, <see langword="false"/>.</param>
        /// <returns>The number of rows affected by the update operation. Returns 0 if no rows were updated,  which may indicate
        /// that the specified user ID does not exist.</returns>
        public static int SetAutomaticTimeTracking(int userId, bool automaticTimeTracking)
        {
            // Verbindung zur Datenbank herstellen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Insert Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                UPDATE settings SET automatic_time_tracking = $automaticTimeTracking
                WHERE user_id = $userId;";
            insertCommand.Parameters.AddWithValue("$userId", userId);
            insertCommand.Parameters.AddWithValue("$automaticTimeTracking", automaticTimeTracking);

            // Insert Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }
    }
}

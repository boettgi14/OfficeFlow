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
    /// Provides helper methods for managing a time-tracking database.
    /// </summary>
    /// <remarks>This class includes methods for initializing the database, adding and deleting time entries, 
    /// and retrieving time-tracking data. It is designed to work with a SQLite database and assumes  a specific schema
    /// for the `times` table. All methods are static and intended for internal use  within the application.</remarks>
    internal class TimeDatabaseHelper
    {
        /// <summary>
        /// The file path to the database used for storing time tracker data.
        /// </summary>
        /// <remarks>This is a static, read-only field that specifies the location of the database file.
        /// The value is set to "timeTrackers.db" by default.</remarks>
        private static readonly string _dbFilePath = "times.db";
        /// <summary>
        /// Represents the connection string used to connect to the database.
        /// </summary>
        /// <remarks>This connection string is constructed using the database file path and is intended
        /// for internal use only.</remarks>
        private static readonly string _connectionString = $"Data Source={_dbFilePath};";

        /// <summary>
        /// Initializes the database by creating the required table if the database file does not already exist.
        /// </summary>
        /// <remarks>This method checks for the existence of the database file at the specified path. If
        /// the file does not exist, it creates the database file and a table named <c>times</c> with the necessary
        /// schema. The table includes columns for tracking user activity, such as start and end times, total duration,
        /// and pause duration.</remarks>
        /// <returns>An integer indicating the result of the operation. Returns the number of rows affected by the table creation
        /// command if the database is successfully initialized. Returns <c>-1</c> if the database file already exists.</returns>
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
                    CREATE TABLE times (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    start DATE NOT NULL,
                    end DATE NOT NULL,
                    total_duration INTEGER NOT NULL,
                    pause_duration INTEGER NOT NULL);";

                // Create Befehl ausführen
                int result = createCommand.ExecuteNonQuery();

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
        /// Adds a new time entry to the database for the specified user.
        /// </summary>
        /// <remarks>This method inserts a new record into the `times` table in the database. The caller
        /// is responsible for ensuring that the provided parameters are valid and consistent (e.g., <paramref
        /// name="endTime"/> is later than  <paramref name="startTime"/>).</remarks>
        /// <param name="userId">The unique identifier of the user associated with the time entry.</param>
        /// <param name="startTime">The start time of the time entry.</param>
        /// <param name="endTime">The end time of the time entry.</param>
        /// <param name="totalDuration">The total duration of the time entry, in minutes.</param>
        /// <param name="pauseDuration">The duration of any pauses during the time entry, in minutes.</param>
        /// <returns>The number of rows affected by the database operation. Typically, this will be 1 if the insertion is
        /// successful.</returns>
        public static int AddTime(int userId, DateTime startTime, DateTime endTime, int totalDuration, int pauseDuration)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Insert Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO times (user_id, start, end, total_duration, pause_duration)
                VALUES ($userId, $startTime, $endTime, $totalDuration, $pauseDuration);";

            // Parameter hinzufügen
            insertCommand.Parameters.AddWithValue("$userId", userId);
            insertCommand.Parameters.AddWithValue("$startTime", startTime);
            insertCommand.Parameters.AddWithValue("$endTime", endTime);
            insertCommand.Parameters.AddWithValue("$totalDuration", totalDuration);
            insertCommand.Parameters.AddWithValue("$pauseDuration", pauseDuration);

            // Insert Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Deletes a record from the "times" table with the specified identifier.
        /// </summary>
        /// <remarks>This method establishes a connection to the database, executes a delete command, and
        /// returns the result. Ensure that the database connection string is properly configured before calling this
        /// method.</remarks>
        /// <param name="id">The unique identifier of the record to delete. Must correspond to an existing record in the "times" table.</param>
        /// <returns>The number of rows affected by the delete operation. Returns 0 if no record with the specified identifier
        /// exists.</returns>
        public static int DeleteTime(int id)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Select Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                DELETE FROM times 
                WHERE id=$id;";

            // Parameter hinzufügen
            insertCommand.Parameters.AddWithValue("$id", id);

            // Select Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        public static int DeleteAllTimes(int userId)
        {
            int result = 0;
            // Holen aller Zeiterfassungen für den Nutzer
            List<Time> times = GetAllTimes(userId);
            foreach (Time time in times)
            {
                // Löschen der Zeiterfassung aus der Datenbank und Überprüfen des Ergebnisses
                result = DeleteTime(time.Id) == 1 ? 1 : 0;
            }
            // Rückgabe des Ergebnisses
            return result;
        }

        public static List<Time> GetAllTimes(int inputUserId)
        {
            // Initalisieren der Aufgabenliste
            List<Time> times = new List<Time>();

            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Select Befehl vorbereiten
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM tasks
                WHERE user_id = $inputUserId;";

            // Parameter hinzufügen
            command.Parameters.AddWithValue("$inputUserId", inputUserId);

            // Select Befehl ausführen
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Auslesen der Ergebnisse
                int id = reader.GetInt32(0);
                int userId = reader.GetInt32(1);
                DateTime start = reader.GetDateTime(2);
                DateTime end = reader.GetDateTime(3);
                TimeSpan totalDuration = TimeSpan.FromSeconds(reader.GetInt32(4));
                TimeSpan pauseDuration = TimeSpan.FromSeconds(reader.GetInt32(5));

                Time time = new Time(id, start, end, totalDuration, pauseDuration);
                times.Add(time);
            }

            // Rückgabe der Nutzerliste
            return times;
        }
    }
}

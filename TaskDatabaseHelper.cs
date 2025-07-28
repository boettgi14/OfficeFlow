using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OfficeFlow
{
    public class TaskDatabaseHelper
    {
        public static int InitializeDatabase()
        {
            if (!File.Exists("tasks.db"))
            {
                // Datenbankdatei erstellen und Verbindung öffnen
                using var connection = new SqliteConnection("Data Source=tasks.db");
                connection.Open();

                // Create Befehl vorbereiten
                var createCommand = connection.CreateCommand();
                createCommand.CommandText = @"
                    CREATE TABLE tasks (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    is_completed BOOLEAN NOT NULL,
                    description TEXT,
                    due_date DATE);";

                // Create Befehl ausführen
                int result = createCommand.ExecuteNonQuery();

                // Schließen der Verbindung
                connection.Close();

                // Rückgabe des Ergebnisses
                return result;
            }
            return -1; // Datenbank existiert bereits
        }

        public static int AddTask(string name, string? description, DateTime? dueDate)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=tasks.db");
            connection.Open();

            // Insert Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO tasks (name, is_completed, description, due_date)
                VALUES ($name, $isCompleted, $description, $dueDate);";

            // Parameter hinzufügen
            insertCommand.Parameters.AddWithValue("$name", name);
            insertCommand.Parameters.AddWithValue("$isCompleted", false);
            insertCommand.Parameters.AddWithValue("$description", description ?? (object)DBNull.Value);
            insertCommand.Parameters.AddWithValue("$dueDate", dueDate.HasValue ? (object)dueDate.Value : DBNull.Value);

            // Insert Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Schließen der Verbindung
            connection.Close();

            // Rückgabe des Ergebnisses
            return result;
        }

        public static Task GetTask(int inputId)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection("Data Source=tasks.db");
            connection.Open();

            // Select Befehl vorbereiten
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = @"
                SELECT id, name, is_completed, description, due_date
                FROM tasks
                WHERE id = $id;";

            // Parameter hinzufügen
            selectCommand.Parameters.AddWithValue("$id", inputId);

            // Select Befehl ausführen
            using var reader = selectCommand.ExecuteReader();
            if (reader.Read())
            {
                // Ergebnisse auslesen
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                bool isCompleted = reader.GetBoolean(2);
                string? description = reader.IsDBNull(3) ? null : reader.GetString(3);
                DateTime? dueDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);

                // Schließen der Verbindung
                connection.Close();

                // Rückgabe des Tasks aus Ergebnissen
                return new Task(id, name, isCompleted, description, dueDate);
            }

            // Schließen der Verbindung
            connection.Close();

            // Fehlerfall gibt null zurück
            return null;
        }
    }
}

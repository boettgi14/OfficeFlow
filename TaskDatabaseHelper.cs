using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace OfficeFlow
{
    /// <summary>
    /// Provides helper methods for managing tasks in a SQLite database.
    /// </summary>
    /// <remarks>The <see cref="TaskDatabaseHelper"/> class includes methods for initializing the database, 
    /// adding, updating, deleting, and retrieving tasks. It operates on a SQLite database file  with a predefined
    /// schema for storing task information. This class is designed to simplify  task management operations and ensure
    /// proper interaction with the database. <para> The database schema includes the following fields in the "tasks"
    /// table: <list type="bullet"> <item><description><c>id</c>: An auto-incrementing primary key.</description></item>
    /// <item><description><c>name</c>: A non-null text field for the task name.</description></item>
    /// <item><description><c>is_completed</c>: A non-null boolean field indicating whether the task is
    /// completed.</description></item> <item><description><c>description</c>: An optional text field for additional
    /// task details.</description></item> <item><description><c>due_date</c>: An optional date field for the task's due
    /// date.</description></item> </list> </para> <para> This class uses a static connection string and database file
    /// path, which are configured internally.  Ensure that the database file is accessible and the schema is correctly
    /// set up before using the methods. </para></remarks>
    public class TaskDatabaseHelper
    {
        /// <summary>
        /// The file path to the database used for storing tasks.
        /// </summary>
        /// <remarks>This field is a static, read-only string that specifies the location of the database
        /// file. It is intended for internal use only and cannot be modified at runtime.</remarks>
        private static readonly string _dbFilePath = "tasks.db";
        /// <summary>
        /// Represents the connection string used to connect to the database.
        /// </summary>
        /// <remarks>The connection string is constructed using the database file path. Ensure that the
        /// <c>_dbFilePath</c> variable is correctly set to a valid file path before using this connection
        /// string.</remarks>
        private static readonly string _connectionString = $"Data Source={_dbFilePath};";

        /// <summary>
        /// Initializes the database by creating the required schema if the database file does not already exist.
        /// </summary>
        /// <remarks>This method checks for the existence of the database file at the specified path. If
        /// the file does not exist, it creates a new SQLite database and defines the schema for a "tasks" table. The
        /// table includes columns for task details such as ID, user ID, name, completion status, internal status,
        /// description, and due date.</remarks>
        /// <returns>The number of rows affected by the schema creation command if the database is initialized successfully;
        /// otherwise, -1 if the database file already exists.</returns>
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
                    CREATE TABLE tasks (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    name TEXT NOT NULL,
                    is_completed BOOLEAN NOT NULL,
                    description TEXT,
                    due_date DATE);";

                // Create Befehl ausführen
                int result = createCommand.ExecuteNonQuery();

                // Rückgabe des Ergebnisses
                return result;
            }
            return -1; // Datenbank existiert bereits
        }

        /// <summary>
        /// Adds a new task to the database for the specified user.
        /// </summary>
        /// <remarks>The task is initially marked as incomplete and is flagged as internal. Ensure that
        /// the database connection string is properly configured before calling this method.</remarks>
        /// <param name="userId">The ID of the user to whom the task belongs. Must be a valid user ID.</param>
        /// <param name="name">The name of the task. Cannot be null or empty.</param>
        /// <param name="description">An optional description of the task. If null, no description will be stored.</param>
        /// <param name="dueDate">An optional due date for the task. If null, no due date will be stored.</param>
        /// <returns>The number of rows affected by the operation. Typically, this will be 1 if the task is successfully added.</returns>
        public static int AddTask(int userId, string name, string? description, DateOnly? dueDate)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Insert Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO tasks (user_id, name, is_completed, description, due_date)
                VALUES ($userId, $name, $isCompleted, $description, $dueDate);";

            // Parameter hinzufügen
            insertCommand.Parameters.AddWithValue("$userId", userId);
            insertCommand.Parameters.AddWithValue("$name", name);
            insertCommand.Parameters.AddWithValue("$isCompleted", false);
            insertCommand.Parameters.AddWithValue("$description", description ?? (object)DBNull.Value);
            insertCommand.Parameters.AddWithValue("$dueDate", dueDate.HasValue ? (object)dueDate.Value : DBNull.Value);

            // Insert Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Deletes a task from the database with the specified identifier.
        /// </summary>
        /// <remarks>This method connects to a SQLite database and removes the task with
        /// the specified <paramref name="id"/> from the "tasks" table. Ensure that the database is accessible and the
        /// "tasks" table exists before calling this method.</remarks>
        /// <param name="id">The unique identifier of the task to delete. Must correspond to an existing task in the database.</param>
        /// <returns>The number of rows affected by the delete operation. Returns 0 if no task with the specified identifier
        /// exists.</returns>
        public static int DeleteTask(int id)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Select Befehl vorbereiten
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                DELETE FROM tasks 
                WHERE id=$id;";

            // Parameter hinzufügen
            insertCommand.Parameters.AddWithValue("$id", id);

            // Select Befehl ausführen
            int result = insertCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        public static int DeleteAllTasks(int userId)
        {
            int result = 0;
            // Holen aller Aufgaben für den Nutzer
            List<Task> tasks = GetAllTasks(userId);
            foreach (Task task in tasks)
            {
                // Löschen der Aufgabe aus der Datenbank und Überprüfen des Ergebnisses
                result = DeleteTask(task.Id) == 1 ? 1 : 0;
            }
            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Updates the name of a task in the database with the specified ID.
        /// </summary>
        /// <remarks>This method updates the name of a task in a SQLite database. Ensure that the database
        /// file exists and is accessible. The method uses a parameterized query to prevent SQL
        /// injection.</remarks>
        /// <param name="id">The unique identifier of the task to update. Must correspond to an existing task in the database.</param>
        /// <param name="name">The new name to assign to the task. Cannot be null or empty.</param>
        /// <returns>The number of rows affected by the update operation. Returns 0 if no task with the specified ID exists.</returns>
        public static int EditName(int id, string name)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Update Befehl vorbereiten
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE tasks 
                SET name = $name
                WHERE id = $id;";

            // Parameter hinzufügen
            updateCommand.Parameters.AddWithValue("$id", id);
            updateCommand.Parameters.AddWithValue("$name", name);

            // Update Befehl ausführen
            int result = updateCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Updates the completion status of a task in the database.
        /// </summary>
        /// <remarks>This method updates the is_completed field of a task in the tasks table.  Ensure that
        /// the database connection string and schema are correctly configured before calling this method.</remarks>
        /// <param name="id">The unique identifier of the task to update.</param>
        /// <param name="isCompleted">A boolean value indicating the new completion status of the task.  true sets the task as completed; false
        /// sets it as not completed.</param>
        /// <returns>The number of rows affected by the update operation. Returns 0 if no task with the specified id exists.</returns>
        public static int EditIsCompleted(int id, bool isCompleted)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Update Befehl vorbereiten
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE tasks 
                SET is_completed = $isCompleted
                WHERE id = $id;";

            // Parameter hinzufügen
            updateCommand.Parameters.AddWithValue("$id", id);
            updateCommand.Parameters.AddWithValue("$isCompleted", isCompleted);

            // Update Befehl ausführen
            int result = updateCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Updates the description of a task in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the task to update.</param>
        /// <param name="description">The new description for the task. If <see langword="null"/>, the description will be set to <see
        /// langword="null"/> in the database.</param>
        /// <returns>The number of rows affected by the update operation. Returns 0 if no task with the specified <paramref
        /// name="id"/> exists.</returns>
        public static int EditDescription(int id, string? description)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Update Befehl vorbereiten
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE tasks 
                SET description = $description
                WHERE id = $id;";

            // Parameter hinzufügen
            updateCommand.Parameters.AddWithValue("$id", id);
            updateCommand.Parameters.AddWithValue("$description", description ?? (object)DBNull.Value);

            // Update Befehl ausführen
            int result = updateCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Updates the due date of a task in the database.
        /// </summary>
        /// <remarks>This method updates the due date of a task in the "tasks" table of the database. If
        /// the specified <paramref name="id"/> does not exist, no rows will be updated, and the method will return
        /// 0.</remarks>
        /// <param name="id">The unique identifier of the task to update.</param>
        /// <param name="dueDate">The new due date for the task. If <see langword="null"/>, the due date will be cleared.</param>
        /// <returns>The number of rows affected by the update operation. Returns 0 if no task with the specified <paramref
        /// name="id"/> exists.</returns>
        public static int EditDueDate(int id, DateOnly? dueDate)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Update Befehl vorbereiten
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE tasks 
                SET due_Date = $dueDate
                WHERE id = $id;";

            // Parameter hinzufügen
            updateCommand.Parameters.AddWithValue("$id", id);
            updateCommand.Parameters.AddWithValue("$dueDate", dueDate.HasValue ? (object)dueDate.Value : DBNull.Value);

            // Update Befehl ausführen
            int result = updateCommand.ExecuteNonQuery();

            // Rückgabe des Ergebnisses
            return result;
        }

        /// <summary>
        /// Retrieves a task from the database by its unique identifier.
        /// </summary>
        /// <remarks>This method queries the database for a task with the specified identifier.  If a
        /// matching task is found, it is returned as a <see cref="Task"/> object.  If no matching task is found, the
        /// method returns <see langword="null"/>.</remarks>
        /// <param name="inputId">The unique identifier of the task to retrieve.</param>
        /// <returns>A <see cref="Task"/> object representing the task with the specified identifier,  or <see langword="null"/>
        /// if no task with the given identifier exists.</returns>
        public static Task? GetTask(int inputId)
        {
            // Datenbankverbindung öffnen
            using var connection = new SqliteConnection(_connectionString);
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
                int userId = reader.GetInt32(1);
                string name = reader.GetString(2);
                bool isCompleted = reader.GetBoolean(3);
                string? description = reader.IsDBNull(4) ? null : reader.GetString(4);
                DateOnly? dueDate = reader.IsDBNull(5) ? null : DateOnly.FromDateTime(reader.GetDateTime(5));

                // Schließen der Verbindung
                connection.Close();

                // Rückgabe des Tasks aus Ergebnissen
                return new Task(id, name, isCompleted, description, dueDate);
            }

            // Fehlerfall gibt null zurück
            return null;
        }

        /// <summary>
        /// Retrieves all tasks from the database.
        /// </summary>
        /// <remarks>This method connects to a SQLite database, executes a query to retrieve all tasks, 
        /// and returns them as a list of <see cref="Task"/> objects. Each task includes its  ID, name, completion
        /// status, optional description, and optional due date.</remarks>
        /// <returns>A list of <see cref="Task"/> objects representing all tasks in the database.  The list will be empty if no
        /// tasks are found.</returns>
        public static List<Task> GetAllTasks(int inputUserId)
        {
            // Initalisieren der Aufgabenliste
            List<Task> tasks = new List<Task>();

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
                string name = reader.GetString(2);
                bool isCompleted = reader.GetBoolean(3);
                string? description = reader.IsDBNull(4) ? null : reader.GetString(4);
                DateOnly? dueDate = reader.IsDBNull(5) ? null : DateOnly.FromDateTime(reader.GetDateTime(5));

                Task task = new Task(id, name, isCompleted, description, dueDate);
                tasks.Add(task);
            }

            // Rückgabe der Nutzerliste
            return tasks;
        }
    }
}

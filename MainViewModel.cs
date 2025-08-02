using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OfficeFlow
{
    /// <summary>
    /// Represents the main view model for managing tasks in the application.
    /// </summary>
    /// <remarks>This class provides functionality to manage a collection of tasks, including updating the
    /// task list  to reflect the latest state from the database. It is designed to be used as the data context for 
    /// views that display or interact with tasks.</remarks>
    public class MainViewModel
    {
        /// <summary>
        /// Gets or sets the collection of tasks that are not yet completed.
        /// </summary>
        public List<Task> InCompleteTasks { get; set; } = new();
        /// <summary>
        /// Gets or sets the collection of completed tasks.
        /// </summary>
        public List<Task> CompleteTasks { get; set; } = new();
        /// <summary>
        /// Gets or sets the collection of all tasks.
        /// </summary>
        public ObservableCollection<ITaskItem> AllTasks { get; set; } = new();
        /// <summary>
        /// Gets or sets a value indicating whether the results should be ordered by date.
        /// </summary>
        bool OrderTasksByDate { get; set; } = false;

        /// <summary>
        /// Updates the task lists by categorizing tasks into completed and incomplete groups.
        /// </summary>
        /// <remarks>This method clears the existing task lists and retrieves all tasks from the database.
        /// It then populates the <see cref="InCompleteTasks"/> list with tasks that are not completed and the <see
        /// cref="CompleteTasks"/> list with tasks that are completed.</remarks>
        public void UpdateTasksListBox(int userId)
        {
            // Löschen aller Taskisten
            InCompleteTasks.Clear();
            CompleteTasks.Clear();
            AllTasks.Clear();

            if (OrderTasksByDate)
            {
                // Sortierung nach Fälligkeitsdatum
                // Alle Tasks aus der Datenbank holen
                List<Task> tasks = TaskDatabaseHelper.GetAllTasks(userId);

                // Füllen der Listen mit den Tasks
                foreach (Task task in tasks)
                {
                    if (!task.IsCompleted)
                    {
                        // Unvollständige Aufgaben hinzufügen
                        InCompleteTasks.Add(task);

                    }
                    if (task.IsCompleted)
                    {
                        // Vollständige Aufgaben hinzufügen
                        CompleteTasks.Add(task);
                    }
                }

                // Sortieren der unvollständigen Aufgaben nach Fälligkeitsdatum
                InCompleteTasks.Sort((x, y) =>
                {
                    if (x.DueDate == null && y.DueDate == null) return 0;
                    if (x.DueDate == null) return 1; // null ans Ende
                    if (y.DueDate == null) return -1;
                    return x.DueDate.Value.CompareTo(y.DueDate.Value);
                });

                // Sortieren der vollständigen Aufgaben nach Fälligkeitsdatum
                CompleteTasks.Sort((x, y) =>
                {
                    if (x.DueDate == null && y.DueDate == null) return 0;
                    if (x.DueDate == null) return 1;
                    if (y.DueDate == null) return -1;
                    return x.DueDate.Value.CompareTo(y.DueDate.Value);
                });

                AllTasks.Add(new TaskSeparator("Nicht erledigte Aufgaben"));

                foreach (Task task in InCompleteTasks)
                {
                    AllTasks.Add(task);
                }

                AllTasks.Add(new TaskSeparator("Erledigte Aufgaben"));

                foreach (Task task in CompleteTasks)
                {
                    AllTasks.Add(task);
                }
            }
            else
            {
                // Sortierung standardmäßig nach Id
                // Alle Tasks aus der Datenbank holen
                List<Task> tasks = TaskDatabaseHelper.GetAllTasks(userId);

                // Füllen der Listen mit den Tasks
                foreach (Task task in tasks)
                {
                    if (!task.IsCompleted)
                    {
                        InCompleteTasks.Add(task);
                    }
                    if (task.IsCompleted)
                    {
                        CompleteTasks.Add(task);
                    }
                }

                AllTasks.Add(new TaskSeparator("Nicht erledigte Aufgaben"));

                foreach (Task task in InCompleteTasks)
                {
                    AllTasks.Add(task);
                }

                // Hinzufügen eines Trenners zwischen unvollständigen und vollständigen Aufgaben
                AllTasks.Add(new TaskSeparator("Erledigte Aufgaben"));

                foreach (Task task in CompleteTasks)
                {
                    AllTasks.Add(task);
                }
            }
        }

        /// <summary>
        /// Sets the sorting order based on the specified input.
        /// </summary>
        /// <param name="input">A string indicating the sorting criterion. Valid values are <see langword="date"/> or <see langword="id"/> 
        /// (case-insensitive).</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="input"/> is not a valid sorting option.</exception>
        public void OrderTasksBy(string input)
        {
            if (input == "date")
            {
                // Sortierung nach Fälligkeitsdatum
                OrderTasksByDate = true;
            }
            else if (input == "id")
            {
                // Sortierung nach Id
                OrderTasksByDate = false;
            }
            else
            {
                // Ungültige Sortieroption
                throw new ArgumentException("Ungültige Sortieroption: " + input);
            }
        }
    }
}

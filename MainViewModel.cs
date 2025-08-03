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
        /// Updates the task lists for the specified user, including incomplete, complete, and all tasks.
        /// </summary>
        /// <remarks>This method retrieves the user's task-related settings, such as whether tasks should
        /// be exported to Outlook  and the preferred task sorting order. If Outlook export is enabled, it imports any
        /// external changes from Outlook. The method then clears the existing task lists and repopulates them based on
        /// the user's tasks retrieved from the database. Tasks are sorted either by due date or by their default order,
        /// depending on the user's settings.</remarks>
        /// <param name="userId">The unique identifier of the user whose tasks are being updated.</param>
        public void UpdateTasksListBox(int userId)
        {
            // Holen der Einstellungen des Nutzers
            bool exportTasksToOutlook = SettingsDatabaseHelper.GetExportTasksToOutlook(userId);
            string orderTasksBy = SettingsDatabaseHelper.GetOrderTasksBy(userId);

            // Überprüfen ob die Einstellungen für den Export nach Outlook gesetzt sind
            if (exportTasksToOutlook)
            {
                // Importieren von externen Änderungen durch Outlook
                OutlookHelper.ImportAllTasks(userId);
            }

            // Löschen aller Taskisten
            InCompleteTasks.Clear();
            CompleteTasks.Clear();
            AllTasks.Clear();

            if (orderTasksBy == "date")
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
    }
}

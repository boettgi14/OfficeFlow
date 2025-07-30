using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Gets or sets the collection of tasks.
        /// </summary>
        /// <remarks>Changes to the collection are automatically observed, making it suitable for data
        /// binding in UI frameworks.</remarks>
        public ObservableCollection<Task> Tasks { get; set; } = new();

        /// <summary>
        /// Updates the task list by clearing the current tasks and reloading them from the database.
        /// </summary>
        /// <remarks>This method retrieves all tasks from the database and repopulates the task list.  It
        /// ensures that the task list reflects the latest state of the database.</remarks>
        public void updateTasksListBox()
        {
            Tasks.Clear();
            List<Task> tasks = TaskDatabaseHelper.GetAllTasks();
            foreach (Task task in tasks)
            {
                Tasks.Add(task);
            }
        }
    }
}

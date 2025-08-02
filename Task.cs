using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    /// <summary>
    /// Represents a task with an identifier, name, completion status, optional description, and optional due date.
    /// </summary>
    /// <remarks>This class is used to model a task in a task management system. Each task has a unique
    /// identifier, a name or title,  and a flag indicating whether it is completed. Optionally, a task can include a
    /// description and a due date.</remarks>
    public class Task : ITaskItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the operation has been completed.
        /// </summary>
        public bool IsCompleted { get; set; }
        /// <summary>
        /// Gets or sets the description associated with the object.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Gets or sets the due date for the task or item.
        /// </summary>
        public DateOnly? DueDate { get; set; }
        /// <summary>
        /// Gets a value indicating whether the item is selectable.
        /// </summary>
        public bool IsSelectable => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class with the specified properties.
        /// </summary>
        /// <param name="id">The unique identifier for the task.</param>
        /// <param name="name">The name of the task. Cannot be null or empty.</param>
        /// <param name="isCompleted">A value indicating whether the task is completed. <see langword="true"/> if the task is completed;
        /// otherwise, <see langword="false"/>.</param>
        /// <param name="description">An optional description of the task. Can be <see langword="null"/> if no description is provided.</param>
        /// <param name="dueDate">The optional due date for the task. Can be <see langword="null"/> if no due date is specified.</param>
        public Task(int id, string name, bool isCompleted, string? description, DateOnly? dueDate)
        {
            Id = id;
            Name = name;
            IsCompleted = isCompleted;
            Description = description;
            DueDate = dueDate;
        }
        /// <summary>
        /// Returns a string representation of the task, including its name, completion status, internal status, due
        /// date, and description.
        /// </summary>
        /// <returns>A string that summarizes the task's details, including its name, whether it is completed, whether it is
        /// internal,  the due date (or "No due date" if not set), and the description (or "No description" if empty).</returns>
        public override string ToString()
        {
            return $"Task: {Name}, Completed: {IsCompleted}, Due: {DueDate?.ToString() ?? "No due date"}, Description: {(string.IsNullOrEmpty(Description) ? "No description" : Description)}";
        }
    }
}
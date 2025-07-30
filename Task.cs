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
    public class Task
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
        /// Initializes a new instance of the <see cref="Task"/> class with the specified properties.
        /// </summary>
        /// <param name="id">The unique identifier for the task.</param>
        /// <param name="name">The name or title of the task. Cannot be null or empty.</param>
        /// <param name="isCompleted">A value indicating whether the task is completed. <see langword="true"/> if the task is completed;
        /// otherwise, <see langword="false"/>.</param>
        /// <param name="description">An optional description providing additional details about the task. Can be <see langword="null"/>.</param>
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
        /// Returns a string representation of the current object, including its ID, name, completion status,
        /// description, and due date.
        /// </summary>
        /// <returns>A string that contains the ID, name, completion status, description (or "No Description" if null),  and due
        /// date (or "No Due Date" if not set) of the object.</returns>
        public override string ToString()
        {
            return $"{Id} {Name} {(IsCompleted ? "(Completed)" : "")} {Description ?? "No Description"} {(DueDate.HasValue ? DueDate.Value.ToShortDateString() : "No Due Date")}";
        }
    }
}
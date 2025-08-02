using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    /// <summary>
    /// Represents a non-selectable separator item in a task list, typically used to group or visually separate tasks.
    /// </summary>
    /// <remarks>A <see cref="TaskSeparator"/> is a utility class that provides a way to organize tasks in a
    /// list by acting as a visual or logical divider.  It is not selectable and is intended for display purposes
    /// only.</remarks>
    public class TaskSeparator : ITaskItem
    {
        /// <summary>
        /// Gets or sets the title of the item.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets a value indicating whether the item is selectable.
        /// </summary>
        public bool IsSelectable => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSeparator"/> class with the specified title.
        /// </summary>
        /// <param name="title">The title of the task separator. This value cannot be null or empty.</param>
        public TaskSeparator(string title)
        {
            Title = title;
        }

        /// <summary>
        /// Returns a string representation of the current <see cref="TaskSeparator"/> instance.
        /// </summary>
        /// <returns>A string that includes the title of the task separator in the format: "TaskSeparator: {Title}".</returns>
        public override string ToString()
        {
            return $"TaskSeparator: {Title}";
        }
    }

}

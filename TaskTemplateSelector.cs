using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OfficeFlow
{
    /// <summary>
    /// Represents an item in a task list that can be queried for its selectability.
    /// </summary>
    /// <remarks>This interface defines a contract for task items that expose a property indicating whether
    /// the item can be selected. Implementations of this interface may represent various types of task-related
    /// entities.</remarks>
    public interface ITaskItem
    {
        bool IsSelectable { get; }
    }

    public class TaskTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template used to define the visual representation of a task.
        /// </summary>
        public DataTemplate TaskTemplate { get; set; }
        /// <summary>
        /// Gets or sets the template used to render task separators in the user interface.
        /// </summary>
        public DataTemplate TaskSeparatorTemplate { get; set; }

        /// <summary>
        /// Selects an appropriate <see cref="DataTemplate"/> based on the type of the provided item.
        /// </summary>
        /// <remarks>This method uses pattern matching to determine the appropriate template for the given
        /// item.  Ensure that <see cref="TaskTemplate"/> and <see cref="TaskSeparatorTemplate"/> are properly defined
        /// and assigned before calling this method.</remarks>
        /// <param name="item">The data object for which to select the template. This can be an instance of <see cref="Task"/>, <see
        /// cref="TaskSeparator"/>, or another type.</param>
        /// <param name="container">The container in which the template will be applied. Typically a UI element in the visual tree.</param>
        /// <returns>A <see cref="DataTemplate"/> corresponding to the type of <paramref name="item"/>.  Returns <see
        /// cref="TaskTemplate"/> for <see cref="Task"/> objects, <see cref="TaskSeparatorTemplate"/> for <see
        /// cref="TaskSeparator"/> objects,  or the base implementation's result for other types.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                Task => TaskTemplate,
                TaskSeparator => TaskSeparatorTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }


}

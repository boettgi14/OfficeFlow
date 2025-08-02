using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OfficeFlow
{
    /// <summary>
    /// Interaktionslogik für AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        /// <summary>
        /// Represents the currently logged-in user.
        /// </summary>
        /// <remarks>This field holds the user information for the active session.  It is intended for
        /// internal use and should not be accessed directly outside of the class.</remarks>
        private User CurrentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddTaskWindow"/> class with the specified user.
        /// </summary>
        /// <param name="user">The user for whom the task window is being created. This parameter cannot be null.</param>
        public AddTaskWindow(User user)
        {
            InitializeComponent();
            CurrentUser = user;
        }

        private void SafeButton_Click(object sender, RoutedEventArgs e)
        {
            // Eingabefelder auslesen
            string name = NameTextBox.Text.Trim();
            string descriptionText = DescriptionTextBox.Text.Trim();
            string? description = string.IsNullOrEmpty(descriptionText) ? null : descriptionText;
            string dueDateText = DueDateDatePicker.Text;
            DateOnly? dueDate = string.IsNullOrEmpty(dueDateText) ? null : DateOnly.Parse(dueDateText);

            if (name == "")
            {
                // Eingabefelder sind leer
                MessageBox.Show("Bitte füllen Sie das Name-Feld aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Aufgabe in Datenbank hinzufügen
                int result = TaskDatabaseHelper.AddTask(CurrentUser.Id, name, description, dueDate);

                if (result == 1)
                {
                    // Schließen des AddTaskWindows
                    this.Close();
                }
                else
                {
                    // Fehler beim Hinzufügen
                    MessageBox.Show("Fehler beim Hinzufügen der Aufgabe! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des AddTaskWindows
            this.Close();
        }
    }
}

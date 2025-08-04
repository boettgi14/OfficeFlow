using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
    /// Interaktionslogik für EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        /// <summary>
        /// Represents a task that is no longer actively used or has been replaced by a newer task.
        /// </summary>
        /// <remarks>This field is intended for internal use only and should not be accessed directly.  It
        /// may hold a reference to a previously executed or deprecated task.</remarks>
        private Task OldTask;
        /// <summary>
        /// Represents the currently logged-in user.
        /// </summary>
        /// <remarks>This field holds the user information for the active session.  It is intended for
        /// internal use and should not be accessed directly outside of the class.</remarks>
        private User CurrentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditTaskWindow"/> class, allowing a user to edit the details of
        /// an existing task.
        /// </summary>
        /// <remarks>This constructor initializes the window with the provided task's details, including
        /// its name, description, and due date (if available). The user can modify these details and save the
        /// changes.</remarks>
        /// <param name="user">The current user performing the task edit operation. Cannot be <see langword="null"/>.</param>
        /// <param name="task">The task to be edited. The task's current details will be pre-filled in the window. Cannot be <see
        /// langword="null"/>.</param>
        public EditTaskWindow(User user, Task task)
        {
            InitializeComponent();
            CurrentUser = user;
            // Setzen der alten Aufgabendaten
            OldTask = task;
            NameTextBox.Text = task.Name;
            DescriptionTextBox.Text = task.Description;
            if (task.DueDate.HasValue)
            {
                // Setzen des Fälligkeitsdatums nur wenn es einen Wert hat
                DueDateDatePicker.SelectedDate = task.DueDate.Value.ToDateTime(TimeOnly.MinValue);
            }
        }

        private void SafeButton_Click(object sender, RoutedEventArgs e)
        {
            // Setzen der alten Aufgabendaten
            int id = OldTask.Id;
            string oldName = OldTask.Name;
            bool oldIsCompleted = OldTask.IsCompleted;
            string? oldDescription = OldTask.Description;
            DateOnly? oldDueDate = OldTask.DueDate;

            // Setzen der neuen Aufgabendaten
            string newName = NameTextBox.Text.Trim();
            string newDescriptionText = DescriptionTextBox.Text.Trim();
            string? newDescription = string.IsNullOrEmpty(newDescriptionText) ? null : newDescriptionText;
            string newDueDateText = DueDateDatePicker.Text;
            DateOnly? newDueDate = string.IsNullOrEmpty(newDueDateText) ? null : DateOnly.Parse(newDueDateText);

            // Ergebniswerte initialisieren
            int resultName = 1;
            int resultDescription = 1;
            int resultDueDate = 1;

            if (newName == "")
            {
                // Eingabefelder sind leer
                MessageBox.Show("Bitte füllen Sie das Name-Feld aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (oldName != newName)
            {
                // Name soll geändert werden
                resultName = TaskDatabaseHelper.EditName(id, newName);
            }

            if (oldDescription != newDescription)
            {
                // Beschreibung soll geändert werden
                resultDescription = TaskDatabaseHelper.EditDescription(id, newDescription);
            }

            if (oldDueDate != newDueDate)
            {
                // Fälligkeitsdatum soll geändert werden
                resultDueDate = TaskDatabaseHelper.EditDueDate(id, newDueDate);
            }

            if (resultName == 1 && resultDescription == 1 && resultDueDate == 1)
            {
                // Alle Daten wurden erfolgreich geändert
                // Prüfen ob Aufgaben nach Outlook exportiert werden sollen
                if (SettingsDatabaseHelper.GetExportTasksToOutlook(CurrentUser.Id))
                {
                    // Exportieren aller Aufgaben nach Outlook
                    OutlookHelper.ExportAllTasks(CurrentUser.Id);
                }
                // Schließen des EditTaskWindows
                this.Close();
            }
            else
            {
                // Fehler beim Speichern der Daten
                MessageBox.Show("Fehler beim Speichern einiger Daten! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                //Schließen des EditTaksWindows
                this.Close();
            }
        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des EditTaksWindows
            this.Close();
        }
    }
}

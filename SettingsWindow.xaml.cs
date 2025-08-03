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

/*
 * TODO
 * - Export von Aufgaben nach Outlook einbauen (Änderung registrieren, Outlook importieren, Änderung speichern, Outlook löschen, Outlook exportieren)
 */

namespace OfficeFlow
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// Gets or sets the currently authenticated user.
        /// </summary>
        private User CurrentUser { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class with the specified user.
        /// </summary>
        /// <param name="user">The user for whom the settings window is being initialized. Cannot be <see langword="null"/>.</param>
        public SettingsWindow(User user)
        {
            InitializeComponent();
            CurrentUser = user;

            // Setzen der Checkboxen für die Einstellungen des Nutzers
            ExportTasksToOutlookCheckBox.IsChecked = SettingsDatabaseHelper.GetExportTasksToOutlook(CurrentUser.Id);
            // Setzen des Buttons zum Löschen exportierter Aufgaben
            DeleteExportedTasksButton.IsEnabled = !ExportTasksToOutlookCheckBox.IsChecked.Value;
        }

        private void ResetSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Bestätigungsdialog anzeigen
            var result = MessageBox.Show("Sind Sie sicher, dass sie ihre Nutzereinstellungen zurücksetzen möchten?", "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                // Abbrechen des Zurücksetzens
                return;
            }

            // Zurücksetzen der Nutzereinstellungen
            int settingsResult = SettingsDatabaseHelper.ResetUser(CurrentUser.Id);


            if (settingsResult != 1)
            {
                // Fehler beim Löschen der Nutzereinstellungen
                MessageBox.Show("Fehler beim Zurücksetzen der Nutzereinstellungen! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteExportedTasksButton_Click(object sender, RoutedEventArgs e)
        {
            // Bestätigungsdialog anzeigen
            var result = MessageBox.Show("Sind Sie sicher, dass sie alle exportierten Aufgaben aus Outlook löschen möchten?", "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                // Abbrechen des Löschens
                return;
            }

            // Löschen der exportierten Aufgaben aus Outlook
            OutlookHelper.DeleteAllExportedTasks();
        }

        private void DeleteExportedAppointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            // Anzeigen des Hinweises dass Termine keine funktionale Komponente sind
            MessageBox.Show("Termine sind keine funktionale Komponente von OfficeFlow. Dieser Button trägt nur zur Vollständigkeit der Programmanischt bei.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des Settingswindows
            this.Close();
        }

        private void ExportTasksToOutlookCheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Aktivieren oder Deaktivieren des Buttons zum Löschen exportierter Aufgaben
            if (ExportTasksToOutlookCheckBox.IsChecked == true)
            {
                DeleteExportedTasksButton.IsEnabled = false;
                SettingsDatabaseHelper.SetExportTasksToOutlook(CurrentUser.Id, true);
                // Löschen aller exportierten Aufgaben aus Outlook
                OutlookHelper.DeleteAllExportedTasks();
                // Holen der Aufgaben des Nutzers aus der Datenbank
                List<Task> tasks = TaskDatabaseHelper.GetAllTasks(CurrentUser.Id);
                // Exportieren der Aufgaben nach Outlook
                OutlookHelper.ExportTasks(tasks);
            }
            else
            {
                DeleteExportedTasksButton.IsEnabled = true;
                SettingsDatabaseHelper.SetExportTasksToOutlook(CurrentUser.Id, false);
            }
        }

        private void ExportAppointmentsToOutlookCheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Aktivieren oder Deaktivieren des Buttons zum Löschen exportierter Aufgaben
            if (ExportAppointmentsToOutlookCheckBox.IsChecked == true)
            {
                DeleteExportedAppointmentsButton.IsEnabled = false;
            }
            else
            {
                DeleteExportedAppointmentsButton.IsEnabled = true;
            }

            // Anzeigen des Hinweises dass Termine keine funktionale Komponente sind
            MessageBox.Show("Termine sind keine funktionale Komponente von OfficeFlow. Diese Checkbox trägt nur zur Vollständigkeit der Programmanischt bei.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

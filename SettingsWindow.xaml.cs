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

            // Aktualisieren der Einstellungen des Nutzers
            SetSettings();
        }

        /// <summary>
        /// Sets the settings for the current user based on the database values.
        /// </summary>
        private void SetSettings()
        {
            // Setzen der Checkboxen für die Einstellungen des Nutzers
            ExportTasksToOutlookCheckBox.IsChecked = SettingsDatabaseHelper.GetExportTasksToOutlook(CurrentUser.Id);
            // Setzen des Buttons zum Löschen exportierter Aufgaben
            DeleteExportedTasksButton.IsEnabled = !ExportTasksToOutlookCheckBox.IsChecked.Value;
            // Setzen der Checkbox für die automatische Zeiterfassung
            AutomaticTimeTrackingCheckBox.IsChecked = SettingsDatabaseHelper.GetAutomaticTimeTracking(CurrentUser.Id);

            // Löschen der Einstellungen für Termine
            ExportAppointmentsToOutlookCheckBox.IsChecked = false;
            DeleteExportedAppointmentsButton.IsEnabled = true;
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


            if (settingsResult == 1)
            {
                // Nutzereinstellungen erfolgreich zurückgesetzt
                MessageBox.Show("Ihre Nutzereinstellungen wurden erfolgreich zurückgesetzt!\nZum Anwenden der Einstellungen der Zeiterfassung muss das Programm neu gestartet werden!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Fehler beim Löschen der Nutzereinstellungen
                MessageBox.Show("Fehler beim Zurücksetzen der Nutzereinstellungen! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Aktualisieren der Einstellungen des Nutzers
            SetSettings();
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
            MessageBox.Show("Termine sind keine funktionale Komponente von OfficeFlow.\nDieser Button trägt nur zur Vollständigkeit der Programmanischt bei.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
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
                // Exportieren aller Aufgaben nach Outlook
                OutlookHelper.ExportAllTasks(CurrentUser.Id);
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
            MessageBox.Show("Termine sind keine funktionale Komponente von OfficeFlow.\nDiese Checkbox trägt nur zur Vollständigkeit der Programmanischt bei.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AutomaticTimeTrackingCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (AutomaticTimeTrackingCheckBox.IsChecked == true)
            {
                SettingsDatabaseHelper.SetAutomaticTimeTracking(CurrentUser.Id, true);
            }
            else
            {
                SettingsDatabaseHelper.SetAutomaticTimeTracking(CurrentUser.Id, false);
            }
            // Anzeigen des Hinweises dass ein Neustart des Programms erforderlich ist
            MessageBox.Show("Die automatische Zeiterfassung wurde geändert.\nZum Anwenden der Einstellungen muss das Programm neu gestartet werden!", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

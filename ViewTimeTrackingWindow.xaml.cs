using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using static System.Net.Mime.MediaTypeNames;

namespace OfficeFlow
{
    /// <summary>
    /// Interaktionslogik für ViewTimeTrackingWindow.xaml
    /// </summary>
    public partial class ViewTimeTrackingWindow : Window
    {
        /// <summary>
        /// Represents the currently logged-in user.
        /// </summary>
        /// <remarks>This field holds the user information for the active session.  It is intended for
        /// internal use and should not be accessed directly outside of the class.</remarks>
        private User CurrentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTimeTrackingWindow"/> class.
        /// </summary>
        /// <remarks>This constructor sets up the time tracking window for the specified user, initializes
        /// the UI components, and updates the displayed time tracking data and button states.</remarks>
        /// <param name="user">The user whose time tracking data will be displayed in the window. Cannot be null.</param>
        public ViewTimeTrackingWindow(User user)
        {
            InitializeComponent();
            CurrentUser = user;

            // Setzen der Namen für die Drucken Buttons
            DateTime now = DateTime.Now;
            ExportCurrentMonthButton.Content = now.ToString("MMMM") + " drucken";
            DateTime last = DateTime.Now.AddMonths(-1);
            ExportLastMonthButton.Content = last.ToString("MMMM") + " drucken";

            // Anzeigen der Nutzerliste
            UpdateTimesListBox();

            // Setzen des Button Status
            SetButtonStatus();
        }

        /// <summary>
        /// Updates the items in the <see cref="TimesListBox"/> to display the start times of all time entries
        /// associated with the current user.
        /// </summary>
        /// <remarks>This method clears the existing items in the <see cref="TimesListBox"/> and
        /// repopulates it  with the start times retrieved from the database for the current user's time
        /// entries.</remarks>
        private void UpdateTimesListBox()
        {
            // Leeren der ListBox
            TimesListBox.Items.Clear();

            List<Time> times = TimeDatabaseHelper.GetAllTimes(CurrentUser.Id);
            foreach (Time time in times)
            {
                // Formatieren des Datums
                string startTime = time.Start.ToString("dd.MM.yyyy HH:mm:ss");
                string endTime = time.End.ToString("dd.MM.yyyy HH:mm:ss");
                string text = startTime + " - " + endTime + "\n" + "Arbeitszeit: " + time.TotalDuration + " Pausenzeit: " + time.PauseDuration;
                TextBlock textBlock = new TextBlock
                {
                    Text = text,
                    Tag = time
                };
                // Setzen der ListBox Items
                int index = TimesListBox.Items.Add(textBlock);
            }
        }

        /// <summary>
        /// Updates the enabled state of the Edit and Delete buttons based on the current selection in the Times list.
        /// </summary>
        /// <remarks>If an item is selected in the Times list, the Edit and Delete buttons are enabled. 
        /// Otherwise, the buttons are disabled.</remarks>
        private void SetButtonStatus()
        {
            if (TimesListBox.SelectedItem != null)
            {
                // Zeit ausgewählt
                // Aktivieren der Buttons
                EditTimeButton.IsEnabled = true;
                DeleteTimeButton.IsEnabled = true;
            }
            else
            {
                // Keine Zeit ausgewählt
                // Deaktivieren der Buttons
                EditTimeButton.IsEnabled = false;
                DeleteTimeButton.IsEnabled = false;
            }
        }

        private void DeleteTimeButton_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen ob eine Zeiterfassung ausgewählt ist
            if (TimesListBox.SelectedItem is TextBlock selectedTextBlock && selectedTextBlock.Tag is Time time)
            {
                // Löschen der Zeiterfassung
                int timeResult = TimeDatabaseHelper.DeleteTime(time.Id);

                if (timeResult == 1)
                {
                    // Zeiterfassung erfolgreich gelöscht
                    // Updaten der Zeitenliste nach dem Löschen
                    UpdateTimesListBox();
                }
                else
                {
                    // Fehler beim Löschen der Zeiterfassung
                    MessageBox.Show("Fehler beim Löschen der Zeiterfassung! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Keine Zeiterfassung ausgewählt
                MessageBox.Show("Bitte wählen Sie eine Zeiterfassung aus, um sie zu löschen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditTimeButton_Click(object sender, RoutedEventArgs e)
        {
            Time? time = null;
            if (TimesListBox.SelectedItem is TextBlock selectedTextBlock && selectedTextBlock.Tag is Time)
            {
                // Ausgewählte Zeiterfassung aus der Datenbank abrufen
                time = TimeDatabaseHelper.GetTime((selectedTextBlock.Tag as Time).Id);
            }

            if (time == null)
            {
                // Zeiterfassung nicht gefunden
                MessageBox.Show("Die Zeiterfassung konnte nicht gefunden werden! Bitte versuche Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Erstellen des EditTimeWindow
                EditTimeWindow editTimeWindow = new EditTimeWindow(CurrentUser, time);
                editTimeWindow.Owner = this; // Besitzer auf ViewTimeTrackingWindow setzen
                editTimeWindow.ShowDialog();

                // Updaten der Zeitenliste nach Schließen des EditTimeWindow
                UpdateTimesListBox();
            }
        }

        private void TimesListBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Überprüfen ob die Entf Taste gedrückt wurde
            if (e.Key == Key.Delete)
            {
                // Überprüfen ob eine Zeiterfassung ausgewählt ist
                if (TimesListBox.SelectedItem is TextBlock selectedTextBlock && selectedTextBlock.Tag is Time time)
                {
                    // Löschen der Zeiterfassung
                    int timeResult = TimeDatabaseHelper.DeleteTime(time.Id);

                    if (timeResult == 1)
                    {
                        // Zeiterfassung erfolgreich gelöscht
                        // Updaten der Zeitenliste nach dem Löschen
                        UpdateTimesListBox();
                    }
                    else
                    {
                        // Fehler beim Löschen der Zeiterfassung
                        MessageBox.Show("Fehler beim Löschen der Zeiterfassung! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Keine Zeiterfassung ausgewählt
                    MessageBox.Show("Bitte wählen Sie eine Zeiterfassung aus, um sie zu löschen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TimesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Überprüfen ob eine Zeit ausgewählt wurde
            SetButtonStatus();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des Fensters
            this.Close();
        }

        private void ExportCurrentMonthButton_Click(object sender, RoutedEventArgs e)
        {
            // Öffnen von Excel mit Zeiten des aktuellen Monats
            ExcelHelper.ExportCurrentMonthToExcel(CurrentUser.Id, CurrentUser.Username);
        }

        private void ExportLastMonthButton_Click(object sender, RoutedEventArgs e)
        {
            // Öffnen von Excel mit Zeiten des letzten Monats
            ExcelHelper.ExportLastMonthToExcel(CurrentUser.Id, CurrentUser.Username);
        }
    }
}

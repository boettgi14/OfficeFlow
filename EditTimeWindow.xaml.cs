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

namespace OfficeFlow
{
    /// <summary>
    /// Interaktionslogik für EditTimeWindow.xaml
    /// </summary>
    public partial class EditTimeWindow : Window
    {
        /// <summary>
        /// Represents the previous recorded time value.
        /// </summary>
        /// <remarks>This field is used to store a prior time value for comparison or calculation
        /// purposes. It is private and not accessible outside the containing class.</remarks>
        private Time OldTime;
        /// <summary>
        /// Repräsentiert den aktuell angemeldeten Benutzer, für den die Zeiterfassung bearbeitet wird.
        /// </summary>
        /// <remarks>
        /// Dieses Feld wird verwendet, um benutzerspezifische Validierungen und Datenbankoperationen durchzuführen,
        /// wie z. B. das Überprüfen von Überschneidungen bei Zeiterfassungen oder das Speichern von Änderungen.
        /// Es ist nur innerhalb der Klasse zugänglich.
        /// </remarks>
        private User CurrentUser;

        public EditTimeWindow(User user, Time time)
        {
            InitializeComponent();
            OldTime = time;
            CurrentUser = user;
            StartDateTimePicker.Value = OldTime.Start;
            EndDateTimePicker.Value = OldTime.End;
            HoursIntegerUpDown.Value = (int)OldTime.PauseDuration.Hours;
            MinutesIntegerUpDown.Value = (int)OldTime.PauseDuration.Minutes;

        }

        private void SafeButton_Click(object sender, RoutedEventArgs e)
        {
            // Neue Nutzerdaten setzen
            DateTime? newStartTime = StartDateTimePicker.Value;
            DateTime? newEndTime = EndDateTimePicker.Value;
            int? newPauseHours = HoursIntegerUpDown.Value;
            int? newPauseMinutes = MinutesIntegerUpDown.Value;

            // Prüfen ob alle Pflichtfelder ausgefüllt sind
            if (newStartTime == null || newEndTime == null || newPauseHours == null || newPauseMinutes == null)
            {
                MessageBox.Show("Bitte füllen Sie alle Pflichtfelder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TimeSpan newPauseDuration = new TimeSpan((int)newPauseHours, (int)newPauseMinutes, OldTime.PauseDuration.Seconds);
            Debug.WriteLine($"Neue Pausenzeit: {newPauseDuration}");

            // Ergebniswerte initialisieren
            int resultStartTime = 1;
            int resultEndTime = 1;
            int resultPauseDuration = 1;

            // Validierung der Eingaben
            // Prüfen ob die Startzeit oder Endzeit in der Zukunft liegen
            if (newStartTime > DateTime.Now || newEndTime > DateTime.Now)
            {
                MessageBox.Show("Die Startzeit und die Endzeit dürfen nicht in der Zukunft liegen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Prüfen ob die Startzeit nach der Endzeit liegt oder gleich ist
            if (newStartTime >= newEndTime)
            {
                MessageBox.Show("Die Startzeit darf nicht nach der Endzeit liegen oder gleich sein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Prüfen ob die Pausenzeit größer als die Gesamtzeit ist
            if (newPauseDuration > (newEndTime.Value - newStartTime.Value))
            {
                MessageBox.Show("Die Pausenzeit darf nicht länger sein als die Gesamtzeit!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Prüfen ob die Zeiterfassung sich mit einer anderen Zeiterfassung überschneidet
            if (TimeDatabaseHelper.IsTimeOverlapping(CurrentUser.Id, OldTime.Id, newStartTime.Value, newEndTime.Value))
            {
                MessageBox.Show("Die Zeiterfassung überschneidet sich mit einer anderen Zeiterfassung!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Speichern der neuen Daten in der Datenbank
            if (newStartTime != null && newStartTime != OldTime.Start)
            {
                // Startzeit soll geändert werden
                resultStartTime = TimeDatabaseHelper.EditStartTime(OldTime.Id, newStartTime.Value);
            }

            if (newEndTime != null && newEndTime != OldTime.End)
            {
                // Endzeit soll geändert werden
                resultEndTime = TimeDatabaseHelper.EditEndTime(OldTime.Id, newEndTime.Value);
            }

            if (newPauseDuration != OldTime.PauseDuration)
            {
                // Pausenzeit soll geändert werden
                resultPauseDuration = TimeDatabaseHelper.EditPauseDuration(OldTime.Id, (int)newPauseDuration.TotalSeconds);
            }

            if (resultStartTime == 1 && resultEndTime == 1 && resultPauseDuration == 1)
            {
                // Alle Daten wurden erfolgreich geändert
                //Schließen des Fensters
                this.Close();
            }
            else
            {
                // Fehler beim Speichern der Daten
                MessageBox.Show("Fehler beim Speichern einiger Daten! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                //Schließen des Fensters
                this.Close();
            }
        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des Fensters ohne Änderungen zu speichern
            this.Close();
        }
    }
}

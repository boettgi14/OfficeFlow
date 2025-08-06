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
        public EditTimeWindow(Time time)
        {
            InitializeComponent();
            OldTime = time;
            StartDateTimePicker.Value = OldTime.Start;
            EndDateTimePicker.Value = OldTime.End;
            HoursIntegerUpDown.Value = (int)OldTime.PauseDuration.Hours;
            MinutesIntegerUpDown.Value = (int)OldTime.PauseDuration.Minutes;

        }

        //TODO: Verhindern der Eingabe von Pausenzeit > Totalzeit
        //TODO: Verhindern der Eingabe von Startzeit > Endzeit
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

            // Ergebniswerte initialisieren
            int resultStartTime = 1;
            int resultEndTime = 1;
            int resultPauseDuration = 1;

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
                resultPauseDuration = TimeDatabaseHelper.EditPauseDuration(OldTime.Id, newPauseDuration);
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

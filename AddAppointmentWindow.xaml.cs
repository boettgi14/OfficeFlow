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
    /// Interaktionslogik für AddAppointmentWindow.xaml
    /// </summary>
    public partial class AddAppointmentWindow : Window
    {
        public AddAppointmentWindow()
        {
            InitializeComponent();
        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des AddAppointmentWindows
            this.Close();
        }

        private void SafeButton_Click(object sender, RoutedEventArgs e)
        {
            // Anzeigen des Hinweises dass Termine keine funktionale Komponente sind
            MessageBox.Show("Termine sind keine funktionale Komponente von OfficeFlow. Dieses Fenster trägt nur zur Vollständigkeit der Programmanischt bei.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            // Schließen des AddAppointmentWindows
            this.Close();
        }
    }
}

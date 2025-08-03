using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/*
 * TODO
 * - Aktualisierungsbutton einbauen (Aktualisiert die Tasklisten und die Einstellungen des Nutzers)
 */

namespace OfficeFlow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Gets the main view model associated with the current data context.
        /// </summary>
        public MainViewModel ViewModel => (MainViewModel)DataContext;
        /// <summary>
        /// Represents the currently logged-in user.
        /// </summary>
        /// <remarks>This field holds the user information for the active session.  It is intended for
        /// internal use and should not be accessed directly outside of the class.</remarks>
        private User CurrentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="user">The user for whom the main window is being initialized. Determines the admin status and UI configuration.</param>
        public MainWindow(User user)
        {
            InitializeComponent();

            CurrentUser = user;

            // Setzen der UI auf Adminstatus des Nutzers
            setAdminStatus(CurrentUser);

            // Setzen der UI auf Einstellungen des Nutzers
            setSettings(CurrentUser);

            // Setzen der UI auf aktuellen Zustand der Listen
            SetTaskButtonStatus();
            SetAppointmentButtonStatus();

            // Updaten der Aufgabenliste im ViewModel
            ViewModel.UpdateTasksListBox(CurrentUser.Id);
        }

        /// <summary>
        /// Configures the task sorting settings for the specified user based on their preferences.
        /// </summary>
        /// <remarks>The method retrieves the user's preferred task sorting order from the settings
        /// database and updates the task view accordingly.  If the user prefers sorting by date, the tasks are sorted
        /// by their due dates. Otherwise, tasks are sorted by their IDs. The corresponding menu items are also updated
        /// to reflect the selected sorting order.</remarks>
        /// <param name="user">The user whose task sorting settings are to be applied. This parameter cannot be null.</param>
        private void setSettings(User user)
        {
            // Sortierung der Aufgabenliste
            string orderTasksBy = SettingsDatabaseHelper.GetOrderTasksBy(CurrentUser.Id);
            if (orderTasksBy == "date")
            {
                // Sortieren nach Fälligkeitsdatum
                OrderTasksByIdMenuItem.IsChecked = false;
                OrderTasksByDateMenuItem.IsChecked = true;
            }
            else
            {
                // Standardmäßig nach Id sortieren
                OrderTasksByIdMenuItem.IsChecked = true;
                OrderTasksByDateMenuItem.IsChecked = false;
            }
        }

        /// <summary>
        /// Updates the administrative status of the specified user and enables or disables administrative functions in
        /// the user interface accordingly.
        /// </summary>
        /// <remarks>This method adjusts the availability of administrative functions in the user
        /// interface based on the <paramref name="user"/>'s <see cref="User.AdminStatus"/>. If the user is an
        /// administrator, administrative functions are enabled; otherwise, they are disabled.</remarks>
        /// <param name="user">The user whose administrative status is being updated. The <see cref="User.AdminStatus"/> property
        /// determines whether administrative functions are enabled.</param>
        private void setAdminStatus(User user)
        {
            if (user.AdminStatus)
            {
                // Nutzer ist Admin
                // Admin Funktionen aktivieren
                UserManagementMenuItem.IsEnabled = true;
            }
            else
            {
                // Nutzer ist kein Admin
                // Admin Funktionen deaktivieren
                UserManagementMenuItem.IsEnabled = false;
            }
        }

        /// <summary>
        /// Updates the enabled status of task-related buttons based on the current selection in the task list.
        /// </summary>
        /// <remarks>If a task is selected in the <see cref="TasksListBox"/>, the buttons for editing and
        /// deleting tasks are enabled. Otherwise, these buttons are disabled.</remarks>
        private void SetTaskButtonStatus()
        {
            if (TasksListBox.SelectedItem != null)
            {
                // Nuter ausgewählt
                // Aktivieren der Buttons
                EditTaskMenuItem.IsEnabled = true;
                DeleteTaskMenuItem.IsEnabled = true;
            }
            else
            {
                // Kein Nutzer ausgewählt
                // Deaktivieren der Buttons
                EditTaskMenuItem.IsEnabled = false;
                DeleteTaskMenuItem.IsEnabled = false;
            }
        }

        /// <summary>
        /// Disables the appointment-related menu items.
        /// </summary>
        /// <remarks>This method sets the <see cref="EditAppointmentMenuItem"/> and  <see
        /// cref="DeleteAppointmentMenuItem"/> menu items to a disabled state,  preventing user interaction with these
        /// options.</remarks>
        private void SetAppointmentButtonStatus()
        {
            // Deaktivieren der Buttons
            EditAppointmentMenuItem.IsEnabled = false;
            DeleteAppointmentMenuItem.IsEnabled = false;
        }

        private void EndProgramMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Schließen des MainWindows
            this.Close();
        }

        private void LogOutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des LoginWindows
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            // Schließen des MainWindows
            this.Close();
        }

        private void GitHubMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Öffnen des Browsers mit der GitHub Seite des Projekts
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/boettgi14/OfficeFlow",
                UseShellExecute = true
            });
        }

        private void UserManagementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des UserManagementWindows
            UserManagementWindow userManagementWindow = new UserManagementWindow(CurrentUser);
            userManagementWindow.Owner = this; // Besitzer auf MainWindow setzen
            userManagementWindow.ShowDialog();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des AboutWindows
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this; // Besitzer auf MainWindow setzen
            aboutWindow.ShowDialog();
        }

        private void InstructionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des InstructionsWindows
            InstructionsWindow instructionsWindow = new InstructionsWindow();
            instructionsWindow.Owner = this; // Besitzer auf MainWindow setzen
            instructionsWindow.ShowDialog();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des SettingsWindows
            SettingsWindow settingsWindow = new SettingsWindow(CurrentUser);
            settingsWindow.Owner = this; // Besitzer auf MainWindow setzen
            settingsWindow.ShowDialog();

            // Setzen der UI auf Einstellungen des Nutzers
            setSettings(CurrentUser);

            // Updaten der Aufgabenliste im ViewModel
            ViewModel.UpdateTasksListBox(CurrentUser.Id);
        }

        private void AddTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des AddTaskWindows
            AddTaskWindow addTaskWindow = new AddTaskWindow(CurrentUser);
            addTaskWindow.Owner = this; // Besitzer auf MainWindow setzen
            addTaskWindow.ShowDialog();
            // Aktualisieren der Aufgabenliste im ViewModel nach dem Hinzufügen
            ViewModel.UpdateTasksListBox(CurrentUser.Id);
        }

        private void EditTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Setzen des ausgewählten Tasks
            Task? task = TasksListBox.SelectedItem as Task;
            if (task != null)
            {
                // Erstellen des EditTaskWindows
                EditTaskWindow editTaskWindow = new EditTaskWindow(task);
                editTaskWindow.Owner = this; // Besitzer auf MainWindow setzen
                editTaskWindow.ShowDialog();
                // Aktualisieren der Aufgabenliste im ViewModel nach dem Bearbeiten
                ViewModel.UpdateTasksListBox(CurrentUser.Id);
            }
            else
            {
                // Keine Aufgabe ausgewählt
                MessageBox.Show("Bitte wählen Sie eine Aufgabe aus, die Sie bearbeiten möchten!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void DeleteTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Löschen der ausgewählten Aufgabe aus der Datenbank
            Task? task = TasksListBox.SelectedItem as Task;
            if (task != null)
            {
                int id = task.Id;
                int result = TaskDatabaseHelper.DeleteTask(id);
                if (result == 1)
                {
                    // Aufgabe erfolgreich gelöscht
                    // Aktualisieren der Aufgabenliste im ViewModel
                    ViewModel.UpdateTasksListBox(CurrentUser.Id);
                }
                else
                {
                    // Fehler beim Löschen der Aufgabe
                    MessageBox.Show("Fehler beim Löschen der Aufgabe! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Keine Aufgabe ausgewählt
                MessageBox.Show("Bitte wählen Sie eine Aufgabe aus, die Sie löschen möchten!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void TasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            // Prüfen ob eine Trenner ausgewählt wurde
            if (listBox?.SelectedItem is ITaskItem item && !item.IsSelectable)
            {
                // Auswahl von Trenner rückgängig machen
                listBox.SelectedItem = null;
            }

            // Überprüfen, ob eine Aufgabe ausgewählt wurde
            SetTaskButtonStatus();
        }

        private void TasksListBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Überprüfen, ob die Entf-Taste gedrückt wurde
            if (e.Key == Key.Delete)
            {
                // Überprüfen, ob eine Aufgabe ausgewählt ist
                var selectedUser = TasksListBox.SelectedItem;
                if (selectedUser != null)
                {
                    // Löschen der ausgewählten Aufgabe aus der Datenbank
                    Task? task = TasksListBox.SelectedItem as Task;
                    if (task != null)
                    {
                        int id = task.Id;
                        int result = TaskDatabaseHelper.DeleteTask(id);

                        if (result == 1)
                        {
                            // Aufgabe erfolgreich gelöscht
                            // Updaten der Aufgabenliste nach dem Löschen
                            ViewModel.UpdateTasksListBox(CurrentUser.Id);
                        }
                        else
                        {
                            // Fehler beim Löschen der Aufgabe
                            MessageBox.Show("Fehler beim Löschen der Aufgabe! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        // Keine Aufgabe ausgewählt
                        MessageBox.Show("Bitte wählen Sie eine Aufgabe aus, die Sie löschen möchten!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void IsCompletedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Checkbox und Task aus dem Sender extrahieren
            CheckBox? checkBox = sender as CheckBox;

            if (checkBox != null && checkBox.DataContext is Task task)
            {
                // Aktualisieren von IsCompleted der Aufgabe in der Datenbank
                int result = TaskDatabaseHelper.EditIsCompleted(task.Id, task.IsCompleted);

                if (result == 1)
                {
                    // Aufgabe erfolgreich aktualisiert
                    // Updaten der Aufgabenliste nach dem Aktualisieren
                    ViewModel.UpdateTasksListBox(CurrentUser.Id);
                }
                else
                {
                    // Fehler beim Aktualisieren der Aufgabe
                    MessageBox.Show("Fehler beim Aktualisieren der Aufgabe! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OrderTasksMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;
            if (sender == OrderTasksByIdMenuItem)
            {
                // Sortieren der Aufgaben nach Id
                OrderTasksByIdMenuItem.IsChecked = true;
                OrderTasksByDateMenuItem.IsChecked = false;
                result = SettingsDatabaseHelper.SetOrderTasksBy(CurrentUser.Id, "id");
                ViewModel.UpdateTasksListBox(CurrentUser.Id);
            }
            else if (sender == OrderTasksByDateMenuItem)
            {
                // Sortieren der Aufgaben nach Fälligkeitsdatum
                OrderTasksByIdMenuItem.IsChecked = false;
                OrderTasksByDateMenuItem.IsChecked = true;
                result = SettingsDatabaseHelper.SetOrderTasksBy(CurrentUser.Id, "date");
                ViewModel.UpdateTasksListBox(CurrentUser.Id);
            }
            if (result != 1)
            {
                // Fehler beim Setzen der Sortierung
                MessageBox.Show("Fehler beim Abspeichern der nutzerspezifischen Einstellung! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAppointmentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des AddAppointmentWindows
            AddAppointmentWindow addAppointmentWindow = new AddAppointmentWindow();
            addAppointmentWindow.Owner = this; // Besitzer auf MainWindow setzen
            addAppointmentWindow.ShowDialog();
        }

        private void RefreshMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Aktualisieren der Aufgabenliste im ViewModel
            ViewModel.UpdateTasksListBox(CurrentUser.Id);
        }
    }
}
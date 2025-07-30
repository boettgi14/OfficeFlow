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
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="user">The user for whom the main window is being initialized. Determines the admin status and UI configuration.</param>
        public MainWindow(User user)
        {
            InitializeComponent();

            // Initialisieren der Datenbank für Aufgaben
            TaskDatabaseHelper.InitializeDatabase();

            // Updaten der Aufgabenliste im ViewModel
            ViewModel.updateTasksListBox();

            // Setzen der UI auf Adminstatus des Benutzers
            setAdminStatus(user);
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
                UserManagementMenuItem.IsEnabled = true;
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
            UserManagementWindow userManagementWindow = new UserManagementWindow();
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
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this; // Besitzer auf MainWindow setzen
            settingsWindow.ShowDialog();
        }

        private void AddTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Erstellen des AddTaskWindows
            AddTaskWindow addTaskWindow = new AddTaskWindow();
            addTaskWindow.Owner = this; // Besitzer auf MainWindow setzen
            addTaskWindow.ShowDialog();
            // Aktualisieren der Aufgabenliste im ViewModel nach dem Hinzufügen
            ViewModel.updateTasksListBox();
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
                ViewModel.updateTasksListBox();
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
                    ViewModel.updateTasksListBox();
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
                            ViewModel.updateTasksListBox();
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
                    ViewModel.updateTasksListBox();
                }
                else
                {
                    // Fehler beim Aktualisieren der Aufgabe
                    MessageBox.Show("Fehler beim Aktualisieren der Aufgabe! Bitte versuchen Sie es noch einmal!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
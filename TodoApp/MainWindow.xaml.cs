using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;

namespace TodoApp
{
    public class TodoList
    {
        public string Name { get; set; }
        public List<TodoTask> Tasks { get; set; } = new List<TodoTask>();
    }
    public class TodoTask
    {
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
    }
    public class CompletedToStrikethroughConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted && isCompleted)
            {
                return TextDecorations.Strikethrough;
            }
            return null; // No decoration if not completed
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    // A simple container to hold multiple lists.
    public class AppData
    {
        public List<TodoList> Lists { get; set; } = new List<TodoList>();
    }
    public partial class MainWindow : Window
    {
        // Our container holding multiple lists
        private AppData appData = new AppData();

        public MainWindow()
        {
            InitializeComponent();
            RefreshListSelector();
        }

        //------------------------------------------------------------------
        //  PRIMARY UI EVENT HANDLERS
        //------------------------------------------------------------------

        // Create a new list
        private void AddNewListButton_Click(object sender, RoutedEventArgs e)
        {
            string newListName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter name for the new list:",
                "New List",
                "Untitled");

            if (!string.IsNullOrWhiteSpace(newListName))
            {
                // Check if a list with that name already exists
                if (appData.Lists.Any(l => l.Name.Equals(newListName, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("A list with that name already exists.",
                                    "Duplicate List Name",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }

                var newList = new TodoList { Name = newListName };
                appData.Lists.Add(newList);
                RefreshListSelector();
                ListSelector.SelectedItem = newList;
            }
        }

        // Remove the currently selected list
        private void RemoveListButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = ListSelector.SelectedItem as TodoList;
            if (selectedList != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to remove the list '{selectedList.Name}'?",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    appData.Lists.Remove(selectedList);
                    RefreshListSelector();
                    TaskList.ItemsSource = null;
                }
            }
        }

        // When a different list is selected, show its tasks
        private void ListSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RefreshTaskList();
        }

        // Add a new task to the currently selected list
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = ListSelector.SelectedItem as TodoList;
            if (selectedList == null)
            {
                MessageBox.Show("Please select or create a list first.",
                                "No List Selected",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            string title = TaskInput.Text?.Trim();
            if (!string.IsNullOrEmpty(title))
            {
                var newTask = new TodoTask
                {
                    Title = title,
                    IsCompleted = false
                };
                selectedList.Tasks.Add(newTask);
                RefreshTaskList();
                TaskInput.Clear();
            }
        }

        // Remove a specific task from the current list
        private void RemoveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = ListSelector.SelectedItem as TodoList;
            if (selectedList == null) return;

            // The button's Tag property is bound to the Task object
            var button = sender as System.Windows.Controls.Button;
            var taskToRemove = button?.Tag as TodoTask;
            if (taskToRemove != null)
            {
                selectedList.Tasks.Remove(taskToRemove);
                RefreshTaskList();
            }
        }

        // Save all lists to a JSON file
        private void SaveTasks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(appData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText("tasks.json", jsonString);
                MessageBox.Show("All lists have been saved to tasks.json.",
                                "Save Successful",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving lists: {ex.Message}",
                                "Save Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        // Load all lists from the JSON file
        private void LoadTasks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists("tasks.json"))
                {
                    MessageBox.Show("No saved file found (tasks.json).",
                                    "Load Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                string jsonString = File.ReadAllText("tasks.json");
                var loadedData = JsonSerializer.Deserialize<AppData>(jsonString);
                if (loadedData != null)
                {
                    appData = loadedData;
                    RefreshListSelector();
                    MessageBox.Show("All lists have been loaded from tasks.json.",
                                    "Load Successful",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading lists: {ex.Message}",
                                "Load Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        //------------------------------------------------------------------
        //  HELPER METHODS
        //------------------------------------------------------------------

        // Refresh the ComboBox that displays the lists
        private void RefreshListSelector()
        {
            ListSelector.ItemsSource = null;
            ListSelector.ItemsSource = appData.Lists;
            ListSelector.DisplayMemberPath = "Name";
        }

        // Refresh the task list display for the currently selected list
        private void RefreshTaskList()
        {
            var selectedList = ListSelector.SelectedItem as TodoList;
            if (selectedList != null)
            {
                TaskList.ItemsSource = null;
                TaskList.ItemsSource = selectedList.Tasks;
            }
            else
            {
                TaskList.ItemsSource = null;
            }
        }
    }
}
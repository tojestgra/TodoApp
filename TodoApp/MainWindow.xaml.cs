using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Common.DTO;

namespace TodoApp
{
    public partial class MainWindow : Window
    {

        #region Dependency properties


        public ObservableCollection<TodoList> MyToDoLists
        {
            get { return (ObservableCollection<TodoList>)GetValue(MyToDoListsProperty); }
            set { SetValue(MyToDoListsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyToDoLists.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyToDoListsProperty =
            DependencyProperty.Register("MyToDoLists", typeof(ObservableCollection<TodoList>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<TodoList>()));




        public TodoList CurrentToDoList
        {
            get { return (TodoList)GetValue(CurrentToDoListProperty); }
            set { SetValue(CurrentToDoListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentToDoList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentToDoListProperty =
            DependencyProperty.Register("CurrentToDoList", typeof(TodoList), typeof(MainWindow), new PropertyMetadata(null));


        #endregion

        public bool IsLoading { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();


        }

        private void THIS_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //await Task.Delay(1500);

                btnLoadTasks_Click(sender, e);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


            IsLoading = false;
        }

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
                if (MyToDoLists.Any(l => l.Name.Equals(newListName, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("A list with that name already exists.",
                                    "Duplicate List Name",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }

                var newList = new TodoList { Name = newListName };

                MyToDoLists.Add(newList);

                //cbxListSelector.SelectedItem = newList;
            }
        }


        // Remove the currently selected list
        private void RemoveListButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = CurrentToDoList;

            if (selectedList != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to remove the list '{selectedList.Name}'?",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    MyToDoLists.Remove(selectedList);
                }
            }
        }

        // Add a new task to the currently selected list
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = CurrentToDoList;

            if (selectedList == null)
            {
                MessageBox.Show("Please select or create a list first.",
                                "No List Selected",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            string title = TaskInput.Text.Trim();

            if (!string.IsNullOrEmpty(title))
            {
                var newTask = new TodoTask
                {
                    Title = title,
                    IsCompleted = false
                };

                selectedList.Tasks.Add(newTask);

                TaskInput.Clear();
            }
        }

        // Remove a specific task from the current list
        private void RemoveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var to_do_item = (sender as Button).DataContext as TodoTask;

            if (to_do_item is null)
                return;

            CurrentToDoList.Tasks.Remove(to_do_item);
        }

        // Save all lists to a JSON file
        private void btnSaveTasks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(MyToDoLists, new JsonSerializerOptions
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
        private void btnLoadTasks_Click(object sender, RoutedEventArgs e)
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
                var loadedData = JsonSerializer.Deserialize<IEnumerable<TodoList>>(jsonString);

                if (loadedData != null)
                {
                    MyToDoLists = new ObservableCollection<TodoList>(loadedData);

                    //MyToDoLists.Clear();

                    //foreach (var todo in loadedData)
                    //{
                    //    MyToDoLists.Add(todo);
                    //};

                    Debug.WriteLine("All lists have been loaded from tasks.json.");
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

    }
}
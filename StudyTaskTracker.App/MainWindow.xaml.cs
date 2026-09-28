using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using StudyTaskTracker.Core.Models;
using StudyTaskTracker.Core.Services;
using StudyTaskTracker.Storage;

namespace StudyTaskTracker.App;

public partial class MainWindow : Window
{
    private TaskManager? _taskManager;

    private TaskFilter _currentFilter = TaskFilter.All;

    public MainWindow()
    {
        InitializeComponent();

        string dataDirectory = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "StudyTaskTracker");

        string dataFile = Path.Combine(
            dataDirectory,
            "tasks.json");

        var repository = new JsonTaskRepo(dataFile);

        _taskManager = new TaskManager(repository);

        RefreshAll();
    }

    private void AddTask_Click(object sender, RoutedEventArgs e)
    {
        if (_taskManager is null)
        {
            return;
        }

        string title = TitleTextBox.Text;
        string subject = SubjectTextBox.Text;

        TaskLevel priority = TaskLevel.Medium;

        if (PriorityComboBox.SelectedItem is ComboBoxItem item)
        {
            Enum.TryParse(
                item.Content?.ToString(),
                out priority);
        }

        try
        {
            _taskManager.AddTask(
                title,
                subject,
                DueDatePicker.SelectedDate,
                priority);

            TitleTextBox.Clear();
            SubjectTextBox.Clear();
            DueDatePicker.SelectedDate = null;
            PriorityComboBox.SelectedIndex = 1;

            _currentFilter = TaskFilter.All;
            FilterComboBox.SelectedIndex = 0;

            RefreshAll();

            StatusText.Text = "Task added successfully.";
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(
                ex.Message,
                "Invalid Task",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private void FilterComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (_taskManager is null)
        {
            return;
        }

        if (FilterComboBox.SelectedItem is ComboBoxItem item)
        {
            string? filterName = item.Content?.ToString();

            if (Enum.TryParse(filterName, out TaskFilter filter))
            {
                _currentFilter = filter;
                RefreshTasks();
            }
        }
    }

    private void TaskCheckBox_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_taskManager is null)
        {
            return;
        }

        if (sender is CheckBox checkBox &&
            checkBox.DataContext is TaskItem task)
        {
            bool isCompleted = checkBox.IsChecked == true;

            _taskManager.SetCompletion(
                task.Id,
                isCompleted);

            RefreshAll();

            StatusText.Text = isCompleted
                ? "Task marked as completed."
                : "Task marked as pending.";
        }
    }

    private void RefreshAll()
    {
        RefreshDashboard();
        RefreshTasks();
    }

    private void RefreshDashboard()
    {
        if (_taskManager is null)
        {
            return;
        }

        TotalCountText.Text =
            _taskManager.GetTotalCount().ToString();

        CompletedCountText.Text =
            _taskManager.GetCompletedCount().ToString();

        PendingCountText.Text =
            _taskManager.GetPendingCount().ToString();

        double percentage =
            _taskManager.GetCompletionPercentage();

        ProgressBarControl.Value = percentage;

        ProgressText.Text = $"{percentage:0}%";
    }

    private void RefreshTasks()
    {
        if (_taskManager is null)
        {
            return;
        }

        var tasks = _taskManager.GetTasks(_currentFilter);

        TaskList.ItemsSource = tasks;

        EmptyStateText.Visibility = tasks.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
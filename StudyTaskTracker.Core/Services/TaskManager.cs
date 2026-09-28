using StudyTaskTracker.Core.Models;
using StudyTaskTracker.Core.Repositories;

namespace StudyTaskTracker.Core.Services;

public class TaskManager
{
    private readonly ITaskRepo _repository;
    private readonly List<TaskItem> _tasks;

    public TaskManager(ITaskRepo repository)
    {
        _repository = repository;
        _tasks = _repository.LoadAll();
    }

    public IReadOnlyList<TaskItem> GetTasks(TaskFilter filter)
    {
        IEnumerable<TaskItem> result = _tasks;

        result = filter switch
        {
            TaskFilter.Pending => result.Where(t => !t.IsCompleted),
            TaskFilter.Completed => result.Where(t => t.IsCompleted),
            _ => result
        };

        return result
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .ToList();
    }

    public void AddTask(
        string title,
        string subject,
        DateTime? dueDate,
        TaskLevel level)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Task title cannot be empty.",
                nameof(title));
        }

        var task = new TaskItem
        {
            Title = title.Trim(),
            Subject = subject.Trim(),
            DueDate = dueDate,
            Level = level,
            IsCompleted = false
        };

        _tasks.Add(task);
        Save();
    }

    public void SetCompletion(Guid taskId, bool isCompleted)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);

        if (task is null)
        {
            throw new InvalidOperationException(
                $"Task with ID {taskId} was not found.");
        }

        task.IsCompleted = isCompleted;
        Save();
    }

    public int GetTotalCount()
    {
        return _tasks.Count;
    }

    public int GetCompletedCount()
    {
        return _tasks.Count(t => t.IsCompleted);
    }

    public int GetPendingCount()
    {
        return _tasks.Count(t => !t.IsCompleted);
    }

    public double GetCompletionPercentage()
    {
        if (_tasks.Count == 0)
        {
            return 0;
        }

        return (double)GetCompletedCount() / _tasks.Count * 100;
    }

    private void Save()
    {
        _repository.SaveAll(_tasks);
    }
}
namespace StudyTaskTracker.Core.Models;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public TaskLevel Level { get; set; } = TaskLevel.Medium;

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
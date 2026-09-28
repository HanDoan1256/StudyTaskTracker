using StudyTaskTracker.Core.Models;

namespace StudyTaskTracker.Core.Repositories;

public interface ITaskRepo
{
    List<TaskItem> LoadAll();

    void SaveAll(List<TaskItem> tasks);
}
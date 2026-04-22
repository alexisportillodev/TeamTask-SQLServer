using TaskStatusModel = TeamTasks.Api.Models.TaskStatus;

namespace TeamTasks.Api.Services;

public static class TaskStatusRules
{
    public static bool CanChangeStatus(TaskStatusModel from, TaskStatusModel to)
    {
        // Business rule: cannot go directly from Pending to Done.
        if (from == TaskStatusModel.Pending && to == TaskStatusModel.Done)
        {
            return false;
        }

        return true;
    }
}


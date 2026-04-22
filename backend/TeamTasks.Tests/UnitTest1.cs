using TeamTasks.Api.Services;
using TaskStatusModel = TeamTasks.Api.Models.TaskStatus;

namespace TeamTasks.Tests;

public sealed class TaskStatusRulesTests
{
    [Fact]
    public void CanChangeStatus_PendingToDone_IsNotAllowed()
    {
        var allowed = TaskStatusRules.CanChangeStatus(TaskStatusModel.Pending, TaskStatusModel.Done);
        Assert.False(allowed);
    }

    [Fact]
    public void CanChangeStatus_PendingToInProgress_IsAllowed()
    {
        var allowed = TaskStatusRules.CanChangeStatus(TaskStatusModel.Pending, TaskStatusModel.InProgress);
        Assert.True(allowed);
    }

    [Fact]
    public void CanChangeStatus_InProgressToDone_IsAllowed()
    {
        var allowed = TaskStatusRules.CanChangeStatus(TaskStatusModel.InProgress, TaskStatusModel.Done);
        Assert.True(allowed);
    }
}

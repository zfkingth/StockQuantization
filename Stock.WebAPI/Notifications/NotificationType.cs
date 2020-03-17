namespace Stock.WebAPI.Notifications
{
    public enum NotificationType
    {
        TaskStart, TaskProgress, TaskFail, TaskSuccess,
        LIKE, UNLIKE, SHARE, STORY_EDIT
    }
}
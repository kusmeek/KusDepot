namespace DataPodServices.DataControl;

internal static class DataControlNotificationTopics
{
    public static String ForItem(Guid itemId) => $"item:{itemId:D}";

    public static String ForSession(Guid sessionId) => $"session:{sessionId:D}";

    public static String ForSourceSession(Guid sourceSessionId) => $"source:{sourceSessionId:D}";
}

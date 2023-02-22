using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;

namespace Ical.Net.CoreUnitTests;

internal static class SerializationHelpers
{
    public static string SerializeToString(CalendarEvent calendarEvent)
        => SerializeToString(new Calendar { Events = { calendarEvent } });

    public static string SerializeToString(Calendar iCalendar)
        => new CalendarSerializer().SerializeToString(iCalendar);
}
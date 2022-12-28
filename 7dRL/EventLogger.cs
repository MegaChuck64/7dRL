
using System.Collections.Generic;
using System.Windows.Controls;

namespace _7dRL;

public static class EventLogger
{
    public static List<string> Events { get; private set; } = new List<string>();
    public static ListBox? EventListBox { get; private set; }

    public static void Init(ListBox eventListBox)
    {
        EventListBox = eventListBox;
        EventListBox.SelectionChanged += EventListBox_SelectionChanged;
    }

    private static void EventListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (EventListBox != null)
        {
            EventListBox.ScrollIntoView(EventListBox.SelectedItem);
        }
    }

    public static void AddEvent(string message, bool includeLineSeperator = false)
    {
        Events.Add(message);

        if (EventListBox != null)
        {
            EventListBox.Items.Add(message);
            EventListBox.SelectedIndex = EventListBox.Items.Count - 1;
            if (includeLineSeperator)
            {
                EventListBox.Items.Add("_______________________________________");
            }      
        }
    }
}
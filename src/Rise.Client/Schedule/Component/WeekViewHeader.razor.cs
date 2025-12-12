using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule.Component;

/// <summary>
/// Week view header component displaying day buttons and navigation.
/// Shows the week range and allows day selection.
/// </summary>
public partial class WeekViewHeader : ComponentBase
{
    /// <summary>Days in the current week.</summary>
    [Parameter]
    public List<DateTime>? WeekDays { get; set; }

    /// <summary>Week number to display.</summary>
    [Parameter]
    public int WeekNumber { get; set; }

    /// <summary>Callback when calendar icon is clicked.</summary>
    [Parameter]
    public EventCallback OnCalendarClick { get; set; }

    /// <summary>Callback when today button is clicked.</summary>
    [Parameter]
    public EventCallback OnTodayButtonClick { get; set; }

    /// <summary>Currently selected date.</summary>
    [Parameter]
    public DateTime? SelectedDate { get; set; }

    /// <summary>Callback when a day is selected.</summary>
    [Parameter]
    public EventCallback<DateTime> OnDaySelected { get; set; }

    /// <summary>
    /// Handles day selection.
    /// </summary>
    private async Task SelectDay(DateTime day)
    {
        if (OnDaySelected.HasDelegate)
        {
            await OnDaySelected.InvokeAsync(day);
        }
    }

    /// <summary>
    /// Gets the abbreviated day name (e.g., MON, TUE) for a date.
    /// </summary>
    /// <param name="day">The date to get the abbreviation for.</param>
    /// <returns>Uppercase abbreviated day name.</returns>
    private string GetDayAbbreviation(DateTime day)
    {
        return day.ToString("ddd", System.Globalization.CultureInfo.CurrentCulture).ToUpper();
    }

    /// <summary>
    /// Gets the week range title (e.g., "01/01 - 07/01").
    /// </summary>
    /// <returns>Formatted date range string.</returns>
    private string GetWeekRangeTitle()
    {
        if (WeekDays == null || !WeekDays.Any())
            return string.Empty;

        return $"{WeekDays.First():dd/MM} - {WeekDays.Last():dd/MM}";
    }
}

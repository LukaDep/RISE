using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule.Component;

public partial class WeekViewHeader : ComponentBase
{
    [Parameter]
    public List<DateTime>? WeekDays { get; set; }

    [Parameter]
    public int WeekNumber { get; set; }

    [Parameter]
    public EventCallback OnCalendarClick { get; set; }

    [Parameter]
    public EventCallback OnTodayButtonClick { get; set; }

    [Parameter]
    public DateTime? SelectedDate { get; set; }

    [Parameter]
    public EventCallback<DateTime> OnDaySelected { get; set; }

    private async Task SelectDay(DateTime day)
    {
        if (OnDaySelected.HasDelegate)
        {
            await OnDaySelected.InvokeAsync(day);
        }
    }

    private string GetDayAbbreviation(DateTime day)
    {
        return day.ToString("ddd", System.Globalization.CultureInfo.CurrentCulture).ToUpper();
    }

    private string GetWeekRangeTitle()
    {
        if (WeekDays == null || !WeekDays.Any())
            return string.Empty;

        return $"{WeekDays.First():dd/MM} - {WeekDays.Last():dd/MM}";
    }
}

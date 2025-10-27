using Microsoft.AspNetCore.Components;
using Rise.Shared.Resto;

namespace Rise.Client.Resto.Components;

public partial class RestoCard
{
    [Parameter, EditorRequired]
    public RestoDto.Index Resto { get; set; } = default!;

    private bool showOpeningHours = false;

    private void ToggleOpeningHours()
    {
        showOpeningHours = !showOpeningHours;
    }

    private IEnumerable<KeyValuePair<DayOfWeek, string>> GetSortedOpeningHours()
    {
        if (Resto.OpeningHours == null) return Enumerable.Empty<KeyValuePair<DayOfWeek, string>>();

        var today = DateTime.Now.DayOfWeek;
        var sorted = Resto.OpeningHours
            .OrderBy(x => ((int)x.Key - (int)today + 7) % 7)
            .ToList();

        return sorted;
    }

    private bool IsToday(DayOfWeek day)
    {
        return day == DateTime.Now.DayOfWeek;
    }

    private string GetDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => L["Day.Monday"],
            DayOfWeek.Tuesday => L["Day.Tuesday"],
            DayOfWeek.Wednesday => L["Day.Wednesday"],
            DayOfWeek.Thursday => L["Day.Thursday"],
            DayOfWeek.Friday => L["Day.Friday"],
            DayOfWeek.Saturday => L["Day.Saturday"],
            DayOfWeek.Sunday => L["Day.Sunday"],
            _ => day.ToString()
        };
    }

    private string GetOpeningHoursDisplay(string? hours)
    {
        return string.IsNullOrWhiteSpace(hours) ? L["Resto.Closed"] : hours;
    }
}

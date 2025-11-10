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

        var sorted = Resto.OpeningHours
            .OrderBy(x => x.Key == DayOfWeek.Sunday ? 7 : (int)x.Key)
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

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        UpdateRestoStatus();
    }

    private void UpdateRestoStatus()
    {
        if (Resto.OpeningHours == null || Resto.OpeningHours.Count == 0)
        {
            Resto.IsCurrentlyOpen = false;
            Resto.NextOpeningTime = null;
            Resto.NextClosingTime = null;
            return;
        }

        var now = DateTime.Now;
        var today = now.DayOfWeek;

        if (Resto.OpeningHours.TryGetValue(today, out var todayHours) && !string.IsNullOrWhiteSpace(todayHours))
        {
            var timeRanges = todayHours.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var range in timeRanges)
            {
                var parts = range.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length != 2) continue;

                if (TimeSpan.TryParse(parts[0], out var openTime) && TimeSpan.TryParse(parts[1], out var closeTime))
                {
                    var openDateTime = now.Date.Add(openTime);
                    var closeDateTime = now.Date.Add(closeTime);

                    if (now >= openDateTime && now <= closeDateTime)
                    {
                        // Resto is nu open
                        Resto.IsCurrentlyOpen = true;
                        Resto.NextClosingTime = closeDateTime;
                        Resto.NextOpeningTime = null;
                        return;
                    }

                    // Als hij nog niet open is vandaag
                    if (now < openDateTime)
                    {
                        Resto.IsCurrentlyOpen = false;
                        Resto.NextOpeningTime = openDateTime;
                        Resto.NextClosingTime = null;
                        return;
                    }
                }
            }
        }
        
        Resto.IsCurrentlyOpen = false;
        Resto.NextClosingTime = null;
        Resto.NextOpeningTime = GetNextOpeningTime(now);
    }

    private DateTime? GetNextOpeningTime(DateTime now)
    {
        // Loop maximaal 7 dagen vooruit
        for (int i = 1; i <= 7; i++)
        {
            var nextDay = now.AddDays(i);
            var nextDayOfWeek = nextDay.DayOfWeek;

            if (Resto.OpeningHours.TryGetValue(nextDayOfWeek, out var hours) && !string.IsNullOrWhiteSpace(hours))
            {
                var firstRange = hours.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
                if (firstRange != null)
                {
                    var parts = firstRange.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length == 2 && TimeSpan.TryParse(parts[0], out var openTime))
                    {
                        return nextDay.Date.Add(openTime);
                    }
                }
            }
        }

        return null;
    }
}

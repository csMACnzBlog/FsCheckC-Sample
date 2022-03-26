namespace TimeSlice.Api.Commands.GetHoursBetween;

public class Handler : IRequestHandler<Request, IReadOnlyList<DateTime>>
{
    public Task<IReadOnlyList<DateTime>> Handle(Request request, CancellationToken cancellationToken)
    {
        var startHour = ToHour(request.Start);
        var difference = request.End - startHour;

        var resultCount = difference >= TimeSpan.Zero
            ? (int)Math.Floor(difference.TotalHours) + 1
            : 0;

        return Task.FromResult<IReadOnlyList<DateTime>>(Range(startHour, resultCount).ToList());
    }

    private DateTime ToHour(DateTime input)
    {
        var hour = new DateTime(input.Year, input.Month, input.Day, input.Hour, 0, 0, input.Kind);
        if(hour != input)
        {
            hour += TimeSpan.FromHours(1);
        }

        return hour;
    }

    private static IEnumerable<DateTime> Range(DateTime start, int resultCount)
    {
        if (resultCount > 0)
        {
            for (var i = 0; i < resultCount; i++)
            {
                yield return start.AddHours(i);
            }
        }
    }
}

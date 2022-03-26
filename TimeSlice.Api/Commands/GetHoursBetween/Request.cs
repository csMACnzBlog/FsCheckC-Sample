namespace TimeSlice.Api.Commands.GetHoursBetween;

public class Request : IRequest<IReadOnlyList<DateTime>>
{
    public Request(DateTime start, DateTime end)
    {
        if (end < start) throw new ArgumentOutOfRangeException(nameof(end));

        Start = start;
        End = end;
    }

    public DateTime Start { get; }
    public DateTime End { get; }
}

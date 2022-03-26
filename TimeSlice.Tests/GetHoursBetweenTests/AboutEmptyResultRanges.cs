using TimeSlice.Api.Commands.GetHoursBetween;

namespace TimeSlice.Tests.GetHoursBetweenTests;

public class AboutEmptyResultRanges
{
    [Property]
    public async Task ZeroRangeDatesReturnNoResults(DateTime dateTime)
    {
        var request = new Request(dateTime, dateTime);
        var handler = new Handler();
        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().HaveCount(0);
    }

    [Property]
    public async Task RangeNotSpanningAnHourBoundaryReturnNoResults(DateTime start)
    {
        var request = new Request(start, start.AddMinutes(59 - start.Minute).AddSeconds(59 - start.Second));
        var handler = new Handler();
        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().HaveCount(0);
    }

    [Property]
    public Property RangeNotSpanningAnHourBoundaryReturnNoResults_V2()
    {
        return Prop.ForAll(
            ArbMap.Default.ArbFor<DateTime>(),
            Gen.Choose(0, 59).ToArbitrary(),
            (start, endMinute) => Prop.When(
                start.Minute < endMinute,
                async () =>
                {
                    var request = new Request(start, start.AddMinutes(endMinute - start.Minute));
                    var handler = new Handler();
                    var result = await handler.Handle(request, CancellationToken.None);

                    result.Should().HaveCount(0);
                }));
    }
}

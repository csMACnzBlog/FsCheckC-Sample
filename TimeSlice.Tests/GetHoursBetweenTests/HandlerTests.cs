using TimeSlice.Api.Commands.GetHoursBetween;

namespace TimeSlice.Tests.GetHoursBetweenTests;

public class HandlerTests
{
    [Property]
    public Property AnyValidRangeWorks(DateTime start, DateTime end)
    {
        return Prop.When(
            start < end,
            async () =>
            {
                var request = new Request(start, end);
                var handler = new Handler();
                var result = await handler.Handle(request, CancellationToken.None);

                result.Should().NotBeNullOrEmpty();
            })
            .Classify(end-start < TimeSpan.FromSeconds(1), "Tiny Ranges")
            .Classify(end - start > TimeSpan.FromDays(1), "Day Ranges")
            .Classify(end - start < TimeSpan.FromDays(365), "Over a Year Ranges");
    }

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

    [Property]
    public Property RangeSpanningNumberOfHoursReturnThatNumberOfResults(DateTime start, int numberOfHours)
    {
        return Prop.When(
            numberOfHours > 0 && start < DateTime.MaxValue.AddHours(-numberOfHours),
            async () =>
            {
                var request = new Request(start, start.AddHours(numberOfHours));
                var handler = new Handler();
                var result = await handler.Handle(request, CancellationToken.None);

                result.Should().HaveCount(numberOfHours);
            });
    }

    [Property]
    public async Task RangeStartingOnTheHourSpanningOneHourReturnsTheTwoHoursInResult(DateTime start)
    {
        var expectedDateTime = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);

        var request = new Request(expectedDateTime, expectedDateTime.AddHours(1));
        var handler = new Handler();
        var result = await handler.Handle(request, CancellationToken.None);


        result.Should().SatisfyRespectively(
            d => d.Should().Be(expectedDateTime),
            d => d.Should().Be(expectedDateTime.AddHours(1)));
    }

    [Property]
    public Property RangeNotStartingOnTheHourSpanningOneHourReturnsThatCorrectHourInResult(DateTime start)
    {
        return Prop.When(
            start.Minute != 0 || start.Second != 0 || start.Millisecond != 0,
            async () =>
            {
                var request = new Request(start, start.AddHours(1));
                var handler = new Handler();
                var result = await handler.Handle(request, CancellationToken.None);

                var expectedDateTime = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0).AddHours(1);
                result.Should().SatisfyRespectively(
                    d => d.Should().Be(expectedDateTime));
            });
    }
}

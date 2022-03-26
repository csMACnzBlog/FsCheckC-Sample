using TimeSlice.Api.Commands.GetHoursBetween;

namespace TimeSlice.Tests.GetHoursBetweenTests;

public class AboutRangesWithResults
{
    private static bool IsNotHourAligned(DateTime dateTime) => (dateTime.Minute != 0 || dateTime.Second != 0 || dateTime.Millisecond != 0);

    [Property(MaxTest = 200)]
    public Property RangeSpanningNumberOfHoursReturnThatNumberOfResults(DateTime start, int numberOfHours)
    {
        return Prop.When(
            numberOfHours > 0 && start < DateTime.MaxValue.AddHours(-numberOfHours)
            && IsNotHourAligned(start),
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
            IsNotHourAligned(start),
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

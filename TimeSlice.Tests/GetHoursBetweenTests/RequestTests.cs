using TimeSlice.Api.Commands.GetHoursBetween;

namespace TimeSlice.Tests.GetHoursBetweenTests;

public class RequestTests
{
    [Property]
    public Property ValidRequestCanBeConstructed(DateTime start, DateTime end)
    {
        return Prop.When(
            start <= end,
            () =>
            {
                var request = new Request(start, end);
                request.Should().NotBeNull();
            });
    }

    [Property]
    public Property InvalidRequestsThrowsArgumentOutOfRangeException(DateTime start, DateTime end)
    {
        return Prop.When(
            start > end,
            () =>
            {
                var action = () => new Request(start, end);
                action.Should().Throw<ArgumentOutOfRangeException>();
            });
    }
}
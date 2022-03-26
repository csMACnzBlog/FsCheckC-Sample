using TimeSlice.Api.Commands.GetHoursBetween;

namespace TimeSlice.Tests.GetHoursBetweenTests;

public class AboutNotCrashing
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

                result.Should().NotBeNull();
            })
            .Classify(end-start < TimeSpan.FromSeconds(1), "Tiny Ranges")
            .Classify(end - start > TimeSpan.FromDays(1), "Day Ranges")
            .Classify(end - start < TimeSpan.FromDays(365), "Over a Year Ranges");
    }

    private static class ValidRequests
    {
        public static Arbitrary<Request> Arbitrary()
        {
            return Gen.Zip(ArbMap.Default.GeneratorFor<DateTime>(), ArbMap.Default.GeneratorFor<DateTime>())
                .Where<(DateTime start, DateTime end)>(x => x.start < x.end)
                .Select(x => new Request(x.start, x.end))
                .ToArbitrary();
        }
    }

    [Property(Arbitrary = new[] {typeof(ValidRequests) })]
    public Property AnyValidRangeWorks_V2(Request request)
    {
        return Prop.ToProperty(async () =>
        {
            var handler = new Handler();
            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().NotBeNull();
        })
        .Classify(request.End - request.Start < TimeSpan.FromSeconds(1), "Tiny Ranges")
        .Classify(request.End - request.Start > TimeSpan.FromDays(1), "Day Ranges")
        .Classify(request.End - request.Start < TimeSpan.FromDays(365), "Over a Year Ranges");
    }

    [Property(Arbitrary = new[] { typeof(ValidRequests) })]
    public async Task AnyValidRangeWorks_V3(Request request)
    {
        var handler = new Handler();
        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
    }
}

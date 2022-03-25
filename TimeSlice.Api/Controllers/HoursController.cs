using Microsoft.AspNetCore.Mvc;

namespace TimeSlice.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HoursController : ControllerBase
{
    private readonly ILogger<HoursController> _logger;

    public HoursController(ILogger<HoursController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<DateTime> GetHoursBetween(DateTime start, DateTime end)
    {
        return Enumerable.Range(1, 5)
        .Select(index => DateTime.UtcNow.Date + TimeSpan.FromHours(index))
        .ToArray();
    }
}

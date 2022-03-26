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
    public async Task<IEnumerable<DateTime>> GetHoursBetween(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromServices] IMediator mediator)
    {
        try
        {
            Commands.GetHoursBetween.Request request = new(start, end);

            return await mediator.Send(request);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new BadHttpRequestException(ex.Message, ex);
        }
    }
}

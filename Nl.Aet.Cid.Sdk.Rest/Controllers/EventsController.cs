using Microsoft.AspNetCore.Mvc;
using Nl.Aet.Cid.Sdk.Rest.Sdk;
using System.Net;

namespace Nl.Aet.Cid.Web.Connectors.Rest.Applicant.Controllers;

[ApiController]
[Produces("application/json")]
[Route("v1/")]
public class EventsController : ControllerBase
{
    private readonly Wrapper _wrapper;

    public EventsController(Wrapper wrapper)
    {
        _wrapper = wrapper;
    }


    // Retrieves a list of pending events for an applicant. 
    [HttpGet("events")]
    public async Task<IActionResult> GetEventMessages()
    {
        List<Message> messages = await Task.Run(() => messages = _wrapper.Messages);
        return StatusCode((int)HttpStatusCode.OK, messages);
    }
}

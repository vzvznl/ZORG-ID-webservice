using Microsoft.AspNetCore.Mvc;
using Nl.Aet.Cid.Sdk.Rest.Sdk;
using Serilog;
using System.Net;

namespace Nl.Aet.Cid.Web.Connectors.Rest.Applicant.Controllers;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public class SessionsController : ControllerBase
{
    private readonly Wrapper _wrapper;

    public SessionsController(Wrapper wrapper)
    {
        _wrapper = wrapper;
    }


    // Closes an open session. 
    [HttpPost("close")]
    public async Task<IActionResult> Close(Guid sessionId)
    {
        const string method = nameof(Close);
        string action = $"{method}, Details: sessionId: {sessionId}";

        try
        {
            await Task.Run(action: () => _wrapper.Manager.SessionManager.Close(sessionId));
            return StatusCode((int)HttpStatusCode.OK);

        }
        catch (Exception exception)
        {
            Log.Error(exception, action, exception);
            return StatusCode((int)HttpStatusCode.InternalServerError, action);
        }
    }

    // Opens a session. 
    [HttpPost("open")]
    public async Task<IActionResult> Open()
    {
        const string method = nameof(Open);
        string action = $"{method}, Details: <nan>";

        try
        {
            var sessionId =await Task.Run(() => _wrapper.Manager.SessionManager.OpenDetached());
            Identifier identifier = new Identifier() { Id=sessionId};
            return StatusCode((int)HttpStatusCode.OK, identifier);        }
        catch (Exception exception)
        {
            Log.Error(exception, action, exception);
            return StatusCode((int)HttpStatusCode.InternalServerError, action);
        }
    }

    private class Identifier { public Guid Id { get; set; } }

}

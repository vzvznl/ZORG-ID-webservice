using Microsoft.AspNetCore.Mvc;
using Nl.Aet.Cid.Client.Sdk;
using Nl.Aet.Cid.Sdk.Rest.Sdk;
using System.Net;

namespace Nl.Aet.Cid.Web.Connectors.Rest.Applicant.Controllers;

[ApiController]
[Produces("application/json")]
[Route("v1/[controller]")]
public class TransactionsController : ControllerBase
{

    private readonly Wrapper _wrapper;

    public TransactionsController(Wrapper wrapper)
    {
        _wrapper = wrapper;
    }

    // Posts a new signing request.  
    [HttpPost]
    public async Task<IActionResult> SubmitSignDataRequest(SignDataRequestBag request)
    {
        var transactionId = await Task.Run(() => _wrapper.Manager.TransactionManager.Sign(request.SessionId, request));
        Identifier identifier = new Identifier() { Id = transactionId };
        return StatusCode((int)HttpStatusCode.OK,identifier);
    }

    private class Identifier { public Guid Id { get; set; } }
}

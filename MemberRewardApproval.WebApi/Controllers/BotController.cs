using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
namespace MemberRewardApproval.WebApi.Controllers;

[Route("api/messages")]
[ApiController]
public class BotController : ControllerBase
{
    private readonly IBot _bot;
    private readonly BotFrameworkHttpAdapter _adapter;

    public BotController(IBot bot, BotFrameworkHttpAdapter adapter)
    {
        _bot = bot;
        _adapter = adapter;
    }

    [HttpPost]
    public async Task PostAsync()
    {
        await _adapter.ProcessAsync(Request, Response, _bot);
    }






}

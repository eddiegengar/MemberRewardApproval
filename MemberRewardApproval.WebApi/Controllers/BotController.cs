using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.AspNetCore.Authorization;

namespace MemberRewardApproval.WebApi.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class BotController : ControllerBase
    {
        private readonly IBot _bot;
        private readonly CloudAdapter _adapter;

        public BotController(IBot bot, CloudAdapter adapter)
        {
            _bot = bot;
            _adapter = adapter;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task PostAsync()
        {
            await _adapter.ProcessAsync(Request, Response, _bot);
        }
    }
}
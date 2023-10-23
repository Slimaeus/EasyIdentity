using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EasyIdentity.ControllerApi.Hubs;

[Authorize]
public sealed class ChatHub : Hub<IChatHubClient>
{
}

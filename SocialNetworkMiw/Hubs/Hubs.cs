using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string who, string message)
        {
            string name = Context.User.Identity.Name;
            await Clients.Group(who).SendAsync("ReceiveMessage", name, message);
        }

        public override Task OnConnectedAsync()
        {
            var claimsIdentity = Context.User.Identity as System.Security.Claims.ClaimsIdentity;
            var id = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            Groups.AddToGroupAsync(Context.ConnectionId, id);
            return base.OnConnectedAsync();
        }
    }
}
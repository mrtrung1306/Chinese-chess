using Libs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace DemoQLDA.Hubs
{

    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "DepartmentPolicy")]

    public class ChatHub : Hub
    {

        public async Task SendMessage(string user, string message)
        {
                await Clients.All.SendAsync("ReceiveMessage", user, message); 
      
        }
        public async Task SendToGroup(string roomId,string user, string message)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            await Clients.Group(roomId).SendAsync("ReceiveMessage", user, message);
        }
        public override Task OnConnectedAsync()
        {
            string id = Context.ConnectionId;
            return base.OnConnectedAsync();
        }
        public string GetConnectionId()
{
    return Context.ConnectionId;
}
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string id = Context.ConnectionId;
            return base.OnDisconnectedAsync(exception);
        }
    }
}

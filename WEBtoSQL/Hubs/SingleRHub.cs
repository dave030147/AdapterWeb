using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class SingleRHub : Hub
    {
        public async Task SendMessage(string message1, string message2, string message3, string message4, string message5, string message6, string message7, 
            string message8, string message9, string message10, string message11, string message12, string message13, string message14, string message15, 
            string message16, string message17, string message18, string message19, string message20, string message21, string message22, string message23, 
            string message24, string message25, string message26, string message27, string message28, string message29,string message30)
        {
            await Clients.All.SendAsync("ReceiveMessage1", message1, message2, message3, message4, message5, message6, message7,message8, message9, message10);
            await Clients.All.SendAsync("ReceiveMessage2", message11, message12, message13, message14, message15, message16, message17,message18, message19, message20);
            await Clients.All.SendAsync("Receivemessage3", message21, message22, message23, message24, message25, message26, message27,message28, message29, message30);
        }
    }
}

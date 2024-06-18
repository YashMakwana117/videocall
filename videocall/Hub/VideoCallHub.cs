using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;

public class VideoCallHub : Hub
{
    private static Dictionary<string, string> connectedUsers = new Dictionary<string, string>();

    public void Register(string username)
    {
        var connectionId = Context.ConnectionId;
        if (!connectedUsers.ContainsKey(username))
        {
            connectedUsers.Add(username, connectionId);
        }
    }

    public void Send(string message)
    {
        var signal = Newtonsoft.Json.JsonConvert.DeserializeObject<SignalMessage>(message);
        if (connectedUsers.TryGetValue(signal.ToUser, out string connectionId))
        {
            Clients.Client(connectionId).receiveMessage(message);
        }
    }

    public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
    {
        var item = connectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (item.Key != null)
        {
            connectedUsers.Remove(item.Key);
        }
        return base.OnDisconnected(stopCalled);
    }

    public class SignalMessage
    {
        public string Type { get; set; }
        public string ToUser { get; set; }
        public string FromUser { get; set; }
        public object Sdp { get; set; }
        public object Candidate { get; set; }
    }
}

using Microsoft.AspNetCore.SignalR;

namespace Syncer.APIs.Hubs;

public class BoardHub(PresentationService presentationService) : Hub
{
    private readonly PresentationService _presentationService = presentationService;

    public override Task OnConnectedAsync()
    {
        _presentationService.Add(new OnlinePresentation
        {
            ConnectionId = Context.ConnectionId,
            PresentationId = GetCallerPresentationId()

        });

        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _presentationService.Remove(Context.ConnectionId);
        return Task.CompletedTask;
    }

    public string? GetCallerPresentationId()

    {
        var ITEM = Context.GetHttpContext().Request.Cookies;

        if (Context.GetHttpContext().Request.Cookies.TryGetValue("cid", out string? pid))
            return pid.Replace("-"," ");


        return null;
    }
}


public class OnlinePresentation
{
    public string ConnectionId { get; set; }

    public string PresentationId { get; set; }
}

public class PresentationService
{
    private readonly List<OnlinePresentation> _presentations = [];

    public void Add(OnlinePresentation onlinePresentation) => _presentations.Add(onlinePresentation);

    public void Remove(string Connectionid)
    {
        var presentation = _presentations.FirstOrDefault(x => x.ConnectionId == Connectionid);
        _presentations.Remove(presentation);
    }

    internal string GetConnectionIdByPresentationId(string presentationId)
    {
        var item = _presentations.FirstOrDefault(x => x.PresentationId == presentationId);
        return item is null ? string.Empty : item.ConnectionId;
    }
}
using Asp.Versioning;
using Doorfail.Core.Shop.Web.Api.Models.Stream;
using Microsoft.AspNetCore.Mvc;
using Stream;
using StreamChat.Clients;
using StreamChat.Models;

namespace Doorfail.Core.Shop.Web.Api.V1.Stream;

[Route("api/v1/stream/[controller]/[action]")]
[Controller]
[ApiVersion(1.0)]
[ApiVersion(0.9, Deprecated = true)]
public class WebhookController :ControllerBase
{
    protected readonly ICommandClient CommandClient;
    protected readonly IChannelClient ChannelClient;
    protected readonly IStreamClient socialClient;
    protected readonly IPermissionClient PermissionClient;
    protected readonly IEventClient EventClient;
    protected readonly IAppClient AppClient;
    protected readonly IMessageClient MessageClient;
   

    public WebhookController(IStreamClientFactory clientFactory, IStreamClient socialClient)
    {
        CommandClient = clientFactory.GetCommandClient();
        ChannelClient = clientFactory.GetChannelClient();
        PermissionClient = clientFactory.GetPermissionClient();
        EventClient = clientFactory.GetEventClient();
        AppClient = clientFactory.GetAppClient();
        MessageClient = clientFactory.GetMessageClient();


    }

    [HttpGet]
    [Route("api/v1/setupWebhooks")]
    public async Task SetupWebhooks()
    {
        await AppClient.UpdateAppSettingsAsync(new AppSettingsRequest
        {
            WebhookUrl = "https://api.doorfail.com/webhook/stream/p",
            BeforeMessageSendHookUrl = "https://api.doorfail.com/webhook/stream/bs",
            CustomActionHandlerUrl = "https://api.doorfail.com/webhook/stream/ca/{type}",
            SqsUrl = "https://api.doorfail.com/webhook/stream/sqs"
        });
    }

    [HttpGet]
    public async Task<StreamRequest> P([FromBody] StreamRequest request)
    {
        return request.type switch
        {
            WebhookEventType.MESSAGE_NEW => throw new NotImplementedException(),
            WebhookEventType.MESSAGE_READ => throw new NotImplementedException(),
            WebhookEventType.MESSAGE_UPDATED => throw new NotImplementedException(),
            WebhookEventType.MESSAGE_DELETED => throw new NotImplementedException(),
            WebhookEventType.MESSAGE_FLAGGED => throw new NotImplementedException(),
            WebhookEventType.REACTION_NEW => throw new NotImplementedException(),
            WebhookEventType.REACTION_DELETED => throw new NotImplementedException(),
            WebhookEventType.REACTION_UPDATED => throw new NotImplementedException(),
            WebhookEventType.MEMBER_ADDED => throw new NotImplementedException(),
            WebhookEventType.MEMBER_UPDATED => throw new NotImplementedException(),
            WebhookEventType.MEMBER_REMOVED => throw new NotImplementedException(),
            WebhookEventType.CHANNEL_CREATED => throw new NotImplementedException(),
            WebhookEventType.CHANNEL_UPDATED => throw new NotImplementedException(),
            WebhookEventType.CHANNEL_MUTED => throw new NotImplementedException(),
            WebhookEventType.CHANNEL_UNMUTED => throw new NotImplementedException(),
            WebhookEventType.CHANNEL_TRUNCATED => throw new NotImplementedException(),
            WebhookEventType.CHANNEL_DELETED => throw new NotImplementedException(),
            WebhookEventType.USER_DEACTIVATED => throw new NotImplementedException(),
            WebhookEventType.USER_DELETED => throw new NotImplementedException(),
            WebhookEventType.USER_REACTIVATED => throw new NotImplementedException(),
            WebhookEventType.USER_UPDATED => throw new NotImplementedException(),
            WebhookEventType.USER_MUTED => throw new NotImplementedException(),
            WebhookEventType.USER_UNMUTED => throw new NotImplementedException(),
            WebhookEventType.USER_BANNED => throw new NotImplementedException(),
            WebhookEventType.USER_UNBANNED => throw new NotImplementedException(),
            WebhookEventType.USER_FLAGGED => throw new NotImplementedException(),
            WebhookEventType.USER_UNREAD_MESSAGE_REMINDER => throw new NotImplementedException(),
        };
    }

    [HttpGet]
    public async Task<BeforeMessageSend> Bs([FromBody] BeforeMessageSend request)
    {
        return request;
    }

    [HttpGet]
    public async Task<CommandRequest> Ca([FromBody] CommandRequest request)
    {
        return request;
    }

    //[HttpGet]
    //public async Task<object> Sqs([FromBody] SqsRequest request)
    //{
    //    request.
    //}
}

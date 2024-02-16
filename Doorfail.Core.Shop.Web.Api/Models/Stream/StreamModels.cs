using System.Text.Json.Serialization;
using StreamChat.Models;
using SC = StreamChat.Models;

namespace Doorfail.Core.Shop.Web.Api.Models.Stream;

public enum WebhookEventType
{
    MESSAGE_NEW,
    MESSAGE_READ,
    MESSAGE_UPDATED,
    MESSAGE_DELETED,
    MESSAGE_FLAGGED,
    REACTION_NEW,
    REACTION_DELETED,
    REACTION_UPDATED,
    MEMBER_ADDED,
    MEMBER_UPDATED,
    MEMBER_REMOVED,
    CHANNEL_CREATED,
    CHANNEL_UPDATED,
    CHANNEL_MUTED,
    CHANNEL_UNMUTED,
    CHANNEL_TRUNCATED,
    CHANNEL_DELETED,
    USER_DEACTIVATED,
    USER_DELETED,
    USER_REACTIVATED,
    USER_UPDATED,
    USER_MUTED,
    USER_UNMUTED,
    USER_BANNED,
    USER_UNBANNED,
    USER_FLAGGED,
    USER_UNREAD_MESSAGE_REMINDER
}

public class StreamRequest
{
    public SC.Message message;
    public SC.User user;
    [JsonConverter(typeof(WebhookEventTypeConverter))]
    public WebhookEventType type;
    public string cid;
    public string channel_type;
    public string channel_id;
    public DateTime created_at;
    public RequestInfo request_info;
}

public class PostRequest :StreamRequest
{
    public List<Member> members;
}

public class MessageFlagged :StreamRequest
{
    public Channel channel;
    public bool automoderation;
    public Dictionary<string, int> automoderation_scores;
}

public struct Member
{
    public string user_id;
    public object user; // this might need further definition based on its actual structure
    public DateTime created_at;
    public DateTime updated_at;
    public bool notifications_muted;
}

public struct Config
{
    public DateTime created_at;
    public DateTime updated_at;
    public string name;
    public bool typing_events;
    public bool read_events;
    public bool connect_events;
    public bool search;
    public bool reactions;
    public bool replies;
    public bool mutes;
    public bool uploads;
    public bool url_enrichment;
    public string message_retention;
    public int max_message_length;
    public string automod;
    public string automod_behavior;
    public object[] commands;
}

public struct RequestInfo
{
    public string type;
    public string ip;
    public string user_agent;
    public string sdk;
}

public struct BeforeMessageSend
{
    public SC.Message message;
    public SC.User user;
    public SC.Channel channel;
    public RequestInfo request_info;
}

public struct FormData
{
    public string action;
    public string name;
    public string email;
}

public struct CommandRequest
{
    public Message message;
    public User user;
    public FormData form_data;
}

public struct UserBanned
{
    [JsonConverter(typeof(WebhookEventTypeConverter))]
    public WebhookEventType type;
    public User user;
    public string reason;
    public DateTime created_by;
    public DateTime created_at;
    public DateTime expiration;
}

public class UserUnreadMessageReminder
{
    [JsonConverter(typeof(WebhookEventTypeConverter))]
    public WebhookEventType type { get; set; }
    public DateTime created_at { get; set; }
    public User user { get; set; }
    public Dictionary<string, ChannelData> channels { get; set; }
}

public class ChannelData
{
    public SC.Channel channel { get; set; }
    public List<Message> messages { get; set; }
}
namespace BabbysFirstBot
{
    public enum PermissionLevel : byte
    {
        User = 0,
        ChannelModerator,   //Manage messages (Channel)
        ChannelAdmin,       //Manage permissions (Channel)
        ServerModerator,    //Manage messages and users (Server)
        ServerAdmin,        //Manage roles (Server)
        ServerOwner,        //Owner (Server)
        BotOwner,           //Bot owner (Global)
    }
}
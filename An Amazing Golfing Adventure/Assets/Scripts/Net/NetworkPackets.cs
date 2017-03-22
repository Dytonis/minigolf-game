using System;
using JAMGG.Enums;

namespace JAMGG.Net
{
    public struct LobbyConnectionPacket
    {
        public PlayerMetadata[] OtherMetadatas;
        public LobbyUpdateGeneralPacket LobbyInfo;
    }

    public struct LobbyUpdatePlayersPacket
    {
        public PlayerMetadata[] OtherMetadatas;
    }

    public struct LobbyUpdateGeneralPacket
    {
        public string LobbyName;
        public MapInfo MapInfo;
    }

    [Serializable]
    public struct PlayerMetadata
    {
        public int ID;
        public string name;
        //skins stuff
    }
}

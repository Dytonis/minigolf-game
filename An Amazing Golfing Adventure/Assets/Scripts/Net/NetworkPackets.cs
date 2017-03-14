using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public string MapSelected;
    }

    public struct PlayerMetadata
    {
        public int netID;
        public string name;
        //skins stuff
    }
}

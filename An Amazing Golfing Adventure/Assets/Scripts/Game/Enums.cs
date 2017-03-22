using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JAMGG.Enums
{
    public enum RegisteredMaps
    {
        Glen,
    }

    public enum RegisteredMapLengths
    {
        Tiny,
        Short,
        Average,
        Long,
        Extreme
    }

    public enum RegisteredMapDifficulty
    {
        Easy,
        Average,
        Hard
    }

    public struct MapInfo
    {
        public string LoadingScene;
        public string Name;
        public string UIName;
        public int Par;
        public int HoleCount;
        public int LengthMeter;
        public RegisteredMaps MapRegistrar;
        public RegisteredMapDifficulty MapRegisteredDifficulty;
        public RegisteredMapLengths MapRegisteredLength;
    }
}

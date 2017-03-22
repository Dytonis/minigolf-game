using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ResourceManagement : MonoBehaviour
{ 
    public static string GetImageThumbForEnum(JAMGG.Enums.RegisteredMaps map)
    {
        if (map == JAMGG.Enums.RegisteredMaps.Glen)
        {
            return ResourceMapThumbs.Glen;
        }

        else return "error";
    }

    public static JAMGG.Enums.MapInfo InfoForRegistrar(JAMGG.Enums.RegisteredMaps map)
    {
        if(map == JAMGG.Enums.RegisteredMaps.Glen)
        {
            return ResourceMapInfo.Glen;
        }

        throw new Exception("Could not find info for map " + map.ToString());
    }
}

public class ResourceMapThumbs
{
    public const string Glen = "MapThumbs/Glen";
}

public class ResourceMapInfo
{
    public static JAMGG.Enums.MapInfo Glen = new JAMGG.Enums.MapInfo
    {
        LoadingScene = "LoadingGlen",
        HoleCount = 9,
        LengthMeter = 50,
        MapRegisteredDifficulty = JAMGG.Enums.RegisteredMapDifficulty.Easy,
        MapRegisteredLength = JAMGG.Enums.RegisteredMapLengths.Average,
        MapRegistrar = JAMGG.Enums.RegisteredMaps.Glen,
        Name = "Glen",
        Par = 26,
        UIName = "Candlewood"
    };
}


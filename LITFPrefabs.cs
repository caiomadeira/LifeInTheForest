using SonsSdk.Attributes;
using UnityEngine;

namespace LifeInTheForest;

[AssetBundle("Bundles/kelvinbeardlong")]
public class LITFRobbyBeardPrefab
{
    [AssetReference("robbybeardbig")]
    public static GameObject beard { get; set; }
}

[AssetBundle("Bundles/grave01")]
public class LITFGrave01Prefab
{
    [AssetReference("grave01")]
    public static GameObject grave { get; set; }
}

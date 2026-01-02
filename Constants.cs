
using Il2CppSystem.Runtime.Remoting.Messaging;
using RedLoader;
using RedLoader.Utils;
using Sons.Ai.Vail;
using Sons.Gameplay;
using Sons.Items;
using Sons.Items.Core;
using Sons.StatSystem;
using SonsSdk;
using System.Reflection;
using TheForest.Utils;
using UnityEngine;
using static CoopPlayerUpgrades;
using static Il2CppMono.Globalization.Unicode.SimpleCollator;
using static RedLoader.RLog;

namespace LifeInTheForest;

/*
 * [21:36:26.962] [Improved Kelvin]Robby group list 0: Wait For Interaction
[21:36:27.043] [Improved Kelvin]Robby group list 1: Run From Explosion
[21:36:27.130] [Improved Kelvin]Robby group list 2: Robby Cower
[21:36:27.220] [Improved Kelvin]Robby group list 3: Point At Enemy
[21:36:27.316] [Improved Kelvin]Robby group list 4: Flee From Enemies
[21:36:27.415] [Improved Kelvin]Robby group list 5: Robby Attack Reaction
[21:36:27.519] [Improved Kelvin]Robby group list 6: Disengage Combat
[21:36:27.629] [Improved Kelvin]Robby group list 7: Lost Sight Of Enemy
[21:36:27.743] [Improved Kelvin]Robby group list 8: Robby Stand Idle
[21:36:27.861] [Improved Kelvin]Robby group list 9: Robby Back Away
[21:36:27.984] [Improved Kelvin]Robby group list 10: Hide Behind Player
[21:36:28.113] [Improved Kelvin]Robby group list 11: Watch Enemy
[21:36:28.246] [Improved Kelvin]Robby group list 12: Wander
*/

public enum KelvinArmorType
{
    None, Golden, Military
}

public struct KelvinConfig
{
    // -- main ---
    public string stageName;
    public int minYear;

    // -- visuals ---
    public string headTexture;
    public string tuxedoTexture;
    public string hoddieTexture;
    public bool hasBeard;
    public bool hasHair;
    public KelvinArmorType armorType;

    // -- stats --
    public float maxHeartRateStat;
    public float maxFullness;
    public float maxFear;
    public float maxAnger;
    public float maxHealth;
    public float maxStamina;
    public float staggerLevel;
    public float staggerPerHit;
    // stagged variables (need to see waht each do firstly)

    // multipliers
    public float moveSpeedMultiplier;
    public float blockDamageMultiplier;

    // -- behavior --
    public bool canCombat;
    public bool canUseWeapons;
    public bool canUseCosmeticArmor;
    public bool canRevive;
    public bool removeBehaviors;
    public bool IsAbleToUseGoldenArmor;
    public bool onlyKilledByPlayer;
    public bool canBlockAttack;

    // -- other behaviors ---
    public float shotgunKnockbackTime;
    public float meleeWeaponDamage;
}

public enum WeaponId { 
    Shotgun = 358, 
    CompactPistol = 355 // timmy pistol
}

public enum ActorStatsIndex
{
    AffectionStat = 0,
    AngerStat = 1,
    HealthStat = 2,
    HeartRateStat = 3,
    EnergyStat = 4,
    FullnessStat = 5,
    HydrationStat = 6,
    BloodAlcoholStat = 7,
    FearStat = 8,
    MoraleStat = 9,
    ReligiousStat = 10,
    TemperatureStat = 11,
    CleanStat = 12,
}

public enum RobbyGroupsIndex
{
        WaitForInteraction = 0,
        RunFromExplosion = 1,
        RobbyCower = 2,
        PointAtEnemy = 3,
        FleeFromEnemies = 4,
        RobbyAttackReaction = 5,
        DisengageCombat = 6,
        LostSightOfEnemy = 7,
        RobbyStandIdle = 8,
        RobbyBackAway = 9,
        HideBehindPlayer = 10,
        WatchEnemy = 11,
        Wander = 12
}

public struct RobbyGroupsNames
{
    public const string WaitForInteraction = "Wait For Interaction";
    public const string RunFromExplosion = "Run From Explosion";
    public const string RobbyCower = "Robby Cower";
    public const string PointAtEnemy = "Point At Enemy";
    public const string FleeFromEnemies = "Flee From Enemies";
    public const string RobbyAttackReaction = "Robby Attack Reaction";
    public const string DisengageCombat = "Disengage Combat";
    public const string LostSightOfEnemy = "Lost Sight Of Enemy";
    public const string RobbyStandIdle = "Robby Stand Idle";
    public const string RobbyBackAway = "Robby Back Away";
    public const string HideBehindPlayer = "Hide Behind Player";
    public const string WatchEnemy = "Watch Enemy";
    public const string Wander = "Wander";
}


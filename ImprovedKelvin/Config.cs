using RedLoader;
using SonsSdk.Attributes;

namespace LifeInTheForest;

[SettingsUiMode(0)]
public static class Config
{
    // Categorias
    public static ConfigCategory MainCategory { get; private set; }
    public static ConfigCategory Stage1Category { get; private set; }
    public static ConfigCategory Stage2Category { get; private set; }
    public static ConfigCategory Stage3Category { get; private set; }
    public static ConfigCategory Stage4Category { get; private set; }

    // --- GLOBAIS ---
    [SettingsUiInclude]
    [SettingsUiHeader("Time configurations", TMPro.TextAlignmentOptions.MidlineLeft, true)]
    public static ConfigEntry<string> AgingGranularity { get; private set; }

    [SettingsUiInclude]
    [SettingsUiHeader("Tips: Seasons Cycle = 1 Year has the sum of four seasons; ", TMPro.TextAlignmentOptions.MidlineLeft, true)]
    public static ConfigEntry<string> YearLengthMode { get; private set; }

    // --- YOUNG (STAGE 1) ---
    [SettingsUiInclude]
    public static ConfigEntry<int> Stage1Threshold { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1Health { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1Stamina { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1HeartRate { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1Fullness { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1Fear { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1Anger { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1Dmg { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1Speed { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1BlockMult { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1Knockback { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1StaggerLvl { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage1StaggerHit { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage1Combat { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage1Weapons { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage1Armor { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage1Revive { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage1RemBehav { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage1KillOnly { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage1CanBlock { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage1Beard { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage1Hair { get; private set; }

    // --- EXPERIENCED (STAGE 2) ---
    [SettingsUiInclude]
    public static ConfigEntry<int> Stage2Threshold { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2Health { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2Stamina { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2HeartRate { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2Fullness { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2Fear { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2Anger { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2Dmg { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2Speed { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2BlockMult { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2Knockback { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2StaggerLvl { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage2StaggerHit { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage2Combat { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage2Weapons { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage2Armor { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage2Revive { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage2RemBehav { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage2KillOnly { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage2CanBlock { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage2Beard { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage2Hair { get; private set; }

    // --- VETERAN (STAGE 3) ---
    [SettingsUiInclude]
    public static ConfigEntry<int> Stage3Threshold { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3Health { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3Stamina { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3HeartRate { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3Fullness { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3Fear { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3Anger { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3Dmg { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3Speed { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3BlockMult { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3Knockback { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3StaggerLvl { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage3StaggerHit { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage3Combat { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage3Weapons { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage3Armor { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage3Revive { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage3RemBehav { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage3KillOnly { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage3CanBlock { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage3Beard { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage3Hair { get; private set; }

    // --- OLD (STAGE 4) ---
    [SettingsUiInclude]
    public static ConfigEntry<int> Stage4Threshold { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4Health { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4Stamina { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4HeartRate { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4Fullness { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4Fear { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4Anger { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4Dmg { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4Speed { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4BlockMult { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4Knockback { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4StaggerLvl { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<float> Stage4StaggerHit { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage4Combat { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage4Weapons { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage4Armor { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage4Revive { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage4RemBehav { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage4KillOnly { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage4CanBlock { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage4Beard { get; private set; }
    [SettingsUiInclude]
    public static ConfigEntry<bool> Stage4Hair { get; private set; }

    public static void Init()
    {
        MainCategory = ConfigSystem.CreateFileCategory("LITF_Main", "Main Configs", "LifeInTheForest.cfg");
        Stage1Category = ConfigSystem.CreateFileCategory("LITF_Young", "Young Kelvin", "LifeInTheForest.cfg");
        Stage2Category = ConfigSystem.CreateFileCategory("LITF_Exp", "Experienced Kelvin", "LifeInTheForest.cfg");
        Stage3Category = ConfigSystem.CreateFileCategory("LITF_Vet", "Veteran Kelvin", "LifeInTheForest.cfg");
        Stage4Category = ConfigSystem.CreateFileCategory("LITF_Old", "Old Kelvin", "LifeInTheForest.cfg");

        AgingGranularity = MainCategory.CreateEntry("aging_granularity", "Year", "Time Granularity", "Define the base unit for aging.");
        AgingGranularity.SetOptions(new string[] { "Year" });

        YearLengthMode = MainCategory.CreateEntry("year_length_mode", "Seasons Cycle", "Year Definition", "How a year is calculated.");
        YearLengthMode.SetOptions(new string[] { "Seasons Cycle" });

        AgingGranularity.OnValueChanged.Subscribe((oldVal, newVal) => UpdateYearVisibility(newVal));
        UpdateYearVisibility(AgingGranularity.Value);

        // YOUNG CONFIGS
        Stage1Threshold = Stage1Category.CreateEntry("young_threshold", 0, "Start Threshold", "The year when the change occurs");
        Stage1Health = Stage1Category.CreateEntry("young_health", 100f, "Max Health");
        Stage1Stamina = Stage1Category.CreateEntry("young_stamina", 100f, "Max Stamina");
        Stage1HeartRate = Stage1Category.CreateEntry("young_heartrate", 100f, "Heart Rate");
        Stage1Fullness = Stage1Category.CreateEntry("young_fullness", 100f, "Fullness");
        Stage1Fear = Stage1Category.CreateEntry("young_fear", 100f, "Fear");
        Stage1Anger = Stage1Category.CreateEntry("young_anger", 20f, "Anger");
        Stage1Dmg = Stage1Category.CreateEntry("young_dmg", 20f, "Melee Dmg");
        Stage1Speed = Stage1Category.CreateEntry("young_speed", 2f, "Speed Mult");
        Stage1BlockMult = Stage1Category.CreateEntry("young_block_mult", 0.5f, "Block Mult");
        Stage1Knockback = Stage1Category.CreateEntry("young_knock", 5.0f, "Shotgun Knock");
        Stage1StaggerLvl = Stage1Category.CreateEntry("young_stagger_lvl", 0f, "Stagger Lvl");
        Stage1StaggerHit = Stage1Category.CreateEntry("young_stagger_hit", 0.8f, "Stagger Hit");
        Stage1Combat = Stage1Category.CreateEntry("young_combat", false, "Can Combat");
        Stage1Weapons = Stage1Category.CreateEntry("young_weapons", false, "Use Weapons");
        Stage1Armor = Stage1Category.CreateEntry("young_armor", false, "Visual Armor");
        Stage1Revive = Stage1Category.CreateEntry("young_revive", true, "Can Revive");
        Stage1RemBehav = Stage1Category.CreateEntry("young_rem_behav", false, "Remove Behaviors");
        Stage1KillOnly = Stage1Category.CreateEntry("young_kill_only", false, "Player Kill Only");
        Stage1CanBlock = Stage1Category.CreateEntry("young_block", false, "Can Block");
        Stage1Beard = Stage1Category.CreateEntry("young_beard", false, "Has Beard");
        Stage1Hair = Stage1Category.CreateEntry("young_hair", true, "Has Hair");

        // EXPERIENCED CONFIGS
        Stage2Threshold = Stage2Category.CreateEntry("exp_threshold", 1, "Start Threshold", "The year when the change occurs");
        Stage2Health = Stage2Category.CreateEntry("exp_health", 120f, "Max Health");
        Stage2Stamina = Stage2Category.CreateEntry("exp_stamina", 110f, "Max Stamina");
        Stage2HeartRate = Stage2Category.CreateEntry("exp_heartrate", 100f, "Heart Rate");
        Stage2Fullness = Stage2Category.CreateEntry("exp_fullness", 100f, "Fullness");
        Stage2Fear = Stage2Category.CreateEntry("exp_fear", 70f, "Fear");
        Stage2Anger = Stage2Category.CreateEntry("exp_anger", 100f, "Anger");
        Stage2Dmg = Stage2Category.CreateEntry("exp_dmg", 50f, "Melee Dmg");
        Stage2Speed = Stage2Category.CreateEntry("exp_speed", 5f, "Speed Mult");
        Stage2BlockMult = Stage2Category.CreateEntry("exp_block_mult", 0.5f, "Block Mult");
        Stage2Knockback = Stage2Category.CreateEntry("exp_knock", 2.0f, "Shotgun Knock");
        Stage2StaggerLvl = Stage2Category.CreateEntry("exp_stagger_lvl", 1f, "Stagger Lvl");
        Stage2StaggerHit = Stage2Category.CreateEntry("exp_stagger_hit", 0.4f, "Stagger Hit");
        Stage2Combat = Stage2Category.CreateEntry("exp_combat", true, "Can Combat");
        Stage2Weapons = Stage2Category.CreateEntry("exp_weapons", true, "Use Weapons");
        Stage2Armor = Stage2Category.CreateEntry("exp_armor", false, "Visual Armor");
        Stage2Revive = Stage2Category.CreateEntry("exp_revive", true, "Can Revive");
        Stage2RemBehav = Stage2Category.CreateEntry("exp_rem_behav", true, "Remove Behaviors");
        Stage2KillOnly = Stage2Category.CreateEntry("exp_kill_only", false, "Player Kill Only");
        Stage2CanBlock = Stage2Category.CreateEntry("exp_block", true, "Can Block");
        Stage2Beard = Stage2Category.CreateEntry("exp_beard", false, "Has Beard");
        Stage2Hair = Stage2Category.CreateEntry("exp_hair", true, "Has Hair");

        // VETERAN CONFIGS
        Stage3Threshold = Stage3Category.CreateEntry("vet_threshold", 2, "Start Threshold", "The year when the change occurs");
        Stage3Health = Stage3Category.CreateEntry("vet_health", 120f, "Max Health");
        Stage3Stamina = Stage3Category.CreateEntry("vet_stamina", 110f, "Max Stamina");
        Stage3HeartRate = Stage3Category.CreateEntry("vet_heartrate", 80f, "Heart Rate");
        Stage3Fullness = Stage3Category.CreateEntry("vet_fullness", 90f, "Fullness");
        Stage3Fear = Stage3Category.CreateEntry("vet_fear", 20f, "Fear");
        Stage3Anger = Stage3Category.CreateEntry("vet_anger", 200f, "Anger");
        Stage3Dmg = Stage3Category.CreateEntry("vet_dmg", 100f, "Melee Dmg");
        Stage3Speed = Stage3Category.CreateEntry("vet_speed", 10f, "Speed Mult");
        Stage3BlockMult = Stage3Category.CreateEntry("vet_block_mult", 0.1f, "Block Mult");
        Stage3Knockback = Stage3Category.CreateEntry("vet_knock", 0.75f, "Shotgun Knock");
        Stage3StaggerLvl = Stage3Category.CreateEntry("vet_stagger_lvl", 2f, "Stagger Lvl");
        Stage3StaggerHit = Stage3Category.CreateEntry("vet_stagger_hit", 0.2f, "Stagger Hit");
        Stage3Combat = Stage3Category.CreateEntry("vet_combat", true, "Can Combat");
        Stage3Weapons = Stage3Category.CreateEntry("vet_weapons", true, "Use Weapons");
        Stage3Armor = Stage3Category.CreateEntry("vet_armor", true, "Visual Armor");
        Stage3Revive = Stage3Category.CreateEntry("vet_revive", true, "Can Revive");
        Stage3RemBehav = Stage3Category.CreateEntry("vet_rem_behav", true, "Remove Behaviors");
        Stage3KillOnly = Stage3Category.CreateEntry("vet_kill_only", false, "Player Kill Only");
        Stage3CanBlock = Stage3Category.CreateEntry("vet_block", true, "Can Block");
        Stage3Beard = Stage3Category.CreateEntry("vet_beard", true, "Has Beard");
        Stage3Hair = Stage3Category.CreateEntry("vet_hair", true, "Has Hair");

        // OLD CONFIGS
        Stage4Threshold = Stage4Category.CreateEntry("old_threshold", 3, "Start Threshold", "The year when the change occurs");
        Stage4Health = Stage4Category.CreateEntry("old_health", 200f, "Max Health");
        Stage4Stamina = Stage4Category.CreateEntry("old_stamina", 50f, "Max Stamina");
        Stage4HeartRate = Stage4Category.CreateEntry("old_heartrate", 50f, "Heart Rate");
        Stage4Fullness = Stage4Category.CreateEntry("old_fullness", 80f, "Fullness");
        Stage4Fear = Stage4Category.CreateEntry("old_fear", 10f, "Fear");
        Stage4Anger = Stage4Category.CreateEntry("old_anger", 200f, "Anger");
        Stage4Dmg = Stage4Category.CreateEntry("old_dmg", 200f, "Melee Dmg");
        Stage4Speed = Stage4Category.CreateEntry("old_speed", 1f, "Speed Mult");
        Stage4BlockMult = Stage4Category.CreateEntry("old_block_mult", 0.1f, "Block Mult");
        Stage4Knockback = Stage4Category.CreateEntry("old_knock", 0.1f, "Shotgun Knock");
        Stage4StaggerLvl = Stage4Category.CreateEntry("old_stagger_lvl", 1f, "Stagger Lvl");
        Stage4StaggerHit = Stage4Category.CreateEntry("old_stagger_hit", 0.1f, "Stagger Hit");
        Stage4Combat = Stage4Category.CreateEntry("old_combat", true, "Can Combat");
        Stage4Weapons = Stage4Category.CreateEntry("old_weapons", true, "Use Weapons");
        Stage4Armor = Stage4Category.CreateEntry("old_armor", true, "Visual Armor");
        Stage4Revive = Stage4Category.CreateEntry("old_revive", false, "Can Revive");
        Stage4RemBehav = Stage4Category.CreateEntry("old_rem_behav", true, "Remove Behaviors");
        Stage4KillOnly = Stage4Category.CreateEntry("old_kill_only", false, "Player Kill Only");
        Stage4CanBlock = Stage4Category.CreateEntry("old_block", true, "Can Block");
        Stage4Beard = Stage4Category.CreateEntry("old_beard", true, "Has Beard");
        Stage4Hair = Stage4Category.CreateEntry("old_hair", false, "Has Hair");

        // ranges
        SetRange(1f, 1000f, Stage1Health, Stage2Health, Stage3Health, Stage4Health);
        SetRange(1f, 1000f, Stage1Stamina, Stage2Stamina, Stage3Stamina, Stage4Stamina);
        SetRange(0f, 300f, Stage1HeartRate, Stage2HeartRate, Stage3HeartRate, Stage4HeartRate);
        SetRange(0f, 200f, Stage1Fullness, Stage2Fullness, Stage3Fullness, Stage4Fullness);

        SetRange(0f, 200f, Stage1Fear, Stage2Fear, Stage3Fear, Stage4Fear);
        SetRange(0f, 200f, Stage1Anger, Stage2Anger, Stage3Anger, Stage4Anger);

        SetRange(0f, 500f, Stage1Dmg, Stage2Dmg, Stage3Dmg, Stage4Dmg);
        SetRange(0.1f, 20f, Stage1Speed, Stage2Speed, Stage3Speed, Stage4Speed);
        SetRange(0f, 5f, Stage1BlockMult, Stage2BlockMult, Stage3BlockMult, Stage4BlockMult);
        SetRange(0f, 10f, Stage1Knockback, Stage2Knockback, Stage3Knockback, Stage4Knockback);

        SetRange(0f, 5f, Stage1StaggerLvl, Stage2StaggerLvl, Stage3StaggerLvl, Stage4StaggerLvl);
        SetRange(0f, 5f, Stage1StaggerHit, Stage2StaggerHit, Stage3StaggerHit, Stage4StaggerHit);

        SetRange(0, 100, Stage1Threshold, Stage2Threshold, Stage3Threshold, Stage4Threshold);
    }

    private static void SetRange(float min, float max, params ConfigEntry<float>[] entries)
    {
        foreach (var entry in entries)
        {
            if (entry != null)
                entry.SetRange(min, max);
        }
    }
    private static void SetRange(int min, int max, params ConfigEntry<int>[] entries)
    {
        foreach (var entry in entries)
        {
            if (entry != null)
                entry.SetRange(min, max);
        }
    }

    private static void UpdateYearVisibility(string granularityValue)
    {
        if (YearLengthMode != null) YearLengthMode.IsHidden = (granularityValue != "Year");
    }
}
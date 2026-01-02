using RedLoader;
using SUI;

namespace LifeInTheForest;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    //public static ConfigEntry<bool> SomeEntry { get; private set; }
    public static ConfigEntry<bool> enableAging { get; private set; }

    // Auto populated after calling SettingsRegistry.CreateSettings...
   //  private static SettingsRegistry.SettingsEntry _settingsEntry;

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("LifeInTheForest", "LifeInTheForest", "LifeInTheForest.cfg");

        enableAging = Category.CreateEntry(
            "enable_aging",
            true,
            "Enable Aging",
            "The player is starting to age.");
    }


    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}
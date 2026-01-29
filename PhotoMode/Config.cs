using RedLoader;
using SUI;

namespace PhotoMode;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    //public static ConfigEntry<bool> SomeEntry { get; private set; }

    // Auto populated after calling SettingsRegistry.CreateSettings...
    private static SettingsRegistry.SettingsEntry _settingsEntry;

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("PhotoMode", "PhotoMode", "PhotoMode.cfg");

        // SomeEntry = Category.CreateEntry(
        //     "some_entry",
        //     true,
        //     "Some entry",
        //     "Some entry that does some stuff.");
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}
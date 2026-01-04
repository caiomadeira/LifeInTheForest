using RedLoader;
using Sons.Ai.Vail;
using SonsSdk;
using SUI;

namespace LifeInTheForest;

public class LifeInTheForest : SonsMod
{
    public LifeInTheForest() { }

    protected override void OnInitializeMod() { Config.Init(); }
    protected override void OnSdkInitialized()
    {
        LifeInTheForestUi.Create();
        SettingsRegistry.CreateSettings(this, null, typeof(Config), false, null);
        SdkEvents.OnGameActivated.Subscribe(OnGameActivated);
    }

    internal static void OnGameActivated()
    {
        if (BoltNetwork.isServerOrNotRunning)
        {
            var worldSim = VailWorldSimulation._instance;
            if (worldSim != null && worldSim.gameObject.GetComponent<LITFWorld>() == null)
            {
                worldSim.gameObject.AddComponent<LITFWorld>();
                RLog.Msg(System.Drawing.Color.Green, "[MAIN] LITFWorld injected.");
            }

            var robbyPrefab = ActorTools.GetPrefab(VailActorTypeId.Robby);
            if (robbyPrefab.GetComponent<LITFImprovedKelvin>() == null)
            {
                robbyPrefab.gameObject.AddComponent<LITFImprovedKelvin>();
                RLog.Msg(System.Drawing.Color.Green, "[MAIN] Improved Kelvin injected.");
            }
        }
    }
    protected override void OnGameStart() {  }
}
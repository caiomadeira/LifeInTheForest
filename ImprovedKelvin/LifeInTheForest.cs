using RedLoader;
using Sons.Ai.Vail;
using SonsSdk;
using SUI;
using UnityEngine;

namespace LifeInTheForest;

public class LifeInTheForest : SonsMod
{
    public LifeInTheForest()
    {
        OnGUICallback = DebugUI;
    }

    protected override void OnInitializeMod() { Config.Init(); }
    protected override void OnSdkInitialized()
    {
        LifeInTheForestUi.Create();
        SettingsRegistry.CreateSettings(this, null, typeof(Config), true, null);
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

    private void DebugUI()
    {
        //GUI.Box(new Rect(10, 10, 200, 400), "SPAWN CARL");

        //if (GUI.Button(new Rect(20, 70, 180, 20), "Spawn Cannibal"))
        //{
        //    var pos = Camera.main.transform.position + (Camera.main.transform.forward * 10);
        //    ActorTools.Spawn(VailActorTypeId.Carl, pos);
        //    RLog.Msg("Cannibal Spawnado!");
        //}
    }
    protected override void OnGameStart() {  }
}
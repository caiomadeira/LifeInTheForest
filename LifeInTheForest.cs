using Sons.Gameplay;
using Sons.StatSystem;
using RedLoader;
using Sons.Ai.Vail;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;
using SUI;

namespace LifeInTheForest;

public class LifeInTheForest : SonsMod
{
    private bool friendSpawned = false;
    private VailActor friendInstance = null;
    private bool _wordInjected = false;

    public LifeInTheForest()
    {
        OnGUICallback = DebugUI;
        OnWorldUpdatedCallback = OnWorldUpdate;
        // HarmonyPatchAll = true;
    }

    protected override void OnInitializeMod()
    {
        Config.Init();
    }

    protected override void OnSdkInitialized()
    {
        LifeInTheForestUi.Create();
        SettingsRegistry.CreateSettings(this, null, typeof(Config));
        SdkEvents.OnGameActivated.Subscribe(OnGameActivated);
    }

    internal static void OnGameActivated()
    {
        if (BoltNetwork.isServerOrNotRunning)
        {
            ActorTools.GetPrefab(VailActorTypeId.Robby).gameObject.AddComponent<ImprovedKelvin>();
        }
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.
        LocalPlayer.FpCharacter.SetWalkSpeed(20);
        var testc = "Game Started";
        RLog.Msg($"Minha Variavel: {testc}");
        _wordInjected = false;
    }

    public void OnWorldUpdate()
    {
        if (!_wordInjected)
        {
            if (VailWorldSimulation._instance != null)
            {
                GameObject world = VailWorldSimulation._instance.gameObject;
                if (world.GetComponent<LITFWorld>() == null)
                {
                    world.AddComponent<LITFWorld>();
                    RLog.Msg(">>>>> LITFWorld injected");
                }
                
                _wordInjected = true;
            }
        }


        if ((Input.GetKeyDown(KeyCode.F2)))
        {
            SetupTimmy();
            SonsTools.ShowMessage("Timmy foi spawnado", 10.0f);
        }
    }

    private void DebugUI()
    {
        // Cria uma caixa no topo esquerdo
        GUI.Box(new Rect(10, 10, 200, 400), "F2 TO SPAWN TIMMY AND KEVIN");

        //// Cria um botão dentro dessa área
        //if (GUI.Button(new Rect(20, 40, 180, 30), "Spawn Timmy"))
        //{
        //    RLog.Msg("Spawn Timmy");
        //    SetupTimmy(VailActorTypeId.Timmy);
        //}

        if (GUI.Button(new Rect(20, 70, 180, 20), "Spawn Cannibal"))
        {
            var pos = Camera.main.transform.position + (Camera.main.transform.forward * 10);
            ActorTools.Spawn(VailActorTypeId.Carl, pos);
            RLog.Msg("Cannibal Spawnado!");
        }
        //if (GUI.Button(new Rect(20, 90, 180, 20), "Spawn Cannibal"))
        //{
        //    var pos = Camera.main.transform.position + (Camera.main.transform.forward * 10);
        //    ActorTools.Spawn(VailActorTypeId.Frank, pos);
        //    RLog.Msg("Cannibal Spawnado!");

        //}
    }

    private void SetupTimmy()
    {
        if (!friendSpawned)
        {
            var pos = Camera.main.transform.position + (Camera.main.transform.forward * 3);
            friendInstance = ActorTools.Spawn(VailActorTypeId.Timmy, pos);
            if (friendInstance != null)
            {
                //friendInstance._healthSettings._health = -200;
                //friendInstance.DrainHealth(240);
                RLog.Msg($"SPAWNED WITH HEALTH: {friendInstance.GetHealth()}");
                RLog.Msg($"SPAWNED WITH MAX HEALTH: {friendInstance.GetMaxHealth()}");
            }
            RLog.Msg("Timmy Spawnado!");
            SonsTools.ShowMessage("Timmy foi spawnado", 2.0f);
            friendSpawned = true;
        }
    }

}
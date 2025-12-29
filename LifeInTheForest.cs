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

    public LifeInTheForest()
    {
        OnGUICallback = DebugUI;
        OnWorldUpdatedCallback = OnWorldUpdate;
        // Uncomment this to automatically apply harmony patches in your assembly.
        // HarmonyPatchAll = true;
    }

    protected override void OnInitializeMod()
    {
        Config.Init();

    }

    protected override void OnSdkInitialized()
    {
        // Do your mod initialization which involves game or sdk references here
        // This is for stuff like UI creation, event registration etc.
        LifeInTheForestUi.Create();

        // Add in-game settings ui for your mod.
        SettingsRegistry.CreateSettings(this, null, typeof(Config));
        SdkEvents.OnGameActivated.Subscribe(OnGameActivated);
    }

    internal static void OnGameActivated()
    {
        if (BoltNetwork.isServerOrNotRunning)
        {
            ActorTools.GetPrefab(VailActorTypeId.Robby).gameObject.AddComponent<ImprovedKelvin>();
        }
        UpdateTextureHandlers();
    }

    public static void UpdateTextureHandlers(bool forceReload = false)
    {
        var handler = new BaseTextureHandler("RobbyHead", VailActorTypeId.Robby, "VisualRoot/RobbyRig/GEO/RobbyHead", 0);
        handler.SetTexture("RobbyHeadCustom2", true);
        RLog.Msg("Updated texture handlers!!!!!!!!!!");
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.
        LocalPlayer.FpCharacter.SetWalkSpeed(20);
        var testc = "Game Started";
        RLog.Msg($"Minha Variavel: {testc}");
    }

    public void OnWorldUpdate()
    {
        if ((Input.GetKeyDown(KeyCode.F2)))
        {
            SetupTimmy();
            SonsTools.ShowMessage("Timmy foi spawnado", 10.0f);
            //SetupRobby();
            //UpdateRobby();
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
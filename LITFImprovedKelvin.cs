
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
using System.Reflection.Metadata.Ecma335;
using TheForest.Utils;
using UnityEngine;
using static CoopPlayerUpgrades;
using static Il2CppMono.Globalization.Unicode.SimpleCollator;
using static RedLoader.RLog;

namespace LifeInTheForest;

[RegisterTypeInIl2Cpp]
public class LITFImprovedKelvin: MonoBehaviour
{
    // --- singleton
    public static LITFImprovedKelvin Instance { get; private set; }

    // --- robby --- 
    private VailActor robby;
    private Sons.Gameplay.MeleeWeapon kickWeapon;
    private StatsManager robbyStats;
    private VailController robbyController;
    private StateSet robbyCombatSet;

    // --- Visual Handlers ---
    // head
    private BaseTextureHandler robbyHeadTextureHandler;
    private BaseTextureHandler robbyTuxedoJacketTextureHandler;
    private BaseTextureHandler robbyHoddieTextureHandler;
    public KelvinArmorType armorType;

    // --- robby paths ---
    const string goldenArmor = "VisualRoot/RobbyRig/GEO/GoldenArmor";
    const string tuxedoJacketMesh = "VisualRoot/RobbyRig/GEO/TuxedoJacketMesh";
    const string hoddieMesh = "VisualRoot/RobbyRig/GEO/HoodieMesh";
    const string tacticalHelmet1 = "VisualRoot/RobbyRig/GEO/TacticalArmorHeadHelmetMesh"; // no texture
    const string tacticalHelmet2 = "VisualRoot/RobbyRig/GEO/helmet"; // no texture
    const string tacticalBodyArmor = "VisualRoot/RobbyRig/GEO/body_armor";
    const string tacticalBoots = "VisualRoot/RobbyRig/GEO/boots";

    // -- Control Variables ---
    private int _lastCheckedYear = -1;
    private bool _initLITFWorld = false;
    private bool _isCosmeticArmorActive = false;
    private float _nextReviveCheckTime = 0f;
    private bool activateLITFKelvinWeaponSystem = false;
    private bool _inventoryHackApplied = false;
    // --- weapon system ---
    private LITFKelvinWeaponSystem weaponSystem;

    // -- Menu ---
    private LITFKelvinMenu menuSystem;

    // --- robby variables ---
    private List<KelvinConfig> _agingConfigs = new List<KelvinConfig>();
    private KelvinConfig currentConfig;

    public void Awake()
    {
        Instance = this;
        this.robby = GetComponent<VailActor>();
        if (robby == null) { RLog.Error("[Improved Kelvin] robby (VailActor) == null."); return; }
        SetupAgingConfig();
        if (activateLITFKelvinWeaponSystem) this.weaponSystem = new LITFKelvinWeaponSystem(robby, (int)WeaponId.Shotgun, currentConfig);
    }
    public void Start()
    {
        // -- kelvin textures
        SetupKelvinTextures();
        // -- kelvin menu pad ---
        menuSystem = this.gameObject.AddComponent<LITFKelvinMenu>();
        if (menuSystem == null ) RLog.Msg("[Improved Kelvin] menuSystem == NULL.");
        SetupKelvinStats();
        // --- update --- 
        //UpdateKelvinState(0, true);
        KelvinDumpConfig(currentConfig);
        var logItem = ItemDatabaseManager.ItemById(78);
        if (logItem != null)
        {
            logItem.MaxAmount = 4; // Diz ao DB que logs stackam até 4
            RLog.Msg("[Improved Kelvin] Log Item MaxAmount set to 4.");
        }

        
    }

    private void ApplyLogInventoryHack()
    {
        RLog.Msg("[Improved Kelvin] Tentando aplicar Hack de Logs...");

        // 1. Aumenta o limite no banco de dados do item
        var logItem = ItemDatabaseManager.ItemById(78);
        if (logItem != null)
        {
            logItem.MaxAmount = 4;
            RLog.Msg($"[Improved Kelvin] Item 78 (Log) MaxAmount alterado para: {logItem.MaxAmount}");
        }

        // 2. Hack dos Slots Visuais (CLONAGEM)
        var attachList = robby._inventoryManager._carryAttachments;
        foreach (var att in attachList)
        {
            // Procura o attachment de Logs
            if (att != null && att._itemTypes.Contains("Log"))
            {
                var transformsList = att._transforms;

                // Só aplica se tiver os 2 originais (evita duplicar infinito se recarregar)
                if (transformsList.Count > 0 && transformsList.Count <= 2)
                {
                    // Pega o último slot válido (o segundo log no ombro)
                    var originalTransform = transformsList[transformsList.Count - 1];

                    // --- O PULO DO GATO: INSTANTIATE ---
                    // Em vez de adicionar o mesmo, criamos uma CÓPIA do GameObject na hierarquia
                    // Isso cria um novo "bone" invisível na mesma posição do anterior.

                    // Slot 3
                    var newSlot3 = UnityEngine.Object.Instantiate(originalTransform.gameObject, originalTransform.parent);
                    newSlot3.name = "HoldLog3"; // Nome para debug
                    transformsList.Add(newSlot3.transform);

                    // Slot 4
                    var newSlot4 = UnityEngine.Object.Instantiate(originalTransform.gameObject, originalTransform.parent);
                    newSlot4.name = "HoldLog4"; // Nome para debug
                    transformsList.Add(newSlot4.transform);
                    att._endTransform = newSlot4.transform;
                    RLog.Msg($"[Improved Kelvin] SUCESSO! Slots de Logs aumentados para: {transformsList.Count}");
                }
                else
                {
                    RLog.Msg($"[Improved Kelvin] Hack pulado. Slots atuais: {transformsList.Count}");
                }
                break;
            }
        }
    }
    public void Update()
    {
        //var attachList = robby._inventoryManager._carryAttachments;
        //Sons.Ai.Vail.Inventory.CarryAttachments logAttach = null;
        //foreach (var att in attachList)
        //{
        //    if (att._itemTypes.Contains("Log"))
        //    {
        //        logAttach = att;
        //        break;
        //    }
        //}

        //if (logAttach != null)
        //{
        //    var transformsList = logAttach._transforms;
        //    if (transformsList.Count > 0)
        //    {
        //        var lastTransform = transformsList[transformsList.Count - 1];
        //        transformsList.Add(lastTransform);
        //        transformsList.Add(lastTransform);
        //    }
        //}

        if (!_inventoryHackApplied)
        {
            // Verifica se o Robby e o InventoryManager já existem
            if (robby != null && robby._inventoryManager != null && robby._inventoryManager._carryAttachments != null)
            {
                ApplyLogInventoryHack();
                _inventoryHackApplied = true; // Trava para não rodar de novo
            }
        }

        if (!_initLITFWorld) CheckInitLITFWorld();
        else CheckYearChange();

        HandleCombatState();

        if (this.robbyStats == null)
        {
            if (robby._statsManager != null)
            {
                this.robbyStats = robby._statsManager._statsManager;
            }
        }

        if (Time.time > _nextReviveCheckTime)
        {
            CheckReviveCondition();
            _nextReviveCheckTime = Time.time + 5.0f;
        }

        // --- INPUTS ---
        Controls();
    }

    private void Controls()
    {

        if (Input.GetKeyDown(KeyCode.G)) PadAction();

        if (Input.GetKeyDown(KeyCode.F10))
        {
            UpdateKelvinState(0, true);
            RLog.Msg($"kelvin state 0.");
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            UpdateKelvinState(1, true);
            RLog.Msg($"kelvin state 1.");
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (activateLITFKelvinWeaponSystem)
                weaponSystem.LITFKelvinWeaponSystemUpdate();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
           
            //var attachmentsList = robby._inventoryManager._carryAttachments;
            //foreach(var attachment in attachmentsList)
            //{
            //    RLog.Msg($"item type: {attachment._itemTypes}");
            //}
            //// robby.TryGiveItem(78, out int itemReturned);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            KelvinDumpInfo();
        }
    }
    private void SetupAgingConfig()
    {
        _agingConfigs.Add(new KelvinConfig
        {
            // -- main ---
            stageName = "Young",
            minYear = 0,
            // -- visuals ---
            headTexture = "RobbyHeadYoung1",
            tuxedoTexture = "TuxedoJacketMesh",
            hoddieTexture = "HoodieMesh",
            hasBeard = false,
            hasHair = true,
            armorType = KelvinArmorType.Military,
            // -- stats --
            maxHealth = 100f,
            maxStamina = 100f,
            maxHeartRateStat = 100f,
            maxFullness = 100f,
            maxFear = 100f,
            maxAnger = 20f,
            staggerLevel = 0,
            staggerPerHit = 0.8f,
            // multipliers
            moveSpeedMultiplier = 2,
            blockDamageMultiplier = 0.5f,
            // -- behavior --
            canCombat = false,
            canUseWeapons = false,
            canRevive = true,
            removeBehaviors = false,
            canUseCosmeticArmor = false,
            onlyKilledByPlayer = false,
            canBlockAttack = false,
            // -- other behaviors ---
            shotgunKnockbackTime = 5.0f,
            meleeWeaponDamage = 20.0f,
        });

        _agingConfigs.Add(new KelvinConfig
        {
            stageName = "Experienced",
            minYear = 1,
            headTexture = "RobbyHeadOld2",
            tuxedoTexture = "RobbyTuxedo2",
            hoddieTexture = "RobbyHoodie1",
            hasBeard = true,
            hasHair = true,
            maxHealth = 120f,
            maxStamina = 110f,
            maxHeartRateStat = 100f,
            maxFullness = 100f,
            maxFear = 70f,
            maxAnger = 100f,
            meleeWeaponDamage = 50.0f,
            moveSpeedMultiplier = 5,
            canCombat = true,
            canUseWeapons = false,
            canRevive = true,
            removeBehaviors = true,
            canUseCosmeticArmor = false,
            armorType = KelvinArmorType.Military,
            onlyKilledByPlayer = false,
            canBlockAttack = true,
            shotgunKnockbackTime = 2.0f,
            blockDamageMultiplier = 0.5f,
            staggerLevel = 1,
            staggerPerHit = 0.4f,
        });

        _agingConfigs.Add(new KelvinConfig
        {
            stageName = "Veteran",
            minYear = 2,
            headTexture = "RobbyHeadOld3",
            tuxedoTexture = "RobbyTuxedo2",
            hoddieTexture = "RobbyHoodie1",
            hasBeard = true,
            hasHair = true,
            maxHealth = 120f,
            maxStamina = 110f,
            maxHeartRateStat = 80f,
            maxFullness = 90f,
            maxFear = 20f,
            maxAnger = 200f,
            meleeWeaponDamage = 100.0f,
            moveSpeedMultiplier = 10,
            canCombat = true,
            canUseWeapons = true,
            canRevive = true,
            removeBehaviors = true,
            canUseCosmeticArmor = true,
            armorType = KelvinArmorType.Golden,
            onlyKilledByPlayer = false,
            canBlockAttack = true,
            shotgunKnockbackTime = 0.75f,
            blockDamageMultiplier = 0.1f,
            staggerLevel = 2,
            staggerPerHit = 0.2f,
        });

        _agingConfigs.Add(new KelvinConfig
        {
            stageName = "Old",
            minYear = 3,
            headTexture = "RobbyHeadOld5",
            tuxedoTexture = "RobbyTuxedo3",
            hoddieTexture = "RobbyHoodie1",
            hasBeard = true,
            hasHair = false,
            maxHealth = 200f,
            maxStamina = 50f,
            maxHeartRateStat = 50f,
            maxFullness = 80f,
            maxFear = 10f,
            maxAnger = 200f,
            meleeWeaponDamage = 200.0f,
            moveSpeedMultiplier = 1,
            canCombat = true,
            canUseWeapons = true,
            canRevive = false,
            removeBehaviors = true,
            canUseCosmeticArmor = true,
            armorType = KelvinArmorType.Golden,
            onlyKilledByPlayer = false,
            canBlockAttack = true,
            shotgunKnockbackTime = 0.1f,
            blockDamageMultiplier = 0.1f,
            staggerLevel = 1,
            staggerPerHit = 0.1f,
        });
        RLog.Msg("[Improved Kelvin] Aging Config done.");
    }

    private void UpdateKelvinState(int year, bool forceUpdate = false)
    {
        KelvinConfig newConfig = _agingConfigs[0];
        foreach(var config in _agingConfigs)
        {
            if (year >= config.minYear) newConfig = config;
        }

        currentConfig = newConfig;

        // -- -behaviors
        CheckIfAbleToUseGoldenArmor();

        RLog.Msg($"[ImprovedKelvin] Updating to Stage: {currentConfig.stageName} (Year {year})");

        if (robbyHeadTextureHandler != null) robbyHeadTextureHandler.SetTexture(currentConfig.headTexture);
        if (robbyTuxedoJacketTextureHandler != null) robbyTuxedoJacketTextureHandler.SetTexture(currentConfig.tuxedoTexture);
        if (robbyHoddieTextureHandler != null) robbyHoddieTextureHandler.SetTexture(currentConfig.hoddieTexture);

        ToggleActivateRobbyMesh("RobbyBeard", currentConfig.hasBeard);
        ToggleActivateRobbyMesh("RobbyHair", currentConfig.hasHair);

        // -- stats ---
        SetupKelvinStats();

        // -- Kick Attack 
        SetupKelvinKickAttack();
        // --- behavior
        SetupKelvinBehavior();

    }
    private void HandleCombatState()
    {
        bool isInCombat = robby.IsAlerted();
        bool shouldHaveArmor = currentConfig.canUseCosmeticArmor && isInCombat;
        bool shouldHaveWeapon = currentConfig.canUseWeapons && isInCombat;
        if (isInCombat) SetStatCurrentValue((int)ActorStatsIndex.AngerStat, currentConfig.maxAnger);

        if (shouldHaveArmor != _isCosmeticArmorActive)
        {
            _isCosmeticArmorActive = shouldHaveArmor;
            UpdateArmorVisuals();
        }

        if (activateLITFKelvinWeaponSystem)
            weaponSystem.LITFKelvinWeaponSystemHandleCombatState(shouldHaveWeapon);
    }

    private void SetupKelvinStats()
    {
        if (robby._statsManager != null)
        {
            robbyStats = robby._statsManager._statsManager;
            robbyStats._stats[(int)ActorStatsIndex.HealthStat].SetMax(currentConfig.maxHealth); // 100f
            robbyStats._stats[(int)ActorStatsIndex.AngerStat].SetMax(currentConfig.maxAnger); // 500f
            robbyStats._stats[(int)ActorStatsIndex.FearStat].SetMax(currentConfig.maxFear); // 0f
            robbyStats._stats[(int)ActorStatsIndex.FullnessStat].SetMax(currentConfig.maxFullness);
            robbyStats._stats[(int)ActorStatsIndex.FullnessStat].SetMax(currentConfig.maxHeartRateStat);
        }
        else { RLog.Error("[Improved Kelvin] (robby._statsManager == null."); }
    }

    private void SetStatCurrentValue(int stats, float value) => this.robbyStats._stats[stats].SetCurrentValue(value);
    private void SetStatDefaultValue(int stats, float value) => this.robbyStats._stats[stats].SetDefaultValue(value);
    private void SetupKelvinKickAttack()
    {
        if (currentConfig.canCombat)
        {
            if (currentConfig.canUseWeapons)
            {
                if (robby._meleeWeapons != null && robby._meleeWeapons.Count > 0)
                {
                    kickWeapon = robby._meleeWeapons[0]._gameObject.GetComponent<Sons.Gameplay.MeleeWeapon>();
                    if (kickWeapon != null) kickWeapon._damage = currentConfig.meleeWeaponDamage;
                }
                RLog.Msg("[Improved Kelvin] Robby Cant Use Weapons. Using Melee attack  currentConfig.meleeWeaponDamage.");
            }
            else  RLog.Msg("[Improved Kelvin] Robby Cant Use Weapons.");
        } else RLog.Msg("[Improved Kelvin] Robby Cant Combat.");
    }

    private void SetupKelvinBehavior()
    {
        robbyController = robby._controller;
        if (robbyController != null && currentConfig.canCombat)
        {
            robbyCombatSet = robbyController.GetStateSet(State.Combat);
            if (robbyCombatSet != null)
            {
                for (int i = 0; i < robbyCombatSet._groupsList.Count; i++)
                {
                    RLog.Msg($"[Improved Kelvin]Robby group list {i}: {robbyCombatSet._groupsList[i].Name}");

                }
                var behaviorToRemove = new System.Collections.Generic.List<string>
                {
                    "Robby Cower", "Flee From Enemies", "Robby Back Away", "Point At Enemy", "Lost Sight Of Enemy", "Disengage Combat", "Hide Behind Player", "Watch Enemy",
                };
                FilterGroups(robbyCombatSet, behaviorToRemove);
                RLog.Msg("[Improved Kelvin] behaviors removed.");
            }
        } else RLog.Msg("[Improved Kelvin] robby Controller is null or canCombat is false.");

        robby._onlyStaggeredByPlayer = currentConfig.onlyKilledByPlayer;
        robby._damageController.SetBlocking(currentConfig.canBlockAttack);
        robby._damageController.SetBlockDamageMultiplier(currentConfig.blockDamageMultiplier);
        robby._shotgunKnockbackTime = currentConfig.shotgunKnockbackTime;
    }

    private void SetupKelvinTextures()
    {
        robbyHeadTextureHandler = new BaseTextureHandler("RobbyHead", VailActorTypeId.Robby, "VisualRoot/RobbyRig/GEO/RobbyHead", 0);
        robbyTuxedoJacketTextureHandler = new BaseTextureHandler("TuxedoJacketMesh", VailActorTypeId.Robby, tuxedoJacketMesh, 0);
        robbyHoddieTextureHandler = new BaseTextureHandler("HoodieMesh", VailActorTypeId.Robby, hoddieMesh, 0);

        if (robbyHeadTextureHandler == null || robbyTuxedoJacketTextureHandler == null || robbyHoddieTextureHandler == null)
            RLog.Error("[Improved Kelvin] some (or more than one) of robbyHeadTextureHandler, TuxedoHandler or HoddieHandler is null.");

        RLog.Msg("[ImprovedKelvin] SetupKelvinTextures successfully initialized.");
    }

    private void CheckReviveCondition()
    {
        if (currentConfig.canRevive || !robby.IsDead()) return;
        var currentState = robby.IsRobbyInjured() || robby._hasInjuredState;
        if (currentState)
        {
            RLog.Msg("[ImprovedKelvin] Kelvin can't take this. By the age...");
            robby.SetDead(true);
        }
    }

    private void CheckIfAbleToUseGoldenArmor()
    {
        // IS ABLE TO USE GOLDEN ARMOR? ONLY IF THE PLAYER FINISH THE GAME
        bool stayed = VailWorldStateNetworked.HasWorldFlag(VailWorldStateNetworked.WorldFlags.EndGameContinue);
        bool escaped = VailWorldStateNetworked.HasWorldFlag(VailWorldStateNetworked.WorldFlags.EndGameEscape);
        if ((stayed || escaped))
        {
            currentConfig.IsAbleToUseGoldenArmor = true;
            RLog.Msg($"[ImprovedKelvin] STATUS CONFIRMADO: Jogador terminou o jogo Stay: {stayed}). Armadura Dourada HABILITADA.");
        }
        else {
            currentConfig.IsAbleToUseGoldenArmor = false;
            RLog.Msg("not able to use golden armor"); 
        }
    }

    private void CheckInitLITFWorld()
    {
        if (LITFWorld.Instance != null && LITFWorld.Instance.IsInitialized)
        {
            _initLITFWorld = true;
            RLog.Msg("[Improved Kelvin] Conectado ao LITFWorld (Dados Sincronizados).");
            CheckYearChange();
        }
        else RLog.Error("[Improved Kelvin] LTIFWorld.Instance == null.");
    }
    private void FilterGroups(StateSet stateSet, System.Collections.Generic.List<string> toRemove)
    {
        var cleanedList = new Il2CppSystem.Collections.Generic.List<GroupListItem>();
        var enumerator = stateSet._groupsList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            GroupListItem current = enumerator.Current;
            if (!toRemove.Contains(current.Name)) cleanedList.Add(current);
            else RLog.Msg($"Removendo comportamento: {current.Name}");
        }

        stateSet._groupsList = cleanedList;
        stateSet.CacheRuntimeGroupList(true);
    }

    private void UpdateArmorVisuals()
    {
        var golden = robby.gameObject.transform.Find(goldenArmor);
        var bodyArmor = robby.gameObject.transform.Find(tacticalBodyArmor);
        var helmet = robby.gameObject.transform.Find(tacticalHelmet1);
        var boots = robby.gameObject.transform.Find(tacticalBoots);

        if (golden) golden.gameObject.SetActive(false);
        if (bodyArmor) bodyArmor.gameObject.SetActive(false);
        if (helmet) helmet.gameObject.SetActive(false);
        if (boots) boots.gameObject.SetActive(false);

        if (!_isCosmeticArmorActive) return;
        switch (currentConfig.armorType)
        {
            case KelvinArmorType.Golden:
                if (currentConfig.IsAbleToUseGoldenArmor && golden)
                    golden.gameObject.SetActive(true);
                break;

            case KelvinArmorType.Military:
                if (bodyArmor) bodyArmor.gameObject.SetActive(true);
                if (helmet) helmet.gameObject.SetActive(true);
                if (boots) boots.gameObject.SetActive(true);
                break;
        }
    }

    /*
     * OneLineFunctions 
     */
    public void PadAction() => UpdateMenuText("comando recebido: g");
    public void UpdateMenuText(string text) { if (menuSystem != null) menuSystem.CustomText = text; }
    private void ToggleActivateRobbyMesh(string meshPart, bool activate)
    {
        robby.gameObject.transform.Find($"VisualRoot/RobbyRig/GEO/{meshPart}").gameObject.SetActive(activate);
        RLog.Msg($"[Improved Kelvin] {meshPart} activate={activate}");
    }

    private void CheckYearChange()
    {
        if (LITFWorld.Instance != null && LITFWorld.Instance.IsInitialized)
        {
            int currentYear = LITFWorld.Instance.currentYearsPassed;
            if (currentYear != _lastCheckedYear)
            {
                RLog.Msg($"[Improved Kelvin] year change of {_lastCheckedYear} to {currentYear}. Updating visual.");
                UpdateKelvinState(currentYear, true);
                _lastCheckedYear = currentYear;
            }
        } else { RLog.Error("[Improved Kelvin] error LITFWorld Instance is null.");  }
    }

    private void KelvinDumpConfig(KelvinConfig currentConfig)
    {
        RLog.Msg("------------------ Kelvin Config ------------------");
        RLog.Msg($"stageName ={currentConfig.stageName}");
        RLog.Msg($"minYear ={currentConfig.minYear}");
        RLog.Msg($"headTexture ={currentConfig.headTexture}");
        RLog.Msg($"tuxedoTexture ={currentConfig.tuxedoTexture}");
        RLog.Msg($"hoddieTexture ={currentConfig.hoddieTexture}");
        RLog.Msg($"hasBeard ={currentConfig.hasBeard}");
        RLog.Msg($"hasHair ={currentConfig.hasHair}");
        RLog.Msg($"armorType ={currentConfig.armorType.ToString()}");
        RLog.Msg($"maxHealth ={currentConfig.maxHealth}");
        RLog.Msg($"maxStamina ={currentConfig.maxStamina}");
        RLog.Msg($"maxHeartRateStat ={currentConfig.maxHeartRateStat}");
        RLog.Msg($"maxFullness ={currentConfig.maxFullness}");
        RLog.Msg($"maxFear ={currentConfig.maxFear}");
        RLog.Msg($"maxAnger ={currentConfig.maxAnger}");
        RLog.Msg($"moveSpeedMultiplier ={currentConfig.moveSpeedMultiplier}");
        RLog.Msg($"meleeWeaponDamage ={currentConfig.meleeWeaponDamage}");
        RLog.Msg($"canCombat ={currentConfig.canCombat}");
        RLog.Msg($"canUseWeapons ={currentConfig.canUseWeapons}");
        RLog.Msg($"canUseCosmeticArmor ={currentConfig.canUseCosmeticArmor}");
        RLog.Msg($"canRevive ={currentConfig.canRevive}");
        RLog.Msg($"removeBehaviors ={currentConfig.removeBehaviors}");
        RLog.Msg($"IsAbleToUseGoldenArmor ={currentConfig.IsAbleToUseGoldenArmor}");
        RLog.Msg($"onlyKilledByPlayer ={currentConfig.onlyKilledByPlayer}");
        RLog.Msg($"shotgunKnockbackTime ={currentConfig.shotgunKnockbackTime}");

        RLog.Msg("------------------ Kelvin Config ----------Z-------");
    }

    private void KelvinDumpInfo()
    {
        RLog.Msg($"ROBBY STAT LIST {robby._staggeredAmount}");

        for (int i = 0; i < robby._statsManager._statsManager.Stats.Count; i++)
        {
            RLog.Msg($"State name {i}: {robby._statsManager._statsManager.Stats[i].GetName()}");
        }

        try
        {
            if (robby._vailGiftManager != null)
                RLog.Msg($"_vailGiftManager.enabled: {robby._vailGiftManager.enabled}");
            else
                RLog.Msg("_vailGiftManager: NULL");
            if (robby.GetWorldSimController() != null)
                RLog.Msg($"GetWorldSimController().name: {robby.GetWorldSimController().name}");
            else RLog.Msg("GetWorldSimController(): NULL");

            if (robby._worldSimController != null)
                for (int i = 0; i < robby.GetWorldSimController()._actions.Count; i++)
                {
                    RLog.Msg($"robby.GetWorldSimController()._actions[i].GetName() {i}: {robby.GetWorldSimController()._actions[i].GetName()}");
                }

            RLog.Msg($"AnimStateParameters: {robby.AnimStateParameters}");
            if (robby._animator != null)
                RLog.Msg($"robby._animator.name: {robby._animator.name}");
            else
                RLog.Msg("_animator: NULL");
            RLog.Msg($"robby._animEvents.name: {robby._animEvents.name}");
            for (int i = 0; i < robby._stateAnimatorSettings.Count; i++)
            {
                RLog.Msg($"robby._stateAnimatorSettings._items[{i}]._state._name: {robby._stateAnimatorSettings._items[i]._state._name}");
            }
            RLog.Msg($"robby.CheckPlayerSentimentLevel(0): {robby.CheckPlayerSentimentLevel(0)}");
            if (robby.ActorStimuli != null)
                RLog.Msg($"robby.ActorStimuli.name: {robby.ActorStimuli.name}");
            if (robby._stimuliGroups != null)
                RLog.Msg($"robby._stimuliGroups.name: {robby._stimuliGroups.name}");
            RLog.Msg($"robby.GetInventoryItemType(): {robby.GetInventoryItemType()}");
            RLog.Msg($"robby._hasInventory: {robby._hasInventory}");

            if (robby._inventoryManager != null)
            {
                for (int i = 0; i < robby._inventoryManager._carryAttachments.Count; i++)
                {
                    RLog.Msg($"robby._inventoryManager._carryAttachments {i}: {robby._inventoryManager._carryAttachments[i].name}");
                }
            }
            RLog.Msg($"speedFloatParam={robby._speedFrame}");
            RLog.Msg($"_staggeredAmount: {robby._staggeredAmount}");
            RLog.Msg($"_staggerLevel: {robby._staggerLevel}");
            RLog.Msg($"_staggeredAmount: {robby._staggerPerHit}");
            RLog.Msg($"_staggeredAmount: {robby._staggeredAmount}");

            RLog.Msg($"_isFemale: {robby._isFemale}");
            RLog.Msg($"_helicopterHeld: {robby._helicopterHeld}");

            if (robby._healthSettings != null)
            {
                RLog.Msg($"_poisonFearRate: {robby._healthSettings._poisonFearRate}");
                RLog.Msg($"_poisonHealthRate: {robby._healthSettings._poisonHealthRate}");
                RLog.Msg($"_poisonEnergyRate: {robby._healthSettings._poisonEnergyRate}");
                RLog.Msg($"_dyingHealthRecover: {robby._healthSettings._dyingHealthRecover}");
                RLog.Msg($"_health: {robby._healthSettings._health}");
                RLog.Msg($"_onlyKilledByPlayer: {robby._healthSettings._onlyKilledByPlayer}");

            }
            else RLog.Msg("_healthSettings: NULL");

            RLog.Msg($"_dropEquipItemsOnDeath: {robby._dropEquipItemsOnDeath}");

            if (robby._dismembermentController != null)
                RLog.Msg($"_dismembermentController: {robby._dismembermentController.name}");
            else
                RLog.Msg("_dismembermentController: NULL");

            RLog.Msg($"_dismemberedParts Count: {robby._dismemberedParts}");
            RLog.Msg($"_damageEnabled: {robby._damageEnabled}");

            if (robby._damageController != null)
                RLog.Msg($"GetBlockDamageMultiplier: {robby._damageController.GetBlockDamageMultiplier()}");
            else
                RLog.Msg("_damageController: NULL");

            RLog.Msg($"_shotgunKnockbackTime: {robby._shotgunKnockbackTime}");

            if (robby._meleeWeapons != null)
            {
                RLog.Msg($"Melee Weapons Count: {robby._meleeWeapons.Count}");
                for (int i = 0; i < robby._meleeWeapons.Count; i++)
                {
                    if (robby._meleeWeapons[i] != null)
                        RLog.Msg($"robby melee weapon {i}: ID={robby._meleeWeapons[i]._id}");
                    else
                        RLog.Msg($"robby melee weapon {i}: ITEM NULL");
                }
            }
            else RLog.Msg("_meleeWeapons List: NULL");

            if (robby._statsManager != null && robby._statsManager._statsManager != null)
            {
                var statsList = robby._statsManager._statsManager.Stats;
                if (statsList != null)
                {
                    for (int i = 0; i < statsList.Count; i++)
                    {
                        if (statsList[i] != null)
                            RLog.Msg($"State name {i}: {statsList[i].GetName()} Value: {statsList[i]._currentValue}");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            RLog.Error($"[DEBUG F5] Erro ao printar stats: {e.Message}");
        }

        RLog.Msg("=== [DEBUG F5] Fim do Dump ===");

    }
}
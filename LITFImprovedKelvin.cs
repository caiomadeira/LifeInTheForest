
using RedLoader;
using Sons.Ai.Vail;
using Sons.StatSystem;
using UnityEngine;
namespace LifeInTheForest;

[RegisterTypeInIl2Cpp]
public class LITFImprovedKelvin: MonoBehaviour
{
    private class OutfitOption
    {
        public string name;
        public List<string> meshesToEnable;
        public int baseWeight;
    }

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
    // config
    public KelvinArmorType armorType;

    // --- robby paths ---
    const string visualRoot = "VisualRoot/RobbyRig/GEO";
    const string goldenArmor = $"{visualRoot}/GoldenArmor";
    const string tuxedoJacketMesh = $"{visualRoot}/TuxedoJacketMesh";
    const string tuxedoPantsMesh = $"{visualRoot}/TuxedoPantsMesh";
    const string tuxedoShoesMesh = $"{visualRoot}/TuxedoShoesMesh";
    const string hoddieMesh = $"{visualRoot}/HoodieMesh";
    const string tacticalHelmet1 = $"{visualRoot}/TacticalArmorHeadHelmetMesh"; // no texture
    const string tacticalHelmet2 = $"{visualRoot}/hemlet"; // no texture
    const string tacticalBodyArmor = $"{visualRoot}/body_armor";
    const string tacticalBoots = $"{visualRoot}/boots";
    const string beard = $"{visualRoot}/RobbyBeard";
    const string head = $"{visualRoot}/RobbyHead";
    const string hair = $"{visualRoot}/RobbyHair";
    const string suitMesh = $"{visualRoot}/SuitMesh";
    const string jacket = $"{visualRoot}/jacket";
    const string boots = $"{visualRoot}/boots";
    const string gloves = $"{visualRoot}/gloves";
    const string sunglasses = $"{visualRoot}/sunglasses";
    const string pants = $"{visualRoot}/pants";
    const string mask = $"{visualRoot}/mask";
    const string pyjamasMesh = $"{visualRoot}/PyjamasMesh";
    const string leatherJacketMesh = $"{visualRoot}/LeatherJacketMesh";
    const string leatherJacketUnderShirt = $"{visualRoot}/LeatherJacketUnderShirt";
    const string priestOutfitShirt = $"{visualRoot}/PriestOutfitShirt";
    const string priestOutfitPants = $"{visualRoot}/PriestOutfitPants";
    const string spaceSuitABodyMesh = $"{visualRoot}/SpaceSuitABodyMesh";
    const string pyjamasBootsMesh = $"{visualRoot}/PyjamasBootsMesh";
    const string leftHand = $"{visualRoot}/LeftHandTrim";
    const string rightHand = $"{visualRoot}/RightHandTrim";

    //private Mesh originalSourceMesh;
    //private Material originalSourceMaterial;

    // -- Control Variables ---
    private int _lastCheckedTimeValue = -1;
    private bool _isCosmeticArmorActive = false;
    private float _nextReviveCheckTime = 0f;
    private bool robbyStatsManagerInstance = false;
    private bool activateLITFKelvinWeaponSystem = false;
    // private bool _beardPrefabInitialized = false;
    private bool _wasInitialized = false;
    private bool _graveSpawned = false;
    private string _currentOutfitName = "";
    private bool _wasNight = false;
    private int _lastDayCheck = -1;

    // --- weapon system ---
    private LITFKelvinWeaponSystem weaponSystem;

    // -- Menu ---
    private LITFKelvinMenu menuSystem;

    // --- robby variables ---
    private List<KelvinConfig> _agingConfigs = new List<KelvinConfig>();
    private List<OutfitOption> _wardrobe = new List<OutfitOption>();
    Dictionary<OutfitOption, int> _cachedWeights = new Dictionary<OutfitOption, int>();

    private KelvinConfig currentConfig;
    public void Awake()
    {
        try
        {
            Instance = this;
            robby = GetComponent<VailActor>();
            SetupKelvinConfig();
            if (Config.Stage1Category == null) { return; } else { SetConfigForYear(0); }
        }
        catch (System.Exception e) { RLog.Error($"[LITF Improved Kelvin]: {e}"); }
    }
    public void Start()
    {
        try
        {
            if (robby == null) { return; }

            SetupKelvinTextures();
            SetupKelvinStats();
            SetupMenuPad();
            SetupWardrobe();

            _wasInitialized = true;
            RLog.Msg("[Improved Kelvin] started.");
        }
        catch (System.Exception e) { RLog.Error($"[LITF Improved Kelvin]: {e}"); }
    }
    public void Update()
    {
        if (!_wasInitialized) return;
        OnTimeChanged();
        CheckOutfitChange();

        if (!robbyStatsManagerInstance) CheckStatsInstance();
        HandleCombatState();

        if (Time.time > _nextReviveCheckTime)
        {
            HandleDeath();
            CheckReviveCondition();
            _nextReviveCheckTime = Time.time + 5.0f;
        }

        // --- INPUTS ---
        Controls();
    }

    private void Controls()
    {
        if (LITFDebug.isDebugActive)
        {
            if (Input.GetKeyDown(KeyCode.G)) PadAction();
            if (Input.GetKeyDown(KeyCode.F10))
            {
                UpdateKelvinState(0, true);
                RLog.Msg($"kelvin state 0. young");
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                UpdateKelvinState(1, true);
                RLog.Msg($"kelvin state 2. veteran");
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                //if (activateLITFKelvinWeaponSystem)
                //    weaponSystem.LITFKelvinWeaponSystemUpdate();
                UpdateKelvinState(3, true);
                RLog.Msg($"kelvin state 3. old");
            }
        }
    }
    private void CheckStatsInstance()
    {
        if (this.robbyStats == null)
        {
            if (robby._statsManager != null)
            {
                this.robbyStats = robby._statsManager._statsManager;
                robbyStatsManagerInstance = true;
            }
        }
    }

    private void UpdateKelvinState(int timeValue, bool forceUpdate = false)
    {
        if (Config.Stage1Category == null || Config.Stage2Category == null || Config.Stage3Category == null || Config.Stage4Category == null)
        {
            RLog.Error("[ImprovedKelvin] FATAL ERROR: Configs not loaded."); 
            return;
        }

        try
        {
            SetConfigForYear(timeValue);

            // -- -behaviors
            CheckIfAbleToUseGoldenArmor();

            RLog.Msg($"[ImprovedKelvin] Updating to Stage: {currentConfig.stageName} (Time Value: {timeValue})");

            if (robbyHeadTextureHandler != null) robbyHeadTextureHandler.SetTexture(currentConfig.headTexture);
            if (robbyTuxedoJacketTextureHandler != null) robbyTuxedoJacketTextureHandler.SetTexture(currentConfig.tuxedoTexture);
            if (robbyHoddieTextureHandler != null) robbyHoddieTextureHandler.SetTexture(currentConfig.hoddieTexture);

            ToggleActivateRobbyMesh(beard, currentConfig.hasBeard);
            ToggleActivateRobbyMesh(hair, currentConfig.hasHair);

            // -- stats ---
            if (robby._statsManager._statsManager == null)
                SetupKelvinStats();

            // -- Kick Attack 
            SetupKelvinKickAttack();
            // --- behavior
            SetupKelvinBehavior();
        } catch (System.Exception e) { RLog.Error($"[ImprovedKelvin] Error: {e}");  }
    }

    private void SetConfigForYear(int timeValue)
    {
        if (_agingConfigs.Count == 0) SetupKelvinConfig();

        if (_agingConfigs.Count == 0)
        {
            return;
        }
        KelvinConfig found = _agingConfigs[0]; // the list is ordered
        foreach(var cfg in _agingConfigs)
        {
            if (timeValue >= cfg.startingThreshold) { found = cfg; }
        }
        currentConfig = found;
    }
    private void HandleCombatState()
    {
        bool isInCombat = robby.IsAlerted();
        bool shouldHaveArmor = currentConfig.canUseCosmeticArmor && isInCombat;
        bool shouldHaveWeapon = currentConfig.canUseWeapons && isInCombat;
        if (isInCombat && currentConfig.stageName != "Young")
        {
            SetStatCurrentValue((int)ActorStatsIndex.AngerStat, currentConfig.maxAnger);
            SetStatCurrentValue((int)ActorStatsIndex.FearStat, 0f);
        }

        if (shouldHaveArmor != _isCosmeticArmorActive)
        {
            _isCosmeticArmorActive = shouldHaveArmor;
            UpdateArmorVisuals();
        }

        if (activateLITFKelvinWeaponSystem)
            weaponSystem.LITFKelvinWeaponSystemHandleCombatState(shouldHaveWeapon);
    }

    private bool SetupKelvinStats()
    {
        if (robby._statsManager != null)
        {
            robbyStats = robby._statsManager._statsManager;
            robbyStats._stats[(int)ActorStatsIndex.HealthStat].SetMax(currentConfig.maxHealth); // 100f
            robbyStats._stats[(int)ActorStatsIndex.AngerStat].SetMax(currentConfig.maxAnger); // 500f
            robbyStats._stats[(int)ActorStatsIndex.FearStat].SetMax(currentConfig.maxFear); // 0f
            robbyStats._stats[(int)ActorStatsIndex.FullnessStat].SetMax(currentConfig.maxFullness);
            robbyStats._stats[(int)ActorStatsIndex.HeartRateStat].SetMax(currentConfig.maxHeartRateStat);
            if (currentConfig.stageName == "Old" || currentConfig.stageName == "Veteran")
            {
                robbyStats._stats[(int)ActorStatsIndex.AngerStat].SetCurrentValue(currentConfig.maxAnger);
            }
            return true;
        }
        else { return false; }
    }


    private void SetStatCurrentValue(int stats, float value) => this.robbyStats._stats[stats].SetCurrentValue(value);
    private void SetStatDefaultValue(int stats, float value) => this.robbyStats._stats[stats].SetDefaultValue(value);
    private void SetupMenuPad()
    {
        menuSystem = this.gameObject.AddComponent<LITFKelvinMenu>();
        if (menuSystem == null) RLog.Msg("[Improved Kelvin] menuSystem == NULL.");
        RLog.Msg("[Improved Kelvin] SetupMenuPad done.");
    }
    private void SetupKelvinKickAttack()
    {
        //         if (currentConfig.canCombat && currentConfig.canUseWeapons)
        if (currentConfig.canCombat && currentConfig.canUseWeapons)
        {
            if (robby._meleeWeapons != null)
            {
                kickWeapon = robby._meleeWeapons[0]._gameObject.GetComponent<Sons.Gameplay.MeleeWeapon>();
                if (kickWeapon != null) kickWeapon._damage = currentConfig.meleeWeaponDamage;
            }
        }
    }

    private void SetupKelvinBehavior()
    {
        robbyController = robby._controller;
        if (robbyController != null && currentConfig.canCombat)
        {
            robbyCombatSet = robbyController.GetStateSet(State.Combat);
            if (robbyCombatSet != null)
            {
                var behaviorToRemove = new System.Collections.Generic.List<string>
                {
                    "Robby Cower", "Flee From Enemies", "Robby Back Away", "Point At Enemy", "Lost Sight Of Enemy", "Disengage Combat", "Hide Behind Player", "Watch Enemy",
                };
                FilterGroups(robbyCombatSet, behaviorToRemove);
                // RLog.Msg("[Improved Kelvin] behaviors removed.");
            }
        }

        robby._onlyStaggeredByPlayer = currentConfig.onlyKilledByPlayer;
        robby._damageController.SetBlocking(currentConfig.canBlockAttack);
        robby._damageController.SetBlockDamageMultiplier(currentConfig.blockDamageMultiplier);
        robby._shotgunKnockbackTime = currentConfig.shotgunKnockbackTime;
    }

    private bool SetupKelvinTextures()
    {
        robbyHeadTextureHandler = new BaseTextureHandler("RobbyHead", VailActorTypeId.Robby, head, 0);
        robbyTuxedoJacketTextureHandler = new BaseTextureHandler("TuxedoJacketMesh", VailActorTypeId.Robby, tuxedoJacketMesh, 0);
        robbyHoddieTextureHandler = new BaseTextureHandler("HoodieMesh", VailActorTypeId.Robby, hoddieMesh, 0);

        if (robbyHeadTextureHandler == null || robbyTuxedoJacketTextureHandler == null || robbyHoddieTextureHandler == null)
        {
            RLog.Error("[Improved Kelvin] some (or more than one) of robbyHeadTextureHandler, TuxedoHandler or HoddieHandler is null.");
            return false;
        }
        RLog.Msg("[ImprovedKelvin] Textures done!");
        return true;
    }

    private void CheckReviveCondition()
    {
        if (currentConfig.canRevive || !robby.IsDead()) return;
        var currentState = robby.IsRobbyInjured() || robby._hasInjuredState;
        if (currentState)
        {
            robby.SetDead(true);
        }

        HandleDeath();
    }

    private void CheckIfAbleToUseGoldenArmor()
    {
        // IS ABLE TO USE GOLDEN ARMOR? ONLY IF THE PLAYER FINISH THE GAME
        bool stayed = VailWorldStateNetworked.HasWorldFlag(VailWorldStateNetworked.WorldFlags.EndGameContinue);
        bool escaped = VailWorldStateNetworked.HasWorldFlag(VailWorldStateNetworked.WorldFlags.EndGameEscape);
        if ((stayed || escaped))
        {
            currentConfig.IsAbleToUseGoldenArmor = true;
        }
        else {
            currentConfig.IsAbleToUseGoldenArmor = false;
        }
    }

    private void OnTimeChanged()
    {
        if (LITFWorld.Instance != null && LITFWorld.Instance.IsInitialized)
        {
            int currentTime = LITFWorld.Instance.GetCurrentAgeValue();
            if (currentTime != _lastCheckedTimeValue)
            {
                _lastCheckedTimeValue = currentTime;
                try { 
                    UpdateKelvinState(currentTime, true);
                    try
                    {
                        string unit = Config.AgingGranularity.Value;
                        RLog.Msg($"[LITFImprovedKelvin] Time was passed. New value= {currentTime}, unit= ({unit})");
                    } catch (System.Exception e) { RLog.Error($"Error on UpdateKelvinState (unit): {e.Message}"); }
                    LITFDebug.KelvinDumpConfig(currentConfig, true);  
                }
                catch (System.Exception e) { RLog.Error($"Error on UpdateKelvinState: {e.Message}"); }
            }
        }
    }
    private void FilterGroups(StateSet stateSet, System.Collections.Generic.List<string> toRemove)
    {
        var cleanedList = new Il2CppSystem.Collections.Generic.List<GroupListItem>();
        var enumerator = stateSet._groupsList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            GroupListItem current = enumerator.Current;
            if (!toRemove.Contains(current.Name)) cleanedList.Add(current);
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
        robby.gameObject.transform.Find(meshPart).gameObject.SetActive(activate);
        RLog.Msg($"[Improved Kelvin] {meshPart} activate={activate}");
    }
    private void SetupKelvinConfig()
    {
        _agingConfigs.Clear();
        try
        {
            // Verifica se a primeira config existe (usando o novo nome)
            if (Config.Stage1Threshold == null)
            {
                RLog.Error("SetupKelvinConfig: Configs not ready (Stage1Threshold is null).");
                return;
            }

            // 1. YOUNG (STAGE 1)
            _agingConfigs.Add(new KelvinConfig
            {
                stageName = "Young",
                headTexture = "RobbyHeadYoung1",
                tuxedoTexture = "RobbyTuxedo1",
                hoddieTexture = "RobbyHoodie1",
                armorType = KelvinArmorType.Military,

                startingThreshold = Config.Stage1Threshold.Value,
                hasBeard = Config.Stage1Beard.Value,
                hasHair = Config.Stage1Hair.Value,
                maxHealth = Config.Stage1Health.Value,
                maxStamina = Config.Stage1Stamina.Value,
                maxHeartRateStat = Config.Stage1HeartRate.Value,
                maxFullness = Config.Stage1Fullness.Value,
                maxFear = Config.Stage1Fear.Value,
                maxAnger = Config.Stage1Anger.Value,
                meleeWeaponDamage = Config.Stage1Dmg.Value,
                moveSpeedMultiplier = Config.Stage1Speed.Value,
                blockDamageMultiplier = Config.Stage1BlockMult.Value,
                shotgunKnockbackTime = Config.Stage1Knockback.Value,
                staggerLevel = (int)Config.Stage1StaggerLvl.Value,
                staggerPerHit = Config.Stage1StaggerHit.Value,
                canCombat = Config.Stage1Combat.Value,
                canUseWeapons = Config.Stage1Weapons.Value,
                canRevive = Config.Stage1Revive.Value,
                removeBehaviors = Config.Stage1RemBehav.Value,
                canUseCosmeticArmor = Config.Stage1Armor.Value,
                onlyKilledByPlayer = Config.Stage1KillOnly.Value,
                canBlockAttack = Config.Stage1CanBlock.Value
            });

            // 2. EXPERIENCED (STAGE 2)
            _agingConfigs.Add(new KelvinConfig
            {
                stageName = "Experienced",
                headTexture = "RobbyHeadOld2",
                tuxedoTexture = "RobbyTuxedo2",
                hoddieTexture = "RobbyHoodie1",
                armorType = KelvinArmorType.Military,

                startingThreshold = Config.Stage2Threshold.Value,
                hasBeard = Config.Stage2Beard.Value,
                hasHair = Config.Stage2Hair.Value,
                maxHealth = Config.Stage2Health.Value,
                maxStamina = Config.Stage2Stamina.Value,
                maxHeartRateStat = Config.Stage2HeartRate.Value,
                maxFullness = Config.Stage2Fullness.Value,
                maxFear = Config.Stage2Fear.Value,
                maxAnger = Config.Stage2Anger.Value,
                meleeWeaponDamage = Config.Stage2Dmg.Value,
                moveSpeedMultiplier = Config.Stage2Speed.Value,
                blockDamageMultiplier = Config.Stage2BlockMult.Value,
                shotgunKnockbackTime = Config.Stage2Knockback.Value,
                staggerLevel = (int)Config.Stage2StaggerLvl.Value,
                staggerPerHit = Config.Stage2StaggerHit.Value,
                canCombat = Config.Stage2Combat.Value,
                canUseWeapons = Config.Stage2Weapons.Value,
                canRevive = Config.Stage2Revive.Value,
                removeBehaviors = Config.Stage2RemBehav.Value,
                canUseCosmeticArmor = Config.Stage2Armor.Value,
                onlyKilledByPlayer = Config.Stage2KillOnly.Value,
                canBlockAttack = Config.Stage2CanBlock.Value
            });

            // 3. VETERAN (STAGE 3)
            _agingConfigs.Add(new KelvinConfig
            {
                stageName = "Veteran",
                headTexture = "RobbyHeadOld3",
                tuxedoTexture = "RobbyTuxedo2",
                hoddieTexture = "RobbyHoodie1",
                armorType = KelvinArmorType.Golden,

                startingThreshold = Config.Stage3Threshold.Value,
                hasBeard = Config.Stage3Beard.Value,
                hasHair = Config.Stage3Hair.Value,
                maxHealth = Config.Stage3Health.Value,
                maxStamina = Config.Stage3Stamina.Value,
                maxHeartRateStat = Config.Stage3HeartRate.Value,
                maxFullness = Config.Stage3Fullness.Value,
                maxFear = Config.Stage3Fear.Value,
                maxAnger = Config.Stage3Anger.Value,
                meleeWeaponDamage = Config.Stage3Dmg.Value,
                moveSpeedMultiplier = Config.Stage3Speed.Value,
                blockDamageMultiplier = Config.Stage3BlockMult.Value,
                shotgunKnockbackTime = Config.Stage3Knockback.Value,
                staggerLevel = (int)Config.Stage3StaggerLvl.Value,
                staggerPerHit = Config.Stage3StaggerHit.Value,
                canCombat = Config.Stage3Combat.Value,
                canUseWeapons = Config.Stage3Weapons.Value,
                canRevive = Config.Stage3Revive.Value,
                removeBehaviors = Config.Stage3RemBehav.Value,
                canUseCosmeticArmor = Config.Stage3Armor.Value,
                onlyKilledByPlayer = Config.Stage3KillOnly.Value,
                canBlockAttack = Config.Stage3CanBlock.Value
            });

            // 4. OLD (STAGE 4)
            _agingConfigs.Add(new KelvinConfig
            {
                stageName = "Old",
                headTexture = "RobbyHeadOld5",
                tuxedoTexture = "RobbyTuxedo3",
                hoddieTexture = "RobbyHoodie1",
                armorType = KelvinArmorType.Golden,

                startingThreshold = Config.Stage4Threshold.Value,
                hasBeard = Config.Stage4Beard.Value,
                hasHair = Config.Stage4Hair.Value,
                maxHealth = Config.Stage4Health.Value,
                maxStamina = Config.Stage4Stamina.Value,
                maxHeartRateStat = Config.Stage4HeartRate.Value,
                maxFullness = Config.Stage4Fullness.Value,
                maxFear = Config.Stage4Fear.Value,
                maxAnger = Config.Stage4Anger.Value,
                meleeWeaponDamage = Config.Stage4Dmg.Value,
                moveSpeedMultiplier = Config.Stage4Speed.Value,
                blockDamageMultiplier = Config.Stage4BlockMult.Value,
                shotgunKnockbackTime = Config.Stage4Knockback.Value,
                staggerLevel = (int)Config.Stage4StaggerLvl.Value,
                staggerPerHit = Config.Stage4StaggerHit.Value,
                canCombat = Config.Stage4Combat.Value,
                canUseWeapons = Config.Stage4Weapons.Value,
                canRevive = Config.Stage4Revive.Value,
                removeBehaviors = Config.Stage4RemBehav.Value,
                canUseCosmeticArmor = Config.Stage4Armor.Value,
                onlyKilledByPlayer = Config.Stage4KillOnly.Value,
                canBlockAttack = Config.Stage4CanBlock.Value
            });
        }
        catch (System.Exception e)
        {
            RLog.Error($"[Improved Kelvin] CRITICAL ERROR defining config: {e}");
            if (_agingConfigs.Count == 0)
            {
                _agingConfigs.Add(new KelvinConfig { stageName = "Fallback", startingThreshold = 0, maxHealth = 100 });
            }
        }
    }
    public void HandleDeath()
    {
        if (robby.IsDead())
        {
            if (!_graveSpawned)
            {
                var gravePrefab = LITFUtils.SpawnPrefab(LITFGrave01Prefab.grave, robby.Position());
                if (gravePrefab) gravePrefab.transform.Rotate(0f, 180f, 0f);

                _graveSpawned = true;
                RLog.Msg("[Improved Kelvin] Grave spawned.");
            }

            var visualRoot = robby.transform.Find("VisualRoot");
            if (visualRoot != null) visualRoot.gameObject.SetActive(false);
        }
        else
        {
            _graveSpawned = false;
        }
    }

    private void SetupWardrobe()
    {
        _wardrobe.Clear();

        _wardrobe.Add(new OutfitOption
        {
            name = "Default",
            baseWeight = 10,
            meshesToEnable = new List<string> { jacket, pants, boots, gloves }
        });

        _wardrobe.Add(new OutfitOption
        {
            name = "Pajamas",
            baseWeight = 5,
            meshesToEnable = new List<string> { pyjamasMesh, pyjamasBootsMesh, leftHand, rightHand }
        });

        _wardrobe.Add(new OutfitOption
        {
            name = "CasualHoodie",
            baseWeight = 50,
            meshesToEnable = new List<string> { hoddieMesh, pants, boots, leftHand, rightHand }
        });

        _wardrobe.Add(new OutfitOption
        {
            name = "ColdHoodie",
            baseWeight = 50,
            meshesToEnable = new List<string> { hoddieMesh, pants, boots, gloves }
        });

        _wardrobe.Add(new OutfitOption
        {
            name = "Priest",
            baseWeight = 20,
            meshesToEnable = new List<string> { priestOutfitShirt, priestOutfitPants, leftHand, rightHand, boots }
        });

        _wardrobe.Add(new OutfitOption
        {
            name = "Tuxedo",
            baseWeight = 10,
            meshesToEnable = new List<string> { tuxedoJacketMesh, tuxedoPantsMesh, tuxedoShoesMesh, gloves }
        });

        _wardrobe.Add(new OutfitOption
        {
            name = "Suit",
            baseWeight = 10,
            meshesToEnable = new List<string> { suitMesh, tuxedoPantsMesh, leftHand, rightHand }
        });

        _wardrobe.Add(new OutfitOption
        {
            name = "LeatherJacket",
            baseWeight = 15,
            meshesToEnable = new List<string> { leatherJacketMesh, leatherJacketUnderShirt, boots, leftHand, rightHand, sunglasses }
        });
    }

    private void CheckOutfitChange()
    {
        if (LITFWorld.Instance == null || !LITFWorld.Instance.IsInitialized) return;

        bool isNight = VailWorldSimulation.IsNight;
        int currentDay = LITFWorld.Instance.currentDaysPassed;
        string season = LITFWorld.Instance.currentSeasonName.ToLower();

        if (currentDay == _lastDayCheck && isNight == _wasNight) return;
        _lastDayCheck = currentDay;
        _wasNight = isNight;

        _cachedWeights.Clear();

        foreach (var outfit in _wardrobe)
        {
            int weight = outfit.baseWeight;
            if (isNight)
            {
                switch (outfit.name)
                {
                    case "Pajamas":
                        weight += 80;
                        if (season == "summer") weight -= 40;
                        break;

                    case "Tuxedo" or "LeatherJacket":
                        weight = 0;
                        break;
                    default:
                        weight = Mathf.Max(1, weight / 4);
                        break;
                }
            }
            else // day
            {
                if (outfit.name == "Pajamas") weight = 0;
            }

            // seasons weight calculation
            switch(season)
            {
                case "winter":
                    if (outfit.name == "ColdHoodie") weight += 70;
                    if (outfit.name == "LeatherJacket") weight += 40;
                    if (outfit.name == "Priest") weight -= 20;
                    if (outfit.name == "CasualHoodie") weight += 60;
                    if (outfit.name == "Default") weight += 40;
                    if (outfit.name == "Suit") weight += 50;
                    if (outfit.name == "Tuxedo") weight += 58;
                    break;

                case "summer":
                    if (outfit.name == "ColdHoodie") weight -= 50;
                    if (outfit.name == "LeatherJacket") weight -= 20;
                    if (outfit.name == "Priest") weight += 30;
                    if (outfit.name == "CasualHoodie") weight += 20;
                    if (outfit.name == "Default") weight += 40;
                    if (outfit.name == "Suit") weight -= 10;
                    if (outfit.name == "Tuxedo") weight -= 30;
                    break;

                case "spring":
                    if (outfit.name == "ColdHoodie") weight -= 40;
                    if (outfit.name == "LeatherJacket") weight -= 15;
                    if (outfit.name == "Priest") weight += 20;
                    if (outfit.name == "CasualHoodie") weight += 15;
                    if (outfit.name == "Default") weight += 40;
                    if (outfit.name == "Suit") weight -= 10;
                    if (outfit.name == "Tuxedo") weight -= 10;
                    break;

                case "fall" or "autumn":
                    if (outfit.name == "ColdHoodie") weight += 20;
                    if (outfit.name == "LeatherJacket") weight += 30;
                    if (outfit.name == "Priest") weight += 20;
                    if (outfit.name == "CasualHoodie") weight += 40;
                    if (outfit.name == "Default") weight += 60;
                    if (outfit.name == "Suit") weight += 30;
                    if (outfit.name == "Tuxedo") weight += 40;
                    break;
            }

            if (weight < 0) weight = 0;
            _cachedWeights.Add(outfit, weight);
        }

        OutfitOption selected = GetWeightedRandomOutfit(_cachedWeights);
        if (selected != null && selected.name != _currentOutfitName) {
            ApplyOutfit(selected);
        }
    }

    private OutfitOption GetWeightedRandomOutfit(Dictionary<OutfitOption, int> weights)
    {
        int totalWeight = 0;
        foreach (var w in weights.Values) totalWeight += w;

        if (totalWeight <= 0)
        {
            foreach (var k in weights.Keys) return k;
            return null;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int cursor = 0;

        foreach (var entry in weights)
        {
            cursor += entry.Value;
            if (randomValue < cursor)
                return entry.Key;
        }

        return null;
    }

    private void ApplyOutfit(OutfitOption outfit)
    {
        RLog.Msg($"[ImprovedKelvin] Trocando roupa para: {outfit.name}");
        _currentOutfitName = outfit.name;

        string[] allMeshes = {
            pyjamasBootsMesh, pyjamasMesh, leatherJacketMesh, leatherJacketUnderShirt, pants, boots, sunglasses,
            tuxedoJacketMesh, tuxedoPantsMesh, tuxedoShoesMesh, priestOutfitShirt, hoddieMesh, suitMesh, jacket, 
            mask
        };

        foreach (var m in allMeshes) ToggleActivateRobbyMesh(m, false);
        foreach (var meshName in outfit.meshesToEnable) ToggleActivateRobbyMesh(meshName, true);
    }
}

using Il2CppSystem.Runtime.Remoting.Messaging;
using RedLoader;
using RedLoader.Utils;
using Sons.Ai.Vail;
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

    // -- stats --
    public float maxHealth;
    public float maxStamina;
    public float moveSpeedMultiplier;
    public float meleeWeaponDamage;

    // -- behavior --
    public bool canCombat;
    public bool canUseWeapons;
    public bool canUseCosmeticArmor;
    public bool canRevive;
    public bool removeBehaviors;
    public bool IsAbleToUseGoldenArmor;
}

[RegisterTypeInIl2Cpp]
public class ImprovedKelvin: MonoBehaviour
{
    // --- singleton
    public static ImprovedKelvin Instance { get; private set; }
    // --- robby --- 
    private VailActor robby;
    private Sons.Gameplay.MeleeWeapon kickWeapon;
    private StatsManager robbyStats;
    private VailController robbyController;
    private StateSet robbyCombatSet;
    private RobbyActorState robbyActorState;

    // --- Visual Handlers ---
    // head
    private BaseTextureHandler robbyHeadTextureHandler;
    private BaseTextureHandler robbyTuxedoJacketTextureHandler;
    private BaseTextureHandler robbyHoddieTextureHandler;

    // --- robby paths ---
    const string goldenArmor = "VisualRoot/RobbyRig/GEO/GoldenArmor";
    const string tuxedoJacketMesh = "VisualRoot/RobbyRig/GEO/TuxedoJacketMesh";
    const string hoddieMesh = "VisualRoot/RobbyRig/GEO/HoodieMesh";
    const string tacticalHelmet1 = "VisualRoot/RobbyRig/GEO/TacticalArmorHeadHelmetMesh"; // no texture
    const string tacticalHelmet2 = "VisualRoot/RobbyRig/GEO/helmet"; // no texture
    const string tacticalBodyArmor = "VisualRoot/RobbyRig/GEO/body_armor";
    const string tacticalBoots = "VisualRoot/RobbyRig/GEO/boots";

    // --- weapons variables ---
    private GameObject _fakeWeaponInstance;
    private Transform _weaponAnchor;
    private bool _isWeaponAttached = false;
    private int compactPistolId = 355;
    private int shotgunId = 358;

    // -- Control Variables ---
    // private bool IsAbleToUseGoldenArmor = true; // temporary
    private int _lastCheckedYear = -1;
    private bool _initLITFWorld = false;
    private bool _isCosmeticArmorActive = false;
    // -- menu system (pad)
    private LITFKelvinMenu menuSystem;

    // --- robby variables ---
    private List<KelvinConfig> _agingConfigs = new List<KelvinConfig>();
    private KelvinConfig currentConfig;

    public void Awake()
    {
        Instance = this;
        this.robby = GetComponent<VailActor>();
        if (robby == null) { RLog.Error("[Improved Kelvin] robby (VailActor) == null."); return; }

        Transform rightHandBone = FindBone(this.transform, "RightHand");
        if (rightHandBone != null)
        {
            // 2. Busca o objeto de anexo que deu certo no seu teste
            _weaponAnchor = rightHandBone.Find("RightHandWeapon");

            // Se achou, garante que o socket está ativo para renderizar a arma
            if (_weaponAnchor != null)
            {
                _weaponAnchor.gameObject.SetActive(true);
                RLog.Msg("[ImprovedKelvin] Anchor 'RightHandWeapon' encontrado com sucesso.");
            } else RLog.Error("[ImprovedKelvin] RightHand encontrado, mas 'RightHandWeapon' não existe dentro dele.");
        }  else RLog.Error("[ImprovedKelvin] Osso 'RightHand' não encontrado.");

        SetupAgingConfig();
    }
    public void Start()
    {
        // -- kelvin textures
        SetupKelvinTextures();
        // -- kelvin menu pad ---
        menuSystem = this.gameObject.AddComponent<LITFKelvinMenu>();
        if (menuSystem == null ) RLog.Msg("[Improved Kelvin] menuSystem == NULL.");

        // -- base stats
        if (robby._statsManager != null)
        {
            this.robbyStats = robby._statsManager._statsManager;
            this.robbyStats._stats[2]._currentValue = 100f;
            this.robbyStats._stats[2]._max = 100f;
            this.robbyStats._stats[1]._currentValue = 500f; // anger
            this.robbyStats._stats[8]._currentValue = 0f; // fear

        }
        else { RLog.Error("[Improved Kelvin] (robby._statsManager == null."); } 
        // --- update --- 
        //UpdateKelvinState(0, true);
    }


    public void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.G)) PadAction();

        if (Input.GetKeyDown(KeyCode.F5))
        {
            UpdateKelvinState(2, true);
            bool newState = !_isWeaponAttached;
            RLog.Msg($"DEBUG: Forçando Arma Visual -> {newState}");
            ToggleWeapon(newState);
            _isWeaponAttached = newState;

            if (newState) robby.transform.LookAt(LocalPlayer.Transform.position);
        }
    }

    public void LateUpdate()
    {
        if (currentConfig.canUseWeapons && _isWeaponAttached && robby.IsAlerted())
        {
            UpdateAimingLogic();
        }
    }

    private void UpdateAimingLogic()
    {
        VailActor target = GetNearestEnemy();
        if (target != null && target.transform != null)
        {
            Vector3 lookPosition = target.transform.position;
            lookPosition.y = robby.transform.position.y; 
            robby.transform.LookAt(lookPosition);
        }
    }

    private VailActor GetNearestEnemy()
    {
        if (VailWorldSimulation._instance == null) return null;
        float shortestDistance = 50f;
        VailActor nearestEnemy = null;
        Vector3 myPos = robby.transform.position;

        foreach (var actor in VailWorldSimulation._instance._actors)
        {
            if (actor == null || actor.IsDead() || actor.GetVailActor() == robby) continue;
            if (actor.TypeId == VailActorTypeId.Carl)
            {
                float dist = Vector3.Distance(myPos, actor.GetVailActor().transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearestEnemy = actor.GetVailActor();
                }
            }
        }
        return nearestEnemy;
    }
    private void SetupAgingConfig()
    {
        _agingConfigs.Add(new KelvinConfig
        {
            stageName = "Young",
            minYear = 0,
            headTexture = "RobbyHeadYoung1",
            tuxedoTexture = "TuxedoJacketMesh",
            hoddieTexture = "HoodieMesh",
            hasBeard = false,
            hasHair = true,
            maxHealth = 100f,
            maxStamina = 100f,
            moveSpeedMultiplier = 2,
            meleeWeaponDamage = 20.0f,
            canCombat = false,
            canUseWeapons = false,
            canRevive = true,
            removeBehaviors = false,
            canUseCosmeticArmor = false,
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
            meleeWeaponDamage = 50.0f,
            moveSpeedMultiplier = 5,
            canCombat = true,
            canUseWeapons = false,
            canRevive = true,
            removeBehaviors = true,
            canUseCosmeticArmor = false,
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
            meleeWeaponDamage = 100.0f,
            moveSpeedMultiplier = 10,
            canCombat = true,
            canUseWeapons = true,
            canRevive = true,
            removeBehaviors = true,
            canUseCosmeticArmor = true,
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
            maxHealth = 100f,
            maxStamina = 100f,
            meleeWeaponDamage = 200.0f,
            moveSpeedMultiplier = 1,
            canCombat = true,
            canUseWeapons = true,
            canRevive = false,
            removeBehaviors = true,
            canUseCosmeticArmor = true,
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
        if (robby._statsManager != null)
        {
            this.robbyStats = robby._statsManager._statsManager;
            this.robbyStats._stats[2]._currentValue = currentConfig.maxHealth;
            this.robbyStats._stats[2]._max = currentConfig.maxHealth;
            this.robbyStats._stats[1]._currentValue = 500f; // anger
            this.robbyStats._stats[8]._currentValue = 0f; // fear

        }
        else { RLog.Error("[Improved Kelvin] (robby._statsManager == null."); }

        // -- WeaponAttack 
        SetupKelvinWeaponAttack();
        // --- behaviors
        if (currentConfig.minYear >= 2)
        {
            SetupKelvinBehavior();
            RLog.Msg("[Improved Kelvin] Kelvin Veteran Behavior setup.");
        } else RLog.Msg("[Improved Kelvin] Vanilla behavior for Kelvin.");
    }

    private void HandleCombatState()
    {
        bool isInCombat = robby.IsAlerted();
        bool shouldHaveArmor = currentConfig.canUseCosmeticArmor && isInCombat;
        bool shouldHaveWeapon = currentConfig.canUseWeapons && isInCombat;

        if (shouldHaveArmor != _isCosmeticArmorActive)
        {
            string armorType = "golden";
            if (currentConfig.minYear >= 1 && currentConfig.minYear <= 2) armorType = "military";
            if (armorType == "military") SetArmor(shouldHaveArmor ? "add_military_armor" : "rm_military_armor");
            else SetArmor(shouldHaveArmor ? "add_golden_armor" : "rm_golden_armor");
            _isCosmeticArmorActive = shouldHaveArmor;
        }

        if (shouldHaveWeapon != _isWeaponAttached)
        {
            ToggleWeapon(shouldHaveWeapon);
            _isWeaponAttached = shouldHaveWeapon;
        }
    }

    private void ToggleWeapon(bool enable)
    {
        if (enable)
        {
            if (_fakeWeaponInstance == null) AttachFakeWeapon();
            if (_fakeWeaponInstance != null) _fakeWeaponInstance.SetActive(true);
        }
        else
        {
            if (_fakeWeaponInstance != null) _fakeWeaponInstance.SetActive(false);
        }
    }

    private void AttachFakeWeapon()
    {
        // Usa o Anchor encontrado no Awake
        if (_weaponAnchor == null)
        {
            RLog.Error("[ImprovedKelvin] WeaponAnchor é nulo. Tentando achar de novo...");
            Transform rh = FindBone(this.transform, "RightHand");
            if (rh != null) _weaponAnchor = rh.Find("RightHandWeapon");

            if (_weaponAnchor == null) return;
        }

        var itemData = ItemDatabaseManager.ItemById(compactPistolId);
        if (itemData != null && itemData.HeldPrefab != null)
        {
            _fakeWeaponInstance = Instantiate(itemData.HeldPrefab).gameObject;

            var colliders = _fakeWeaponInstance.GetComponentsInChildren<Collider>();
            foreach (var c in colliders) c.enabled = false;
            var rbs = _fakeWeaponInstance.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs) Destroy(rb);

            _fakeWeaponInstance.transform.SetParent(_weaponAnchor, false);

            // --- AQUI ESTÁ A CORREÇÃO BASEADA NO SEU CÓDIGO ---
            // Você usou Zero e Identity, então vamos usar isso
            _fakeWeaponInstance.transform.localPosition = new Vector3(0f, -0.0195f, 0.0724f); // from timmy
            _fakeWeaponInstance.transform.localRotation = Quaternion.Euler(90, 90, 90);
            _fakeWeaponInstance.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

            RLog.Msg("[ImprovedKelvin] Shotgun anexada no RightHandWeapon com coords ZERO.");
        }
    }
    private void SetupKelvinWeaponAttack()
    {
        if (currentConfig.canCombat)
        {
            if (currentConfig.canUseWeapons)
            {
                if (robby._meleeWeapons != null && robby._meleeWeapons.Count > 0)
                {
                    kickWeapon = robby._meleeWeapons[0]._gameObject.GetComponent<Sons.Gameplay.MeleeWeapon>();
                    if (kickWeapon != null) kickWeapon._damage = 200f;
                }
                RLog.Msg("[Improved Kelvin] Robby Cant Use Weapons. Using Melee attack  kickWeapon._damage = 200f.");
            }
            else
            {
                if (robby._meleeWeapons != null && robby._meleeWeapons.Count > 0)
                {
                    kickWeapon = robby._meleeWeapons[0]._gameObject.GetComponent<Sons.Gameplay.MeleeWeapon>();
                    if (kickWeapon != null) kickWeapon._damage = currentConfig.meleeWeaponDamage;
                }
                RLog.Msg("[Improved Kelvin] Robby Cant Use Weapons. Using Melee attack  currentConfig.meleeWeaponDamage.");
            }
        }
        else RLog.Msg("[Improved Kelvin] Robby Cant Combat.");
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
                    "Robby Cower", "Flee From Enemies", "Robby Back Away", " Point At Enemy"
                };
                FilterGroups(robbyCombatSet, behaviorToRemove);
                RLog.Msg("[Improved Kelvin] behaviors removed.");
            }
        } else RLog.Msg("[Improved Kelvin] robby Controller is null or canCombat is false.");
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

    private void SetArmor(string name)
    {
        switch (name)
        {
            case "add_golden_armor":
                robby.gameObject.transform.Find(goldenArmor).gameObject.SetActive(value: true);
                break;
            case "rm_golden_armor":
                robby.gameObject.transform.Find(goldenArmor).gameObject.SetActive(value: false);
                break;
            case "add_military_armor":
                robby.gameObject.transform.Find(tacticalBodyArmor).gameObject.SetActive(value: true);
                robby.gameObject.transform.Find(tacticalHelmet1).gameObject.SetActive(value: true);
                robby.gameObject.transform.Find(tacticalBoots).gameObject.SetActive(value: true);
                break;
            case "rm_military_armor":
                robby.gameObject.transform.Find(tacticalBodyArmor).gameObject.SetActive(value: false);
                robby.gameObject.transform.Find(tacticalHelmet1).gameObject.SetActive(value: false);
                robby.gameObject.transform.Find(tacticalBoots).gameObject.SetActive(value: false);
                break;
            default:
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
                //UpdateAgeVisual(currentYear, false);
                //UpdateAgeCloths(currentYear, false);
                _lastCheckedYear = currentYear;
            }
        } else { RLog.Error("[Improved Kelvin] error LITFWorld Instance is null.");  }
    }

    private Transform FindBone(Transform current, string name)
    {
        if (current.name == name) return current;
        for (int i = 0; i < current.childCount; ++i)
        {
            var found = FindBone(current.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }
}
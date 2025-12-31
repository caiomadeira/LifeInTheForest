
using RedLoader;
using RedLoader.Utils;
using Sons.Ai.Vail;
using Sons.Items.Core;
using Sons.StatSystem;
using SonsSdk;
using System.Reflection;
using UnityEngine;
using static CoopPlayerUpgrades;
using static Il2CppMono.Globalization.Unicode.SimpleCollator;
using static RedLoader.RLog;

namespace LifeInTheForest;

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

    // --- robby textures ---
    // head
    private BaseTextureHandler robbyHeadTextureHandler;
    private string _currentRobbyHeadTextureName = "RobbyHead";

    // tuxedo jacket
    private BaseTextureHandler robbyTuxedoJacketTextureHandler;
    private string _currentTuxedoJacketTextureName = "TuxedoJacketMesh";

    private BaseTextureHandler robbyHoddieTextureHandler;
    private string _currentHoddieTextureName = "HoodieMesh";

    // --- Robby Clothes ---
    const string goldenArmor = "VisualRoot/RobbyRig/GEO/GoldenArmor";
    const string tuxedoJacketMesh = "VisualRoot/RobbyRig/GEO/TuxedoJacketMesh";
    const string hoddieMesh = "VisualRoot/RobbyRig/GEO/HoodieMesh";

    private GameObject _fakeShotgunInstance;

    // -- Control Variables ---
    // TODO: back to make this based if the player finish the game
    private bool IsAbleToUseGoldenArmor = true; // temporary
    private bool _handTrimFounded = false;
    // -- menu system (pad)
    private LITFKelvinMenu menuSystem;

    public void Awake()
    {
        Instance = this;
        robby = GetComponent<VailActor>();
    }
    public void Start()
    {
        SetupKelvinAttack(100.0f, 200.0f, 100.0f);
        // -- general stats
        SetupKelvinStats(1000.0f, 1000.0f);

        // --- controller
        SetupKelvinBehavior();

        // -- kelvin menu pad ---
        menuSystem = this.gameObject.AddComponent<LITFKelvinMenu>();

        // --- texture handlers --- 
        robbyHeadTextureHandler = new BaseTextureHandler("RobbyHead", VailActorTypeId.Robby, "VisualRoot/RobbyRig/GEO/RobbyHead", 0);
        RLog.Msg("[ImprovedKelvin] Face Handler inicializado.");

        robbyTuxedoJacketTextureHandler = new BaseTextureHandler("TuxedoJacketMesh", VailActorTypeId.Robby, tuxedoJacketMesh, 0);
        RLog.Msg("[ImprovedKelvin] Tuxedo Handler inicializado.");

        robbyHoddieTextureHandler = new BaseTextureHandler("HoodieMesh", VailActorTypeId.Robby, hoddieMesh, 0);
        RLog.Msg("[ImprovedKelvin] Hoddie Handler inicializado.");
        // CheckIfAbleToUseGoldenArmor();

        robby.InitEquippedItems();

        // --- CORREÇÃO DO ERRO DA LISTA ---
        // Você precisa usar Il2CppSystem.Collections.Generic explicitamente
        if (robby._equipItems == null)
        {
            robby._equipItems = new Il2CppSystem.Collections.Generic.List<VailActor.EquipItem>();
            RLog.Msg("[ImprovedKelvin] Lista _equipItems criada manualmente (IL2CPP).");
        }
    }

    private void SetupKelvinAttack(float weaponDamage, float weaponSwingSpeed, float weaponSwingSpeedTired)
    {
        kickWeapon = robby._meleeWeapons[0]._gameObject.GetComponent<Sons.Gameplay.MeleeWeapon>();
        kickWeapon._damage = weaponDamage;
        kickWeapon._meleeWeaponData.SwingSpeed = weaponSwingSpeed;
        kickWeapon._meleeWeaponData.SwingSpeedTired = weaponSwingSpeedTired;
        RLog.Msg("[ImprovedKelvin] Robby Weapon Attack.");
    }

    private void SetupKelvinStats(float currentHealthValue, float maxHealtValue)
    {

        VailStatsManager stats = robby.GetStatsManager();
        if (stats == null)
        {

        }
        robbyStats = robby._statsManager._statsManager;
        robby._statsManager._statsManager._stats[2]._currentValue = currentHealthValue;
        robby._statsManager._statsManager._stats[2]._max = maxHealtValue;
    }

    private void SetupKelvinBehavior()
    {
        // - ACTIONS TO REMOVE
        robbyController = robby._controller;
        if (robbyController != null)
        {
            robbyCombatSet = robbyController.GetStateSet(State.Combat);
            var behaviorToRemove = new System.Collections.Generic.List<string>();
            behaviorToRemove.Add("Robby Cower");
            behaviorToRemove.Add("Flee From Enemies");
            behaviorToRemove.Add("Hide Behind Player");
            behaviorToRemove.Add("Robby Back Away");
            behaviorToRemove.Add("Point At Enemy");
            FilterGroups(robbyCombatSet, behaviorToRemove);
        }
    }

    //private void CheckIfAbleToUseGoldenArmor()
    //{
    //    // IS ABLE TO USE GOLDEN ARMOR? ONLY IF THE PLAYER FINISH THE GAME
    //    bool stayed = IsPlayerStayedOnIsland();
    //    bool escaped = IsPlayerEscapeFromIsland();
    //    if (stayed || escaped)
    //    {
    //        IsAbleToUseGoldenArmor = true;
    //        RLog.Msg($"[ImprovedKelvin] STATUS CONFIRMADO: Jogador terminou o jogo Stay: {stayed}). Armadura Dourada HABILITADA.");
    //    } else { RLog.Msg("not able to use golden armor");  }
    //}

    public void UpdateAgeVisual(int yearsPassed, bool forceReload = false)
    {
        if (robbyHeadTextureHandler == null) return;

        string newTextureName = "";
        switch(yearsPassed)
        {
            case 0:
                newTextureName = "RobbyHead";
                ToggleActivateRobbyMesh("RobbyBeard", false);
                RLog.Msg($"[ImprovedKelvin] Aging case 0");
                break;
            case 1:
                newTextureName = "RobbyHeadOld1";
                ToggleActivateRobbyMesh("RobbyBeard", true);
                RLog.Msg($"[ImprovedKelvin] Aging case 1");
                break;
            case 2:
                newTextureName = "RobbyHeadOld2";
                RLog.Msg($"[ImprovedKelvin] Aging case 2");
                break;
            case 3:
                newTextureName = "RobbyHeadOld3";
                RLog.Msg($"[ImprovedKelvin] Aging case 3");
                break;
            default:
                newTextureName = "RobbyHeadOld4";
                ToggleActivateRobbyMesh("RobbyHair", false);
                RLog.Msg($"[ImprovedKelvin] Aging case DEFAULT");
                break;
        }

        if (_currentRobbyHeadTextureName != newTextureName)
        {
            _currentRobbyHeadTextureName = newTextureName;
            UpdateTextureHandler(_currentRobbyHeadTextureName, forceReload);
            RLog.Msg($"[ImprovedKelvin] Textura do rosto alterada para: {_currentRobbyHeadTextureName}");
        }
    }

    //TODO: I need to check if the player already has adquired the cloth AND THEM able to start to aging.
    public void UpdateAgeCloths(int yearsPassed, bool forceReload = false)
    {
        var findHoddieMesh = robby.gameObject.transform.Find(hoddieMesh);
        var findTuxedoMesh = robby.gameObject.transform.Find(tuxedoJacketMesh);

        if (findHoddieMesh != null)
        {
            if (robbyHoddieTextureHandler == null) return;
            string newHoddieTextureName = "RobbyHoodie1";
            if (_currentHoddieTextureName != newHoddieTextureName)
            {
                _currentHoddieTextureName = newHoddieTextureName;
                robbyHoddieTextureHandler.SetTexture(newHoddieTextureName, forceReload);
                RLog.Msg($"[ImprovedKelvin] Textura do Hoodie alterada para: {_currentHoddieTextureName}");
            }

        }

        if (findTuxedoMesh != null)
        {
            if (robbyTuxedoJacketTextureHandler == null) return;
            string newTuxedoTextureName = "TuxedoJacketMesh";

            if (yearsPassed >= 1 && yearsPassed < 2) newTuxedoTextureName = "RobbyTuxedo2";
            else if (yearsPassed >= 2) newTuxedoTextureName = "RobbyTuxedo3";

            if (_currentTuxedoJacketTextureName != newTuxedoTextureName || forceReload)
            {
                _currentTuxedoJacketTextureName = newTuxedoTextureName;
                robbyTuxedoJacketTextureHandler.SetTexture(newTuxedoTextureName, forceReload);
                RLog.Msg($"[ImprovedKelvin] Textura do Tuxedo alterada para: {_currentTuxedoJacketTextureName}");
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
            if (!toRemove.Contains(current.Name))
            {
                cleanedList.Add(current);
            }
            else
            {
                RLog.Msg($"Removendo comportamento: {current.Name}");
            }
        }

        stateSet._groupsList = cleanedList;
        stateSet.CacheRuntimeGroupList(true);
    }

    public void Update()
    {
        //OnKelvinFall();
        //if (robby.IsAlerted() && IsAbleToUseGoldenArmor == true)
        if (robby.IsAlerted())
        {
            RLog.Msg("[ImprovedKelvin] able to use golden armor");
            robbyStats._stats[1]._currentValue = 500f; // anger
            robbyStats._stats[4]._currentValue = 100.0f; // stamina
            robbyStats._stats[8]._currentValue = 0f; // fear
            ChangeArmor("add_golden_armor");
        } else {
            ChangeArmor("rm_golden_armor"); 
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            PadAction();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Transform geoFolder = transform.Find("VisualRoot/RobbyRig/Root/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightForeArmTwistNew1/RightForeArmTwistNew2/RightForeArmTwistNew3/RightForeArmTwistNew4/RightHand");
            for(int i = 0; i < geoFolder.childCount; i++)
            {
                var childTransform = geoFolder.GetChild(i);
                var childGameObject = childTransform.gameObject;

                bool isActive = childGameObject.activeSelf;
                RLog.Msg($"{childGameObject.name}, isActive={isActive}");
                if (childTransform.name.Equals("RightHandWeapon") && isActive)
                {
                    RLog.Msg("FOUND RightHandWeapon AND ITS ACTIVE");
                    if (_fakeShotgunInstance == null)
                    {
                        var itemData = ItemDatabaseManager.ItemById(358);
                        if (itemData != null && itemData.HeldPrefab != null)
                        {
                            _fakeShotgunInstance = Instantiate(itemData.HeldPrefab).gameObject;

                            if (childTransform.transform != null)
                            {
                                _fakeShotgunInstance.transform.SetParent(childTransform.transform, false);
                                // change to local scale
                                _fakeShotgunInstance.transform.localPosition = Vector3.zero;
                                _fakeShotgunInstance.transform.localRotation = Quaternion.identity;

                                var colliders = _fakeShotgunInstance.GetComponentsInChildren<Collider>();
                                foreach (var c in colliders) c.enabled = false;

                                RLog.Msg("[ImprovedKelvin] Shotgun colada no RightHandWeapon  COM SUCESSO!");
                            }
                            else { RLog.Error("[ImprovedKelvin] Child transform are null"); }
                        }
                    }
                }
            }
            robby.StartAim();

            if (_fakeShotgunInstance != null)
            {
                robby.WeaponShoot(_fakeShotgunInstance);
            }
        }
    }
    //private Transform FindBone(Transform current, string name)
    //{
    //    if (current.name == name) return current;
    //    for (int i = 0; i < current.childCount; ++i)
    //    {
    //        var found = FindBone(current.GetChild(i), name);
    //        if (found != null) return found;
    //    }
    //    return null;
    //}

    private void OnKelvinFall()
    {
        if (robby.IsRobbyInjured() || robby.IsDying())
        {
            robby.Revive();
            robby.DyingRecoverHealth(100f);
            RLog.Msg("kelvin revivido.");
        }
    }

    private void ChangeArmor(string name)
    {
        switch (name)
        {
            case "add_golden_armor":
                robby.gameObject.transform.Find(goldenArmor).gameObject.SetActive(value: true);
                break;
            case "rm_golden_armor":
                robby.gameObject.transform.Find(goldenArmor).gameObject.SetActive(value: false);
                break;
            default:
                break;
        }
    }

    public void PadAction()
    {
        RLog.Msg("action executed Worked!!!");
        UpdateMenuText("comando recebido: g");
    }

    public void UpdateTextureHandler(string textureName, bool forceReload = false)
    {
        if (robbyHeadTextureHandler != null)
        {
            robbyHeadTextureHandler.SetTexture(textureName, forceReload);
            RLog.Msg($"[ImprovedKelvin] Updated texture handlers: {textureName}");
        }
    }

    public void ReloadTextures()
    {
        RLog.Msg("[ImprovedKelvin] Reloading textures");
        TextureCache.Clear();
        UpdateTextureHandler(_currentRobbyHeadTextureName, true);
        UpdateTextureHandler(_currentTuxedoJacketTextureName, true);
    }

    public void UpdateMenuText(string text)
    {
        if (menuSystem != null)
        {
            menuSystem.CustomText = text;
        }
    }
    private void ToggleActivateRobbyMesh(string meshPart, bool activate)
    {
        robby.gameObject.transform.Find($"VisualRoot/RobbyRig/GEO/{meshPart}").gameObject.SetActive(activate);
        RLog.Msg($"[Improved Kelvin] {meshPart} activate={activate}");
    }
}


using RedLoader;
using Sons.Ai.Vail;
using Sons.Gameplay;
using Sons.StatSystem;
using UnityEngine;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using Sons.Items.Core;

namespace LifeInTheForest;

[RegisterTypeInIl2Cpp]
public class ImprovedKelvin: MonoBehaviour
{
    // --- robby --- 
    private VailActor robby;
    private Sons.Gameplay.MeleeWeapon kickWeapon;
    private StatsManager robbyStats;
    private VailController robbyController;
    private StateSet robbyCombatSet;
    private GameObject robbyWeaponObj;

    public void Awake()
    {
        robby = GetComponent<VailActor>();
    }
    public void Start()
    {

        kickWeapon = robby._meleeWeapons[0]._gameObject.GetComponent<Sons.Gameplay.MeleeWeapon>();
        kickWeapon._damage = 100.0f;
        kickWeapon._meleeWeaponData.SwingSpeed = 200.0f;
        kickWeapon._meleeWeaponData.SwingSpeedTired = 100.0f;
        RLog.Msg("Kelvin: Arma configurada para matar!");

        // -- general stats
        robby._statsManager._statsManager._stats[2]._currentValue = 1000.0f;
        robby._statsManager._statsManager._stats[2]._max = 1000.0f;
        robbyStats = robby._statsManager._statsManager;

        // --- controller
        robbyController = robby._controller;
        if (robbyController != null)
        {
            robbyCombatSet = robbyController.GetStateSet(State.Combat);
            foreach (var group in robbyCombatSet._groupsList)
            {
              RLog.Msg($"KELVIN GROUP NAME: {group.Name}");
            }

            var behaviorToRemove = new System.Collections.Generic.List<string>();
            // TODO: Nao sei se TODOS os grupos funcionam. Tem que testar melhor.
            behaviorToRemove.Add("Flee Area");
            behaviorToRemove.Add("Run Away, Run To Cover");
            behaviorToRemove.Add("Flee From Enemies");
            behaviorToRemove.Add(" Back away");
            behaviorToRemove.Add("Point At Enemies");
            behaviorToRemove.Add("Step Back Cautious");
            behaviorToRemove.Add("Cross Back Away");
            FilterGroups(robbyCombatSet, behaviorToRemove);
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
        if (robby.IsAlerted())
        {
            robbyStats._stats[1]._currentValue = 500f; // anger
            robbyStats._stats[4]._currentValue = 100.0f; // stamina
            robbyStats._stats[8]._currentValue = 0f; // fear
            ChangeVest("add_golden_armor");
        } else { ChangeVest("rm_golden_armor"); }
    }

    private void ChangeVest(string name)
    {
        switch(name)
        {
            case "add_golden_armor":
                robby.gameObject.transform.Find("VisualRoot/RobbyRig/GEO/GoldenArmor").gameObject.SetActive(value: true);
                break;
            case "rm_golden_armor":
                robby.gameObject.transform.Find("VisualRoot/RobbyRig/GEO/GoldenArmor").gameObject.SetActive(value: false);
                break;
            default:
                break;
        }
    } 

    private void AddWeapon()
    {
       
    }
}



using RedLoader;
using Sons;
using Sons.Ai.Vail;
using Sons.StatSystem;
using SonsSdk;
using UnityEngine;
using Sons.Atmosphere;

namespace LifeInTheForest;

[RegisterTypeInIl2Cpp]
public class LITFWorld : MonoBehaviour
{
    public static LITFWorld Instance { get; private set; }
    private VailWorldSimulation worldSimulation;
    private SeasonsManager seasonsManager;
    public int currentYearsPassed { get; private set; } = 0;
    public bool IsInitialized { get; private set; } = false;
    public void Awake()
    {
        Instance = this;
        RLog.Msg("LITFWorld astarted");
    }
    public void Start()
    {
        worldSimulation = GetComponent<VailWorldSimulation>();
        seasonsManager = UnityEngine.Object.FindObjectOfType<SeasonsManager>();
        RLog.Msg($"[LITF Setup] Simulação: {(worldSimulation != null ? "OK" : "NULL")}");
        RLog.Msg($"[LITF Setup] SeasonsManager: {(seasonsManager != null ? "OK" : "NULL")}");
    }

    public void Update()
    {
        if (seasonsManager == null)
        {
            RLog.Msg("buscando seasonsManager again");
            seasonsManager = UnityEngine.Object.FindObjectOfType<SeasonsManager>();
        }

        if (worldSimulation == null) return;
        currentYearsPassed = GetYearsPassed();
        if (!IsInitialized)
        {
            IsInitialized = true;
            RLog.Msg($"[LITFWorld] Cálculos iniciais prontos. Dias: {worldSimulation.DaysPassed}, Anos: {currentYearsPassed}");
        }
    }

    /*
     * I assume a Year = Summer Duration + Spring Duration + Winter Duration + Atumn Duration.
     * Well, Year / DaysPassed
     */
    private float GetYearDuration()
    {
        if (seasonsManager == null) return 20f; // default duration for Normal Difficult
        seasonsManager.GetSeasonDurations(out float springMagnitude,
                                          out float summerMagnitude,
                                          out float fallMagnitude,
                                          out float winterMagnitude);
        float total = springMagnitude + summerMagnitude + fallMagnitude + winterMagnitude;
        return total > 0 ? total : 20f;
    }
    private int GetYearsPassed()
    {
        float year = GetYearDuration();
        if (year <= 0) return 0;
        return (int)(worldSimulation.DaysPassed / year);
    }

    private float GetYearsMonthPassed()
    {
        float year = GetYearDuration();
        return (worldSimulation.DaysPassed / year);
    }

    private float GetMonthsPassed()
    {
        float yearsPassed = GetYearsMonthPassed();
        int monthIndex = (int)(yearsPassed * 12) % 12;
        return monthIndex + 1; // just to range from 1 to 12 instead of 0 to 11
    }

    private void LogWorldStats()
    {
        int daysPassed = worldSimulation.DaysPassed;
        float timeInHours = worldSimulation.TimeInHours;

        RLog.Msg($"================ WORLD STATS ================");
        RLog.Msg($"Dias Passados: {daysPassed}");
        RLog.Msg($"Hora Atual: {timeInHours:F2}"); // F2 formata para 2 casas decimais
        RLog.Msg($"É noite?: {VailWorldSimulation.IsNight}"); // Propriedade estática

        RLog.Msg($"CurrentSeason: {worldSimulation.CurrentSeason}");
        RLog.Msg($"CurrentWetness (Umidade): {worldSimulation.CurrentWetness}");
        RLog.Msg($"DarknessFactor: {worldSimulation.DarknessFactor}");

        RLog.Msg($"Raiva dos Canibais: {worldSimulation.GetCannibalAngerLevel()}");
        RLog.Msg($"Get Cannibals Killed By Player Count: {worldSimulation.GetCannibalsKilledByPlayerCount()}");
        RLog.Msg($"Get Cannibals Killed By Player Count RECENT: {worldSimulation.GetCannibalsKilledByPlayerCountRecent()}");

        //var progression = simulation.GetGameProgression();
        RLog.Msg($"simulation._gameProgression: {worldSimulation._gameProgression}");


        RLog.Msg($"--- Atores Próximos (Top 5) ---");
        int count = 0;
        foreach (var actor in worldSimulation._nearbyActors)
        {
            if (count >= 5) break;
            RLog.Msg($"-------------------------------");

            RLog.Msg($"actor id={actor.ClassId}");

            RLog.Msg($"actor state={actor.State._name}");

            RLog.Msg($"actor is dead={actor.IsDead()}");

            RLog.Msg($"actor position: {actor._position}");

            RLog.Msg($"actor destination={actor._destination}");

            RLog.Msg($"actor current zone={actor.CurrentZone.ToString()}");

            RLog.Msg($"actor IsFamilyMoveInProgress={actor.IsFamilyMoveInProgress()}");

            RLog.Msg($"actor LastVisitTimeHours={actor.LastVisitTimeHours}");

            RLog.Msg($"actor unique id={actor._uniqueId}");
            count++;
        }
        RLog.Msg($"=============================================");
        SonsTools.ShowMessage($"Dia: {daysPassed} | Hora: {timeInHours:F1}", 3f);
    }
}


using RedLoader;
using Sons;
using Sons.Ai.Vail;
using Sons.StatSystem;
using SonsSdk;
using UnityEngine;
using Sons.Atmosphere;
using TheForest.SerializableTaskSystem;

namespace LifeInTheForest;

[RegisterTypeInIl2Cpp]
public class LITFWorld : MonoBehaviour
{
    public static LITFWorld Instance { get; private set; }
    private VailWorldSimulation worldSimulation;
    private SeasonsManager seasonsManager;
    public int currentYearsPassed { get; private set; } = 0;
    public int currentDaysPassed => worldSimulation != null ? worldSimulation.DaysPassed : 0;

    private float _nextUpdateCheck = 0f;
    private float _cachedYearDuration = 20f;
    public string currentSeasonName { get; set; } = string.Empty;
    public bool IsInitialized { get; private set; } = false;
    public void Awake()
    {
        Instance = this;
        RLog.Msg("[LITFWorld] started");
    }
    public void Start()
    {
        worldSimulation = GetComponent<VailWorldSimulation>();
        seasonsManager = UnityEngine.Object.FindObjectOfType<SeasonsManager>();
        if (worldSimulation != null)
        {
            IsInitialized = true;
            CalculateYearDuration();
            currentYearsPassed = CalculateYearsPassed();
            RLog.Msg($"[LITFWorld] Initialized. Days passed: {currentDaysPassed}, Years passed: {currentYearsPassed}");
        } else
        {
            RLog.Error("[LITFWorld] FATAL Error: VailWorldSimulation is null.");
        }
    }

    public void Update()
    {
        if (!IsInitialized) return;

        if (Time.time > _nextUpdateCheck)
        {
            _nextUpdateCheck = Time.time + 2.0f;

            if (seasonsManager == null)
            {
                seasonsManager = UnityEngine.Object.FindObjectOfType<SeasonsManager>();
            }
            CalculateYearDuration();
            int newYears = CalculateYearsPassed();
            if (newYears != currentYearsPassed)
            {
                currentYearsPassed = newYears;
                RLog.Msg($"[LITFWorld] New year: {currentYearsPassed}");
            }
        }
    }

    public int GetCurrentAgeValue()
    {
        if (LITFWorld.Instance == null) return 0;
        if (Config.AgingGranularity == null) return LITFWorld.Instance.currentYearsPassed;
        string granularity = Config.AgingGranularity.Value;
        if (granularity == "Year") return LITFWorld.Instance.currentYearsPassed;
        else return LITFWorld.Instance.currentDaysPassed;
    }

    /*
     * I assume a Year = Summer Duration + Spring Duration + Winter Duration + Atumn Duration.
     * Well, Year / DaysPassed
     */
    private void CalculateYearDuration()
    {
        if (Config.YearLengthMode == null || Config.AgingGranularity == null)
        {
            _cachedYearDuration = 20f;
            return;
        }
        string mode = Config.YearLengthMode.Value;
        if (Config.AgingGranularity.Value == "Year" && mode == "365 Days")
        {
            _cachedYearDuration = 365f;
            return;
        }
        if (seasonsManager != null)
        {
            seasonsManager.GetSeasonDurations(out float springMagnitude,
                                    out float summerMagnitude,
                                    out float fallMagnitude,
                                    out float winterMagnitude);
            float total = springMagnitude + summerMagnitude + fallMagnitude + winterMagnitude;
            _cachedYearDuration = total > 0 ? total : 20f; // TODO: This change based on difficult. The default duration for Normal Difficult. I must change later.
        }
        else
        {
            _cachedYearDuration = 20f; // TODO: This change based on difficult. The default duration for Normal Difficult. I must change later.
        }
    }
    private int CalculateYearsPassed()
    {
        if (worldSimulation == null) return 0;
        if (_cachedYearDuration <= 0) return 0;
        return (int)(worldSimulation.DaysPassed / _cachedYearDuration);
    }
    private void GetCurrentSeasonName()
    {
        if (worldSimulation == null) return;
        currentSeasonName = worldSimulation.CurrentSeason.ToString();
        RLog.Msg($"[LITF World] current season name: {currentSeasonName}");
    }

    //private float GetYearsMonthPassed()
    //{
    //    float year = GetYearDuration();
    //    return (worldSimulation.DaysPassed / year);
    //}

    //private float GetMonthsPassed()
    //{
    //    float yearsPassed = GetYearsMonthPassed();
    //    int monthIndex = (int)(yearsPassed * 12) % 12;
    //    return monthIndex + 1; // just to range from 1 to 12 instead of 0 to 11
    //}
}

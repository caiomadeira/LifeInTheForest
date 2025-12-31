
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

public struct VirginiaConfig
{
    public string stageName;
    public int minYear;
    public string activeOutfitMesh;
}

[RegisterTypeInIl2Cpp]
public class LITFImprovedVirginia: MonoBehaviour
{
    public static LITFImprovedVirginia Instance {  get; private set; }

    // -- references --
    private VailActor virginia;

    // -- visuals ---
    const string dressMesh = "VisualRoot/VirginiaRig/C1:dress";
    const string camoSuitMesh = "VisualRoot/VirginiaRig/C1:camosuit";

    // --- control variables ---
    private Mesh originalDressMesh;
    private Material originalDressMaterial;

    public void Awake()
    {
        Instance = this;
        virginia = GetComponent<VailActor>();
        if (virginia == null) { RLog.Error("[ImprovedVirginia] VailActor não encontrado!"); return; }
    }
    public void Start()
    {
        
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            RLog.Msg("[ImprovedVirginia]: Injetando Mesh do CamoSuit no Dress...");
            SwapModelGeometry(dressMesh, camoSuitMesh);
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            RestoreOriginalMesh(dressMesh);
        }
    }

    
    private void SwapModelGeometry(string targetPath, string sourcePath)
    {
        GameObject targetObj = virginia.transform.Find(targetPath).gameObject;
        GameObject sourceObj = virginia.transform.Find(sourcePath).gameObject;

        if (targetObj == null || sourceObj == null)
        {
            RLog.Error($"[ImprovedVirginia] Erro ao achar objetos. Target: {targetObj != null}, Source: {sourceObj != null}");
            return;
        }
        SkinnedMeshRenderer targetRenderer = targetObj.GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer sourceRenderer = sourceObj.GetComponent<SkinnedMeshRenderer>();

        if (targetRenderer == null || sourceRenderer == null)
        {
            RLog.Error("[ImprovedVirginia] Um dos objetos não tem SkinnedMeshRenderer!");
            return;
        }

        if (originalDressMesh == null)
        {
            originalDressMesh = targetRenderer.sharedMesh;
            originalDressMaterial = targetRenderer.sharedMaterial;
        }

        targetRenderer.sharedMesh = sourceRenderer.sharedMesh;
        targetRenderer.sharedMaterial = sourceRenderer.sharedMaterial;
        targetObj.gameObject.SetActive(true);
        sourceObj.gameObject.SetActive(false);

        RLog.Msg($"[ImprovedVirginia] Sucesso! O objeto '{targetObj.name}' agora está usando a mesh do '{sourceObj.name}'.");
    }

    private void RestoreOriginalMesh(string targetPath)
    {
        if (originalDressMesh == null) return;

        Transform targetObj = virginia.transform.Find(targetPath);
        if (targetObj != null)
        {
            SkinnedMeshRenderer targetRenderer = targetObj.GetComponent<SkinnedMeshRenderer>();
            if (targetRenderer != null)
            {
                targetRenderer.sharedMesh = originalDressMesh;
                targetRenderer.sharedMaterial = originalDressMaterial;
                RLog.Msg("[ImprovedVirginia] Mesh original restaurada.");
            }
        }
    }
}

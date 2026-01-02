
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
    // private VailActor robbyActor;

    // -- visuals ---
    const string dressMesh = "VisualRoot/VirginiaRig/C1:dress";
    const string camoSuitMesh = "VisualRoot/VirginiaRig/C1:camosuit";
    const string robbyJacket = "Robby0/VisualRoot/RobbyRig/GEO/jacket";

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
        //if (Input.GetKeyDown(KeyCode.F9))
        //{
        //    //RLog.Msg("[ImprovedVirginia]: Injetando Mesh do CamoSuit no Dress...");
        //    //SwapModelGeometry(dressMesh, camoSuitMesh);
        //    ////RLog.Msg("================ HIERARQUIA VIRGINIA ================");
        //    ////DumpHierarchy(virginia.transform, "");
        //    //RLog.Msg("=====================================================");
        //    Animator anim = virginia._animator;
        //    if (anim != null)
        //    {
        //        RLog.Msg("ANIMATOR OK.");

        //        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //        RLog.Msg($"actual state (hash): {stateInfo.shortNameHash}");
        //        RuntimeAnimatorController controller = anim.runtimeAnimatorController;
        //        if (controller != null)
        //        {
        //            RLog.Msg($"nome of controller: {controller.name}");
        //        }

        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.F10))
        //{
        //    // RestoreOriginalMesh(dressMesh);
        //    Animator anim = virginia.GetComponentInChildren<Animator>();
        //    if (anim != null)
        //    {
        //        virginia.GetComponent<VailActor>().enabled = false;
        //        string animToTest = "VirginiaJumpOverBig";
        //        RLog.Msg($"Tentando forçar animação: {animToTest}");
        //        anim.CrossFade(animToTest, 0.2f);
        //        RLog.Msg($"sla se foi {animToTest}");

        //    }
        //}
    }

    private void DumpHierarchy(Transform current, string indent)
    {
        RLog.Msg($"{indent}{current.name}");
        for (int i = 0; i < current.childCount; i++) DumpHierarchy(current.GetChild(i), indent + "  > ");
    }

    private void SwapModelGeometry(string targetPath, string sourcePath)
    {
        RLog.Msg($"[DEBUG] Buscando Target: {targetPath}");
        Transform targetTrans = virginia.transform.Find(targetPath);

        if (targetTrans == null)
        {
            RLog.Error($"[DEBUG] FALHA: Não encontrou TARGET no caminho: {targetPath}");
            Transform rig = virginia.transform.Find("VisualRoot/VirginiaRig");
            if (rig != null)
            {
                RLog.Msg("Listando filhos de VirginiaRig:");
                for (int i = 0; i < rig.childCount; i++) RLog.Msg($"- {rig.GetChild(i).name}");
            }
            return;
        }
        else RLog.Msg("[DEBUG] Target Encontrado!");

        RLog.Msg($"[DEBUG] Buscando Source: {sourcePath}");
        Transform sourceTrans = virginia.transform.Find(sourcePath);

        if (sourceTrans == null)
        {
            RLog.Error($"[DEBUG] FALHA: Não encontrou SOURCE no caminho: {sourcePath}");
            return;
        }
        else RLog.Msg("[DEBUG] Source Encontrado!");

        SkinnedMeshRenderer targetRenderer = targetTrans.GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer sourceRenderer = sourceTrans.GetComponent<SkinnedMeshRenderer>();

        if (targetRenderer == null) RLog.Error("[DEBUG] Target não tem SkinnedMeshRenderer!");
        if (sourceRenderer == null) RLog.Error("[DEBUG] Source não tem SkinnedMeshRenderer!");

        if (targetRenderer != null && sourceRenderer != null)
        {
            if (originalDressMesh == null)
            {
                originalDressMesh = targetRenderer.sharedMesh;
                originalDressMaterial = targetRenderer.sharedMaterial;
                RLog.Msg("[DEBUG] Backup Original Salvo.");
            }

            targetRenderer.sharedMesh = sourceRenderer.sharedMesh;
            targetRenderer.sharedMaterial = sourceRenderer.sharedMaterial;

            // need to modify the bones. Thte skinnedmeshrenderer maps the edges of mesh for a transform list (the bones btw).
            // i need to copy the bones and rootarray form renderer source to target too to not make
            // minvisible mesh with infinite edges (or 0) - preventing for culling
            targetRenderer.bones = sourceRenderer.bones;
            targetRenderer.rootBone = sourceRenderer.rootBone;

            targetTrans.gameObject.SetActive(true);
            sourceTrans.gameObject.SetActive(false);

            RLog.Msg($"[ImprovedVirginia] SUCESSO TOTAL! Mesh Trocada.");
        }
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

    private Transform FindRecursive(Transform current, string name)
    {
        if (current.name.Equals(name)) return current;

        for (int i = 0; i < current.childCount; ++i)
        {
            var found = FindRecursive(current.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }
}

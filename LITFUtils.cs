using RedLoader;
using SonsSdk;
using Sons.Ai.Vail;
using UnityEngine;

namespace LifeInTheForest;

public class LITFUtils
{
    public static LITFUtils Instance { get; private set; }

    public LITFUtils()
    {
        Instance = this;
    }

    public static GameObject SpawnPrefab(GameObject prefab, Vector3 position)
    {
        var p = UnityEngine.Object.Instantiate(prefab, position, Quaternion.Euler(Vector3.zero));
        if (p == null) { RLog.Error("prefab transform is null"); return null; }
        p.GetChildren().ForEach(child =>
        {
            var meshRend = child.GetComponent<MeshRenderer>();
            if (meshRend == null) { RLog.Error("meshRend is null"); }
            if (meshRend) meshRend.sharedMaterial.shader = Shader.Find("Sons/HDRPLit");
        });
        RLog.Msg($"[LITFUtils] prefab {prefab.name} Spawned.");
        return p;
    }

    public static void DumpHierarchy(Transform current, string indent)
    {
        RLog.Msg($"{indent}{current.name}");
        for (int i = 0; i < current.childCount; i++) DumpHierarchy(current.GetChild(i), indent + "  > ");
    }

    public static void SwapModelGeometry(VailActor actor, string targetPath, string sourcePath, Mesh originalMesh, Material originalMaterial)
    {
        RLog.Msg($"[DEBUG] Buscando Target: {targetPath}");
        Transform targetTrans = actor.transform.Find(targetPath);

        if (targetTrans == null)
        {
            RLog.Error($"[DEBUG] FALHA: Não encontrou TARGET no caminho: {targetPath}");
            Transform rig = actor.transform.Find("VisualRoot/VirginiaRig");
            if (rig != null)
            {
                RLog.Msg("Listando filhos de VirginiaRig:");
                for (int i = 0; i < rig.childCount; i++) RLog.Msg($"- {rig.GetChild(i).name}");
            }
            return;
        }
        else RLog.Msg("[DEBUG] Target Encontrado!");

        RLog.Msg($"[DEBUG] Buscando Source: {sourcePath}");
        Transform sourceTrans = actor.transform.Find(sourcePath);

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
            if (originalMesh == null)
            {
                originalMesh = targetRenderer.sharedMesh;
                originalMaterial = targetRenderer.sharedMaterial;
            }

            targetRenderer.sharedMesh = sourceRenderer.sharedMesh;
            targetRenderer.sharedMaterial = sourceRenderer.sharedMaterial;

            targetRenderer.bones = sourceRenderer.bones;
            targetRenderer.rootBone = sourceRenderer.rootBone;

            targetTrans.gameObject.SetActive(true);
            sourceTrans.gameObject.SetActive(false);

            RLog.Msg($"[ImprovedVirginia] mesh changed.");
        }
    }

    public static void SwapModelGeometry2(VailActor actor, string targetPath, string sourcePath, ref Mesh originalMesh, ref Material originalMaterial)
    {
        Transform targetTrans = actor.transform.Find(targetPath);

        if (targetTrans == null) { RLog.Error($"Target not found: {targetPath}"); return; }

        Transform sourceTrans = actor.transform.Find(sourcePath);
        if (sourceTrans == null) { RLog.Error($"Sourcenot found: {sourcePath}"); return; }

        SkinnedMeshRenderer targetRenderer = targetTrans.GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer sourceSMR = sourceTrans.GetComponentInChildren<SkinnedMeshRenderer>(true);
        MeshFilter sourceFilter = sourceTrans.GetComponentInChildren<MeshFilter>(true);
        MeshRenderer sourceMR = sourceTrans.GetComponentInChildren<MeshRenderer>(true);

        if (targetRenderer == null) { RLog.Error("[DEBUG] Target doesn't have SkinnedMeshRenderer!"); return; }
        if (originalMesh == null)
        {
            originalMesh = targetRenderer.sharedMesh;
            originalMaterial = targetRenderer.sharedMaterial;
        }

        Mesh newMesh = null;
        Material newMat = null;

        if (sourceSMR != null)
        {
           // RLog.Msg($"[DEBUG] found mesh: {sourceSMR.name}");
            newMesh = sourceSMR.sharedMesh;
            newMat = sourceSMR.sharedMaterial;
        }
        else if (sourceFilter != null)
        {
            //RLog.Msg($"[DEBUG] Mesh found in MeshFilter (child): {sourceFilter.name}");
            newMesh = sourceFilter.sharedMesh;
            if (sourceMR != null) newMat = sourceMR.sharedMaterial;
        }

        if (newMesh != null)
        {
            targetRenderer.sharedMesh = newMesh;

            if (newMat != null) targetRenderer.sharedMaterial = newMat;

            targetRenderer.localBounds = newMesh.bounds;
            targetRenderer.updateWhenOffscreen = true; 

            //RLog.Msg($"[ImprovedVirginia] Smesh changed: {newMesh.name}");

            sourceTrans.gameObject.SetActive(true);
            targetTrans.gameObject.SetActive(false);
        }
        else
        {
           // RLog.Error($"[DEBUG] O objeto Source ({sourceTrans.name}) e seus filhos NÃO possuem Mesh!");
            for (int i = 0; i < sourceTrans.childCount; i++)
                RLog.Error($"Child {i}: {sourceTrans.GetChild(i).name}");
        }
    }
}


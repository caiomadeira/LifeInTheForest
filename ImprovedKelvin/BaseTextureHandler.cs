using Il2CppInterop.Runtime.InteropTypes;
using RedLoader;
using RedLoader.Utils;
using Sons.Ai.Vail;
using SonsSdk;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LifeInTheForest;

public class BaseTextureHandler
{
    private static PathObject TexturesPath = LoaderEnvironment.GetModDataPath(Assembly.GetExecutingAssembly()) / "Textures";

    private static int BaseColorMapId = Shader.PropertyToID("_BaseColorMap");

    private string _textureBaseName;

    private VailActorTypeId _actorType;

    private string _transformPath;

    private int _materialIndex;

    public string CurrentTexturePath { get; private set; }

    public Texture2D DefaultTexture { get; private set; }

    public Texture2D CurrentTexture { get; private set; }

    public BaseTextureHandler(string textureBaseName, VailActorTypeId actorType, string transformPath, int materialIndex = 0)
    {
        _textureBaseName = textureBaseName;
        _actorType = actorType;
        _transformPath = transformPath;
        _materialIndex = materialIndex;
        if (!TexturesPath.DirectoryExists())
        {
            Directory.CreateDirectory(TexturesPath);
            RLog.Msg($"[TextureHandler] Pasta criada em: {TexturesPath}");
        }
        else
        {
            RLog.Msg($"[TextureHandler] Diretório de texturas detectado: {TexturesPath}");
        }
    }

    private void OnNewActorAdded(VailActor actor)
    {
        if (actor.TypeId == _actorType)
        {
            if ((bool)CurrentTexture)
            {
                SetTextureForActor(actor, CurrentTexture);
            }
            else
            {
                SetTextureForActor(actor, DefaultTexture);
            }
        }
    }

    public void SetTexture(string textureName, bool forceReload = false)
    {
        string text = ((textureName == "Default") ? null : string.Concat(TexturesPath / textureName, ".png"));
        if (!File.Exists(text))
        {
            RLog.Error($"[TextureHandler] FALHA: Arquivo não encontrado!");
            RLog.Error($"[TextureHandler] Procurando por: {text}");
            RLog.Error($"[TextureHandler] Verifique se o nome do arquivo é exatamente '{textureName}.png'");
            return; 
        }
        if ((forceReload || !(text == CurrentTexturePath)) && File.Exists(text))
        {
            CurrentTexturePath = text;
            CurrentTexture = ((textureName == "Default") ? null : TextureCache.GetOrAdd(CurrentTexturePath));
            Update();
        }
    }

    private string PathFromName(string name)
    {
        return TexturesPath / (name + ".png");
    }

    public void Update()
    {
        if (!DefaultTexture)
        {
            DefaultTexture = GetMaterialTexture();
            ExportDefaultIfNeeded();
        }

        if ((bool)CurrentTexture)
        {
            SetMaterialTexture(CurrentTexture);
        }
        else
        {
            SetMaterialTexture(DefaultTexture);
        }
    }

    private void SetMaterialTexture(Texture2D tex)
    {
        SetTextureForActor(ActorTools.GetPrefab(_actorType), tex);
        foreach (VailActor actor in ActorTools.GetActors(_actorType))
        {
            SetTextureForActor(actor, tex);
        }
    }

    private void SetTextureForActor(VailActor actor, Texture2D tex)
    {
        GetMaterialFromTransform(actor, _transformPath, _materialIndex).SetTexture(BaseColorMapId, tex);
    }

    private Texture2D GetMaterialTexture()
    {
        return ((Il2CppObjectBase)(object)GetMaterialFromTransform(ActorTools.GetPrefab(_actorType), _transformPath, _materialIndex).GetTexture(BaseColorMapId)).TryCast<Texture2D>();
    }

    //private Texture2D GetMaterialTexture()
    //{
    //    var texture = GetMaterialFromTransform(ActorTools.GetPrefab(_actorType), _transformPath, _materialIndex).GetTexture(BaseColorMapId);
    //    return (Texture2D)texture;
    //}

    private Material GetMaterialFromTransform(VailActor actor, string path, int materialIndex)
    {
        return actor.transform.Find(path).GetComponent<Renderer>().sharedMaterials[materialIndex];
    }

    public string[] GetTextureNames()
    {
        return (from x in TexturesPath.GetFiles(_textureBaseName + "*.png").Select(Path.GetFileNameWithoutExtension)
                where x != _textureBaseName
                select x).Append("Default").ToArray();
    }

    public PathObject GetDefaultTexturePath()
    {
        return TexturesPath / (_textureBaseName + ".png");
    }

    public void ExportDefaultIfNeeded()
    {
        if (!DefaultTexture)
        {
            throw new Exception("Default texture not set");
        }

        PathObject defaultTexturePath = GetDefaultTexturePath();
        if (!defaultTexturePath.FileExists())
        {
            ExportTex(DefaultTexture, defaultTexturePath);
        }
    }

    public static void ExportTex(Texture tex, string path)
    {
    }
}
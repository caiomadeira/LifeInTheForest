using System.Collections.Generic;
using System.IO;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace LifeInTheForest;

public static class TextureCache
{
    private static Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();

    public static Texture2D GetOrAdd(string path)
    {
        if (_cache.TryGetValue(path, out var value))
        {
            return value;
        }

        value = new Texture2D(2, 2);
        byte[] fileData = File.ReadAllBytes(path);
        Il2CppStructArray<byte> il2cppBytes = fileData;

        ImageConversion.LoadImage(value, il2cppBytes); _cache[path] = value;
        value.hideFlags = HideFlags.HideAndDontSave;
        _cache[path] = value;
        return value;
    }

    public static void Clear()
    {
        foreach (Texture2D value in _cache.Values)
        {
            UnityEngine.Object.Destroy(value);
        }

        _cache.Clear();
    }
}
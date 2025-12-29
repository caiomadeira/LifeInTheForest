using Il2CppInterop.Runtime.InteropTypes.Arrays;
using RedLoader;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LifeInTheForest;

// Token: 0x02000007 RID: 7
public static class TextureCache
{
    // Token: 0x06000050 RID: 80 RVA: 0x00002A44 File Offset: 0x00000C44
    public static Texture2D GetOrAdd(string path)
    {
        Texture2D texture2D;
        if (TextureCache._cache.TryGetValue(path, out texture2D))
        {
            return texture2D;
        }
        texture2D = new Texture2D(2, 2);
        ImageConversion.LoadImage(texture2D, File.ReadAllBytes(path));
        TextureCache._cache[path] = texture2D;
        return texture2D;
    }

    // Token: 0x06000051 RID: 81 RVA: 0x00002A8C File Offset: 0x00000C8C
    public static void Clear()
    {
        foreach (Texture2D obj in TextureCache._cache.Values)
        {
            UnityEngine.Object.Destroy(obj);
        }
        TextureCache._cache.Clear();
    }

    // Token: 0x04000020 RID: 32
    private static Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();
}

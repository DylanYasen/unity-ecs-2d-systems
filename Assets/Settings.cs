using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [Serializable]
    public struct SpriteAnimAsset
    {
        public string ID;
        public Sprite[] sprites;
    }

    public Sprite[] spriteAssets;
    public SpriteAnimAsset[] spriteAnimAssets;


    [ContextMenu("Sort Frames by Name")]
    void SortAnimFrames()
    {
        foreach (var animAsset in spriteAnimAssets)
        {
            System.Array.Sort(animAsset.sprites, (a, b) => GetIndexFromAssetName(b.name) - GetIndexFromAssetName(a.name));
        }
    }

    static int GetIndexFromAssetName(string name)
    {
        var index = name.LastIndexOf("-");
        return int.Parse(name.Substring(index, name.Length - index));
    }
}
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
        public int[] spriteAssetHashes;
    }

    public Sprite[] spriteAssets;
    public SpriteAnimAsset[] spriteAnimAssets;


    [ContextMenu("Sort Frames by Name && Generate Asset Hash")]
    void SortAnimFrames()
    {
        for (int i = 0; i < spriteAnimAssets.Length; i++)
        {
            ref var animAsset = ref spriteAnimAssets[i];

            System.Array.Sort(animAsset.sprites, (a, b) => GetIndexFromAssetName(b.name) - GetIndexFromAssetName(a.name));

            //
            // note: preping the data for job consumption, can't hash on the fly because Sprite.name is only accesiable on the main thread
            animAsset.spriteAssetHashes = new int[animAsset.sprites.Length];
            for (int j = 0; j < animAsset.sprites.Length; j++)
            {
                var sprite = animAsset.sprites[j];
                animAsset.spriteAssetHashes[j] = Animator.StringToHash(sprite.name);
            }
            spriteAnimAssets[i] = animAsset;
        }
    }

    static int GetIndexFromAssetName(string name)
    {
        var index = name.LastIndexOf("-");
        return int.Parse(name.Substring(index, name.Length - index));
    }
}
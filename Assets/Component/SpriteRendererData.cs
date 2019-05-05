using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpriteRendererData : IComponentData
{
    public int spriteAssetHash;
    public int layer;
}

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpriteAnimationData : IComponentData
{
    public int animAssetHash;
    public int framesPerSec;
    public float playRate;

    public float timer;
    public float frameTime;
    public int spriteIndex;
}
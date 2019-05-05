using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Transforms;
using System;
using System.Collections;
using System.Collections.Generic;
using static Settings;

public class Bootstrap : MonoBehaviour
{
    public static Settings Settings;

    public static Dictionary<int, Sprite> spriteAssetRegistry { get; private set; }
    public static Dictionary<int, SpriteAnimAsset> animAssetRegistry { get; private set; }

    public static void LoadAsset()
    {
        spriteAssetRegistry = new Dictionary<int, Sprite>();
        animAssetRegistry = new Dictionary<int, SpriteAnimAsset>();

        foreach (var spriteAsset in Settings.spriteAssets)
        {
            spriteAssetRegistry.Add(Animator.StringToHash(spriteAsset.name), spriteAsset);
        }

        foreach (var animAsset in Settings.spriteAnimAssets)
        {
            animAssetRegistry.Add(Animator.StringToHash(animAsset.ID), animAsset);
        }
    }

    public static void NewGame()
    {
        LoadAsset();

        var entityManager = World.Active.EntityManager;
        var playerArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(SpriteRendererData),
            typeof(SpriteAnimationData),
            typeof(PlayerInputData)
        );

        var player = entityManager.CreateEntity(playerArchetype);
        entityManager.SetComponentData(player, new SpriteRendererData
        {
            spriteAssetHash = Animator.StringToHash("bomber-idle-1"),
        });
        entityManager.SetComponentData(player, new Rotation
        {
            Value = Quaternion.identity
        });
        entityManager.SetComponentData(player, new Translation
        {
            Value = new float3(0, 0, 0)
        });
        entityManager.SetComponentData(player, new SpriteAnimationData
        {
            animAssetHash = Animator.StringToHash("bomber-idle"),
            framesPerSec = 20,
            playRate = 1,
            frameTime = 1.0f / 20.0f,
        });
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        var settingsGo = GameObject.Find("Settings");
        Settings = settingsGo?.GetComponent<Settings>();
        Assert.IsNotNull(Settings);
    }

    public static Sprite GetSpriteAsset(int assetHash)
    {
        Sprite asset;
        spriteAssetRegistry.TryGetValue(assetHash, out asset);
        return asset;
    }

    public static SpriteAnimAsset GetAnimAsset(int assetHash)
    {
        SpriteAnimAsset asset;
        animAssetRegistry.TryGetValue(assetHash, out asset);
        return asset;
    }
}

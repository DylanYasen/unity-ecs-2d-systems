using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
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
        var entityManager = World.Active.EntityManager;
        var playerArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(SpriteRendererData),
            typeof(PlayerInputData),
            typeof(SpriteAnimationData),
            typeof(SpriteAnimTransitionData)
        );

        for (int i = 0; i < 5000; i++)
        {
            var player = entityManager.CreateEntity(playerArchetype);
            entityManager.SetComponentData(player, new SpriteRendererData
            {
                spriteAssetHash = GetAssetHash("bomber-idle-1"),
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
                animAssetHash = GetAssetHash("bomber-idle"),
                framesPerSec = 20,
                frameTime = 1.0f / 20.0f,
                playRate = 1.0f,
            });

            DynamicBuffer<SpriteAnimTransitionData> animTransitionBuffer = entityManager.GetBuffer<SpriteAnimTransitionData>(player);
            animTransitionBuffer.Add(new SpriteAnimTransitionData
            {
                Hash = GetAssetHash("idle-to-run"),
                SourceAnimAssetHash = GetAssetHash("bomber-idle"),
                TargetAnimAssetHash = GetAssetHash("bomber-run")
            });
            animTransitionBuffer.Add(new SpriteAnimTransitionData
            {
                Hash = GetAssetHash("run-to-idle"),
                SourceAnimAssetHash = GetAssetHash("bomber-run"),
                TargetAnimAssetHash = GetAssetHash("bomber-idle"),
            });
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        var settingsGo = GameObject.Find("Settings");
        Settings = settingsGo?.GetComponent<Settings>();
        Assert.IsNotNull(Settings);

        LoadAsset();
    }

    public static Sprite GetSpriteAsset(int assetHash)
    {
        Sprite asset;
        spriteAssetRegistry.TryGetValue(assetHash, out asset);
        return asset;
    }

    public static SpriteAnimAsset GetAnimAsset(int assetHash)
    {
        SpriteAnimAsset asset = default;
        if (animAssetRegistry != null)
        {
            animAssetRegistry.TryGetValue(assetHash, out asset);
        }
        return asset;
    }

    public static SpriteAnimAsset GetAnimAsset(string assetName)
    {
        SpriteAnimAsset asset = default;
        if (animAssetRegistry != null)
        {
            animAssetRegistry.TryGetValue(GetAssetHash(assetName), out asset);
        }
        return asset;
    }

    public static int GetAssetHash(string assetName)
    {
        return Animator.StringToHash(assetName);
    }

    public static int[] GetAssetHashes(string[] assetNames)
    {
        int[] hashes = new int[assetNames.Length];
        for (int i = 0; i < assetNames.Length; i++)
        {
            hashes[i] = GetAssetHash(assetNames[i]);
        }
        return hashes;
    }

    public static void GetAssetHashesNonAlloc(string[] assetNames, ref int[] hashes)
    {
        for (int i = 0; i < assetNames.Length; i++)
        {
            hashes[i] = GetAssetHash(assetNames[i]);
        }
    }
}

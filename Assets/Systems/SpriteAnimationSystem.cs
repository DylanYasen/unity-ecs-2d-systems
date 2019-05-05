using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SpriteAnimationSystem : ComponentSystem
{
    private EntityQuery query;

    protected override void OnCreate()
    {
        query = GetEntityQuery(
            ComponentType.ReadWrite<SpriteRendererData>(),
            ComponentType.ReadWrite<SpriteAnimationData>()
        );
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var dt = Time.deltaTime;

        Entities.With(query).ForEach(
        (Entity entity, ref SpriteRendererData renderData, ref SpriteAnimationData animData) =>
        {
            var animAssset = Bootstrap.GetAnimAsset(animData.animAssetHash);
            var frameCount = animAssset.sprites.Length;

            // todo: handle play rate
            if (animData.timer >= animData.frameTime)
            {
                animData.spriteIndex = (animData.spriteIndex + 1) % frameCount;
                animData.timer -= animData.frameTime;
            }

            var sprite = animAssset.sprites[animData.spriteIndex];
            renderData.spriteAssetHash = Animator.StringToHash(sprite.name);

            animData.timer += dt;
        });
    }
}

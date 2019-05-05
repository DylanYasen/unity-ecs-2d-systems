using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[DisableAutoCreation]
public class SpriteAnimationSystem : JobComponentSystem
{
    private struct UpdateAnimJob : IJobForEach<SpriteRendererData, SpriteAnimationData>
    {
        public float dt;

        public void Execute(ref SpriteRendererData renderer, ref SpriteAnimationData animData)
        {
            var animAsset = Bootstrap.GetAnimAsset(animData.animAssetHash);

            // todo: handle play rate
            if (animData.timer >= animData.frameTime)
            {
                animData.spriteIndex = (animData.spriteIndex + 1) % animAsset.spriteAssetHashes.Length;
                animData.timer -= animData.frameTime;
            }

            renderer.spriteAssetHash = animAsset.spriteAssetHashes[animData.spriteIndex];

            animData.timer += dt;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new UpdateAnimJob
        {
            dt = Time.deltaTime
        };
        inputDeps = job.Schedule(this, inputDeps);
        return inputDeps;
    }
}

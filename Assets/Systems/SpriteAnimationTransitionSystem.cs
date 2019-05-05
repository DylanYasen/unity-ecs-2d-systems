using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class SpriteAnimationTransitionSystem : JobComponentSystem
{
    [BurstCompile]
    struct SpriteAnimationTransitionSystemJob : IJobForEachWithEntity<SpriteAnimationData>
    {

        // Allow buffer read write in parralel jobs
        // Ensure, no two jobs can write to same entity, at the same time.
        // !! "You are somehow completely certain that there is no race condition possible here, because you are absolutely certain that you will not be writing to the same Entity ID multiple times from your parallel for job. (If you do thats a race condition and you can easily crash unity, overwrite memory etc) If you are indeed certain and ready to take the risks.
        // https://forum.unity.com/threads/how-can-i-improve-or-jobify-this-system-building-a-list.547324/#post-3614833
        [NativeDisableParallelForRestriction]
        public BufferFromEntity<SpriteAnimTransitionData> transitionData;

        public void Execute(Entity entity, int index, ref SpriteAnimationData animData)
        {
            DynamicBuffer<SpriteAnimTransitionData> transitionBuffer = transitionData[entity];

            for (int i = 0; i < transitionBuffer.Length; i++)
            {
                var transition = transitionBuffer[i];

                if (transition.SourceAnimAssetHash == animData.animAssetHash
                    && transition.Valid)
                {
                    animData.animAssetHash = transition.TargetAnimAssetHash;
                    animData.timer = 0;
                    animData.spriteIndex = 0;
                    break;
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new SpriteAnimationTransitionSystemJob()
        {
            transitionData = GetBufferFromEntity<SpriteAnimTransitionData>(true)
        };

        return job.Schedule(this, inputDependencies);
    }
}
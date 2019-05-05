
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerMovementSystem : ComponentSystem
{
    private EntityQuery query;

    protected override void OnCreate()
    {
        query = GetEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadOnly<PlayerInputData>()
        );
    }

    protected override void OnUpdate()
    {
        var dt = Time.deltaTime;

        Entities.With(query).ForEach(
            (Entity entity, ref Translation translation, ref PlayerInputData input) =>
            {
                Vector3 move = new Vector3(input.Move.x, input.Move.y, 0);
                var newPos = (Vector3)translation.Value + (Vector3)move.normalized * dt * 100;
                translation.Value = newPos;


                var animTransitionbuffer = World.EntityManager.GetBuffer<SpriteAnimTransitionData>(entity);
                if (input.Move.x != 0 || input.Move.y != 0)
                {
                    for (int i = 0; i < animTransitionbuffer.Length; i++)
                    {
                        var transition = animTransitionbuffer[i];
                        if (animTransitionbuffer[i].Hash == Bootstrap.GetAssetHash("idle-to-run"))
                        {
                            transition.Valid = true;
                            animTransitionbuffer[i] = transition;

                        }
                        else if(transition.Hash == Bootstrap.GetAssetHash("run-to-idle"))
                        {
                            transition.Valid = false;
                            animTransitionbuffer[i] = transition;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < animTransitionbuffer.Length; i++)
                    {
                        var transition = animTransitionbuffer[i];
                        if (transition.Hash == Animator.StringToHash("idle-to-run"))
                        {
                            transition.Valid = false;
                            animTransitionbuffer[i] = transition;
                        }
                        else if (transition.Hash == Animator.StringToHash("run-to-idle"))
                        {
                            transition.Valid = true;
                            animTransitionbuffer[i] = transition;
                        }
                    }
                }
            });
    }
}

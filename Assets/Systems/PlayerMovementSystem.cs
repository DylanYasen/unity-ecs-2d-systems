
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
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
                //new float3(newPos.x, newPos.y, translation.Value.z);
            });
    }
}

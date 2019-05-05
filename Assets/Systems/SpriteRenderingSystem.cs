using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpriteRenderingSystem : ComponentSystem
{
    private EntityQuery query;

    private Dictionary<int, Mesh> meshRegistry = new Dictionary<int, Mesh>();
    private Dictionary<int, Material> materialRegistry = new Dictionary<int, Material>();

    protected override void OnCreate()
    {
        query = GetEntityQuery(
            ComponentType.ReadWrite<SpriteRendererData>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<Rotation>()
        );
    }

    protected override void OnUpdate()
    {
        Entities.With(query).ForEach(
        (Entity entity, ref SpriteRendererData renderData, ref Translation position, ref Rotation rot) =>
        {
            var sprite = Bootstrap.GetSpriteAsset(renderData.spriteAssetHash);

            Mesh mesh;
            Material material;
            if (!meshRegistry.TryGetValue(entity.Index, out mesh))
            {
                var size = math.max(sprite.texture.width, sprite.texture.height) / (float)sprite.pixelsPerUnit;

                mesh = GenerateQuad(size, sprite.pivot);
                meshRegistry.Add(entity.Index, mesh);

                material = new Material(Shader.Find("Sprites/Default"));

                materialRegistry.Add(entity.Index, material);
            }
            materialRegistry.TryGetValue(entity.Index, out material);

            material.mainTexture = sprite.texture;

            Matrix4x4 matrix = Matrix4x4.TRS(position.Value, Quaternion.identity, Vector3.one);
            Graphics.DrawMesh(mesh, matrix, material, renderData.layer, Camera.main, 0, null, false, false, false);
            // Graphics.DrawMesh(mesh, position.Value, rot.Value, material, 1, Camera.main, 0, null, false, false, false);
        });
    }

    public static Mesh GenerateQuad(float size, Vector2 pivot)
    {
        Vector3[] _vertices =
        {
            new Vector3(size - pivot.x, size - pivot.y, 0),
            new Vector3(size - pivot.x, 0 - pivot.y, 0),
            new Vector3(0 - pivot.x, 0 - pivot.y, 0),
            new Vector3(0 - pivot.x,  size - pivot.y, 0)
        };

        Vector2[] _uv =
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };

        int[] triangles =
        {
            0, 1, 2,
            2, 3, 0
        };

        return new Mesh
        {
            vertices = _vertices,
            uv = _uv,
            triangles = triangles
        };
    }
}

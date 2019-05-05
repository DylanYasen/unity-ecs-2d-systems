using Unity.Entities;
using UnityEngine;

public class SystemUpdateHandler : MonoBehaviour
{
    private PlayerInputSystem inputSystem;
    private PlayerMovementSystem movementSystem;
    private SpriteRenderingSystem spriteRenderingSystem;
    private SpriteAnimationSystem spriteAnimSystem;

    private void Start()
    {
        inputSystem = World.Active.GetOrCreateSystem<PlayerInputSystem>();
        movementSystem = World.Active.GetOrCreateSystem<PlayerMovementSystem>();
        spriteRenderingSystem = World.Active.GetOrCreateSystem<SpriteRenderingSystem>();
        spriteAnimSystem = World.Active.GetOrCreateSystem<SpriteAnimationSystem>();
    }

    private void Update()
    {
        inputSystem.Update();
        movementSystem.Update();
        spriteRenderingSystem.Update();
        spriteAnimSystem.Update();
    }
}

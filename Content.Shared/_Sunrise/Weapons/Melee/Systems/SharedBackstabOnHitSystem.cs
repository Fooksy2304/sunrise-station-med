using System.Numerics;
using Content.Shared.Damage;
using Content.Shared._Sunrise.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared._Sunrise.Weapons.Melee.Systems;

public abstract class SharedBackstabOnHitSystem : EntitySystem
{
    // Targets count as backstabbed when attacker is in the rear hemisphere (dot product <= 0).
    private const float BackstabRearHemisphereDotThreshold = 0f;
    // Prevents unstable direction checks when attacker and target are effectively at the same position.
    private const float MinimumBackstabDistanceSquared = 0.0001f;

    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BackstabOnHitComponent, MeleeHitEvent>(OnMeleeHit);
    }

    private void OnMeleeHit(Entity<BackstabOnHitComponent> ent, ref MeleeHitEvent args)
    {
        if (!args.IsHit)
            return;

        // Wide/heavy swings do not receive backstab bonuses.
        if (args.Direction != null)
            return;

        // Bonus is only supported for direct single-target hits.
        if (args.HitEntities.Count != 1)
            return;

        if (!TryComp<TransformComponent>(args.User, out var attackerTransform))
            return;

        var attackerPosition = _transformSystem.GetWorldPosition(attackerTransform);
        var target = args.HitEntities[0];
        if (!TryComp<TransformComponent>(target, out var targetTransform))
            return;

        var vectorToAttacker = attackerPosition - _transformSystem.GetWorldPosition(targetTransform);
        if (vectorToAttacker.LengthSquared() <= MinimumBackstabDistanceSquared)
            return;

        var targetForward = _transformSystem.GetWorldRotation(targetTransform).ToWorldVec();
        var targetForwardDot = Vector2.Dot(targetForward, Vector2.Normalize(vectorToAttacker));

        if (targetForwardDot > BackstabRearHemisphereDotThreshold)
            return;

        if (ent.Comp.BonusDamage != null)
            args.BonusDamage += ent.Comp.BonusDamage;

        if (ent.Comp.DamageModifierSet != null)
            args.ModifiersList.Add(ent.Comp.DamageModifierSet);
    }
}

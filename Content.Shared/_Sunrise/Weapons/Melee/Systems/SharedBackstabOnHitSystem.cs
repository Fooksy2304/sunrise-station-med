using System.Numerics;
using Content.Shared.Damage;
using Content.Shared._Sunrise.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared._Sunrise.Weapons.Melee.Systems;

public abstract class SharedBackstabOnHitSystem : EntitySystem
{
    // Targets count as backstabbed when the attacker is anywhere in the rear hemisphere.
    private const float BackstabRearHemisphereDotThreshold = 0f;

    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BackstabOnHitComponent, MeleeHitEvent>(OnMeleeHit);
    }

    private void OnMeleeHit(Entity<BackstabOnHitComponent> ent, ref MeleeHitEvent args)
    {
        if (!args.IsHit || args.HitEntities.Count == 0)
            return;

        if (!TryComp<TransformComponent>(args.User, out var attackerTransform))
            return;

        var attackerPosition = _transform.GetWorldPosition(attackerTransform);

        foreach (var target in args.HitEntities)
        {
            if (!TryComp<TransformComponent>(target, out var targetTransform))
                continue;

            var targetToAttacker = attackerPosition - _transform.GetWorldPosition(targetTransform);
            if (targetToAttacker.LengthSquared() <= float.Epsilon)
                continue;

            var targetForward = _transform.GetWorldRotation(targetTransform).ToWorldVec();
            var targetForwardDot = Vector2.Dot(targetForward, Vector2.Normalize(targetToAttacker));

            if (targetForwardDot > BackstabRearHemisphereDotThreshold)
                continue;

            if (ent.Comp.BonusDamage != null)
                args.BonusDamage += ent.Comp.BonusDamage;

            if (ent.Comp.DamageModifierSet != null)
                args.ModifiersList.Add(ent.Comp.DamageModifierSet);

            break;
        }
    }
}

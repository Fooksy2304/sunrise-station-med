using Content.Shared.Damage;
using Robust.Shared.GameStates;

namespace Content.Shared._Sunrise.Weapons.Melee.Components;

/// <summary>
/// Adds bonus melee damage when a direct single-target melee hit lands from behind the target.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class BackstabOnHitComponent : Component
{
    /// <summary>
    /// Flat damage added when a backstab is detected.
    /// </summary>
    [DataField]
    public DamageSpecifier? BonusDamage;

    /// <summary>
    /// Damage modifiers applied when a backstab is detected.
    /// </summary>
    [DataField]
    public DamageModifierSet? DamageModifierSet;
}

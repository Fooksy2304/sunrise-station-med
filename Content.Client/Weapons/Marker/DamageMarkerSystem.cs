using Content.Shared.Weapons.Marker;
using Robust.Client.GameObjects;
using Robust.Shared.Timing;

namespace Content.Client.Weapons.Marker;

public sealed class DamageMarkerSystem : SharedDamageMarkerSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DamageMarkerComponent, ComponentStartup>(OnMarkerStartup);
        SubscribeLocalEvent<DamageMarkerComponent, ComponentShutdown>(OnMarkerShutdown);
    }

    private void OnMarkerStartup(Entity<DamageMarkerComponent> ent, ref ComponentStartup args)
    {
        if (!_timing.ApplyingState || !TryComp<SpriteComponent>(ent, out var sprite))
            return;

        var layer = _sprite.LayerMapReserve((ent.Owner, sprite), DamageMarkerKey.Base);
        _sprite.LayerSetRsi((ent.Owner, sprite), layer, ent.Comp.Effect.RsiPath, ent.Comp.Effect.RsiState);

        // Sunrise-Edit
        if (ent.Comp.EffectLight != null)
        {
            var lightLayer = _sprite.LayerMapReserve((ent.Owner, sprite), DamageMarkerKey.Light);
            _sprite.LayerSetRsi((ent.Owner, sprite), lightLayer, ent.Comp.EffectLight.RsiPath, ent.Comp.EffectLight.RsiState);
            sprite.LayerSetShader(lightLayer, "unshaded");
        }
    }

    private void OnMarkerShutdown(Entity<DamageMarkerComponent> ent, ref ComponentShutdown args)
    {
        if (!_timing.ApplyingState || !TryComp<SpriteComponent>(ent, out var sprite))
            return;

        RemoveMarkerLayer((ent.Owner, sprite), DamageMarkerKey.Base);
        // Sunrise-Edit
        RemoveMarkerLayer((ent.Owner, sprite), DamageMarkerKey.Light);
    }

    // Sunrise-Edit
    private void RemoveMarkerLayer(Entity<SpriteComponent> ent, DamageMarkerKey key)
    {
        if (_sprite.LayerMapTryGet(ent, key, out var layer, false))
            _sprite.RemoveLayer(ent, layer);
    }

    private enum DamageMarkerKey : byte
    {
        Base,
        // Sunrise-Edit
        Light
    }
}

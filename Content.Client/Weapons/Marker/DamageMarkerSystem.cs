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

    private void OnMarkerStartup(EntityUid uid, DamageMarkerComponent component, ComponentStartup args)
    {
        if (!_timing.ApplyingState || !TryComp<SpriteComponent>(uid, out var sprite))
            return;

        var layer = _sprite.LayerMapReserve((uid, sprite), DamageMarkerKey.Base);
        _sprite.LayerSetRsi((uid, sprite), layer, component.Effect.RsiPath, component.Effect.RsiState);

        // Sunrise-Edit
        if (component.EffectLight != null)
        {
            var lightLayer = _sprite.LayerMapReserve((uid, sprite), DamageMarkerKey.Light);
            _sprite.LayerSetRsi((uid, sprite), lightLayer, component.EffectLight.RsiPath, component.EffectLight.RsiState);
            _sprite.LayerSetShader((uid, sprite), lightLayer, "unshaded");
        }
    }

    private void OnMarkerShutdown(EntityUid uid, DamageMarkerComponent component, ComponentShutdown args)
    {
        if (!_timing.ApplyingState || !TryComp<SpriteComponent>(uid, out var sprite))
            return;

        RemoveMarkerLayer((uid, sprite), DamageMarkerKey.Base);
        // Sunrise-Edit
        RemoveMarkerLayer((uid, sprite), DamageMarkerKey.Light);
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

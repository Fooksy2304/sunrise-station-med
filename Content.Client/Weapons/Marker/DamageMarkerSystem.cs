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

        if (component.Effect != null)
        {
            var layer = _sprite.LayerMapReserve((uid, sprite), DamageMarkerKey.Base);
            _sprite.LayerSetRsi((uid, sprite), layer, component.Effect.RsiPath, component.Effect.RsiState);
        }

        // Sunrise-Edit
        if (component.EffectLight != null)
        {
            var layer = _sprite.LayerMapReserve((uid, sprite), DamageMarkerKey.Light);
            _sprite.LayerSetRsi((uid, sprite), layer, component.EffectLight.RsiPath, component.EffectLight.RsiState);
            _sprite.LayerSetShader((uid, sprite), layer, "unshaded");
        }
    }

    private void OnMarkerShutdown(EntityUid uid, DamageMarkerComponent component, ComponentShutdown args)
    {
        if (!_timing.ApplyingState || !TryComp<SpriteComponent>(uid, out var sprite))
            return;

        if (_sprite.LayerMapTryGet((uid, sprite), DamageMarkerKey.Base, out var baseLayer, false))
            _sprite.RemoveLayer((uid, sprite), baseLayer);

        // Sunrise-Edit
        if (_sprite.LayerMapTryGet((uid, sprite), DamageMarkerKey.Light, out var lightLayer, false))
            _sprite.RemoveLayer((uid, sprite), lightLayer);
    }

    private enum DamageMarkerKey : byte
    {
        Base,
        // Sunrise-Edit
        Light
    }
}

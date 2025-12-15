# Asset Health

| Severity | Finding | Rationale | Recommendation |
| --- | --- | --- | --- |
| Medium | Sprite import settings keep mipmaps and disable alpha transparency | `Assets/Wave/Sprites/Circle.png.meta` (and Square/Triangle) have `enableMipMap: 1`, `alphaIsTransparency: 0`, and `spritePixelsToUnits: 4`, inflating memory and risking halo artefacts. | Disable mipmaps, enable alpha transparency, and align pixels-per-unit to design values (commonly 100) per sprite size. Create import presets for consistency. |
| Medium | Player object is untagged and relies on its name | `Assets/Wave/Scenes/wave.unity:1465` shows `m_TagString: Untagged`, so systems search by name (`"player"`) to find it. | Define and apply a dedicated `Player` tag in `ProjectSettings/TagManager.asset` and update references (prefabs, scripts) to use tags or injected references. |
| Medium | Collision matrix allows every layer interaction | `ProjectSettings/Physics2DSettings.asset` `m_LayerCollisionMatrix` is all 1s, so FX/UI/obstacle layers cannot be filtered, producing unnecessary contacts. | Audit required interactions, add explicit layers for obstacles/items/FX, and trim the collision matrix to reduce unnecessary physics work. |
| Low | Audio clips are decompressed on load and preloaded | `Assets/Wave/Audio/*.wav.meta` use `loadType: 0`, `preloadAudioData: 1`, which keeps ~0.5 MB WAVs resident at startup. | For longer clips, switch to `CompressedInMemory` or `Streaming` and disable preload unless UX demands instant playback. |
| Low | Quality and sorting layers minimal | Only the default sorting layer exists (`ProjectSettings/TagManager.asset`) which limits UI/FX layering options. | Define dedicated sorting layers for gameplay, FX, and UI to avoid manual Z hacks. |

## Inventory Notes
- Prefabs: 52 assets (40+ obstacle variants) sourced from the `[Hyper-Casual Game] Wave` package—consider converting repeated layouts into prefab variants to ease balancing.
- Animations: 7 controller/clip pairs powering UI transitions and FX; they rely on legacy `AnimatorController` assets without state documentation.
- Physics materials: 3 `.physicsMaterial2D` assets with high bounciness values (player/wall/obstacles) — confirm these still match the updated movement tuning.

## Suggested Next Steps
1. Create sprite/audio import presets and reapply them via the asset database to normalise compression settings.
2. Refactor obstacle prefabs into composable parts (base + variant modifiers) to reduce update churn when balancing.
3. Add validation scripts (e.g., `Editor` checks) that flag missing references or unintended default assets before committing.

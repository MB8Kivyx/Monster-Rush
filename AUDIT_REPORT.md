# Technical Audit

| Severity | Finding | Rationale | Recommendation |
| --- | --- | --- | --- |
| High | Android custom keystore missing from repository | `ProjectSettings/ProjectSettings.asset:268` requires `{inproject}: user.keystore`, but `Test-Path user.keystore` returns false, blocking Android builds. | Commit a development keystore (with secrets excluded) or disable custom signing in source. Document the release signing procedure. |
| High | No automated PlayMode/EditMode tests | `Assets/` contains zero `*Test*.cs` files and no `Tests/` folders, so regressions in movement, scoring, or spawning go undetected. | Stand up NUnit-based Edit/PlayMode suites covering Player movement, score increments, and obstacle lifecycle. Integrate with CI. |
| Medium | Fast Enter Play Mode skips domain & scene reload | `ProjectSettings/EditorSettings.asset` sets `m_EnterPlayModeOptions: 3`, so static/singleton data persists between plays and can hide bugs. | Either disable the option for the team or ensure every singleton/service explicitly resets state (continue auditing beyond quick wins). |
| Medium | Obstacle spawning destroys/instantiates constantly | `Assets/Wave/Scripts/ObstacleManager.cs` instantiates prefabs every checkpoint and `ObstacleParent` destroys them, driving GC, physics churn, and hitching on mid-tier devices. | Implement pooled obstacles (per difficulty lane) and toggle active/inactive instead of allocating/destroying. |
| Medium | 2D physics auto-sync transforms enabled | `ProjectSettings/Physics2DSettings.asset` has `m_AutoSyncTransforms: 1`, forcing expensive transform sync every frame. | Disable auto-sync and manually call `Physics2D.SyncTransforms()` only when needed (e.g., after teleporting). |
| Medium | Incremental GC disabled | `ProjectSettings/ProjectSettings.asset:918` shows `gcIncremental: 0`, increasing GC spike risk on mobile. | Enable incremental GC for Android/iOS/Standalone to smooth frame times. |
| Medium | Quality settings aggressive for target platforms | `ProjectSettings/QualitySettings.asset` sets Standalone default to Very High (`m_CurrentQuality: 4`) and Android to Medium without project-specific tuning. | Create explicit quality tiers for mobile vs. standalone and prune unused tiers to simplify QA. |
| Low | Legacy Input Manager only | `ProjectSettings/ProjectSettings.asset:1002` uses `activeInputHandler: 0`, limiting modern input/remapping support. | Plan migration to the new Input System or wrap input access behind an abstraction for future proofing. |
| Low | Unused scripting define `UNITY_POST_PROCESSING_STACK_V2` | `ProjectSettings/ProjectSettings.asset:880-889` defines the symbol across platforms, but no Post Processing package is present. | Remove the define or add the package to avoid confusion and redundant shader stripping. |
| Low | Unity Analytics enabled in test mode | `ProjectSettings/UnityConnectSettings.asset` keeps Analytics on (`m_Enabled: 1`, `m_TestMode: 1`), which may be unintended for release builds. | Confirm data policy, disable if unused, or configure proper environments/IDs. |

## Observability Gaps
- No logging helpers or diagnostic overlays exist; crashes or soft-locks will be hard to reproduce outside the Editor.
- There is no centralized error handling for coroutines or `ScoreManager` persistence failures.

## Suggested Deep-Dive Follow-Ups
1. Profile obstacle spawning on a mid-range Android device with the Unity Profiler to quantify GC and physics spikes.
2. Verify that audio playback stays within memory budgets once pooling is added (current WAVs are decompressed on load).
3. Validate fast enter play mode against all managers to ensure static state is reset (e.g., `BackgroundColorManager`, FX pools once implemented).

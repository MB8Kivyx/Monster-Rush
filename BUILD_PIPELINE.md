# Build & CI Pipeline

| Severity | Finding | Rationale | Recommendation |
| --- | --- | --- | --- |
| High | Missing `{inproject}: user.keystore` breaks Android signing | `ProjectSettings/ProjectSettings.asset:268` expects an in-project keystore, but the file is absent, so Gradle builds fail or fall back to unsigned APKs. | Check in a development keystore (with passwords stored in CI secrets) or disable custom signing for debug builds; document release signing separately. |
| High | No automated build or test pipeline defined | Repository lacks `.github/`, `AzurePipelines`, or custom Editor scripts, so builds/tests are manual and error-prone. | Introduce CI (GitHub Actions/Azure/TeamCity) running EditMode, PlayMode (once created), and platform builds (Android APK/AAB, Standalone). |
| Medium | Application identifiers unset | `ProjectSettings/ProjectSettings.asset:165-174` leaves `applicationIdentifier` blank for all targets, causing default bundle IDs (`com.Company.Product`) at build time. | Define per-platform identifiers (e.g., `com.yourstudio.wave`) to avoid store rejections and keystore mismatches. |
| Medium | Consistent scripting define applied without package support | `UNITY_POST_PROCESSING_STACK_V2` is defined globally (`ProjectSettings/ProjectSettings.asset:880-889`) with no matching package. | Remove or gate the define to avoid unnecessary shader stripping and clarify build intent. |
| Medium | Incremental GC disabled | `ProjectSettings/ProjectSettings.asset:918` sets `gcIncremental: 0`, risking frame spikes during builds targeted at low-memory devices. | Enable incremental GC for all shipping platforms before profiling/QA. |
| Low | Android target SDK left to auto (`0`) | `ProjectSettings/ProjectSettings.asset:176` delegates target SDK to Unity, which may lag behind store requirements. | Explicitly set the target SDK to the current Play Store requirement (e.g., API level 34). |

## Platform Configuration Summary
- **Android**: Min SDK 23, IL2CPP backend, ARMv7 + ARM64 architectures, custom keystore expected, app bundle generation unspecified.
- **iOS/tvOS**: Default bundle versions `1.0` with no identifiers or capabilities set.
- **Standalone**: Very High quality tier active by default; no platform-specific scripting symbols.

## Pipeline Recommendations
1. Add a deterministic build script (`Editor/Build/BuildPlayer.cs`) orchestrating Android AAB and Standalone builds with proper `Scripting Define Symbols`.
2. Introduce a `Packages/manifest.dev.json` (or UPM scoped registries) if additional tooling is needed for CI to keep production manifests lean.
3. Store sensitive signing data in CI secrets and mask them from the repo; include onboarding docs outlining build prerequisites.

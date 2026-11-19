# Unity 2022.3 compatibility fork

Based on Unity Pipeline 1.0.0, revision `ce87150fe76742893213dac8aa3fac1a3fadf577`.
The original at `C:/Tools/com.unity.pipeline-1.0.0/package` is preserved.
This fork is embedded in `Packages/com.unity.pipeline`, discovered without a manifest dependency.

## Compatibility changes

- Minimum Editor version is 2022.3. Original source provenance and licenses are retained.
- Bundled Roslyn DLL PluginImporter metadata uses version 2 array serialization so Unity 2022 imports the dependencies. Unity 6 can also read this older format. DLL contents and GUIDs are unchanged.
- Scene Preview implementation is compiled only on Unity 6. Its runtime assembly keeps the original Editor/preview-player gating; the settings UI explains the version requirement on 2022.
- Unity 2022 records analytics locally only when requested. Unity 6 keeps the original Analytics API and transport.
- Console counts use `LogEntries.GetCountsByType` reflection on 2022 and continue polling on Editor updates. A failed lookup does not publish fabricated zero counts.
- Physical material creation accepts both spellings and uses `PhysicMaterial` on 2022.
- Material reads use `m_CustomRenderQueue` on 2022 to preserve both explicit queues and the `-1` shader-inheritance value.
- Input System injection lives in a separate optional assembly, gated on package presence and active input handling. The core package has no mandatory Input System reference. Missing/inactive input uses the existing explicit failure messages.

## Support boundaries

Build Profiles require Unity 6; their existing commands return an explicit version error.
Use the active build target and `EditorBuildSettings` scenes on 2022.
Scene Preview and Unity 6 multiplayer roles/scenarios are unavailable on 2022.
Unity 6 paths remain conditional but have not been run on this machine.
The optional Input System enabled configuration has not been exercised in this project (the package is absent).

## Verification on 2022.3.60f1c1 / CLI 1.0.0

- All six core assemblies compiled against the installed 2022 Editor references, then compiled and loaded by the actual Editor.
- CLI discovery: port 7800, `ready`; `script eval` returned 2.
- `console status`: compilationFailed=false, consoleErrors=0, seeded=true.
- `script recompile_status`: up_to_date, failed=false, errors=[].
- Temporary GameObject creation and removal, asset search, and Console window capture succeeded.
- URP 14 camera capture reproduced a Render2DLightingPass exception despite returning a PNG. The 2022 SRP Scene View camera path now rejects the operation before rendering; use source=screen to capture the existing rendered view.
- Executed assertions for the console reflection API, inherited/explicit material render queues, and physical material construction.
- Build Profiles returned the expected Unity 6 requirement. Windows64 build dry-run validated the selected scene.
- Test Runner command completed with zero selected tests; this is not a passing test suite. Upstream `com.unity.pipeline.tests` is not distributed in this package.

Screenshots and build outputs are under the project's ignored `Temp/PipelineCompatibility` directory.

## Windows launch environment

Unity 2022 Package Manager can fail with `The "path" argument must be of type string. Received undefined`
if the launching shell omits `ALLUSERSPROFILE`. Set `$env:ALLUSERSPROFILE = $env:ProgramData`
for that launch process; do not alter package paths to work around this environment error.

## Final build and session results

The Windows64 development Player build succeeded (0 errors, 212098 ms), after a first attempt failed
in Burst 1.8.27 while creating a temporary linker response file. The retry used a writable temporary
directory set only in this Editor process: `Temp/PipelineCompatibility/BurstTemp` (`TEMP` and `TMP`).
This process-local setting remains until the Editor exits. System environment variables were not changed.
The build kept the project's runtime Pipeline disabled; the enabled runtime core was separately compiled
against 2022 references with `ENABLE_RUNTIME_PIPELINE_DEV` and without `UNITY_EDITOR`.

The CLI reconnected after domain reload and recompile_status completed with failed=false.
`capture scene_view --source screen` produced a 1173x487 PNG. The guarded SRP Scene View camera
request returned the expected error. Console history includes this intentional rejection and the earlier
URP/Burst failures; these are not current C# compilation failures.
Auto tick was disabled after verification. Play Mode is stopped and the open SampleScene is clean.
Original Library recovery directories were preserved.

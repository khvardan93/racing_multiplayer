# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

RacerCar is a Unity 6 (`6000.3.18f1`) multiplayer racing game. Networking is
Photon Fusion 2 (client-predicted, host/client). Dependency injection is
Zenject/Extenject 9.2.0. Rendering is URP; camera work uses Cinemachine 3.

## Build / Run / Test

There is no CLI build or test pipeline — everything runs through the Unity
Editor:

- **Run:** open the project in Unity `6000.3.18f1`, load `Assets/Scenes/Arena.unity`
  (the gameplay scene) and press Play. `Assets/Scenes/Intro.unity` is the entry
  scene.
- **Tests:** Window > General > Test Runner. The only test suites present are
  Zenject's own (`Zenject-UnitTests-Editor`, `Zenject-IntegrationTests`); there
  are no project-specific tests yet.
- **Photon setup:** a valid Photon App ID must be configured (Fusion Hub /
  `NetworkProjectConfig`) or `StartGame` will fail to reach the Name Server.

Game code (`Assets/Scripts`) has no `.asmdef`, so it compiles into the default
`Assembly-CSharp`. Fusion and Zenject live in their own assemblies under
`Assets/Photon` and `Assets/Plugins/Zenject`.

## Architecture

### Dependency injection (Zenject)

DI is scene-scoped, not project-scoped. The `Arena` scene holds a `SceneContext`
GameObject running `GameplayInstaller` (`Assets/Scripts/Installers/`), which
binds scene instances: `ISceneService` (→ `SceneManager`), `NetworkSceneManager`,
`InputsManager`, and the `StartGameConfig` ScriptableObject.

**Critical Fusion + Zenject interaction:** `Runner.Spawn()` and
`Instantiate(runnerPrefab)` bypass Zenject's instantiation path, so
`[Inject]` fields on spawned objects are NOT populated automatically. Any code
that spawns a networked prefab or the runner must manually call
`_container.InjectGameObject(obj)` afterward — see `CarSpawner.PlayerJoined`
and `FusionConnectionManager.ConnectWithRetry`. Follow this pattern for any new
Fusion-spawned prefab that needs injected services.

### Networking flow

1. `FusionConnectionManager` (scene object) starts on `Start()`, builds
   `StartGameArgs` from the injected `StartGameConfig`, and calls
   `runner.StartGame` with exponential-backoff retry (the first UDP connection
   often times out on NAT setup). A fresh `NetworkRunner` is instantiated per
   attempt.
2. `StartGameConfig` is a designer-facing ScriptableObject (Create > Fusion >
   Start Game Config). It can't serialize a live `SceneManager` or `SceneRef`,
   so it stores the scene *name* and resolves the build-index `SceneRef` at
   runtime inside `BuildArgs()`, which takes the `NetworkSceneManager` as a
   parameter.
3. On player join, `CarSpawner` (server-only, `IPlayerJoined`) spawns the car
   prefab at a spawn point from `ISceneService.SpawnPoints`, injects it, and
   points the Cinemachine camera at it via `ISceneService.SetCameraTarget`.

### Input flow (networked)

Input is Fusion's tick-synced input, NOT read directly in the controller:
`InputsManager.OnInput` gathers axes/buttons into a `NetworkCarInputData`
(`INetworkInput`) struct and calls `input.Set(...)`. Discrete presses (e.g.
Reset on `R`) are latched in `Update()` and cleared after being sent, so they
aren't missed between ticks. `CarControl.FixedUpdateNetwork` reads them back via
`GetInput(out NetworkCarInputData ...)`, which also replays historical ticks
during server rollback/prediction — so all driving physics run inside
`FixedUpdateNetwork` for both host and client.

### Two car controllers — don't confuse them

- `CarControl` (`NetworkBehaviour`) is the **networked, authoritative** car used
  in multiplayer via `CarSpawner`. Drives `WheelControl` wheels, handles the
  networked reset/teleport.
- `SportsCarController` (plain `MonoBehaviour`) is a **standalone, non-networked**
  car reading `Input.GetAxis` directly in `Update`/`FixedUpdate`, driving
  `CarWheel` wheels. It's a local/demo controller and shares no code with
  `CarControl`.

## Conventions

- Private serialized fields use `_camelCase` with `[SerializeField]`; expose via
  read-only properties rather than public fields.
- Prefer constructor/field `[Inject]` over `SceneManager.Instance`-style
  singletons — the static singleton was intentionally replaced with DI.

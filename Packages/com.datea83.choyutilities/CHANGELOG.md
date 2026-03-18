# Changelog

## [0.1.6] - 2026-03-18

### Added
* Added back the test folder
* GenericSingleton, now comes with its own get ECS.World (GetWorld())
* GenericSingleton, added GetSingletonEntity<TComponent>()
* GenericSingleton, added regions for ECS related and Async related
* GenericUIManager, poolCount is now ignored, spawn[] size is forced to 1, extra loop is removed
* GenericUIManager, now add a toggle that spawn a PhysicsCollider on the UI element
* GenericUIManager, added ECS region
* UiHelper, added TransformRect validation
* UiHelper, added id for tag
* Added Entities/UI folder
* Added UIData
* Added UiHandleSystemBase

### Removed
* GenericSingleton, removed KeepSingleton() and all child classes that using the said method

## [0.1.5] - 2026-03-16

### Fixed
* Fixed GenericAudioManager always return -1 when using GetPoolIndex()(Added an override);

### Added
* Added SpawnDelayEntityAuthoring

## Removed
* Removed old folders saves in FolderModificationData.json

### Changed
* GenericPoolingManager, field "initializeOnStart" and "collectionCheck" changed to abstract properties

## [0.1.4] - 2026-03-15

### Added

* Added GenericPoolingManager
* Added new GenericAudioManager, GenericParticleManager and GenericUIManager that inherit from GenericPoolingManager
* Added GenericSpawnManager
* HelperCollection, added "RandomValue2" and "RandomValue3"

### Changed
* Moved legacy GenericAudioManager, GenericParticleManager and GenericUIManager to ObsoleteV2 folder and marked [Obsolete]
* HelperCollection, all "RandomValue" with GameObject parameter changed to Component instead

## [0.1.3] - 2026-03-14

### Changed
* Reformatted all coding styles (no changes in functionality)

## [0.1.2] - 2026-03-13

### Added
* Added RemoveMissingScriptsEditor
* Added EditorUtils

### Changed
* Moved EditorBackgroundColor from LoadIconDisplayEditor to EditorUtils

## [0.1.1] - 2026-03-13

### Fixed
* LoadIconDisplayEditor now do extra checks when the gameobject has missing scripts

### Changed
* Renamed LoadIconDisplay to LoadIconDisplayEditor
* Renamed AnimationRecorder to AnimationRecorderEditor
* Moved CameraControllerEditor, CameraTagFollowerEditor, DestroyEntityEditor and FlatPlaneEditor to new folder called Component Editor
* Static Stuff, changed all "!=" to "is not" in CallStaticMethod, CallGenericInstanceMethod and CallInstanceMethod

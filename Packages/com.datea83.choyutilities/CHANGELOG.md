# Changelog

## [0.1.4] - 2026-03-15

### Added

* Added GenericPoolingManager
* Added new GenericAudioManager, GenericParticleManager and GenericUIManager
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

### Added
* LoadIconDisplayEditor now do extra checks when the gameobject has missing scripts

### Changed
* Renamed LoadIconDisplay to LoadIconDisplayEditor
* Renamed AnimationRecorder to AnimationRecorderEditor
* Moved CameraControllerEditor, CameraTagFollowerEditor, DestroyEntityEditor and FlatPlaneEditor to new folder called Component Editor
* Static Stuff, changed all "!=" to "is not" in CallStaticMethod, CallGenericInstanceMethod and CallInstanceMethod

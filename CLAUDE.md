# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6000.1.2f1 2D action/platformer game called "apocalypse". The game features a chapter-based progression system with stages, event-driven gameplay mechanics, and a comprehensive save/load system. The architecture is built around an event system, addressable assets, and modular game components.

## Development Commands

### Unity Editor Operations
- **Open Project**: Open in Unity Editor 6000.1.2f1 or later
- **Build Game**: Use Unity Editor Build Settings (File → Build Settings)
- **Addressables**: Window → Asset Management → Addressables → Groups (build addressable content before building game)

### Key Build Steps
1. Build Addressables content first (required for proper asset loading)
2. Build player through Unity Editor
3. No custom build scripts or CLI automation detected

## Architecture Overview

### Core Systems

**Event System (`Assets/Scripts/Event/`)**
- Central event-driven architecture using `GameEventFactory` and `GameEventManager`
- All major game actions (scene transitions, data loading, UI changes) are handled through events
- Events are pooled for performance and support both synchronous and asynchronous execution
- Event types include: DataLoad, DataSave, SceneLoad, UI changes, Story events, Choice events

**Asset Management**
- Unity Addressables system for efficient asset loading and memory management
- Organized asset groups: Maps, Stage Profile, Story, Systems, UIAssets, Weapons, Enemy Utilities
- All major game assets (characters, stages, UI) loaded asynchronously via Addressables

**Save/Load System (`Assets/Scripts/Player/DataManager.cs`)**
- JSON-based save system with 18 save slots
- Screenshot capture for save slot previews
- Play time tracking and data serialization using Newtonsoft.Json
- Supports both continue game and new game creation

**Stage Management (`Assets/Scripts/Scene/Stage/`)**
- Dynamic stage loading with previous/current/next stage management
- Tile-based stage construction with object replacement system
- Stage transitions and snap points for seamless level connections
- Stage elements implement `IStageElement` interface

### Scene Architecture

**Scene Types:**
- `TitleScene`: Main menu and game initialization
- `StageScene`: Primary gameplay scene with dynamic stage loading
- `SplashScreenScene`: Initial loading screen

**Stage System:**
- Stages are prefabs loaded via Addressables
- Each stage contains PlayerStart, SnapPoints (Enter/Exit), and StageTransitionTriggers
- Composite object system for multi-tile objects using `IPartObject` and `ICompositeObject`

### Key Patterns

**Async Loading:**
- Extensive use of UniTask for async operations
- `IAsyncLoadObject` interface for components requiring async initialization
- Addressables integration for non-blocking asset loading

**Component Architecture:**
- Interface-based design (`IScene`, `IStageElement`, `IPartObject`)
- Modular systems with clear separation of concerns
- Extensive use of assertions for development-time validation

**Event-Driven Design:**
- All game state changes flow through the event system
- Events can be chained using `SequentialEvent`
- Factory pattern for event creation with support for both runtime and data-driven creation

## Development Guidelines

### Working with Events
- Use `GameEventFactory` to create events rather than direct instantiation
- Events support both direct creation and DTO-based creation for serialization
- Always handle async events properly using UniTask

### Asset Organization
- Place gameplay assets in appropriate Addressables groups
- Use consistent naming: `{ChapterType}_{StageIndex}` for stages
- Store event configurations as ScriptableObjects in `Assets/GameResources/GameData/EventInfo/`

### Stage Development
- Implement `IStageElement` for interactive stage objects
- Use `ObjectReplacementTile` for tile-to-object conversion
- Ensure stages have required components: PlayerStart, Enter/Exit SnapPoints, StageTransitionTriggers

### Code Style
- Use Unity Assertions extensively for validation
- Follow the existing async/await patterns with UniTask
- Implement proper disposal and cleanup for Addressables handles
- Maintain the singleton pattern for managers (PlayerManager, DataManager)

## Key Dependencies

- **UniTask**: Primary async/await library (replaces Unity Coroutines)
- **Newtonsoft.Json**: JSON serialization for save data
- **Unity Addressables**: Asset management and loading
- **Unity 2D Feature Set**: Sprites, Animation, Tilemap, Pixel Perfect
- **NuGet for Unity**: Package management for external dependencies

## Testing

- Unity Test Framework available but no specific test structure detected
- Test scenes may be located in `Assets/GameResources/Stage/Test/`
- Manual testing primarily through Unity Editor play mode

## Performance Considerations

- Addressables used for memory-efficient asset loading
- Object pooling implemented for events (`GameEventPool`)
- Burst compilation enabled for performance-critical code
- Async loading prevents frame rate drops during asset loading

## Common File Locations

- **Player Data**: `Application.persistentDataPath + "/saveData{slotNum}.json"`
- **Event Configs**: `Assets/GameResources/GameData/EventInfo/`
- **Stage Prefabs**: `Assets/GameResources/Stage/`
- **UI Assets**: Organized in Addressables UIAssets group
- **Scripts**: `Assets/Scripts/` with clear subsystem organization

## Commit Guidelines

When creating commits:
- Use concise, descriptive commit messages
- Do not include Claude AI attribution or co-author information in commit messages
- Focus on the what and why of the changes
# Gemini Project Conventions

This document outlines the conventions and structure for the `apocalypse` project to ensure consistency and guide AI-assisted development.

## 1. Project Overview

- **Engine:** Unity
- **Language:** C#
- **Project Type:** 2D Game (likely post-apocalyptic theme)
- **Core Architecture:** The project is structured around standard Unity practices, with a clear separation of concerns in the `Assets/Scripts` directory.

## 2. Core Technologies & Libraries

- **Unity Engine:** The primary development platform.
- **Unity Addressable Asset System:** Used for managing and loading assets dynamically. All new dynamic assets should be integrated into this system.
- **Newtonsoft.Json:** The preferred library for all JSON serialization and deserialization tasks, located in `Assets/Packages`. Avoid using `UnityEngine.JsonUtility` for complex data structures.
- **TextMesh Pro:** Used for all text rendering in the UI.
- **Python:** May be used for external tooling, as seen with `Tool/story_editor.py`.

## 3. Code Conventions

### Naming Conventions

- **Classes, Enums, and Methods:** Use `PascalCase` (e.g., `PlayerController`, `GameState`).
- **Public Fields and Properties:** Use `PascalCase` (e.g., `public int Health;`).
- **Private Fields:** Use `_camelCase` with a leading underscore (e.g., `private int _currentHealth;`).
- **Local Variables:** Use `camelCase` (e.g., `var targetPosition = Vector3.zero;`).

### Directory Structure

- **C# Scripts:** All runtime scripts must be placed within `Assets/Scripts`. The folder structure is organized by feature or subsystem (e.g., `AI Subsystem`, `GamePlay`, `Player`, `UI`). New scripts must be added to the appropriate existing folder or a new feature-specific folder.
- **Assets:**
    - **Dynamic Assets:** Use the **Addressable Asset System** for assets that need to be loaded/unloaded at runtime (e.g., prefabs, sprites, stage data).
    - **Static Assets:** Other resources are organized within `Assets/GameResources`.
- **Prefabs:** Located in `Assets/GameResources/Prefab`.

### Best Practices

- **Comments:** All comments must be written in English.
- **Logging:** Use the custom `Logger` class found in `Assets/Scripts/Utility/Logger.cs` for all logging purposes instead of `UnityEngine.Debug`.
- **Class Structure:** Code within classes should be organized by access modifier in the following order: `public`, `protected`, and then `private`. Each section should be separated by a comment line. There should be two blank lines before each separator and one blank line after.
    ```csharp
    public class Example
    {
        /****** Public Members ******/
        public int publicField;


        /****** Protected Members ******/

        protected int protectedField;


        /****** Private Members ******/

        private int _privateField;
    }
    ```
- **Comparison:** When comparing, place the constant on the left side of the operator. This helps prevent accidental assignment errors (e.g., `if (null == variable)`). For boolean comparisons, avoid the `!` operator and use explicit comparison (e.g., `if (false == variable)`).
- **Dependency Injection:** Use `[SerializeField]` to expose fields to the Unity Editor for assigning dependencies. Avoid hard-coded `FindObjectOfType` or singleton patterns where possible.
- **Asynchronous Code:** Use Coroutines (`IEnumerator`) for time-based operations. For more complex async logic, `async/await` with `UniTask` (if present) or standard `Task` can be used, but match the style of the existing file.
- **Data Management:** Game data (e.g., stats, configurations) is stored in `ScriptableObject` assets located under `Assets/GameResources/GameData`.
- **Precondition Checks:** Use `UnityEngine.Assertions.Assert.IsTrue` to validate preconditions at the beginning of a function. This helps catch errors early and ensures the function operates under expected conditions.

## 4. Asset Management

- **Primary System:** The **Addressable Asset System** is the standard for managing assets. When adding new assets that will be instantiated or loaded at runtime, they must be added to an appropriate Addressable Group.
- **Asset Naming:** Use `PascalCase` for asset files (e.g., `PlayerPrefab.prefab`, `RedButton.sprite`).

## 5. Testing & Validation

- No automated testing framework (e.g., Unity Test Framework) is currently evident in the project structure.
- All changes must be manually tested for regressions by running the relevant scenes in the Unity Editor.

## 6. Build & Deployment

- Builds are performed manually through the Unity Editor (`File > Build Settings`).
- There is no automated CI/CD pipeline configured.

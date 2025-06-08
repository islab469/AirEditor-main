# AirEditor Unity Project

AirEditor is a Unity 3D application that combines AR features with a question and answer workflow. The project integrates Firebase services and real-time communication via WebSocket to drive interactive scenes.

## Features

- Multiple interactive scenes such as `AIQA`, `QAInterface`, and `AR`.
- Firebase authentication and data storage.
- WebSocket client for real-time messaging.
- Example animal models and sample assets for demo purposes.

## Requirements

- **Unity** 2022.3.36f1 (LTS) or newer.
- .NET Framework 4.7.1 or compatible (for optional Word interop utilities).

## Getting Started

1. Clone this repository.
2. Run `./unity_git_safety_setup.sh` to generate `.gitignore` and `.gitattributes` and add `.gitkeep` files to empty folders.
3. Open the project using the Unity Hub with the matching editor version.
4. Enter your Firebase configuration if required.

## Directory Overview

- `Assets/` – Main game assets and C# scripts.
- `Packages/` – Unity package manifest and lock file.
- `ProjectSettings/` – Unity project configuration files.
- `NativeWebSocket/` – WebSocket library for Unity.

## Build

Open the project in Unity, then use **File → Build Settings** to create the desired player build (for example Android or Windows). Make sure all required scenes are added to the build list.

## Tests

The project includes the `com.unity.test-framework` package. To run play mode or edit mode tests, open the **Test Runner** window from *Window → General → Test Runner*.

## Version Control Tips

Large binary assets such as models or textures can be tracked using Git LFS. After cloning, ensure LFS is installed and run `git lfs pull` if needed. The provided helper script (`unity_git_safety_setup.sh`) can assist in configuring a clean repository state.

## License

This project is provided under the MIT License. See the `LICENSE` file for details.


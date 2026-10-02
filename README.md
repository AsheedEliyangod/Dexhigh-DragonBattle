DEXHIGH DRAGON BATTLE
Junior Unity Developer Technical Assessment
Dexhigh Services Pvt Ltd

Developer: Asheed Eliyangod
Studio: AE Studios
Engine: Unity 6.3 LTS (6000.3.9f1)
Platform: Windows 64-bit

==================================================
PROJECT OVERVIEW
==================================================

Dexhigh Dragon Battle is a small 2.5D top-down combat scene featuring two dragons:
- A player-controlled gold dragon
- An AI-controlled dark dragon

The objective is to defeat the enemy dragon using movement and three combat abilities while managing ability cooldowns and health.

==================================================
CONTROLS
==================================================

WASD        - Move
Left Shift  - Run
1           - Tail Attack
2           - Fire Breath Attack
3           - Fly Attack
Esc         - Quit Game

UI:
Restart     - Starts a new battle after the result screen
Quit        - Closes the standalone game

==================================================
COMBAT / FEATURES
==================================================

- Player-controlled dragon movement
- Enemy dragon AI with distance-based combat decisions
- Tail Attack: close-range melee attack
- Fire Breath: forward ranged fire attack
- Fly Attack: takeoff, forward aerial attack, landing
- Health system for both dragons
- Ability cooldown system and cooldown UI
- Hit feedback VFX
- Attack and impact sound effects
- Background battle music
- Camera shake on successful attacks
- Hit and death animations
- Player/enemy visual differentiation
- Battle winner screen
- Restart and Quit functionality
- Arena boundaries preventing dragons from leaving the combat area

==================================================
HOW TO RUN
==================================================

1. Extract the Windows build ZIP.
2. Open the extracted folder.
3. Run:

   Dexhigh_DragonBattle.exe

Keep the EXE together with the Data folder, DLL files and other generated Unity build folders.

==================================================
ASSET SOURCES
==================================================

All third-party game assets used were free assets.

Known project assets:
- Dragon model / animations: Obsidian Dragon asset used in the project
  Source: Unity Asset Store
  Link: [ADD EXACT ASSET STORE LINK]

- Lava floor texture: Lava Rocks Texture #1 (Hand-painted, Seamless + Tileable)
  Creator: Texture Me!
  Source: Unity Asset Store
  Link: [ADD EXACT ASSET STORE LINK]

- Arena stone texture: Hand Painted Stone Texture
  Source: Unity Asset Store
  Link: [ADD EXACT ASSET STORE LINK]

- UI artwork / game icon / cursor artwork:
  Created/generated specifically for this technical assessment with AI-assisted image generation.

- Sound effects:
  [ADD THE SOURCE WEBSITE / PACK NAME / LINK USED]

- Background music:
  [ADD THE SOURCE WEBSITE / TRACK NAME / LINK USED]

No paid or ripped assets were intentionally used.

==================================================
AI USAGE NOTE
==================================================

AI tools used:
- ChatGPT

How AI was used:
- Planning the combat system and implementation order
- Assisting with C# scripting
- Debugging Unity errors
- Reviewing gameplay logic and balancing
- Improving UI and combat polish
- Helping plan VFX, audio, camera shake and build preparation
- Creating/generating some UI artwork, game icon and cursor artwork

Example where AI was wrong and how I fixed it:
While adding the Quit feature, the first suggested implementation used:

Input.GetKeyDown(KeyCode.Escape)

My Unity project was configured to use the newer Unity Input System, so this caused an InvalidOperationException in the Console because the old UnityEngine.Input API was being used.

I identified the error from the Console and changed the input check to the new Input System:

Keyboard.current.escapeKey.wasPressedThisFrame

After this change, the Quit button and Escape key worked correctly without the input-handler error.

How AI improved the workflow:
AI helped me iterate faster by providing implementation ideas, full script updates, debugging guidance and polish suggestions. I still tested each feature inside Unity, adjusted values based on actual gameplay, checked Console errors, and changed AI suggestions when they did not match the project setup.

==================================================
BUILD NOTES
==================================================

Build target:
- Windows
- Intel 64-bit / x86_64
- Development Build disabled

The final standalone build was tested after extracting the ZIP into a separate folder.

==================================================
SUBMISSION CONTENTS
==================================================

The assessment submission includes:
- Source project / source code link
- Playable Windows build ZIP
- 1-2 minute gameplay video
- README
- AI Usage Note

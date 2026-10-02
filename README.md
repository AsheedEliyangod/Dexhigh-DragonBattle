# DEXHIGH DRAGON BATTLE

### Junior Unity Developer Technical Assessment
**Dexhigh Services Pvt Ltd**

**Developer:** Asheed Eliyangod  
**Studio:** AE Studios  
**Engine:** Unity 6.3 LTS (6000.3.9f1)  
**Platform:** Windows 64-bit  

---

# PROJECT OVERVIEW

Dexhigh Dragon Battle is a small 2.5D top-down combat scene created for the
Junior Unity Developer technical assessment at Dexhigh Services Pvt Ltd.

The battle features two dragons:

- A player-controlled gold dragon
- An AI-controlled dark dragon

The objective is to defeat the enemy dragon using movement and three combat
abilities while managing health, positioning and ability cooldowns.

The project focuses on a clean and fully working combat loop with responsive
controls, readable enemy AI, visual feedback, sound, UI and a complete
win/restart flow.

---

# CONTROLS

| Input | Action |
|---|---|
| W / A / S / D | Move |
| Left Shift | Run |
| 1 | Tail Attack |
| 2 | Fire Breath Attack |
| 3 | Fly Attack |
| Esc | Quit Game |

### Result Screen

- **Restart** - Starts a new battle
- **Quit** - Closes the standalone game

---

# COMBAT ABILITIES

## Tail Attack

A close-range melee attack.

- Deals direct damage to nearby enemies
- Uses a tail attack animation
- Has its own cooldown
- Includes hit VFX
- Includes attack and impact sound effects
- Triggers camera shake when damage lands
- Cannot damage the player while the player is airborne during Fly Attack

## Fire Breath

A forward ranged fire attack.

- Fire damage is applied in multiple damage ticks
- Fire hitbox grows outward from the dragon
- Uses a dedicated fire-breath animation
- Includes fire particle effects
- Includes fire-breath sound
- Includes hit feedback
- Uses its own cooldown
- Enemy AI can use Fire Breath when the player is outside Tail Attack range

## Fly Attack

An aerial offensive ability.

- Dragon takes off
- Moves forward in the direction it is facing
- Deals strong initial impact damage
- Can deal additional small damage ticks during contact
- Uses Takeoff, Air Attack and Landing animations
- Includes aerial / wing audio
- Includes impact feedback and camera shake
- Player is treated as airborne during the ability

---

# ENEMY AI

The enemy dragon uses a simple readable combat state system.

The AI can:

- Detect and move toward the player
- Chase when outside attack range
- Use Tail Attack at close range
- Use Fire Breath at longer range
- Respect ability cooldowns
- React differently while the player is airborne
- Avoid using Tail Attack against an airborne player
- Stop combat when either dragon dies

The AI is intentionally kept simple and readable for the assessment.

---

# HEALTH AND COMBAT SYSTEMS

The project includes:

- Player health system
- Enemy health system
- Player and enemy health bars
- Damage handling
- Hit reaction animations
- Death animations
- Combat state locking after death
- Automatic battle-end detection
- Winner determination
- Delayed result screen so the death animation can finish first

---

# USER INTERFACE

The game includes:

- Player health bar
- Enemy health bar
- Three ability icons
- Radial cooldown indicators
- Cooldown countdown text
- Ability key indicators
- Battle Over screen
- Winner text
- Restart button
- Quit button

The UI was designed around a dark fantasy / dragon / lava theme.

---

# VISUAL AND GAMEPLAY POLISH

The project includes:

- 2.5D angled top-down camera
- Arena boundaries
- Lava-rock arena floor
- Stone arena walls
- Corner pillars
- Fire particle effects
- Point lights
- Environmental ember particles
- Dark sky / background
- Fog
- Hit impact VFX
- Fire breath VFX
- Fly attack impact effect
- Camera shake
- Smooth movement and rotation
- Smooth animation transitions
- Player and enemy visual differentiation
- Background battle music
- Attack and impact sound effects

The player dragon uses a gold color scheme while the enemy dragon uses a
dark / black color scheme with red eyes so both characters can be identified
immediately during combat.

---

# HOW TO RUN

1. Download the Windows build ZIP.
2. Extract the ZIP to a folder.
3. Open the extracted folder.
4. Run:

   `Dexhigh_DragonBattle.exe`

Do not move the EXE away from the accompanying Data folder, DLL files and
other Unity-generated build folders.

The final build was also tested after being extracted into a separate
directory to confirm that the packaged build runs correctly.

---

# SOURCE CODE

GitHub repository:

https://github.com/AsheedEliyangod/Dexhigh-DragonBattle

The Unity source repository contains:

- `Assets/`
- `Packages/`
- `ProjectSettings/`
- `.gitignore`
- `README.md`

Unity-generated folders such as Library, Temp, Logs, obj and UserSettings
are excluded from source control.

---

# ASSET SOURCES

All third-party resources used for this assessment were free resources.

No paid or ripped assets were intentionally used.

## Dragon Model, Rig and Animations

### Obsidian Dragon

The dragon used for both the player and enemy is a custom asset created for
this technical assessment.

The dragon model was created and prepared in Blender with an AI-assisted
workflow using Codex GPT Astra.

The model includes the rig and animations used by the combat system,
including animations such as:

- Idle
- Walk
- Run
- Tail Attack
- Fire Breath Attack
- Takeoff
- Flying / Hover
- Air Attack
- Landing
- Hit Reaction
- Death

The same base dragon is used for both characters.

Separate Unity materials were created for each character:

- Player Dragon - gold / bronze materials
- Enemy Dragon - dark / black materials with red eyes

This dragon asset was not taken from a paid or ripped game asset.

---

## Arena Textures

### Lava Rocks Texture #1 (Free)

**Creator:** Texture Me!  
**Source:** Unity Asset Store  
**Used for:** Arena lava-rock floor

The original texture was adapted into a URP-compatible material for the
arena floor.

---

### Hand Painted Stone Texture

**Source:** Unity Asset Store  
**Used for:** Arena walls and stone environment

The texture was tiled and adjusted to suit the scale of the battle arena.

---

# AUDIO SOURCES

The audio files used in the project were downloaded as free audio resources.

## Background Music

### Epic - Cinematic Epic 21 Second

**Creator:** BombinSound  
**Source:** Pixabay  
**Used for:** Looping battle background music

Project file:

`bombinsound-epic-cinematic-epic-21-second-490538.mp3`

---

## Sound Effects

### Dragon / Wing Sounds

**Creator:** Dragon Studio  
**Source:** Pixabay

Used files:

- `dragon-studio-bird-wings-463212.mp3`
- `dragon-studio-dragon-breathing-fire-364475.mp3`
- `dragon-studio-dragon-flapping-wings-364476.mp3`

Used for:

- Fly movement
- Dragon wing movement
- Fire Breath attack

---

### Fireball Whoosh 5

**Creator:** floraphonic  
**Source:** Pixabay

Project file:

`floraphonic-fireball-whoosh-5-179129.mp3`

Used for attack / projectile-style whoosh feedback.

---

### Landing on Grass

**Creator:** freesound_community  
**Source:** Pixabay

Project file:

`freesound_community-landing-on-grass-40650.mp3`

Used as landing / impact feedback.

---

### Golf Swing Practice 01

**Creator:** linhmitto  
**Source:** Pixabay

Project file:

`linhmitto-golf-swing-practice-01-255625.mp3`

Used as a fast swing / tail attack sound.

---

### Punch Impact Hit

**Creator:** universfield  
**Source:** Pixabay

Project file:

`universfield-punch-impact-hit-567196.mp3`

Used as combat impact feedback.

---

### SFX28 Attack

**Creator:** yarzur_ofc  
**Source:** Pixabay

Project file:

`yarzur_ofc-sfx28-attack-338386.mp3`

Used as additional attack feedback.

---

# UI / GENERATED ARTWORK

Several visual UI elements were created specifically for this assessment
with AI-assisted image generation.

These include:

- Health bar artwork
- Ability UI elements
- Battle Over / Winner Screen artwork
- Restart button artwork
- Quit button artwork
- Game executable icon
- Custom game cursor artwork

These generated elements were then imported, positioned, scaled and
configured manually inside Unity.

---

# UNITY-CREATED VFX AND PRESENTATION

The following presentation elements were created or configured directly
inside Unity:

- Hit impact particle effect
- Fire attack particles
- Arena ember particles
- Pillar flame effects
- Point lights
- Lava lighting
- Camera shake
- Fog
- Sky/background setup
- Character material variations
- Ability cooldown presentation
- Health UI behaviour
- Winner screen transitions

---

# AI USAGE NOTE

AI tools were intentionally used as part of the development workflow, as
requested by the technical assessment.

## AI Tools Used

- ChatGPT
- Codex GPT Astra

---

## How ChatGPT Was Used

ChatGPT was used for:

- Planning the implementation order
- Designing the combat systems
- Assisting with C# scripting
- Reviewing and updating scripts
- Debugging Unity errors
- Designing enemy AI behaviour
- Planning ability cooldown logic
- Improving combat balancing
- Reviewing screenshots and gameplay
- Planning UI structure
- Planning VFX and audio implementation
- Adding camera shake
- Build and submission preparation
- Generating some UI artwork, game icon and cursor artwork

I tested each system inside Unity and adjusted values based on the actual
gameplay instead of using AI output without verification.

---

## How Codex GPT Astra Was Used

Codex GPT Astra was used as part of the Blender workflow while creating and
preparing the custom dragon asset used by the project.

It assisted with parts of the asset creation / Blender workflow while I
reviewed and integrated the resulting dragon into Unity.

The dragon was then configured in Unity with:

- Animator Controllers
- Materials
- Character movement
- Combat abilities
- Hit detection
- Health
- AI behaviour
- Audio
- VFX

---

# EXAMPLE OF AI GETTING SOMETHING WRONG

While implementing the Quit feature, an initial AI suggestion used:

```csharp
Input.GetKeyDown(KeyCode.Escape)

#!/bin/bash

REPO="BrightestShade/TurnBasedBattle"

# Milestone Names
M1="Milestone 1 - Core Prototype"
M2="Milestone 2 - Vertical Slice"
M3="Milestone 3 - Art & Atmosphere"
M4="Milestone 4 - Polish & Optimization"
M5="Milestone 5 - Release Build"

echo "🚀 Creating Issues with Progress Bar Checklists for $REPO..."

# --- MILESTONE 1 ---
gh issue create --repo $REPO --title "Player Controller" --milestone "$M1" --body "### Tasks
- [ ] Walk & Sprint Logic
- [ ] Crouch & Jump Mechanics
- [ ] Head Bobbing Effect
- [ ] Bodycam View System
- [ ] Camera Noise & Distortion
- [ ] Movement Polish & Friction
- [ ] Player State Machine
- [ ] Basic Animation Setup"

gh issue create --repo $REPO --title "Interaction System" --milestone "$M1" --body "### Tasks
- [ ] Raycast Detection Logic
- [ ] Door Open/Close Mechanics
- [ ] Drawer & Cabinet Interaction
- [ ] Valve Rotation System
- [ ] Item Pickup (Gun/Keycards)
- [ ] Contextual UI Prompts
- [ ] Audio Triggers for Objects
- [ ] Interaction Haptic Feedback"

gh issue create --repo $REPO --title "Zombie AI (Basic)" --milestone "$M1" --body "### Tasks
- [ ] Patrol Pathing
- [ ] Vision/Chase Logic
- [ ] Attack Range & Timers
- [ ] Door Blocking Behavior
- [ ] NavMesh Integration
- [ ] Basic Animation Blend Tree
- [ ] Damage Detection
- [ ] Death States"

gh issue create --repo $REPO --title "Pipe Puzzle System" --milestone "$M1" --body "### Tasks
- [ ] Valve Rotation Math
- [ ] Pressure Gauge Logic
- [ ] Gas Leak VFX
- [ ] Screen Blur/Coughing Effect
- [ ] Heartbeat Sound Escalation
- [ ] Puzzle Timer System
- [ ] Door Unlock Condition
- [ ] Death/Failure State"

gh issue create --repo $REPO --title "Death System" --milestone "$M1" --body "### Tasks
- [ ] Zombie Kill Cam
- [ ] Gas Suffocation Death
- [ ] 'Signal Lost' UI Screen
- [ ] Jump Scare Camera Snap
- [ ] Scene Reset Logic
- [ ] Audio Fade-out
- [ ] Checkpoint Reloading
- [ ] Death VFX"

# --- MILESTONE 2 ---
gh issue create --repo $REPO --title "Checkpoint System" --milestone "$M2" --body "### Tasks
- [ ] Checkpoint Trigger Boxes
- [ ] Save Player Transform
- [ ] Save Puzzle Completion State
- [ ] Persistent Door States
- [ ] Enemy Position Reset
- [ ] Loading Screen UI
- [ ] Soft-lock Prevention Logic"

gh issue create --repo $REPO --title "Gun System" --milestone "$M2" --body "### Tasks
- [ ] Inventory Slot Setup
- [ ] Limited Ammo Counter
- [ ] Shooting Raycast/Projectile
- [ ] Recoil & Kickback VFX
- [ ] Empty Click Sound
- [ ] Muzzle Flash Sprite
- [ ] Zombie Impact Reactions"

gh issue create --repo $REPO --title "Control Room Logic" --milestone "$M2" --body "### Tasks
- [ ] Event Trigger Sequences
- [ ] Panic Timer Setup
- [ ] Keycard Spawn Logic
- [ ] Drawer Loot Probability
- [ ] Security Camera Monitor VFX
- [ ] Escape Route Unlock"

# --- MILESTONE 3 ---
gh issue create --repo $REPO --title "Lighting & Atmosphere" --milestone "$M3" --body "### Tasks
- [ ] Volumetric Fog Setup
- [ ] Flickering Light Scripts
- [ ] Post-Processing Profile
- [ ] Blood Spatter Decals
- [ ] Shadow Performance Tuning
- [ ] Darkness/Stealth Balancing"

gh issue create --repo $REPO --title "Animation System" --milestone "$M3" --body "### Tasks
- [ ] Zombie Walk/Attack Cycle
- [ ] FPS Hand Animations
- [ ] Weapon Reload/Shoot
- [ ] Environmental Animations (Pipes/Doors)
- [ ] Camera Shake Profiles"

# --- MILESTONE 4 & 5 (Simplified for Speed) ---
gh issue create --repo $REPO --title "Optimization & Polish" --milestone "$M4" --body "### Tasks
- [ ] Occlusion Culling
- [ ] Texture Compression
- [ ] Audio Mixing & Compression
- [ ] Memory Usage Audit
- [ ] Bug Hunting Phase"

gh issue create --repo $REPO --title "Release Prep" --milestone "$M5" --body "### Tasks
- [ ] Final Build Export
- [ ] Gameplay Trailer Capture
- [ ] Storefront Assets (Icons/Screenshots)
- [ ] Demo Build Polish
- [ ] Version Tagging (v1.0)"

echo "✅ All detailed issues created! Check the [Issues tab](https://github.com) to see the progress bars."

#!/bin/bash

REPO="BrightestShade/TurnBasedBattle"

echo "Creating Milestones..."

gh api repos/$REPO/milestones -f title="Milestone 1 - Core Prototype"
gh api repos/$REPO/milestones -f title="Milestone 2 - Vertical Slice"
gh api repos/$REPO/milestones -f title="Milestone 3 - Art & Atmosphere"
gh api repos/$REPO/milestones -f title="Milestone 4 - Polish & Optimization"
gh api repos/$REPO/milestones -f title="Milestone 5 - Release Build"

echo "Fetching milestone numbers..."

M1=$(gh api repos/$REPO/milestones | jq '.[] | select(.title=="Milestone 1 - Core Prototype") | .number')
M2=$(gh api repos/$REPO/milestones | jq '.[] | select(.title=="Milestone 2 - Vertical Slice") | .number')
M3=$(gh api repos/$REPO/milestones | jq '.[] | select(.title=="Milestone 3 - Art & Atmosphere") | .number')
M4=$(gh api repos/$REPO/milestones | jq '.[] | select(.title=="Milestone 4 - Polish & Optimization") | .number')
M5=$(gh api repos/$REPO/milestones | jq '.[] | select(.title=="Milestone 5 - Release Build") | .number')

echo "Creating Issues..."

# Milestone 1
gh issue create --repo $REPO --title "Player Controller" --milestone $M1 --body "
- [ ] Walk
- [ ] Sprint
- [ ] Crouch
- [ ] Jump
- [ ] Head Bob
- [ ] Bodycam System
- [ ] Camera Noise Effects
- [ ] Movement Polish
- [ ] Player States
- [ ] Basic Animation Setup
"

gh issue create --repo $REPO --title "Interaction System" --milestone $M1 --body "
- [ ] Raycast Interaction
- [ ] Door Open/Close
- [ ] Drawer Interaction
- [ ] Valve Rotation
- [ ] Gun Pickup
- [ ] Keycard Pickup
- [ ] Context UI Prompt
- [ ] Sound Trigger
- [ ] Interaction Feedback
- [ ] Lock System
"

gh issue create --repo $REPO --title "Zombie AI (Basic)" --milestone $M1 --body "
- [ ] Patrol Behavior
- [ ] Chase Behavior
- [ ] Attack Logic
- [ ] Door Blocking
- [ ] NavMesh Setup
- [ ] Basic Animation
- [ ] Sound Triggers
- [ ] Damage Player
- [ ] Death Trigger
- [ ] Basic Optimization
"

gh issue create --repo $REPO --title "Pipe Puzzle System" --milestone $M1 --body "
- [ ] Valve Rotation Logic
- [ ] Pressure System
- [ ] Wrong Valve Penalty
- [ ] Gas System
- [ ] Vision Blur Effect
- [ ] Heartbeat Effect
- [ ] Timer System
- [ ] Door Unlock Trigger
- [ ] Reset Logic
- [ ] Death Condition
"

gh issue create --repo $REPO --title "Death System" --milestone $M1 --body "
- [ ] Zombie Death
- [ ] Gas Death
- [ ] Signal Lost Screen
- [ ] Jump Scare Trigger
- [ ] Scene Reset
- [ ] Audio Fade
- [ ] Visual Effects
- [ ] Player Disable
- [ ] Checkpoint Reload
- [ ] Basic Polish
"

# Milestone 2
gh issue create --repo $REPO --title "Checkpoint System" --milestone $M2 --body "
- [ ] Define Checkpoints
- [ ] Save Player Position
- [ ] Save Puzzle State
- [ ] Save Door States
- [ ] Reset Enemies
- [ ] Reload Scene Section
- [ ] Prevent Soft Locks
- [ ] Trigger System
- [ ] Debug Mode
- [ ] Testing
"

gh issue create --repo $REPO --title "Cutscene System" --milestone $M2 --body "
- [ ] Bodycam Wake Scene
- [ ] Zombie First Reveal
- [ ] Control Room Entry
- [ ] Security Camera Scene
- [ ] Final Escape Scene
- [ ] Camera Switching
- [ ] Player Lock Control
- [ ] Audio Sync
- [ ] Animation Triggers
- [ ] Skip Option
"

gh issue create --repo $REPO --title "Gun System" --milestone $M2 --body "
- [ ] Gun Pickup
- [ ] Limited Ammo
- [ ] Shooting Mechanic
- [ ] Recoil
- [ ] No Ammo Click
- [ ] Bullet Impact
- [ ] Zombie Damage
- [ ] Sound Effects
- [ ] Muzzle Flash
- [ ] Disable After Ammo
"

gh issue create --repo $REPO --title "Control Room Logic" --milestone $M2 --body "
- [ ] Zombie Spawn Timing
- [ ] Gun Panic Timer
- [ ] Keycard Spawn
- [ ] Drawer Search Logic
- [ ] Door Lock
- [ ] Pipe Blocked Entrance
- [ ] Security Camera Trigger
- [ ] Dead Body Interaction
- [ ] Escape Unlock
- [ ] Reset Logic
"

gh issue create --repo $REPO --title "Audio System" --milestone $M2 --body "
- [ ] Submarine Engine Loop
- [ ] Footsteps
- [ ] Zombie Sounds
- [ ] Door Sounds
- [ ] Gun Sounds
- [ ] Gas Leak
- [ ] Heartbeat
- [ ] Ambient Tension
- [ ] Audio Mixer Setup
- [ ] Audio Optimization
"

# Milestone 3
gh issue create --repo $REPO --title "3D Asset Production" --milestone $M3 --body "
- [ ] Submarine Corridor
- [ ] Control Room
- [ ] Pipe Room
- [ ] Doors
- [ ] Valves
- [ ] Drawers
- [ ] Zombie Model
- [ ] Player Hands
- [ ] Gun Model
- [ ] Texturing
"

gh issue create --repo $REPO --title "Lighting & Atmosphere" --milestone $M3 --body "
- [ ] Low Light Setup
- [ ] Flicker Effects
- [ ] Volumetric Fog
- [ ] Post Processing
- [ ] Blood Effects
- [ ] Camera Noise
- [ ] Shadow Optimization
- [ ] Color Grading
- [ ] Darkness Balancing
- [ ] Performance Check
"

gh issue create --repo $REPO --title "Animation System" --milestone $M3 --body "
- [ ] Zombie Walk
- [ ] Zombie Attack
- [ ] Player Hands
- [ ] Gun Animations
- [ ] Death Animations
- [ ] Cutscene Animations
- [ ] Idle Movements
- [ ] Camera Shake
- [ ] Transitions
- [ ] Polish
"

# Milestone 4
gh issue create --repo $REPO --title "Optimization" --milestone $M4 --body "
- [ ] FPS Testing
- [ ] Light Baking
- [ ] LOD Setup
- [ ] Occlusion Culling
- [ ] Texture Compression
- [ ] Audio Compression
- [ ] Memory Testing
- [ ] Build Testing
- [ ] Bug Fixing
- [ ] Stability Check
"

gh issue create --repo $REPO --title "Game Balancing" --milestone $M4 --body "
- [ ] Puzzle Difficulty
- [ ] Ammo Balance
- [ ] Zombie Speed
- [ ] Gas Timing
- [ ] Checkpoint Placement
- [ ] Death Timing
- [ ] Audio Levels
- [ ] Jump Scare Timing
- [ ] Player Feedback
- [ ] Final Testing
"

# Milestone 5
gh issue create --repo $REPO --title "Release Preparation" --milestone $M5 --body "
- [ ] Final Build
- [ ] Playtesting
- [ ] Trailer Capture
- [ ] Store Assets
- [ ] Screenshots
- [ ] Description Writing
- [ ] Demo Build
- [ ] Bug Sweep
- [ ] Version Tagging
- [ ] Publish
"

echo "All Milestones and Issues Created."

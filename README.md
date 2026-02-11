🧱 1. Player Controller

 Walk
Basic movement input and speed. Foundation of player control.

 Sprint
Increased speed with optional stamina. Used for chase tension.

 Crouch
Lower camera height and speed. Adds realism and possible stealth moments.

 Jump (if needed)
Only if level design requires vertical movement. Avoid if unnecessary.

 Head bob
Subtle camera movement while walking. Makes bodycam feel physical.

 Camera sway
Slight movement when turning. Adds realism and prevents robotic feel.

 Bodycam fisheye effect
Post-processing lens distortion. Sells the bodycam aesthetic.

 Flashlight
Attached light source with limited cone. Controls visibility and tension.

 Breathing states
Calm / stressed / gas breathing. Supports immersion and tension escalation.

 Stamina system (if used)
Limits sprint spam. Prevents player from outrunning all danger.

🖱 2. Interaction System

 Raycast interaction
Detects objects player is looking at. Core interaction method.

 Interaction prompt
Small UI hint (like “Press E”). Prevents confusion.

 Door open / close
Basic environment interaction. Required for flow.

 Locked door logic
Checks keycard flag before allowing access.

 Drawer open
Used for tension during searching moments.

 Valve rotate
Interactive escape mechanic. Requires hold or timed input.

 Pressure wheel rotate
Puzzle interaction for pipe room.

 Gun pickup (bool flag)
Sets playerHasGun = true. No inventory UI needed.

 Keycard pickup (bool flag)
Sets playerHasKeycard = true. Enables progression.

 Keycard door unlock logic
Checks flag before opening next area.

🧟 3. Zombie AI

 Idle state
Default state before detecting player.

 Patrol (optional)
Movement loop for tension when not chasing.

 Chase state
Moves toward player when detected.

 Attack state
Triggers damage and animation when close.

 Damage player
Reduces health or triggers death.

 Death state
Disables AI and collisions when killed.

 Reanimation animation
Used in control room cutscene twist.

 Door interaction (arm reach)
Scripted scare event in pipe room.

 Audio triggers
Plays sounds depending on state.

 AI reset on checkpoint
Restores correct state after respawn.

🧩 4. Puzzle System

 Pressure logic
Tracks correct pressure level internally.

 Correct wheel sequence
Defines which order stabilizes system.

 Wrong input penalty
Increases danger or reduces attempts.

 Gas level system
Tracks gas fill over time.

 Vision blur effect
Visual feedback for gas exposure.

 Gas damage trigger
Kills player if threshold reached.

 Puzzle reset logic
Restores puzzle state on death.

💾 5. Checkpoint System

 Checkpoint trigger volumes
Saves progress when player enters zone.

 Save player position
Stores respawn location.

 Save gun state
Remembers if gun was collected.

 Save keycard state
Remembers if keycard was collected.

 Save puzzle state
Prevents replaying solved puzzles.

 Save zombie state
Prevents dead enemies respawning incorrectly.

 Respawn at checkpoint
Moves player back after death.

 Prevent cutscene replay
Avoids repeating already seen cinematics.

 Section-based reset
Only resets current gameplay area.

🔄 6. Section Reset System

 Reset enemies in section
Reinitializes AI for that zone only.

 Reset puzzle in section
Clears partial puzzle attempts.

 Reset gas level
Ensures fair retry.

 Reset doors in section
Returns doors to intended checkpoint state.

 Restore player states from checkpoint
Applies saved flags and position.

 Full scene reset option
Used when restarting game completely.

💀 7. Death System

 Zombie death sequence
Jump scare + immediate failure.

 Gas death sequence
Slower fade to black.

 Timer death
Used for gun panic failure.

 Freeze player input
Prevents movement during death.

 Fade to black
Smooth transition before reload.

 Load last checkpoint
Returns to saved state.

 Signal Lost screen
Thematic end screen.

🎥 8. Cutscene System

 Trigger zones
Activates cinematic at specific points.

 Lock player control
Prevents movement during scene.

 Camera switch system
Switches to cinematic or CCTV view.

 Return control to player
Restores gameplay smoothly.

 Wake-up cutscene
Introduces bodycam and setting.

 First zombie reveal
Establishes threat.

 Control room reanimation (CCTV)
Major twist moment.

 Final door stand-up scene
Final scare before escape.

🔊 9. Audio System

 Ambient submarine loop
Constant background pressure.

 Metal creaks randomizer
Prevents repetition and builds unease.

 Player footsteps
Surface-based realism.

 Breathing audio states
Syncs with tension levels.

 Zombie movement audio
Audio-driven fear.

 Zombie scream
Used sparingly for impact.

 Gunshot audio
Realistic, not arcade.

 Gun empty click sound
Psychological fake-out moment.

 Gas hiss
Environmental warning sound.

 Audio distance scaling
Makes enemies feel spatially real.

 Audio distortion effects
Bodycam microphone realism.

💡 10. Lighting System

 Base dark lighting setup
Establishes horror mood.

 Flashlight cone tuning
Controls visibility and tension.

 Flickering lights
Adds instability.

 Volumetric fog (pipe room)
Makes gas visually threatening.

 Monitor glow (control room)
Small light sources for contrast.

 Post-processing setup
Adds grain, color grading, lens effects.

🏗 11. Level Design (Whitebox → Final)

 Finalize layout
Lock spatial design before art pass.

 Replace greybox walls
Swap with modular kit.

 Replace greybox floors
Add detail and materials.

 Replace greybox ceilings
Add pipe supports and lighting.

 Add pipe modular kit
Build industrial environment.

 Add doors
Define progression flow.

 Add control room props
Storytelling elements.

 Add bunk room props
Establish living space realism.

 Add escape area props
Define final goal visually.

 Add collision meshes
Ensure smooth movement.

 Optimize navigation flow
Remove confusing paths.

🎨 12. 3D Asset Production (Modular Kit)

 Wall module
Reusable corridor building block.

 Corner wall module
Allows directional changes.

 Floor panel
Modular base surface.

 Ceiling panel
Supports lighting and pipes.

 Submarine door
Core interaction element.

 Heavy control room door
Keycard progression gate.

 Pipe straight module
Core industrial detail.

 Pipe elbow module
Allows directional variation.

 Pipe junction module
Adds realism and complexity.

 Valve wheel
Puzzle interaction prop.

 Broken pipe variant
Used for gas leak scene.

 Bunk bed
Spawn room prop.

 Locker
Environmental storytelling.

 Control desk
Central control room asset.

 Drawer cabinet
Search mechanic object.

 Zombie character
Main threat model.

 Player hands
First-person immersion.

 UV unwrap
Required for texturing.

 PBR textures
Realistic material response.

 LOD creation
Performance optimization.

 Collision meshes
Gameplay functionality.

🎞 13. Animation

 Zombie idle
Subtle life-like motion.

 Zombie run
Chase behavior.

 Zombie attack
Damage animation.

 Zombie stand-up
Twist cutscene.

 Zombie arm reach
Pipe room scripted scare.

 Player valve turn
Interactive animation.

 Player wheel turn
Puzzle feedback.

 Player gun hold
Combat readiness pose.

 Camera pickup animation
Opening immersion.

⚙️ 14. Optimization

 LOD groups
Reduce distant mesh cost.

 Occlusion culling
Don’t render unseen objects.

 Light baking
Improve performance in static areas.

 Reduce draw calls
Merge materials when possible.

 Audio balancing
Prevent clipping and overload.

 Performance testing
Ensure stable FPS.

🧪 15. Playtesting & Polish

 Balance zombie speed
Fair but stressful.

 Balance puzzle difficulty
Challenging but solvable.

 Adjust checkpoint placement
Avoid frustration.

 Fix soft locks
Prevent unwinnable states.

 Fix collision issues
Smooth movement.

 Adjust lighting intensity
Maintain visibility + tension.

 Final bug pass
Stability before release.

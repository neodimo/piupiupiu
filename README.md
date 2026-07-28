# piupiupiu

One-finger neon arena survivor (Geometry Wars-flavored). **Now a Godot 4 rewrite.**

## Versions
- **`master`** — Godot 4 rewrite (current).
- **branch `unity-original`** / **tag `v1-unity`** — the original Unity build, preserved untouched. `git checkout v1-unity` to get it back exactly.

## Run (Godot 4.7+)
Open the folder in Godot, or: `godot4 res://scenes/Main.tscn`

## Current slice
Player drag-move + auto-fire at nearest enemy, homing enemies, ramping wave spawner, score/multiplier, and a native mass-spring "vector grid" background (SpringGrid.gd) rebuilt from the Unity VectorGrid asset. Art (player/enemy sprite sheets, music) reused from the Unity build.

# BeatSync

BeatSync is a virtual reality rhythm-based game prototype that combines automated audio analysis with upper-body motion tracking to create an immersive, dance-oriented experience.

## Overview

The system analyzes user-selected music tracks to extract timing and energy features such as tempo, beat onsets, and amplitude-derived intensity. These features are used to procedurally generate beat maps that drive music-synchronized visual prompts inside a VR environment.

Players interact with the experience through real-time tracking of the head and hand joints, matching their movements to the generated cues. This pairing of music feature extraction and embodied interaction is intended to improve engagement, presence, and movement coordination during gameplay.

A key goal of BeatSync is scalable content generation. By automating beat-map creation, the project removes the need for manual chart authoring and makes it possible to support arbitrary music libraries with a reproducible pipeline.

## Current System

- Loads beat-map JSON from `Assets/Audio`
- Spawns orb prompts synchronized to music playback
- Generates simple motion targets such as `Left`, `Right`, `Up`, `Down`, and `Center`
- Supports a backend pipeline for automatic beat-map generation from audio files
- Provides a foundation for future hold, slide, and richer guided movement interactions

## Project Structure

- `Assets/` Unity game project and gameplay scripts
- `Backend/analyzeSong.py` Python audio analysis script
- `Assets/Audio/*.json` generated beat maps used by Unity

## Beat Data

The backend generates JSON with:

- `bpm`
- `beats`
- `energy`
- `targets`
- `holds`

Unity reads this data through `RhythmManager` and spawns guide orbs with `OrbSpawner`, using beat timing and generated motion targets to cue player movement.

## Regenerating Song Data

Run the backend script after adding or updating audio files:

```powershell
python Backend\analyzeSong.py
```

Note: the backend currently requires Python packages such as `librosa`, `numpy`, and `scipy`.

## Research Direction

BeatSync is intended as a foundation for future work in rhythm-based VR exergames, including studies of user engagement, motor coordination, and possible therapeutic applications.

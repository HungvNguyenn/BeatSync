import librosa
import json
import os
import numpy as np
from scipy.ndimage import uniform_filter1d

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
AUDIO_FOLDER = os.path.join(SCRIPT_DIR, "..", "Assets", "Audio")
AUDIO_FOLDER = os.path.normpath(AUDIO_FOLDER)

def analyze_song(file_path):
    y, sr = librosa.load(file_path)

    # -------------------
    # BEATS
    # -------------------
    tempo, beat_frames = librosa.beat.beat_track(y=y, sr=sr)
    tempo = float(tempo[0] if hasattr(tempo, "__iter__") else tempo)
    beat_times = librosa.frames_to_time(beat_frames, sr=sr)

    # -------------------
    # ENERGY (RMS)
    # -------------------
    rmse = librosa.feature.rms(y=y)[0]
    times = librosa.frames_to_time(range(len(rmse)), sr=sr)

    # Smooth energy (VERY important for hold detection)
    smooth_energy = uniform_filter1d(rmse, size=10)

    # -------------------
    # BEAT ENERGY (your current system)
    # -------------------
    beat_energy = []
    for b in beat_times:
        idx = np.abs(times - b).argmin()
        beat_energy.append(float(rmse[idx]))

    # -------------------
    # HOLD / SUSTAINED SEGMENTS
    # -------------------
    HOLD_THRESHOLD = np.mean(smooth_energy) * 1.2

    hold_segments = []
    start = None

    for i in range(len(smooth_energy)):
        if smooth_energy[i] > HOLD_THRESHOLD:
            if start is None:
                start = times[i]
        else:
            if start is not None:
                end = times[i]
                if end - start > 0.6:  # duration requirement
                    hold_segments.append({
                        "start": float(start),
                        "end": float(end)
                    })
                start = None

    return {
        "bpm": tempo,
        "beats": beat_times.tolist(),
        "energy": beat_energy,
        "holds": hold_segments
    }

def process_folder(folder):
    print(f"Looking for audio in: {folder}")

    if not os.path.exists(folder):
        print("Error: folder does not exist!")
        return

    for file in os.listdir(folder):
        if file.endswith(".mp3") or file.endswith(".wav"):
            audio_path = os.path.join(folder, file)
            json_path = os.path.splitext(audio_path)[0] + ".json"

            if os.path.exists(json_path):
                print(f"Skipping {file} (already processed)")
                continue

            print(f"Processing {file}...")
            data = analyze_song(audio_path)

            with open(json_path, "w") as f:
                json.dump(data, f, indent=4)

            print(f"Saved: {json_path}")

if __name__ == "__main__":
    process_folder(AUDIO_FOLDER)
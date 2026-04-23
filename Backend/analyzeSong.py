import json
import os

import librosa
import numpy as np
from scipy.ndimage import uniform_filter1d

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
AUDIO_FOLDER = os.path.join(SCRIPT_DIR, "..", "Assets", "Audio")
AUDIO_FOLDER = os.path.normpath(AUDIO_FOLDER)


def normalize(values):
    values = np.asarray(values, dtype=float)
    if values.size == 0:
        return values

    minimum = np.min(values)
    maximum = np.max(values)
    if maximum - minimum < 1e-6:
        return np.full(values.shape, 0.5)

    return (values - minimum) / (maximum - minimum)


def generate_targets(beat_times, beat_energy, bpm):
    normalized_energy = normalize(beat_energy)
    targets = []

    beat_interval = 60.0 / bpm if bpm > 0 else 0.5
    side_targets = ["Left", "Right"]
    side_index = 0

    for index, beat_time in enumerate(beat_times):
        energy = normalized_energy[index]
        previous_energy = normalized_energy[index - 1] if index > 0 else energy
        delta_energy = energy - previous_energy
        gap = beat_time - beat_times[index - 1] if index > 0 else beat_interval
        phrase_reset = gap > beat_interval * 1.5

        if phrase_reset or index % 16 == 0:
            target = "Center"
        elif energy > 0.75 or delta_energy > 0.12:
            target = "Up"
        elif energy < 0.22 and gap <= beat_interval * 1.1:
            target = "Down"
        else:
            target = side_targets[side_index]
            side_index = 1 - side_index

        targets.append(target)

    return targets


def analyze_song(file_path):
    y, sr = librosa.load(file_path)

    tempo, beat_frames = librosa.beat.beat_track(y=y, sr=sr)
    tempo = float(tempo[0] if hasattr(tempo, "__iter__") else tempo)
    beat_times = librosa.frames_to_time(beat_frames, sr=sr)

    rmse = librosa.feature.rms(y=y)[0]
    times = librosa.frames_to_time(range(len(rmse)), sr=sr)
    smooth_energy = uniform_filter1d(rmse, size=10)

    beat_energy = []
    for beat_time in beat_times:
        idx = np.abs(times - beat_time).argmin()
        beat_energy.append(float(rmse[idx]))

    targets = generate_targets(beat_times, beat_energy, tempo)

    hold_threshold = np.mean(smooth_energy) * 1.2
    hold_segments = []
    start = None

    for index in range(len(smooth_energy)):
        if smooth_energy[index] > hold_threshold:
            if start is None:
                start = times[index]
        elif start is not None:
            end = times[index]
            if end - start > 0.6:
                hold_segments.append({
                    "start": float(start),
                    "end": float(end)
                })
            start = None

    return {
        "bpm": tempo,
        "beats": beat_times.tolist(),
        "energy": beat_energy,
        "targets": targets,
        "holds": hold_segments
    }


def process_folder(folder):
    print(f"Looking for audio in: {folder}")

    if not os.path.exists(folder):
        print("Error: folder does not exist!")
        return

    for file_name in os.listdir(folder):
        if file_name.endswith(".mp3") or file_name.endswith(".wav"):
            audio_path = os.path.join(folder, file_name)
            json_path = os.path.splitext(audio_path)[0] + ".json"

            print(f"Processing {file_name}...")
            data = analyze_song(audio_path)

            with open(json_path, "w") as file_handle:
                json.dump(data, file_handle, indent=4)

            print(f"Saved: {json_path}")


if __name__ == "__main__":
    process_folder(AUDIO_FOLDER)

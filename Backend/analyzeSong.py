import librosa
import json
import os

# Resolve audio folder
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
AUDIO_FOLDER = os.path.join(SCRIPT_DIR, "..", "Assets", "Audio")
AUDIO_FOLDER = os.path.normpath(AUDIO_FOLDER)

def analyze_song(file_path):
    y, sr = librosa.load(file_path)
    
    # Beat detection
    tempo, beat_frames = librosa.beat.beat_track(y=y, sr=sr)
    if hasattr(tempo, "__iter__"):
        tempo = float(tempo[0])
    else:
        tempo = float(tempo)
    beat_times = librosa.frames_to_time(beat_frames, sr=sr)
    
    # Energy (RMS)
    rmse = librosa.feature.rms(y=y)[0]  # RMS returns a 2D array
    rmse_times = librosa.frames_to_time(range(len(rmse)), sr=sr)
    
    # Map each beat to nearest energy value
    beat_energy = []
    for b in beat_times:
        idx = (abs(rmse_times - b)).argmin()  # closest energy sample to beat
        beat_energy.append(float(rmse[idx]))

    return {
        "bpm": tempo,
        "beats": beat_times.tolist(),
        "energy": beat_energy
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
using System.Collections.Generic;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    [System.Serializable]
    public class BeatData
    {
        public float bpm;
        public List<float> beats;
        public List<float> energy;
        public List<string> targets;
        public List<SlideData> holds; // reserved for slide notes
    }

    [System.Serializable]
    public class SlideData
    {
        public float start;
        public float end;
    }

    public AudioManager audioManager;
    public OrbSpawner orbSpawner;
    [Min(0f)] public float spawnDelaySeconds = 0.25f;

    private BeatData data;
    private int beatIndex = 0;
    private int slideIndex = 0;
    private bool isPlaying = false;
    private float lastSpawnedBeatTime = float.NegativeInfinity;

    void Update()
    {
        if (!isPlaying || data == null || audioManager == null || audioManager.audioSource == null)
            return;

        float songTime = audioManager.audioSource.time;

        while (slideIndex < GetHoldCount() && songTime >= data.holds[slideIndex].start)
        {
            var hold = data.holds[slideIndex];
            orbSpawner.SpawnSlide(hold.start, hold.end);
            slideIndex++;
        }

        while (beatIndex < data.beats.Count && songTime >= data.beats[beatIndex])
        {
            float beatTime = data.beats[beatIndex];

            if (!IsBeatInsideHold(beatTime) && beatTime - lastSpawnedBeatTime >= spawnDelaySeconds)
            {
                orbSpawner.SpawnOrb(
                    GetEnergyForBeat(beatIndex),
                    GetTargetForBeat(beatIndex));
                lastSpawnedBeatTime = beatTime;
            }

            beatIndex++;
        }
    }

    public void StartSong(Song song)
    {
        data = JsonUtility.FromJson<BeatData>(song.beatJson.text);
        beatIndex = 0;
        slideIndex = 0;
        lastSpawnedBeatTime = float.NegativeInfinity;
        isPlaying = true;
    }

    public void StopSong()
    {
        isPlaying = false;
        beatIndex = 0;
        slideIndex = 0;
        lastSpawnedBeatTime = float.NegativeInfinity;
        data = null;
    }

    int GetHoldCount()
    {
        return data.holds != null ? data.holds.Count : 0;
    }

    bool IsBeatInsideHold(float beatTime)
    {
        if (data.holds == null)
            return false;

        for (int i = 0; i < data.holds.Count; i++)
        {
            var hold = data.holds[i];

            if (beatTime >= hold.start && beatTime <= hold.end)
                return true;
        }

        return false;
    }

    float GetEnergyForBeat(int index)
    {
        if (data.energy == null || index < 0 || index >= data.energy.Count)
            return 0.5f;

        return data.energy[index];
    }

    string GetTargetForBeat(int index)
    {
        if (data.targets != null && index >= 0 && index < data.targets.Count && !string.IsNullOrWhiteSpace(data.targets[index]))
            return data.targets[index];

        // Fallback for older beatmaps.
        if (index % 8 == 0)
            return "Center";

        return index % 2 == 0 ? "Left" : "Right";
    }
}

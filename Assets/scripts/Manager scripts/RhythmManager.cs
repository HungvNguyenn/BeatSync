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
        public List<SlideData> holds; // <-- now treated as SLIDES
    }

    [System.Serializable]
    public class SlideData
    {
        public float start;
        public float end;
    }

    public AudioManager audioManager;
    public OrbSpawner orbSpawner;

    private BeatData data;
    private int beatIndex = 0;
    private int slideIndex = 0;
    private bool isPlaying = false;

    void Update()
    {
        if (!isPlaying || data == null || beatIndex >= data.beats.Count)
            return;

        float songTime = audioManager.audioSource.time;

        if (songTime >= data.beats[beatIndex])
        {
            float energy = data.energy[beatIndex];

            orbSpawner.SpawnOrb(energy);

            beatIndex++;
        }
    }

    public void StartSong(Song song)
    {
        data = JsonUtility.FromJson<BeatData>(song.beatJson.text);
        beatIndex = 0;
        slideIndex = 0;
        isPlaying = true;
    }

    public void StopSong()
    {
        isPlaying = false;
        beatIndex = 0;
        slideIndex = 0;
        data = null;
    }
}
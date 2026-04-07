using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    private Coroutine previewLoopCoroutine;

    // Play a preview segment and loop it properly
    public void PlaySongPreviewLoop(AudioClip clip, float previewLength = 20f, float startTime = 0f)
    {
        if (clip == null) return;

        StopCurrentPreview(); // Stop previous preview

        // Clamp startTime so preview fits
        startTime = Mathf.Clamp(startTime, 0f, Mathf.Max(0f, clip.length - previewLength));

        previewLoopCoroutine = StartCoroutine(PreviewSegmentLoop(clip, startTime, previewLength));
    }

    private IEnumerator PreviewSegmentLoop(AudioClip clip, float startTime, float previewLength)
    {
        audioSource.clip = clip;
        audioSource.time = startTime;
        audioSource.Play();

        while (true)
        {
            if (audioSource.time >= startTime + previewLength)
            {
                audioSource.time = startTime;
            }
            yield return null;
        }
    }

    public void StopCurrentPreview()
    {
        if (previewLoopCoroutine != null)
        {
            StopCoroutine(previewLoopCoroutine);
            previewLoopCoroutine = null;
        }

        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    // Play the full song and call a callback when finished
    public void PlaySongWithCallback(AudioClip clip, System.Action onFinished)
    {
        if (clip == null) return;

        StopCurrentPreview();

        StartCoroutine(PlaySongCoroutine(clip, onFinished));
    }

    private IEnumerator PlaySongCoroutine(AudioClip clip, System.Action onFinished)
    {
        audioSource.clip = clip;
        audioSource.time = 0f;
        audioSource.Play();

        while (audioSource.isPlaying)
            yield return null;

        onFinished?.Invoke();
    }
}
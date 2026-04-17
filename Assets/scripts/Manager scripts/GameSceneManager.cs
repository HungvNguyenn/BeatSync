using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    private AudioManager audioManager;

    public RhythmManager rhythmManager;

    public TMP_Text countdownText;

    public int menuSceneIndex = 1;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        if (GameManager.instance != null && GameManager.instance.currentSong != null)
        {
            audioManager.StopCurrentPreview();

            StartCoroutine(StartSongCountdown(GameManager.instance.currentSong));
        }
        else
        {
            Debug.LogError("GameSceneManager: No song selected or GameManager missing!");
        }
    }

    private IEnumerator StartSongCountdown(Song song)
    {
        // Countdown: 3 → 2 → 1 → GO
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownText.text = "";

        // Play song + start rhythm system
        audioManager.PlaySongWithCallback(
            song.clip,
            () =>
            {
                // Song finished → return to menu
                SceneManager.LoadScene(menuSceneIndex);
            },
            song,
            rhythmManager
        );
    }
}
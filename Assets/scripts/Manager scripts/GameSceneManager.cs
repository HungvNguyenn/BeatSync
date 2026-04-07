using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // Needed for IEnumerator

public class GameSceneManager : MonoBehaviour
{
    private AudioManager audioManager;
    public TMP_Text countdownText; // assign in inspector
    public int menuSceneIndex = 1; // assign your menu scene index

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        if (GameManager.instance != null && GameManager.instance.currentSong != null)
        {
            // Stop any preview from the menu
            audioManager.StopCurrentPreview();

            // Start countdown and play the song
            StartCoroutine(StartSongCountdown(GameManager.instance.currentSong));
        }
        else
        {
            Debug.LogError("GameSceneManager: No song selected or GameManager missing!");
        }
    }

    private IEnumerator StartSongCountdown(Song song)
    {
        // Countdown 3 → 2 → 1 → GO!
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownText.text = "";

        // Play the full song once and return to menu when done
        audioManager.PlaySongWithCallback(song.clip, () =>
        {
            // Return to menu scene
            SceneManager.LoadScene(menuSceneIndex);
        });
    }
}
using UnityEngine;

public class SongManager : MonoBehaviour
{
    public playlistManager playlistManager; // reference to the song list
    private AudioManager audioManager;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void PlaySongPreview(Song song, float startTime = 0f, float duration = 20f)
    {
        if (audioManager != null && song.clip != null)
        {
            audioManager.PlaySongPreviewLoop(song.clip, duration, startTime);
        }
    }

    public Song GetSong(int index)
    {
        if (playlistManager != null && index >= 0 && index < playlistManager.songs.Length)
            return playlistManager.songs[index];
        return null;
    }
}
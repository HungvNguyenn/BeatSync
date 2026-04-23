using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaylistUI : MonoBehaviour
{
    public Transform contentParent;       // ScrollView -> Viewport -> Content
    public GameObject songItemPrefab;     // Button prefab
    public PlaylistManager playlistManager;

    private AudioManager audioManager;
    private GameObject currentPlayingItem;
    Color highlightColor = new Color(0.529f, 0.808f, 0.980f);

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        foreach (var song in playlistManager.songs)
        {
            GameObject item = Instantiate(songItemPrefab, contentParent);

            TMP_Text text = item.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = song.songName;

            RectTransform rt = item.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;

            Button button = item.GetComponent<Button>();
            if (button != null)
            {
                float startTime = Mathf.Clamp(song.clip.length / 2f - 5f, 0f, song.clip.length - 20f);
                button.onClick.AddListener(() =>
                {
                    // 1️⃣ Play a short preview
                    audioManager.PlaySongPreviewLoop(song.clip, 20f, startTime);

                    // 2️⃣ Store selected song for Game Scene
                    GameManager.instance.currentSong = song;

                    // 3️⃣ Highlight the button
                    HighlightPlaying(item);
                });
            }
        }
    }

    void HighlightPlaying(GameObject item)
    {
        if (currentPlayingItem != null)
            currentPlayingItem.GetComponent<Image>().color = Color.clear;

        currentPlayingItem = item;
        currentPlayingItem.GetComponent<Image>().color = highlightColor;
    }
}

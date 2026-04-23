using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpotifyBackend : MonoBehaviour
{
    [System.Serializable]
    public class TrackInfo
    {
        public string name;
        public string[] artists;
        public float tempo;
    }

    // Call this with the Spotify track ID
    public void FetchTrackInfo(string trackID)
    {
        StartCoroutine(GetTrackInfo(trackID));
    }

    IEnumerator GetTrackInfo(string trackID)
    {
        string url = "http://localhost:5000/track/" + trackID;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Backend request failed: " + request.error);
                yield break;
            }

            TrackInfo track = JsonUtility.FromJson<TrackInfo>(request.downloadHandler.text);

            string artistNames = string.Join(", ", track.artists);

            Debug.Log($"Track Name: {track.name}");
            Debug.Log($"Artist(s): {artistNames}");
            Debug.Log($"Tempo (BPM): {(track.tempo > 0 ? track.tempo.ToString() : "N/A")}");
        }
    }

}
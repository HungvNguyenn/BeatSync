// Example usage
using UnityEngine;

public class SpotifyBackendTest : MonoBehaviour
{
    public SpotifyBackend backend;

    void Start()
    {
        backend.FetchTrackInfo("32OlwWuMpZ6b0aN2RZOeMS"); 
    }
}

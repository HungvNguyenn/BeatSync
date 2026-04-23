using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Button myButton;
    public int sceneIndexToLoad; // use the build index now

    void Start()
    {
        if (myButton != null)
            myButton.onClick.AddListener(OnButtonClicked);
        else
            Debug.LogError("MenuButton: myButton is not assigned in the Inspector!");
    }

    void OnButtonClicked()
    {
        // Optional: check index validity
        if (sceneIndexToLoad >= 0 && sceneIndexToLoad < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(sceneIndexToLoad);
        else
            Debug.LogError("MenuButton: sceneIndexToLoad is out of build settings range!");
    }
}
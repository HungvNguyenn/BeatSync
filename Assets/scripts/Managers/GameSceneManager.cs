using System.Collections;
using TMPro;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    private AudioManager audioManager;

    public RhythmManager rhythmManager;
    public TMP_Text countdownText;
    public int menuSceneIndex = 1;

    [Header("Combo UI")]
    public TMP_Text comboText;
    public float comboDisplaySeconds = 0.9f;

    [Header("Feedback UI")]
    public TMP_Text feedbackText;
    public float feedbackDisplaySeconds = 0.75f;

    [Header("Results UI")]
    public TMP_Text resultsText;

    private Coroutine comboDisplayCoroutine;
    private Coroutine feedbackDisplayCoroutine;
    private int comboCount = 0;
    private int highestCombo = 0;
    private int hitCount = 0;
    private int missCount = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        EnsureComboText();
        EnsureFeedbackText();
        EnsureResultsText();
        ResetRunStats();

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

    public void RegisterTapResult(HandMovementType expectedMovement, HandMovementType measuredMovement)
    {
        bool isCorrect =
            expectedMovement == HandMovementType.Unknown ||
            measuredMovement == HandMovementType.Unknown ||
            measuredMovement == expectedMovement;

        if (isCorrect)
        {
            RegisterHit(1);
            ShowFeedbackMessage($"Good {measuredMovement}");
        }
        else
        {
            BreakCombo();
            ShowFeedbackMessage($"Wrong Move: {measuredMovement}");
        }
    }

    public void RegisterSliderCheckpoint()
    {
        RegisterHit(1);
        ShowFeedbackMessage("Slider +1");
    }

    public void RegisterMiss()
    {
        missCount++;
        BreakCombo();
        ShowFeedbackMessage("Miss");
    }

    public void BreakCombo()
    {
        if (comboCount > 0)
            ShowComboMessage($"Combo Break ({comboCount})");

        comboCount = 0;
    }

    public void ResetRunStats()
    {
        comboCount = 0;
        highestCombo = 0;
        hitCount = 0;
        missCount = 0;

        if (comboDisplayCoroutine != null)
        {
            StopCoroutine(comboDisplayCoroutine);
            comboDisplayCoroutine = null;
        }

        if (feedbackDisplayCoroutine != null)
        {
            StopCoroutine(feedbackDisplayCoroutine);
            feedbackDisplayCoroutine = null;
        }

        if (comboText != null)
            comboText.text = string.Empty;

        if (feedbackText != null)
            feedbackText.text = string.Empty;

        if (resultsText != null)
            resultsText.text = string.Empty;
    }

    private IEnumerator StartSongCountdown(Song song)
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownText.text = string.Empty;

        audioManager.PlaySongWithCallback(
            song.clip,
            () =>
            {
                ShowResults();
            },
            song,
            rhythmManager
        );
    }

    void RegisterHit(int amount)
    {
        hitCount += amount;
        comboCount += amount;
        highestCombo = Mathf.Max(highestCombo, comboCount);
        ShowComboMessage($"Combo x{comboCount}");
    }

    void ShowComboMessage(string message)
    {
        if (comboText == null)
            return;

        comboText.text = message;

        if (comboDisplayCoroutine != null)
            StopCoroutine(comboDisplayCoroutine);

        comboDisplayCoroutine = StartCoroutine(HideComboMessageAfterDelay());
    }

    void ShowFeedbackMessage(string message)
    {
        if (feedbackText == null)
            return;

        feedbackText.text = message;

        if (feedbackDisplayCoroutine != null)
            StopCoroutine(feedbackDisplayCoroutine);

        feedbackDisplayCoroutine = StartCoroutine(HideFeedbackMessageAfterDelay());
    }

    IEnumerator HideComboMessageAfterDelay()
    {
        yield return new WaitForSeconds(comboDisplaySeconds);

        if (comboText != null)
            comboText.text = string.Empty;

        comboDisplayCoroutine = null;
    }

    IEnumerator HideFeedbackMessageAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDisplaySeconds);

        if (feedbackText != null)
            feedbackText.text = string.Empty;

        feedbackDisplayCoroutine = null;
    }

    void ShowResults()
    {
        if (countdownText != null)
            countdownText.text = string.Empty;

        if (comboText != null)
            comboText.text = string.Empty;

        if (feedbackText != null)
            feedbackText.text = string.Empty;

        if (resultsText != null)
        {
            resultsText.text =
                "Song Complete\n" +
                $"Hits: {hitCount}\n" +
                $"Misses: {missCount}\n" +
                $"Highest Combo: {highestCombo}";
        }
    }

    void EnsureComboText()
    {
        if (comboText != null || countdownText == null)
            return;

        Canvas parentCanvas = countdownText.canvas;
        if (parentCanvas == null)
            return;

        GameObject comboObject = new GameObject("Combo Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        comboObject.transform.SetParent(parentCanvas.transform, false);

        RectTransform rectTransform = comboObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0.56f);
        rectTransform.anchorMax = new Vector2(0f, 0.56f);
        rectTransform.pivot = new Vector2(0f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(40f, 0f);
        rectTransform.sizeDelta = new Vector2(360f, 80f);

        TextMeshProUGUI createdText = comboObject.GetComponent<TextMeshProUGUI>();
        createdText.text = string.Empty;
        createdText.font = countdownText.font;
        createdText.fontSize = 28f;
        createdText.color = countdownText.color;
        createdText.alignment = TextAlignmentOptions.Left;

        comboText = createdText;
    }

    void EnsureFeedbackText()
    {
        if (feedbackText != null || countdownText == null)
            return;

        Canvas parentCanvas = countdownText.canvas;
        if (parentCanvas == null)
            return;

        GameObject feedbackObject = new GameObject("Feedback Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        feedbackObject.transform.SetParent(parentCanvas.transform, false);

        RectTransform rectTransform = feedbackObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0.46f);
        rectTransform.anchorMax = new Vector2(0f, 0.46f);
        rectTransform.pivot = new Vector2(0f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(40f, 0f);
        rectTransform.sizeDelta = new Vector2(420f, 80f);

        TextMeshProUGUI createdText = feedbackObject.GetComponent<TextMeshProUGUI>();
        createdText.text = string.Empty;
        createdText.font = countdownText.font;
        createdText.fontSize = 24f;
        createdText.color = countdownText.color;
        createdText.alignment = TextAlignmentOptions.Left;

        feedbackText = createdText;
    }

    void EnsureResultsText()
    {
        if (resultsText != null || countdownText == null)
            return;

        Canvas parentCanvas = countdownText.canvas;
        if (parentCanvas == null)
            return;

        GameObject resultsObject = new GameObject("Results Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        resultsObject.transform.SetParent(parentCanvas.transform, false);

        RectTransform rectTransform = resultsObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(560f, 280f);

        TextMeshProUGUI createdText = resultsObject.GetComponent<TextMeshProUGUI>();
        createdText.text = string.Empty;
        createdText.font = countdownText.font;
        createdText.fontSize = 44f;
        createdText.color = countdownText.color;
        createdText.alignment = TextAlignmentOptions.Center;

        resultsText = createdText;
    }
}

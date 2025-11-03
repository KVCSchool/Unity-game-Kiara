using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreLabel : MonoBehaviour
{
    [SerializeField]
    private ScoreManager _scoreManager;

    private TextMeshProUGUI _label;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _label = GetComponent<TextMeshProUGUI>();

        UpdateLabelText();
        _scoreManager.OnScoreChanged.AddListener(OnScoreChanged);
    }

    private void UpdateLabelText()
        => _label.text = $"Score: {_scoreManager.Score:N0}";

    private void OnScoreChanged(int score)
        => UpdateLabelText();
}

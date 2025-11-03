using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private int _score;

    [SerializeField]
    private UnityEvent<int> _onScoreChanged;

    public int Score
    {
        get => _score;
        set
        {
            if (_score != value)
            {
                _score = value;
                _onScoreChanged.Invoke(_score);
            }
        }
    }

    public UnityEvent<int> OnScoreChanged { get => _onScoreChanged; set => _onScoreChanged = value; }

    public void AwardScore(int score)
        => Score += score;
}

using UnityEngine;

public class ScoreAwarder : MonoBehaviour
{
    private ScoreManager _scoreManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
        => _scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
    
    public void AwardScore(int score)
        => _scoreManager.AwardScore(score);
}

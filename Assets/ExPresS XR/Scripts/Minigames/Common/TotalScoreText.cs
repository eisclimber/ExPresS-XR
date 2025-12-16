using ExPresSXR.Minigames.Common;
using TMPro;
using UnityEngine;

public class TotalScoreText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;

    [SerializeField]
    private GameObject _diffDisplayPrefab;

    [SerializeField]
    private float _pointsDisplayScale = 0.35f;

    [SerializeField]
    private Vector3 _pointsDisplayOffset;


    [SerializeField]
    private int _displayedScore;
    public int DisplayedScore
    {
        get => _displayedScore;
        set
        {
            int diff = value - _displayedScore;
            _displayedScore = value;
            _text.text = value.ToString();
            ShowDiffDisplay(diff);
        }
    }


    private void OnEnable()
    {
        if (_text == null && !TryGetComponent(out _text))
        {
            Debug.LogError("Did not find a text component to display the score.", this);
        }
    }

    private void ShowDiffDisplay(int diff)
    {
        if (diff == 0 || _diffDisplayPrefab == null)
        {
            return;
        }

        GameObject scoreNumbersGo = Instantiate(_diffDisplayPrefab); // Instantiate without parent to avoid canvas rotation problems
        scoreNumbersGo.transform.SetPositionAndRotation(transform.position + _pointsDisplayOffset, transform.rotation);
        scoreNumbersGo.transform.localScale = Vector3.one * _pointsDisplayScale;

        if (scoreNumbersGo.TryGetComponent(out ScoreNumbers scoreNumbers))
        {
            scoreNumbers.Score = diff;
        }
    }
}

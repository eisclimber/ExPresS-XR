using UnityEngine;
using TMPro;


namespace ExPresSXR.Minigames.Archery
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Ref to the textfield containing the score")]
        public TMP_Text Score_Text;
        private int Score = 0;
        [SerializeField, Tooltip("is this Score Game Specific?")]
        public bool game = false;

        // Start is called before the first frame update
        void Start()
        {
            Score_Text.text = Score.ToString();
        }

        public void AlterPoints(Collision col)
        {
            if (!game)
            {
                if (col.gameObject.CompareTag("Target"))
                {
                    Score++;
                    Score_Text.text = Score.ToString();
                }
                else
                {
                    if (Score > 0)
                    {
                        Score--;
                        Score_Text.text = Score.ToString();
                    }
                }
            }
        }

        public void Reset()
        {
            Score = 0;
            Score_Text.text = Score.ToString();
        }
    }
}
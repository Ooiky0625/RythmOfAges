using UnityEngine;
using Platinio.TweenEngine;
using TMPro;

namespace VRBeats
{
    public class FinalScoreLabel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText = null;
        [SerializeField] private float scoreFadeTime = 10.0f;
        [SerializeField] private int length = 10;

        private string initialValue = "";
        private ScoreManager scoreManager = null;

        private void Awake()
        {
            for (int n = 0; n < length; n++)
            {
                initialValue += "0";
            }

            scoreManager = FindObjectOfType<ScoreManager>();

        }

        public void ShowScore()
        {
            PlatinioTween.instance.ValueTween( 0.0f , scoreManager.CurrentScore , scoreFadeTime).SetOnUpdateFloat(delegate (float v)
            {
                SetScore( (int)v );
            });
        }

        public void ResetValues()
        {
            gameObject.CancelAllTweens();
            scoreText.text = initialValue;
        }


        private void SetScore(int score)
        {
            if (this.scoreText == null)
                return;

            string scoreText = score.ToString();
            int addLength = Mathf.Max( length - scoreText.Length  , 0);
            string addZeros = "";
            for (int n = 0; n < addLength; n++)
            {
                addZeros += "0";
            }

            this.scoreText.text = addZeros + scoreText;

        }

    }
}


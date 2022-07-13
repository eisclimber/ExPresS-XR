using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ExPresSXR.Misc
{
    public class CakeDemoScript : MonoBehaviour
    {
        const float END_CONTENT_DURATION = 2.0f;

        private Button _yesButton;
        private Button _noButton;
        private Button _quitButton;

        private GameObject _cakeContent;
        private GameObject _lieContent;
        private GameObject _endContent;


        private void Awake()
        {
            Transform yesTransform = RuntimeUtils.RecursiveFindChild(transform, "Yes Button");
            Transform noTransform = RuntimeUtils.RecursiveFindChild(transform, "No Button");
            Transform quitTransform = RuntimeUtils.RecursiveFindChild(transform, "Quit Button");

            _yesButton = yesTransform.GetComponent<Button>();
            _noButton = noTransform.GetComponent<Button>();
            _quitButton = quitTransform.GetComponent<Button>();

            _cakeContent = RuntimeUtils.RecursiveFindChild(transform, "Cake Content").gameObject;
            _lieContent = RuntimeUtils.RecursiveFindChild(transform, "Lie Content").gameObject;
            _endContent = RuntimeUtils.RecursiveFindChild(transform, "End Content").gameObject;

            _yesButton.onClick.AddListener(ShowLieContent);
            _noButton.onClick.AddListener(ShowLieContent);
            _quitButton.onClick.AddListener(ShowEndContent);

            ShowCakeContent();
        }

        private void ShowCakeContent()
        {
            _cakeContent.SetActive(true);
            _lieContent.SetActive(false);
            _endContent.SetActive(false);
        }

        private void ShowLieContent()
        {
            _cakeContent.SetActive(false);
            _lieContent.SetActive(true);
            _endContent.SetActive(false);
        }

        private void ShowEndContent()
        {
            _cakeContent.SetActive(false);
            _lieContent.SetActive(false);
            _endContent.SetActive(true);
            StartCoroutine(CloseCoroutine());
        }

        private IEnumerator CloseCoroutine()
        {
            yield return new WaitForSeconds(END_CONTENT_DURATION);
            CloseApplication();
        }


        private void CloseApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
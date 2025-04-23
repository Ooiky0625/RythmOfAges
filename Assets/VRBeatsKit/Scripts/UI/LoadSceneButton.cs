using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VRBeats
{
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField] private Button button = null;
        [SerializeField] private string sceneName = "";

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            SceneManager.LoadScene(sceneName);
        }
    }

}


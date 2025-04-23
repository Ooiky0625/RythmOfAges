using Platinio.TweenEngine;
using UnityEngine;
using UnityEngine.Events;

namespace VRBeats
{
    public class Popup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup cg = null;
        [SerializeField] private float fadeTime = 1.0f;
        [SerializeField] private Ease fadeEase = Ease.Linear;
        [SerializeField] private UnityEvent onShow = null;
        [SerializeField] private UnityEvent onHide = null;

        private void Awake()
        {
            cg.alpha = 0.0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        public void Show()
        {
            SetCanvasGroupInteractable(cg, true);
            FadeTween(1.0f).SetOnComplete( onShow.Invoke );
        }

        public void Hide()
        {
            SetCanvasGroupInteractable(cg, false);
            FadeTween(0.0f).SetOnComplete( onHide.Invoke );
        }

        private void SetCanvasGroupInteractable(CanvasGroup cg , bool state)
        {
            cg.interactable = state;
            cg.blocksRaycasts = state;
        }

        private BaseTween FadeTween(float alpha)
        {
            gameObject.CancelAllTweens();
            return cg.Fade(alpha, fadeTime).SetEase(fadeEase).SetOwner(gameObject).SetOnComplete(delegate 
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;                
            });
        }

    }

}

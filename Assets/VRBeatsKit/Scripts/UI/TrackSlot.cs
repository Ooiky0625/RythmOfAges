using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Platinio.TweenEngine;
using System;

namespace VRBeats.UI
{
    public class TrackSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  , IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI songName = null;
        [SerializeField] private TextMeshProUGUI authorName = null;
        [SerializeField] private Image bg = null; 
        [SerializeField] private Image potrait = null;
        [SerializeField] private Color textSelectedColor;
        [SerializeField] private Color textNormalColor;
        [SerializeField] private Color backgroundSelectedColor;
        [SerializeField] private Color backgroundNormalColor;
        [SerializeField] private float fadeTime = 1.0f;
        [SerializeField] private Ease ease = Ease.EaseOutExpo;

        private Action<TrackInfo> onClick = null;
        private TrackInfo trackInfo = null;

        private void Awake()
        {
            songName.color = textNormalColor;
            authorName.color = textNormalColor;
            bg.color = backgroundNormalColor;
        }

        public void Construct(TrackInfo info , Action<TrackInfo> onClick)
        {
            potrait.sprite = info.potrait;
            string songTile = string.Format("{0} ({1})" , info.songName , info.Mode.ToString());
            songName.text = songTile;
            authorName.text = info.author;
            trackInfo = info;
            this.onClick = onClick;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.CancelAllTweens();
            PlatinioTween.instance.ColorTween(songName.color , textSelectedColor , fadeTime).SetOnUpdateColor( delegate (Color c)
            {
                songName.color = c;
                authorName.color = c;
            } ).SetOwner(gameObject).SetEase(ease);

            bg.ColorTween( backgroundSelectedColor , fadeTime ).SetOwner(gameObject).SetEase(ease);

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.CancelAllTweens();
            PlatinioTween.instance.ColorTween(songName.color, textNormalColor, fadeTime).SetOnUpdateColor(delegate (Color c)
            {
                songName.color = c;
                authorName.color = c;
            }).SetOwner(gameObject).SetEase(ease);

            bg.ColorTween(backgroundNormalColor, fadeTime).SetOwner(gameObject).SetEase(ease);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke(trackInfo);
        }
    }
}



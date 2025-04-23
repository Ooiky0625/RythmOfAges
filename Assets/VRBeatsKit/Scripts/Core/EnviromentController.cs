using UnityEngine;
using Platinio.TweenEngine;

namespace VRBeats
{
    public class EnviromentController : MonoBehaviour
    {
        [SerializeField] private MaterialBindings enviromentMaterialBindings = null;
        [SerializeField] private float lightsOffTime = 0.5f;
        [SerializeField] private float startingEmmisiveValue = 5.0f;
        [SerializeField] private Color startingEmmisiveColor = Color.black;

        private float currentEmmisiveValue = 0.0f;
                    
        private void Awake()
        {
            currentEmmisiveValue = startingEmmisiveValue;
            enviromentMaterialBindings.SetEmmisiveColor(startingEmmisiveColor * startingEmmisiveValue);
        }

        public void TurnLightsOff()
        {
            Color from = enviromentMaterialBindings.GetEmmisiveColor();
            PlatinioTween.instance.ColorTween(from , Color.black , lightsOffTime).SetOnUpdateColor( delegate (Color c)
            {
                if(enviromentMaterialBindings != null)
                    enviromentMaterialBindings.SetEmmisiveColor(c);
            } ).SetEase(Ease.EaseOutExpo);
        }

        public void TurnLightsOn()
        {
            Color from = enviromentMaterialBindings.GetEmmisiveColor();
            PlatinioTween.instance.ColorTween(from, startingEmmisiveColor * startingEmmisiveValue , lightsOffTime).SetOnUpdateColor(delegate (Color c)
            {
                if (enviromentMaterialBindings != null)
                    enviromentMaterialBindings.SetEmmisiveColor(c);
            }).SetEase(Ease.EaseOutExpo);
        }

        public void FadeToColor(Color to , float time , Ease ease)
        {
            Color from = enviromentMaterialBindings.GetEmmisiveColor();

            PlatinioTween.instance.ColorTween(from, to * currentEmmisiveValue, time).SetOnUpdateColor(delegate (Color c)
            {
                if (enviromentMaterialBindings != null)
                    enviromentMaterialBindings.SetEmmisiveColor(c );
            }).SetEase(ease);

        }

        public void FadeEmmisiveValue(float to, float time, Ease ease)
        {
            Color color = enviromentMaterialBindings.GetEmmisiveColor();

            PlatinioTween.instance.ColorTween(color, color * to, time).SetOnUpdateColor(delegate (Color c)
            {
                if (enviromentMaterialBindings != null)
                    enviromentMaterialBindings.SetEmmisiveColor(c);
            }).SetEase(ease);
        }

    }
}

using UnityEngine;
using UnityEngine.Audio;
using Platinio.TweenEngine;

namespace VRBeats
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup mixerGroup = null;
        [SerializeField] private float fadeOutTime = 4.0f;

        private AudioSource audioSource = null;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = mixerGroup;

            ResetThisComponent();

        }

        private void ResetThisComponent()
        {
            SetAudioMixerPitch(1.0f);
            gameObject.CancelAllTweens();
        }

        public BaseTween BlendAudioMixerPitch(float from ,float to)
        {
            return PlatinioTween.instance.ValueTween(from , to , fadeOutTime).SetEase(Ease.EaseOutExpo).SetOnUpdateFloat(delegate (float v)
            {
                if(audioSource != null)
                    SetAudioMixerPitch(v);
            }).SetOwner(gameObject);
        }

        public void SetAudioMixerPitch(float value)
        {
            audioSource.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", value);
        }

    }

}


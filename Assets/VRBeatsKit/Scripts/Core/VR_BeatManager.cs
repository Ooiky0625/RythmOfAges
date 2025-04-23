using UnityEngine;
using Platinio;
using UnityEngine.Playables;
using VRBeats.ScriptableEvents;
using VRSDK;

namespace VRBeats
{
    public class VR_BeatManager : Singleton<VR_BeatManager>
    {
        [SerializeField] private BoxCollider playZone = null;
        [SerializeField] private Transform player = null;
        [SerializeField] private VR_BeatSettings settings = null;
        [SerializeField] private GameEvent onGameOver = null;
        
        private AudioManager audioManager = null;
        private EnviromentController enviromentController = null;
        private PlayableDirector playableDirector = null;
        private bool isGameRunning = true;

        public Color RightColor { get { return settings.RightColor * settings.GlowIntensity; } }
        public Color LeftColor { get { return settings.LeftColor * settings.GlowIntensity; } }
        public VR_BeatSettings GameSettings { get { return settings; } }

        public Transform Player { get { return player; } }

        private int playerConsecutiveMiss = 0;

        protected override void Awake()
        {
            base.Awake();
            audioManager = FindObjectOfType<AudioManager>();
            enviromentController = FindObjectOfType<EnviromentController>();
            playableDirector = FindObjectOfType<PlayableDirector>();
        }

        protected override void Start()
        {
            base.Start();
            playerConsecutiveMiss = 0;
        }
        

        public Color GetColorFromColorSide(ColorSide side)
        {
            return side == ColorSide.Right ? RightColor : LeftColor;
        }

        public Color GetColorFromControllerType(VR_ControllerType controller)
        {
            return controller == VR_ControllerType.Right ? RightColor : LeftColor;
        }

        private int frame = 0;
        private void Update()
        {
            frame++;                       
        }

        public void Spawn(Spawneable spawneable , SpawnEventInfo info)
        {
            if (!isGameRunning)
                return;
           
            Vector3 finalPosition = CalculateSpawnPosition( info.position);
            Vector3 travelOffset = Vector3.forward * -settings.TargetTravelDistance;
            Vector3 spawnPosition = finalPosition - travelOffset;

            Spawneable clone = Instantiate( spawneable , spawnPosition , Quaternion.Euler( info.rotation ) );
            SetSpeedRelativeToPlayZone(info);
            clone.Construct(info);
            
            Vector3 finalScale = clone.transform.localScale;
            clone.transform.localScale = Vector3.zero;

            
            clone.transform.Move(finalPosition, settings.TargetTravelTime).SetEase(settings.TargetTravelEase).SetOnComplete(delegate
            {
                clone.OnSpawn();
            }).SetUpdateMode(Platinio.TweenEngine.UpdateMode.Update);     

            
            clone.transform.ScaleTween(finalScale, settings.TargetTravelTime).SetEase(settings.TargetTravelEase);

        }

        private void SetSpeedRelativeToPlayZone(SpawnEventInfo info)
        {
            info.speedMultiplier = (int) Mathf.Sign(playZone.transform.forward.z * -1.0f);
        }
       
        private Vector3 CalculateSpawnPosition(Vector3 relativePosition)
        {
            Vector3 pos = CalculatePlayZoneCenter();

            pos += Vector3.right * relativePosition.x * playZone.size.x;
            pos += Vector3.up * relativePosition.y * playZone.size.y;
            pos += Vector3.forward * relativePosition.z * playZone.size.z;

            return pos;
        }

        private Vector3 CalculatePlayZoneCenter()
        {
            return playZone.transform.position + playZone.center;
        }

        public void GameOver()
        {
            //the game is already stopped
            if (!isGameRunning)
            {
                return;
            }

            isGameRunning = false;
            //slowdown the music to 0 and stop the playabledirector
            audioManager.BlendAudioMixerPitch(1.0f , 0.0f).SetOnComplete( delegate {
                if (playableDirector != null)
                    playableDirector.Stop();
                } 
            ).SetOwner(gameObject);
            enviromentController.TurnLightsOff();
            
        }

        public void RestartLevel()
        {
            gameObject.CancelAllTweens();

            isGameRunning = true;
            audioManager.SetAudioMixerPitch(1.0f);
            enviromentController.TurnLightsOn();
            playableDirector.time = 0.0f;
            playableDirector.Play();
        }

    }

}

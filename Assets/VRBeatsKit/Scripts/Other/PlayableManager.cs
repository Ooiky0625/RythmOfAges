using UnityEngine;
using UnityEngine.Playables;

namespace VRBeats
{
    public class PlayableManager : MonoBehaviour
    {
        [SerializeField] private TracksDatabase tracks = null;

        private PlayableDirector playableDirector = null;

        private static int selectedTrackIndex = 0;

        private void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
            playableDirector.playOnAwake = false;

            playableDirector.playableAsset = tracks.TrackList[selectedTrackIndex].playableAsset;            
        }

        private void Start()
        {
            playableDirector.Play();
        }

        public static void SetSelectedTrackIndex(int index)
        {
            selectedTrackIndex = index;
        }
    }

}

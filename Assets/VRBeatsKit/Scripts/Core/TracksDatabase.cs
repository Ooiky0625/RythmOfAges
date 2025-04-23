using UnityEngine;
using UnityEngine.Playables;

public enum Mode
{
    Saber,
    Boxing
}

namespace VRBeats
{
    [CreateAssetMenu(fileName ="TracksDatabase" , menuName = "VR Beats/Create Tracks Database")]
    public class TracksDatabase : ScriptableObject
    {
        [SerializeField] private TrackInfo[] trackList = null;

        public TrackInfo[] TrackList { get { return trackList; } }

    }

    [System.Serializable]
    public class TrackInfo
    {
        public PlayableAsset playableAsset = null;
        public Sprite potrait = null;
        public Mode Mode = Mode.Saber;
        public string songName = null;
        public string author = null;
    }

}


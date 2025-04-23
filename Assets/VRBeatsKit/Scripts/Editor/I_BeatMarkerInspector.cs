using UnityEditor;

namespace VRBeats.EditorCode
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VR_BeatSpawnMarker))]
    public class I_BeatMarkerInspector : Editor
    {
        private VR_BeatSpawnMarker beatMarker = null;
       
        private void OnEnable()
        {
            beatMarker = (VR_BeatSpawnMarker)target;
            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (beatMarker.spawneable == null)
                return;
            beatMarker.spawneable.CustomInspector(beatMarker.spawInfo , targets);           


        }
        


    }

}


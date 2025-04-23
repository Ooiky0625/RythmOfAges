using UnityEngine;

namespace VRBeats
{
    [CreateAssetMenu(menuName = "VR Beats/Create Cube Slice Action", fileName = "CubeSliceAction")]
    public class BeatCubeSliceAction : OnSliceAction
    {
        public override bool OnSlice(VR_BeatCube beat, BeatDamageInfo info)
        {
            if (info.velocity < beat.MinCutSpeed)
                return false;

            //no matter the hit direction as soon as we have the right velocity for a cube that has a dot
            if (beat.HitDirection == Direction.Center)
                return true;

            float cutAngle = Vector2.Angle(beat.transform.up, info.hitDir);
            return info.colorSide == beat.ThisColorSide && cutAngle < 80.0f;
        }
    }

}


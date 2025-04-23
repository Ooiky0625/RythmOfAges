using UnityEditor;

namespace VRSDK.EditorCode
{
    [CustomEditor(typeof(AnimatorHelper))]
    public class I_AnimatorHelperInspector : Editor
    {
        private void Awake()
        {
            ( target as AnimatorHelper ).ConstructAnimatorControllerInfo();
        }
    }

}


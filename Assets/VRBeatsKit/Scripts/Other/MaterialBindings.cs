using UnityEngine;
using UnityEngine.Events;

namespace VRBeats
{
    public class MaterialBindings : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderArray = null;
        [SerializeField] private UnityEvent onSetMaterialColor = null;

        public void SetBaseColor(Color c)
        {
            SetColorBinding("_BaseColor" , c);
        }

        public void SetEmmisiveColor(Color c)
        {
            SetColorBinding("_EmissionColor", c );

            onSetMaterialColor.Invoke();

        }

        public Color GetEmmisiveColor()
        {
            return renderArray[0].material.GetColor("_EmissionColor");
        }

        public void SetUseEmmisiveIntensity(bool value)
        {
            SetIntegerBinding("_UseEmissiveIntensity", value ? 1 : 0);
        }

        private void SetColorBinding(string binding ,Color c)
        {
            for (int n = 0; n < renderArray.Length; n++)
            {                
                renderArray[n].material.SetColor(binding , c);

            }
            
        }

        private void SetFloatBinding(string binding, float v)
        {
            for (int n = 0; n < renderArray.Length; n++)
            {
                renderArray[n].material.SetFloat(binding, v);
            }
        }

        private void SetIntegerBinding(string binding, int v)
        {
            for (int n = 0; n < renderArray.Length; n++)
            {
                renderArray[n].material.SetInt(binding, v);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace VRSDK
{
    public class VR_DropZoneUI : MonoBehaviour
    {
        [SerializeField] private Text countText = null;
        [SerializeField] private VR_DropZone dropZone = null;

        private int currentCounterValue = 0;

        private void Awake()
        {
            countText.text = "";

            dropZone.OnDrop.AddListener( OnDrop );
            dropZone.OnUnDrop.AddListener( OnUnDrop );
        }

        private void OnDrop(VR_Grabbable grabbable)
        {
            currentCounterValue++;
            UpdateText();
        }

        private void OnUnDrop(VR_Grabbable grabbbale)
        {
            currentCounterValue--;
            UpdateText();            
        }

        private void UpdateText()
        {
            if (currentCounterValue > 1)
            {
                countText.text = currentCounterValue.ToString();
            }
            else
            {
                countText.text = "";
            }
        }



    }
}


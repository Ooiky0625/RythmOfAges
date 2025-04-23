using UnityEngine;
using System.Collections;

namespace VRBeats
{
    public class SaberSelectionPanel : MonoBehaviour
    {
        [SerializeField] private Carousel saberCarousel = null;       

        private IEnumerator Start()
        {
            
            yield return new WaitForEndOfFrame();
       
            saberCarousel.Focus( VR_SaberContainer.SelectedSaberIndex );
            saberCarousel.OnIndexValueChange.AddListener(OnSaberSelectionIndexChange);
        }

        private void OnSaberSelectionIndexChange(int index)
        {
            VR_SaberContainer.SetSelectedSaberIndex(index);
        }

    }
}


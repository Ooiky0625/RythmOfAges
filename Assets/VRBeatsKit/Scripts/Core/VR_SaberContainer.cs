using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBeats
{
    public class VR_SaberContainer : MonoBehaviour
    {
        private static int selectedSaberIndex = 0;
        public static int SelectedSaberIndex { get { return selectedSaberIndex; } }


        private void Awake()
        {
            EnableDesireSaber();
        }

        public static void SetSelectedSaberIndex(int index)
        {
            selectedSaberIndex = index;
        }

        private void EnableDesireSaber()
        {

            for (int n = 0; n < transform.childCount; n++)
            {
                transform.GetChild(n).gameObject.SetActive(n == selectedSaberIndex);
            }
        }

    }

}


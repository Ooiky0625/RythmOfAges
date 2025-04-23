using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRBeats.UI 
{
    public class TracksUIList : MonoBehaviour
    {
        [SerializeField] private TracksDatabase database = null;
        [SerializeField] private Scroller scroller = null;
        [SerializeField] private TrackSlot trackSlotPrefab = null;
        [SerializeField] private Transform contentParent = null;

        private bool sceneIsLoading = false;
        private const string boxingStyleSceneName = "BoxingStyle";
        private const string saberStyleScenName = "SaberStyle";

        private float timer = 0.0f;
        private bool trigger = false;

        private void Start()
        {
            PopulateList();
        }

        private void PopulateList()
        {
            for (int n = 0; n < database.TrackList.Length; n++)
            {
                int index = n;
                TrackSlot slot = Instantiate(trackSlotPrefab , contentParent);
                slot.transform.localScale = Vector3.one;
                slot.Construct(database.TrackList[n] , delegate (TrackInfo info){ OnSlotClick(info ,index); });
                scroller.AddElement(slot.gameObject);
            }
        }

        private void Update()
        {
            if (!trigger && timer >= 5.0f)
            {
                trigger = true;
                OnSlotClick(database.TrackList[0], 0);
            }
        }

        private void OnSlotClick(TrackInfo trackInfo , int index)
        {
            if (sceneIsLoading)
                return;

            sceneIsLoading = true;
            PlayableManager.SetSelectedTrackIndex(index);
            string sceneName = trackInfo.Mode == Mode.Boxing ? boxingStyleSceneName : saberStyleScenName;
            SceneManager.LoadScene( sceneName );
        }


    }

}


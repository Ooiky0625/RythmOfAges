using UnityEngine;
using Platinio.UI;
using VRBeats.Events;

namespace VRBeats
{
    public class Carousel : MonoBehaviour
    {
        [SerializeField] private RectTransform viewRect = null;
        [SerializeField] private float moveTime = 1.0f;
        [SerializeField] private Ease ease = Ease.EaseOutExpo;
        [SerializeField] private OnIntValueChange onIndexValueChange = null;
        

        private RectTransform[] elements = null;

        public OnIntValueChange OnIndexValueChange 
        {
            get 
            {
                return onIndexValueChange;
            }
        }

        public int CurrentIndex 
        {
            get
            {
                return currentIndex;
            }

            private set
            {
                currentIndex = value;
                onIndexValueChange.Invoke(currentIndex);

            }
        }

        private int currentIndex = 0;
        private bool isBusy = false;

        private void Start()
        {
            CurrentIndex = transform.childCount - 1;
            SetupCarouselElements();            
        }

        private void SetupCarouselElements()
        {
            elements = new RectTransform[transform.childCount];
            //this is the canvas center
            float x = 0.5f;

            for (int n = 0; n < transform.childCount; n++)
            {
                
                elements[n] = transform.GetChild(n).GetComponent<RectTransform>();
                elements[n].anchoredPosition = elements[n].FromAbsolutePositionToAnchoredPosition(new Vector2(x, 0.5f) , viewRect);               
                x -= 1.0f;
            }
        }

        public void MoveLeft()
        {            
            if (currentIndex + 1 >= transform.childCount || isBusy)
                return;

            CurrentIndex++;
            isBusy = true;

            for (int n = 0; n < transform.childCount; n++)
            {                
                Vector2 currentPos = elements[n].FromAnchoredPositionToAbsolutePosition(viewRect);
                elements[n].MoveUI(currentPos + new Vector2(-1.0f , 0.0f ) , viewRect , moveTime ).SetOnComplete(delegate { isBusy = false; }).SetEase(ease);
            }
        }

        public void MoveRight()
        {           

            if (currentIndex - 1 < 0 || isBusy)
                return;


            CurrentIndex--;
            isBusy = true;      
            for (int n = 0; n < transform.childCount; n++)
            {
                Vector2 currentPos = elements[n].FromAnchoredPositionToAbsolutePosition(viewRect);
                elements[n].MoveUI(currentPos + new Vector2(1.0f, 0.0f), viewRect, moveTime).SetOnComplete(delegate { isBusy = false; }).SetEase(ease);
            }
        }

        public void Focus(int index)
        {
            int offset = currentIndex - index;
            CurrentIndex = index;
            isBusy = true;
            
            for (int n = 0; n < transform.childCount; n++)
            {
                Vector2 currentPos = elements[n].FromAnchoredPositionToAbsolutePosition(viewRect);
                elements[n].MoveUI(currentPos + new Vector2(1.0f * offset, 0.0f), viewRect, moveTime).SetOnComplete(delegate { isBusy = false; }).SetEase(ease);
            }
        }


    }
}

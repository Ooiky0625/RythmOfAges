﻿using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Platinio.UI;

/// <summary>
/// Code to handle generic scroll list, like leaderboards or achievements
/// </summary>
public class Scroller : MonoBehaviour , IPointerDownHandler , IPointerUpHandler , IPointerExitHandler
{
    public enum ScrollMode
    {
        Horizontal,
        Vertical
    }

    #region INSPECTOR
    [SerializeField] protected ScrollRect scroll = null;
    [SerializeField] protected GridLayoutGroup gridLayout = null;
    [SerializeField] protected RectTransform viewRect = null;
    [SerializeField] protected ScrollMode scrollMode = ScrollMode.Horizontal;
    [SerializeField] protected bool shouldAling = false;
    [SerializeField] protected float scrollVelocityThreshold = 0.2f;
    #endregion

    #region PRIVATE
    private List<GameObject> elements = new List<GameObject>();
    private RectTransform contentRect = null;
    private Coroutine shouldAlingCoroutine = null;
    private float minSize = 0.0f;
    private bool userInteraction = false;
    #endregion

    protected RectTransform scrollRect = null;
    protected Action onReachEndOfList = null;

    public GridLayoutGroup GridLayout
    {
        get { return gridLayout; }
    }

    protected virtual void Awake()
    {
        scrollRect = scroll.GetComponent<RectTransform>();
        contentRect = gridLayout.GetComponent<RectTransform>();
        minSize = contentRect.rect.size.y;

        SetupScrollingComponets();

        //set listener for end of list
        scroll.onValueChanged.AddListener( delegate (Vector2 v)
         {
             if (userInteraction && scroll.velocity.magnitude > scrollVelocityThreshold)
             {
                 
                 if (shouldAlingCoroutine != null)
                     StopCoroutine( shouldAlingCoroutine );
                 shouldAlingCoroutine = StartCoroutine( ShouldAlingRoutine() );
             }
             

             if (GetNormalizedPosition() <= 0.0f && onReachEndOfList != null)
             {
                 onReachEndOfList();
             }
         } );
    }

    private void Start()
    {
        GetStartingElements();
        ResizeContentRect();
    }

    private void GetStartingElements()
    {
        for (int n = 0; n < gridLayout.transform.childCount; n++)
        {
            AddElement( gridLayout.transform.GetChild(n).gameObject );
        }
    }

    public float GetNormalizedPosition()
    {
        return scrollMode == ScrollMode.Horizontal ? scroll.horizontalNormalizedPosition : scroll.verticalNormalizedPosition;
    }

    private void SetupScrollingComponets()
    {
        gridLayout.startAxis = scrollMode == ScrollMode.Horizontal ? GridLayoutGroup.Axis.Horizontal : GridLayoutGroup.Axis.Vertical;
        scroll.vertical = scrollMode == ScrollMode.Vertical;
        scroll.horizontal = scrollMode == ScrollMode.Horizontal;
    }


    /// <summary>
    /// add element to scroll list
    /// </summary>
    public void AddElement(GameObject element)
    {
        element.transform.SetParent(gridLayout.transform, false);
        element.transform.localScale = Vector3.one;
        elements.Add( element );
        ResizeContentRect();
    }

    private IEnumerator ShouldAlingRoutine()
    {
        while (scroll.velocity.magnitude > 0.00001f)
        {
            yield return new WaitForEndOfFrame();
        }

        AlingToCloserElement();
    }

    private void AlingToCloserElement()
    {
        userInteraction = false;

        Vector2 pos = elements[0].GetComponent<RectTransform>().FromAnchoredPositionToAbsolutePosition(viewRect);
        Debug.Log(pos);
    }
    

    /// <summary>
    /// Resizes the content rect to fit new elements
    /// </summary>
    protected void ResizeContentRect()
    {

        float size = 0.0f;

        if (scrollMode == ScrollMode.Horizontal)
        {
            size = ((gridLayout.cellSize.x + gridLayout.spacing.x) * elements.Count) + gridLayout.padding.left;
        }
        else if (scrollMode == ScrollMode.Vertical)
        {
            size = (( gridLayout.cellSize.y + gridLayout.spacing.y ) * elements.Count) + gridLayout.padding.top;
        }
        

        //resize contentRect to fit new element
        if (size > minSize)
        {
            if (scrollMode == ScrollMode.Vertical)
            {
                contentRect.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, size );
            }
            else if (scrollMode == ScrollMode.Horizontal)
            {
                contentRect.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, size );
            }


            //reposition
            Vector2 pos = contentRect.anchoredPosition;

            if (scrollMode == ScrollMode.Vertical)
            {
                contentRect.anchoredPosition = new Vector2( pos.x, pos.y - ( ( gridLayout.cellSize.y + gridLayout.spacing.y ) / 2.0f ) );
            }
            else if (scrollMode == ScrollMode.Horizontal)
            {
                contentRect.anchoredPosition = new Vector2( pos.x + ( ( gridLayout.cellSize.x + gridLayout.spacing.x ) / 2.0f ), pos.y );
            }

        }


    }

    /// <summary>
    /// Deletes all elements from list
    /// </summary>
    protected void ClearList()
    {
        for (int n = 0; n < elements.Count; n++)
        {
            Destroy( elements[n].gameObject );
        }

        elements = new List<GameObject>();
        contentRect.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, minSize );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        userInteraction = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        userInteraction = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        userInteraction = false;
    }
}

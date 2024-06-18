using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]

public class TabBtn : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
{
    public TabGroup tabGroup;
    public Image backgroundImage;

    void Start()
    {
        tabGroup.Subscribe(this);
        backgroundImage = GetComponent<Image>();
    }
    public void Selected()
    {
        /*What should be done when selected,
         for example:Raising a event in Event Manager*/
    }
    public void Deslected()
    {
        /*What should be done when deselected,
         for example:Raising a event in Event Manager*/
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    

    
}

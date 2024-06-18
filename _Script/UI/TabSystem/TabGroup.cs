using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TabGroup : MonoBehaviour
{
    public List<TabBtn> tabButtons;
    public List<GameObject> tabPages;
    public TabBtn selectedButton;
    public Sprite buttonIdle;
    public Sprite buttonHover;
    public Sprite buttonActive;
    public void Subscribe(TabBtn tabButton)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabBtn>();
        }
        tabButtons.Add(tabButton);
    }
    public void OnTabEnter(TabBtn tabButton)
    {
        ResetButtonBackgroundImage();
        if(tabButtons == null||tabButton!=selectedButton) 
        { 
            tabButton.backgroundImage.sprite = buttonHover;
        }

    }
    public void OnTabSelected(TabBtn tabButton)
    {
        ResetButtonBackgroundImage();
        if (selectedButton)
        {
            selectedButton.Deslected();
        }
        selectedButton = tabButton;
        tabButton.Selected();
        tabButton.backgroundImage.sprite = buttonActive;
        int index=tabButton.transform.GetSiblingIndex();
        for(int i=0;i<tabPages.Count;i++)
        {
            if (i == index)
            {
                tabPages[i].gameObject.SetActive(true);
            }
            else
            {
                tabPages[i].gameObject.SetActive(false);
            }
        }

    }
    public void OnTabExit(TabBtn tabButton)
    {
        ResetButtonBackgroundImage();
    }
    public void ResetButtonBackgroundImage()
    {
        foreach(TabBtn tabButton in tabButtons)
        {
            if (!selectedButton|| selectedButton ==tabButton) { continue; }
            tabButton.backgroundImage.sprite = buttonIdle;
        }
    }
}

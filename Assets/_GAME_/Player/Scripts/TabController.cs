using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;  

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ActivateTab(int tabNo)
    {
        for(int i = 0; i < pages.Length; i++)
        {
           pages[i].SetActive(false);
            tabImages[i].color = Color.gray;
        }
        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

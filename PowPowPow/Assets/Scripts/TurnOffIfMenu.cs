using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnOffIfMenu : MonoBehaviour
{
    GameObject[] sliders;

    // Start is called before the first frame update
    void Start()
    {
        sliders = GameObject.FindGameObjectsWithTag("Slider");
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                if (sliders[i].name == "SuckSlider")
                {
                    sliders[i].GetComponentInChildren<Image>().enabled = false;
                }
                else if (sliders[i].name == "DashSlider")
                {
                    sliders[i].GetComponentInChildren<Image>().enabled = false;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //stateOfSliders();
    }

    void stateOfSliders()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                if (sliders[i].name == "SuckSlider")
                {
                    sliders[i].GetComponentInChildren<Image>().enabled = false;
                }
                else if (sliders[i].name == "DashSlider")
                {
                    sliders[i].GetComponentInChildren<Image>().enabled = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                if (sliders[i].name == "SuckSlider")
                {
                    sliders[i].GetComponentInChildren<Image>().enabled = true;
                }
                else if (sliders[i].name == "DashSlider")
                {
                    sliders[i].GetComponentInChildren<Image>().enabled = true;
                }
            }
        }
    }
}

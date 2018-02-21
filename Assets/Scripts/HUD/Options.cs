using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class Options : MonoBehaviour {

    public Slider mainSlider;
    public int Size_Aspect;
    public int height;
    public int width;

    //Invoked when a submit button is clicked.
    public string DisplaySize () {
        //Displays the value of the slider in the console.
        Size_Aspect = (int)mainSlider.value;
        Debug.Log(Size_Aspect);

        switch (Size_Aspect) {
            case 0:
                width = 960;
                height = 540;
                break;

            case 1:
                width = 1024;
                height = 576;
                break;

            case 2:
                width = 1280;
                height = 720;
                break;

            case 3:
                width = 1600;
                height = 768;
                break;

            case 4:
                width = 1440;
                height = 1080;
                break;

            case 5:
                width = 1600;
                height = 1024;
                break;

            case 6:
                width = 1680;
                height = 1050;
                break;

            case 7:
                width = 1920;
                height = 1080;
                break;
        }

        string my_string = width.ToString() + " x " + height.ToString();
        return my_string;
    }
    
    public void SubmitSliderSetting() {
        Screen.SetResolution(width, height, true, 60);
    }
}

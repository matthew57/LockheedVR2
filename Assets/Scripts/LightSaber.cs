// ===================== DOCUMENTATION ======================= //
/* Drag this script onto either one of the controllers. This tool allows you
 to whip out a lightsaber and cut through your assembly (it acts like a 
 glorified eraser). Simply drag the LightSaber gameobject into the Light
 Saber slot on the script. The assigned button is clicked to turn it on
 and off. Enjoy the cool sound every time you whip out this cool tool. */

using UnityEngine;
using System.Collections;

public class LightSaber : Tools {

    public GameObject lightSaber;
    public enum state { on , off };
    public state lightSaberState = state.off;

    //protected override void buttonPressEvent(object sender, ClickedEventArgs e)
    //{
    //    if (lightSaberState == state.off)
    //    {
    //        lightSaber.SetActive(true);
    //        lightSaber.transform.parent = this.transform;
    //        lightSaber.transform.localPosition = new Vector3(0, -0.01f, 0.35f);
    //        lightSaber.GetComponent<AudioSource>().enabled = true;
    //        lightSaberState = state.on;
    //    }
    //    else if (lightSaberState == state.on)
    //    {
    //        lightSaber.SetActive(false);
    //        lightSaberState = state.off;
    //    }
    //}

    void Update()
    {
        if (lightSaberState == state.on)
        {
            lightSaber.transform.up = this.transform.forward;
        }
    }
}

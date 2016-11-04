// ===================== DOCUMENTATION ======================= //
/* Drag this script onto either one of the controllers. This tool allows 
 * you to measure the distance between two points. Instructions are these:
 * Drag StartSphere gameobject into Startsphere slot on script
 * Drag EndSphere gameobject into Endsphere slot on script
 * Drag Line Render gameobject into Line Renderer slot on script
 * Drag GrabSphere from the controller into Grab Sphere slot on script
 * Drag DistanceBox gameobject into Distancebox slot on script
 * First click of assigned button - creates starting point of line
 * Second click of assigned button - creates ending point of line
 * Third click of assigned button - erases line to reset
 * You can hold down on the second click and watch the line move with 
 * your remote, constantly updating the length text box. The line
 * measurement is in inches. The GrabSphere ball on the controller is 
 * where the starting and ending points will be placed in the scene on click.*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class Measurement : Tools
{
    public GameObject measurementPrefab;
    private GameObject measurement;
    private GameObject startsphere;
    private GameObject endsphere;
    private GameObject measureLine;

    private float dist;
    private string dist_string;
    private GameObject grabSphere;
    private GameObject camera_eyes;

    //DISTANCE TEXT UPDATER//
    TextMesh text;
    private GameObject distancebox;

    public enum state { idle, start, end };
    public state measureState = state.idle;

    public button measureButton;
    private ButtonSubHandler measure;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        measure.clicked = measureClicked;
        measure.unclicked = null;
        buttonMap.Add(measureButton, measure);

        grabSphere = controllerInit.gameObject.transform.Find("GrabSphere").gameObject;
        camera_eyes = GameObject.Find("Camera (eye)");
    }

    public override void unsub()
    {
        base.unsub();
        if (measurement)
        {
            measureState = state.idle;
            Destroy(measurement);
        }
    }

    IEnumerator measureCoroutine()
    {
        startsphere.transform.position = grabSphere.transform.position;

        while (measureState != state.idle)
        {
            if (measureState == state.start)
            {
                
                measureLine.GetComponent<LineRenderer>().SetPosition(0, startsphere.transform.position);
                measureLine.GetComponent<LineRenderer>().SetPosition(1, grabSphere.transform.position);

                distancebox.transform.position = ((startsphere.transform.position + grabSphere.transform.position) / 2);
                distancebox.transform.LookAt(camera_eyes.transform.position);
                distancebox.transform.Rotate(0, 180, 0);

                dist = Vector3.Distance(startsphere.transform.position, grabSphere.transform.position);
                dist = dist * 39.3701f;
                dist_string = dist.ToString("0.00");
                text.text = "Distance: " + dist_string + " inches";
            }
            else if (measureState == state.end)
            {
                distancebox.transform.LookAt(camera_eyes.transform.position);
                distancebox.transform.Rotate(0, 180, 0);
            }
            yield return null;
        }
        yield return null;
    }

    protected void measureClicked(object sender, ClickedEventArgs e)
    {
        if (measureState == state.idle)
        {
            measureState = state.start;
            measurement = Instantiate(measurementPrefab);

            foreach (var child in measurement.GetComponentsInChildren<Transform>())
            {
                switch (child.gameObject.name)
                {
                    case ("StartSphere"):
                        startsphere = child.gameObject;                       
                        break;
                    case ("EndSphere"):
                        endsphere = child.gameObject;
                        child.GetComponent<MeshRenderer>().enabled = false;
                        break;
                    case ("MeasureLine"):
                        measureLine = child.gameObject;
                        break;
                    case ("DistanceBox"):
                        distancebox = child.gameObject;
                        text = distancebox.GetComponent("TextMesh") as TextMesh;
                        break;
                    default:
                        break;
                }
            }
            StartCoroutine("measureCoroutine");     
        }
        else if (measureState == state.start)
        {
            measureState = state.end;

            endsphere.GetComponent<MeshRenderer>().enabled = true;
            endsphere.transform.position = grabSphere.transform.position;

            measureLine.GetComponent<LineRenderer>().SetPosition(1, endsphere.transform.position);

            distancebox.transform.position = ((startsphere.transform.position + endsphere.transform.position) / 2);
            dist = Vector3.Distance(startsphere.transform.position, endsphere.transform.position);
            dist = dist * 39.3701f;
            dist_string = dist.ToString("0.00");
            text.text = "Distance: " + dist_string + " inches";
        }
        else if (measureState == state.end)
        {
            Destroy(measurement);
            measureState = state.idle;
        }
    }

}




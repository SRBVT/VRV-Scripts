using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Globalization;
using UnityEngine;

public class Tracing : MonoBehaviour
{
    public GameObject objective; //Things required to be inputed from the editor
    public int range, trialDuration; //Max range of ray, then amount of trials
    public Texture crosshair; 
    public Vector2 boxSize = new Vector2(0,0); //Size of the crosshair

    public string path = "Assets/Resources/"; //Where should the test results go?
    public string testResultsFileName = "TestResults1.txt";

    private System.Random ran = new System.Random();
    private bool lastUpdate = false; //This is a condition for if the last frame had the ray contacting the ball
    private int iter = 0;
    private long lastTime = 0;

    private string timeTrials ;
    private long lostTime, timeStart;
    // Start is called before the first frame update
    void Start()
    {
        path += testResultsFileName;
        lostTime = System.DateTime.Now.ToFileTimeUtc();
        timeStart = lostTime;
        trialDuration = trialDuration * (int)Mathf.Pow(10, 7);
    }

    // I need to do: 
    // Always moving, until it hits a range limit (do that by checking x, y, and z repeatedly (make sure to use >= not ==))
    // When it hits the wall, regenerate the direction it's going in, but with opposite the direction it contacted

    // Update is called once per frame
    void Update()
    {
        if (System.DateTime.Now.ToFileTimeUtc() - timeStart <= trialDuration){ //if the current time is less than the duration
            RaycastHit hit;
            Vector3 CameraCenter = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, GetComponent<Camera>().nearClipPlane)); //Ray placement

            if (Physics.Raycast(CameraCenter, this.transform.forward, out hit, range))  // Does the ray hit something at all? 
            {
                var obj = hit.transform.gameObject; //What did it hit?
                if (objective != null && obj.name == objective.name) //Is that the same object as what we want? This only checks for name
                {
                    objective.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                    if (!lastUpdate) //is this a first contact?
                    { 
                        Debug.Log("First Contact");
                        iter++; //iterate touch counter
                        Debug.Log("This is the contact number " + iter);
                        Debug.Log("it took: " + ((System.DateTime.Now.ToFileTimeUtc()- lostTime) * Mathf.Pow(10,-7)) +" seconds to find it"); //Report how long it took
                    }
                    
                    lastUpdate = true;
                    
                }
                else //Did not hit the right object
                {
                    if (lastUpdate) //Did it hit something last frame? Remember, it also didn't hit anything this frame
                    {
                        lostTime = System.DateTime.Now.ToFileTimeUtc(); //Record what  time they lost it
                    }
                    objective.GetComponent<Renderer>().material.SetColor("_Color", Color.red); //change color to red
                    lastUpdate = false; // Set it to where it didn't contact anything on this frame
                }
            }
            else //The ray did not hit anything (Rare case, but applicable when range is small)
            {
                if (lastUpdate) //Did it hit something last frame? Remember, it also didn't hit anything this frame
                {
                    lostTime = System.DateTime.Now.ToFileTimeUtc();
                }
                objective.GetComponent<Renderer>().material.SetColor("_Color", Color.red); //change color to red 
                lastUpdate = false; //Set it to where it didn't contact anything on this frame
            }
        }
        else{ //"all trials has ended" condition
            Debug.Log("trial over");
            objective.SetActive(false);

            //Re-import the file to update the reference in the editor
            //AssetDatabase.ImportAsset(path); 
            //TextAsset asset = Resources.Load(timeTrials); //Puts it into a text file

            //Print the text from the file
            //Debug.Log(asset.text);
        }
    }

    private void OnGUI() //Dumb gui code to create the crosshairs
    {
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(new Vector2(Screen.width / 2 - boxSize.x / 2, Screen.height / 2 - boxSize.y / 2), boxSize);
        GUI.Box(rect, crosshair, style);
        style.border.Remove(rect);
    }

    private long timeTaken()
    {
        long TimeElapsed = System.DateTime.Now.ToFileTimeUtc() - lastTime;//record time
        Debug.Log("Time Elapsed: " + TimeElapsed + "\n Trial Number: " + iter);
        timeTrials += "Trial Number: " + iter + "\n" + "Time Elapsed: " + TimeElapsed + "\n\n";
        lastTime = System.DateTime.Now.ToFileTimeUtc();
        return TimeElapsed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Globalization;
using UnityEngine;

public class BallMover : MonoBehaviour
{
    public Vector3 minRange, maxRange; //For establishing max range of ball

    public GameObject objective; //Things required to be inputed from the editor
    public int range, trialCount; //Max range of ray, then amount of trials
    public Texture crosshair; 
    public Vector2 boxSize = new Vector2(0,0); //Size of the crosshair

    public string path = "Assets/Resources/"; //Where should the test results go?
    public string testResultsFileName = "TestResults1.txt";

    private System.Random ran = new System.Random();
    private bool lastUpdate = false; //This is a condition for if the last frame had the ray contacting the ball
    private int iter = 0;
    private long lastTime = 0;

    private string timeTrials ;
    // Start is called before the first frame update
    void Start()
    {
        path += testResultsFileName;
    }

    // I need to do: 
    // Always moving, until it hits a range limit (do that by checking x, y, and z repeatedly (make sure to use >= not ==))
    // When it hits the wall, regenerate the direction it's going in, but with opposite the direction it contacted
    // 1. If contact, and last update, then keep green color
    // 2. If contact, and no last update, then record "TouchTime"
    // 3. If no contact, and last update, then turn red, change last update, record the time from TouchTime to now, as well as the iteration of lost contacts
    // 4. If no contact, and no last update, then turn red, move on

    // Update is called once per frame
    void Update()
    {
        Vector3 randVect = new Vector3((float)((-minRange + maxRange).x * ran.NextDouble()+minRange.x), 
            (float)((-minRange + maxRange).y * ran.NextDouble() + minRange.y), 
            (float)((-minRange + maxRange).z * ran.NextDouble()) + minRange.z); //Used to determine where the ball should be teleported to, new each frame

        if (iter <= trialCount){ //Is this one of trials or is this experiment over?
            RaycastHit hit;
            Vector3 CameraCenter = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, GetComponent<Camera>().nearClipPlane)); //Ray placement

            if (Physics.Raycast(CameraCenter, this.transform.forward, out hit, range))  // Does the ray hit something at all? 
            {
                var obj = hit.transform.gameObject; //What did it hit?
                if (objective != null && obj.name == objective.name) //Is that the same object as what we want? This only checks for name
                {
                    Debug.Log("Hit!");
                    objective.GetComponent<Renderer>().material.SetColor("_Color", Color.green); 
                    lastUpdate = true;
                    
                }
                else //Did not hit the right object
                {
                    if (lastUpdate) //Did it hit something last frame? Remember, it also didn't hit anything this frame
                    {
                        //if so{
                        objective.transform.localPosition = randVect;//Teleport ball

                        long TimeElapsed = System.DateTime.Now.ToFileTimeUtc() - lastTime; //Record time
                        Debug.Log("Time Elapsed: " + TimeElapsed+ "\n Trial Number: " + iter);
                        timeTrials += "Trial Number: " + iter + "\n" + "Time Elapsed: " + TimeElapsed + "\n\n" ;
                        lastTime = System.DateTime.Now.ToFileTimeUtc();

                        iter++; //iterate the amount of trials }

                        //Notably, after the first time it contacts, the number it reads is quite literally file UTC time, 
                        //  which is a long number, because time in real life hasn't stopped in a while :p
                    }
                    objective.GetComponent<Renderer>().material.SetColor("_Color", Color.red); //change color to red
                    lastUpdate = false; // Set it to where it didn't contact anything on this frame
                }
            }
            else //The ray did not hit anything (Rare case, but applicable when range is small)
            {
                if (lastUpdate) //Did it hit something last frame? Remember, it also didn't hit anything this frame
                {
                    //If so {
                    objective.transform.localPosition = randVect;  //teleport ball 
                    
                    long TimeElapsed = System.DateTime.Now.ToFileTimeUtc() - lastTime;//record time
                    Debug.Log("Time Elapsed: " + TimeElapsed + "\n Trial Number: " + iter);
                    timeTrials += "Trial Number: " + iter + "\n" + "Time Elapsed: " + TimeElapsed + "\n\n";
                    lastTime = System.DateTime.Now.ToFileTimeUtc();
                    
                    iter++; //iterate the amount of trials }
                }
                objective.GetComponent<Renderer>().material.SetColor("_Color", Color.red); //change color to red 
                lastUpdate = false; //Set it to where it didn't contact anything on this frame
            }
        }
        else{ //"all trials has ended" condition
            Debug.Log("trial over");
            objective.transform.localPosition = new Vector3(0, -5, 0);
            
            //Re-import the file to update the reference in the editor
            AssetDatabase.ImportAsset(path); 
            TextAsset asset = Resources.Load(timeTrials); //Puts it into a text file

            //Print the text from the file
            Debug.Log(asset.text);
        }
    }

    private void OnGUI() //Dumb gui code to create the crosshairs
    {
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(new Vector2(Screen.width / 2 - boxSize.x / 2, Screen.height / 2 - boxSize.y / 2), boxSize);
        GUI.Box(rect, crosshair, style);
        style.border.Remove(rect);
    }
}

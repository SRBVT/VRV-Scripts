using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveMovement : MonoBehaviour
{
    public float speed;
    public int timeTillSwitch;
    public Vector3 max, min;

    private long lastSwitch = 0;
    private bool left = false;
    // Start is called before the first frame update
    void Start()
    {
        timeTillSwitch = timeTillSwitch * (int)Mathf.Pow(10, 7);
        Debug.Log(timeTillSwitch);
        bool left = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x >= max.x || transform.position.x <= min.x)
        {
            if(transform.position.x == max.x)
            {
                transform.Translate(speed*Vector3.right);
                left = true;

                Debug.Log("out of range max");
            }
            else
            {
                transform.Translate(speed*Vector3.left);
                left = false;

                Debug.Log("out of range");
            }
        }
        else {
            if (left){

                transform.Translate(speed * Vector3.left);
            }
            else
            {

                transform.Translate(speed * Vector3.right);
            }
        }
        //3D.MovePosition(new Vector3(100, 100, 100));
        /*if (timeTillSwitch <= System.DateTime.Now.ToFileTimeUtc() - lastSwitch ) { 
            lastSwitch = System.DateTime.Now.ToFileTimeUtc();
            left = !left;
            rb3D.ResetInertiaTensor();
            Debug.Log("reset");
        }
        if (left)
        {
            rb3D.AddForce(Vector3.left * speed);
            //rb3D.transform.Translate()
            Debug.Log("left");
        }
        else
        {
            rb3D.ResetInertiaTensor();
            Debug.Log("right");
        }
        */
    }
}

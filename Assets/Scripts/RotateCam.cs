using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOrotation : MonoBehaviour
{
    private float camRotation;
    public GameObject viewAngle;

    // Start is called before the first frame update
    void Start()
    {
        camRotation = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (camRotation == 1)
        { 
            transform.Rotate(2 * Time.deltaTime, 0, 0);
            Debug.Log("heres the number___" + (2 * Time.deltaTime));
            camRotation = 2;
        }
        
        

    }
}

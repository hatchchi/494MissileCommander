using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAngle : MonoBehaviour
{
    private float camRotation;
    public GameObject viewAngle;
    


    // Start is called before the first frame update
    void Start()
    {
        camRotation = (viewAngle.transform.localRotation.eulerAngles.x);


    }

    // Update is called once per frame
    void Update()
    {

        while (camRotation >= -420)
        {
            //transform.Rotate(50 * Time.deltaTime, 0, 0);
            viewAngle.transform.Rotate(0.5f * Time.deltaTime, 0, 0);
            Debug.Log("heres the number___" + viewAngle.transform.localRotation.eulerAngles.x);
            camRotation --;
       }



    }
}

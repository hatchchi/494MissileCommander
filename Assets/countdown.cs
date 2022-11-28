using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class countdown : MonoBehaviour
{
    public float timeLeft; //500 = 5 seconds
    
    void Start()
    {
        while (timeLeft > 0)
        {

            Debug.Log("counting down in coundown script: " + timeLeft);
            timeLeft -= Time.deltaTime;
        }
        Debug.Log("completed countdown: " + timeLeft);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}

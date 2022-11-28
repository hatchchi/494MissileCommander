using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    private GameManager gameManager;
    public AudioSource splash;


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        splash = GameObject.Find("Splash2").GetComponent<AudioSource> ();
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameManager.CheckHit(collision.gameObject);
        Destroy(gameObject);
        splash.Play();
        //Debug.Log("attempted enemy splash sound");
    }
}



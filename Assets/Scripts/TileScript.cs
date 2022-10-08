
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class TileScript : MonoBehaviour
{

    
    GameManager gameManager;
    Ray ray;
    RaycastHit hit;
    //vid 2-13.57
    private bool missileHit = false;
    Color32[] hitColor = new Color32[2];
    
    void Start()
    {
        

        //vid 2-16.39
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hitColor[0] = gameObject.GetComponent<MeshRenderer>().material.color;
        hitColor[1] = gameObject.GetComponent<MeshRenderer>().material.color;
    }






    void Update()
    {


        //vid 2-16.57
        // define ray, as from cmaeras perspective, input the mouse position assigining it to an xy point (mousePosition)
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // to pay attention only to correct tile
        //Debug.Log("ray 1 ______________________________" + ray); // provides 3 coordinates of mouse for Origin and Direction, eg (10.94, 19.64, 2.32), (-0.83, -0.54, -0.11) or when off screen: (10.94, 19.64, 2.32) and (0.68, -0.64, -0.36)
                                                                 // before first click:  (10.04, 19.68, 2.44), Dir: (-0.80, -0.58, -0.16)
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log("ray ____________________" + ray); //when clicked
            
            //check its the tile that was selected through the mouse
            //is mouse button down AND is the name of what we just collided (with ray) the same as this item (gameObject.name)
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.name == gameObject.name)
            {

                Debug.Log("_____(from TileScript)_________you just pressed " + gameObject.name); //"Tile (32)"  this updates first and sends to game manager


                if (missileHit == false)
                //if square not already used?
                {

                    //check collider name  (vid 3-12.37)
                    //pass to the game manager, the hit object, the tile that was just clicked. 
                    //therefore this is where it reports which tile was chosen (and not whether theres a ship there yet)
                    //Debug.Log("hit.collider.gameObject ____________________" + hit.collider.gameObject.name); //"Tile (32)"
                    gameManager.TileClicked(hit.collider.gameObject);

                }
            }
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Missile"))
        {
            missileHit = true;
        }
        else if (collision.gameObject.CompareTag("EnemyMissile"))
        {
            hitColor[0] = new Color32(38, 57, 76, 255);
            GetComponent<Renderer>().material.color = hitColor[0];
        }
                
    }

    public void SetTileColor(int index, Color32 color)
    {
        hitColor[index] = color;
    }

    public void SwitchColors(int colorIndex)
    {
        GetComponent<Renderer>().material.color = hitColor[colorIndex];
    }
}

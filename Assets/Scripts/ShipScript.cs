using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{

    public float xOffset = 0;
    public float zOffset = 0;
    private float nextZRotation = 90f;
    private GameObject clickedTile;
    int hitCount = 0;
    public int shipSize;
    Ray ray;
    RaycastHit hit;
    //private bool missileHit = false;


    private Material[] allMaterials;

    public List<GameObject> touchTiles = new List<GameObject>();
    List<Color> allColors = new List<Color>();

    private void Start()
    {
        allMaterials = GetComponent<Renderer>().materials;
        for (int i = 0; i < allMaterials.Length; i++)
            allColors.Add(allMaterials[i].color);
    }


    void Update()
    {
        // define ray, as from cameras perspective, input the mouse position assigining it to an xy point (mousePosition)
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // to pay attention only to correct tile

        if (Physics.Raycast(ray, out hit))
        {

            //check it is the tile that was selected through the mouse
            //is clik AND is the name of what we just collided (with ray) the same as this item (gameObject.name)

            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.tag == "Ship")
            {
                Debug.Log("hit.collider.gameObject.name ____________________" + hit.collider.gameObject.name); //gives name of object being tapped
                //hit.collider.gameObject;
                //Debug.Log(" currentShip _____" + currentShip); //Cuiser v7 (gameobject)
                FlashColor(Color.yellow);




            }

        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            touchTiles.Add(collision.gameObject);
        }
    }

    public void ClearTileList()
    {
        touchTiles.Clear();
    }


    public Vector3 GetOffsetVec(Vector3 tilePos)
    {
        //return new Vector3(tilePos.x + xOffset, 20, tilePos.z + zOffset);
        //Debug.Log("_____________(shipscript)___________________________________tilePos value is_" + tilePos);
        // return new Vector3(tilePos.x - 62 + xOffset, 20, tilePos.z -47 + zOffset);  //62 and 47 is how much off the ships are in the new positions
        return new Vector3(tilePos.x + xOffset, 2, tilePos.z + zOffset);  //62 and 47 is how much off the ships are in the new positions
        // y value is height ship appears before falling onto board
    }

    public void RotateShip()
    {
        //Debug.Log("rotate command_______"); 
        if (clickedTile == null) return;  //clickedtile is active tile xyz 
        touchTiles.Clear();  //Reset touched tiles count
        transform.localEulerAngles += new Vector3(0, 0, nextZRotation); //rotate by 90

        nextZRotation *= -1;  //next time will rotate -90

        //switch x offset value with z offset value
        float temp = xOffset;
        xOffset = zOffset;
        zOffset = temp;
        SetPosition(clickedTile.transform.position);
        //Debug.Log("rotate command_______");
    }

    public void ShipAppear()
    {
        gameObject.SetActive(true);
        //FlashColor(Color.yellow);

    }

    public void SetPosition(Vector3 newVec)
    {
        ClearTileList();
        //transform.localPosition = new Vector3(newVec.x -62 + xOffset, 20, newVec.z -47 + zOffset);
        transform.localPosition = new Vector3(newVec.x + xOffset, 2, newVec.z + zOffset);
        //Debug.Log("this is where Setposition() was used");
    }

    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
    }

    public bool OnGameBoard()
    {
        return touchTiles.Count == shipSize;
    }

    public bool HitCheckSank()
    {
        hitCount++;
        return shipSize <= hitCount;
    }

    public void FlashColor(Color tempColor)
    {
        foreach (Material mat in allMaterials)
        {
            mat.color = tempColor;
        }
        Invoke("ResetColor", 0.5f);


    }


    private void ResetColor()
    {
        int i = 0;
        foreach (Material mat in allMaterials)
        {
            mat.color = allColors[i++];
        }
    }
}

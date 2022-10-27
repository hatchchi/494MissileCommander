using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class GameManager : MonoBehaviour
{
    [Header("Ships")]
    public GameObject[] ships;
    public EnemyScript enemyScript;
    private ShipScript shipScript;
    private List<int[]> enemyShips;
    private int shipIndex = 0;
    public List<TileScript> allTileScripts;

    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public Text topText;
    public Text playerShipText;
    public Text enemyShipText;

    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject enemyMissilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;
    //
    public GameObject tile;
    public string tileName;

    private bool setupComplete = false;
    private bool playerTurn = true;

    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();

    private int enemyShipCount = 5;
    private int playerShipCount = 5;

    // VOICE  //////////////////////////////////
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    //////////////////////////////////


    // Start is called before the first frame update
    void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
        replayBtn.onClick.AddListener(() => ReplayClicked());
        enemyShips = enemyScript.PlaceEnemyShips();

    


        /*
        // VOICE
        actions.Add("forward", Forward); //worked
       // actions.Add("bee", Forward);//worked on low
      //  actions.Add("cee", Forward);//worked on low
      //  actions.Add("dee", Forward); //worked on low
      //  actions.Add("ee", Forward); //failed on low
      //  actions.Add("ef", Forward); //worked on low
      //  actions.Add("gee", Forward); //worked
      //  actions.Add("aych", Forward); //worked on low
      //  actions.Add("eye", Forward); //worked on low
      //  actions.Add("B3", Forward); //worked
      //  actions.Add("D 4", Forward); //nope
        actions.Add("Deefor", Forward); //worked on low
        actions.Add("I 2", Forward); //worked on low
        actions.Add("shoot at J2", Forward); //worked
        actions.Add("fire at G3", Forward); //nope
        //actions.Add("fire at D4", Forward); //nope
         // actions.Add("kill D 4", Forward); //nope
        actions.Add("drop at B3", Forward); //worked

        ////https://docs.unity3d.com/ScriptReference/Windows.Speech.KeywordRecognizer.html

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray(), ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();

        //////////////////////////////////
     */
    }
    /*
    // VOICE //////////////////////////////////
    
    public void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    public void Forward()
    {
        if (setupComplete && playerTurn)
        {
           
            Vector3 tilePos = tile.transform.position;
            
            // 2.25 per tile in each direction. 
            //set new bomb starting position.
            tilePos.y = 15 ;
            tilePos.x = 9 ;
            tilePos.z = 9 ;
            playerTurn = false;
            Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
        }
    }
    */
        //////////////////////////////////


        private void NextShipClicked()
    {
        if (!shipScript.OnGameBoard())  //if size of ship is not equal to # of tiles it touches, flash red
        {
            shipScript.FlashColor(Color.red);
        }
        else
        {
            //if (shipIndex <= ships.Length - 2)  //if not all ships positioned, flash next one yellow
                if (shipIndex <= ships.Length - 5)  //-6 to skip
                {
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScript>();
                shipScript.FlashColor(Color.yellow);
            }
            else  //otherwise go into game mode
            {
                rotateBtn.gameObject.SetActive(false);
                nextBtn.gameObject.SetActive(false);
                woodDock.SetActive(false);
                topText.text = "Guess an enemy tile.";
                setupComplete = true;
                for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
            }
        }

    }

    public void TileClicked(GameObject tile)
    {
        if (setupComplete && playerTurn)
        {
            
            Vector3 tilePos = tile.transform.position;  // tilePos is created on selected tile's xyz
            tilePos.y += 25;  
            //Debug.Log("tilePos is _______________ for missile_______" + tilePos); // (9.00, 0.58, 11.12) [for Tile 75 /E3] and (11.25, 0.58, 11.12) [for Tile 76 /F3]
            playerTurn = false;
            Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
        }
        else if (!setupComplete)  //if still positioning ships, place ship on selected tile
        {
            tileName = tile.transform.name;

            if (tileName.Contains("EnemyTile") == false)  //check chosen tile is on player board
            {
                PlaceShip(tile);
                shipScript.SetClickedTile(tile);  // script states simply:  clickedTile = tile;
                
                Debug.Log("'tile.transform.name is____________________________" + tile.transform.name);
            }
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Debug.Log("__________________shipScript.GetOffsetVec(tile.transform.position) is_____" + shipScript.GetOffsetVec(tile.transform.position)); 
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);  //sends tile's xyz to shipscript to add offsets
        Debug.Log ("________________________________________________________now neVec is_____" + newVec); //(-53.00, 20.00, -43.62) stayed at 20. = Tile 45
        ships[shipIndex].transform.localPosition = newVec; //moves ship to newVec location
    }

    void RotateClicked()
    {
        shipScript.RotateShip();
    }

    public void CheckHit(GameObject tile)
    {
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;
        foreach (int[] tileNumArray in enemyShips)
        {
            if (tileNumArray.Contains(tileNum))
            {
                for (int i = 0; i < tileNumArray.Length; i++)
                {
                    if (tileNumArray[i] == tileNum)
                    {
                        tileNumArray[i] = -5;
                        hitCount++;
                    }
                    else if (tileNumArray[i] == -5)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == tileNumArray.Length)
                {
                    enemyShipCount--;
                    topText.text = "SUNK!!!!!!";
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                else
                {
                    topText.text = "HIT!!";
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                break;
            }

        }
        if (hitCount == 0)
        {
            tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileScript>().SwitchColors(1);
            topText.text = "Missed, there is no ship there.";
        }
        Invoke("EndPlayerTurn", 1.0f);
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)
    {
        enemyScript.MissileHit(tileNum);
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        if (hitObj.GetComponent<ShipScript>().HitCheckSank())
        {
            playerShipCount--;
            playerShipText.text = playerShipCount.ToString();
            enemyScript.SunkPlayer();
        }
        Invoke("EndEnemyTurn", 2.0f);
    }

    private void EndPlayerTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
        foreach (GameObject fire in playerFires) fire.SetActive(true);
        foreach (GameObject fire in enemyFires) fire.SetActive(false);
        enemyShipText.text = enemyShipCount.ToString();
        topText.text = "Enemy's turn";
        enemyScript.NPCTurn();
        ColorAllTiles(0);
        if (playerShipCount < 1) GameOver("ENEMY WINs!!!");
    }

    public void EndEnemyTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
        foreach (GameObject fire in playerFires) fire.SetActive(false);
        foreach (GameObject fire in enemyFires) fire.SetActive(true);
        playerShipText.text = playerShipCount.ToString();
        topText.text = "Select a tile";
        playerTurn = true;
        ColorAllTiles(1);
        if (enemyShipCount < 1) GameOver("YOU WIN!!");
    }

    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileScript tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    void GameOver(string winner)
    {
        topText.text = "Game Over: " + winner;
        replayBtn.gameObject.SetActive(true);
        playerTurn = false;
    }

    void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}

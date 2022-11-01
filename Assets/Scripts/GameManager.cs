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
    public TimerScript timerScript;
  
    private List<int[]> enemyShips;
    private int shipIndex = 0;
    public List<TileScript> allTileScripts;
    

    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public Text enemyComments;
    public Text playerComments;
    public Text spokenLetters;
    public Text spokenNumbers;
    public Text playerShipText;
    public Text enemyShipText;
    public Text instructions;
    public Text fire;


    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject enemyMissilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;
    public GameObject viewAngle;
    public GameObject enemyBoard;
    public GameObject scores;
    public GameObject timer;
    public GameObject CountdownText;
    


    public float timeLeft; 
    public bool flipBoard;
    
    public Vector3 tilePosTemp;

    //
    public GameObject tile;
    public GameObject Etile;
    public GameObject targetTile;
    public string tileName;
    

    private bool setupComplete = false;
    private bool playerTurn = true;
    

    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();

    private int enemyShipCount = 5;
    private int playerShipCount = 5;
    private float angle;


    public float countdownLeft;
    public bool timerOn;
    public Text TimerText;
    public bool timerCompleted;

    public bool spokenWord;
    public string whatWasSaid;
    private string keywordKeys;
    public int tileSend;
    public int tileReceived;
    private int tileLetter;
    private int tileNumber;
    public bool yourTileHit;


    // VOICE  //////////////////////////////////
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    //////////////////////////////////


    // Start is called before the first frame update
    void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
        replayBtn.onClick.AddListener(() => ReplayClicked());
        enemyShips = enemyScript.PlaceEnemyShips();

        fire.text = " ";
        

        //keyword list



        keywords.Add("gee", () => { spokenLetters.text = "G"; tileLetter = 1; }); //__________STRONG
        keywords.Add("h", () => { spokenLetters.text = "H"; tileLetter = 2; });
        keywords.Add("jay", () => { spokenLetters.text = "J"; tileLetter = 3; }); //__________STRONG
        keywords.Add("L", () => { spokenLetters.text = "L"; tileLetter = 4; });
        keywords.Add("Q", () => { spokenLetters.text = "Q"; tileLetter = 5; });    //__________STRONG  
        keywords.Add("tea", () => { spokenLetters.text = "T"; tileLetter = 6; });// __________MED
        keywords.Add("W", () => { spokenLetters.text = "W"; tileLetter = 7; }); //__________STRONG
        keywords.Add("X", () => { spokenLetters.text = "X"; tileLetter = 8; }); //__________STRONG
        keywords.Add("why", () => { spokenLetters.text = "Y"; tileLetter = 9; }); //__________STRONG
        keywords.Add("Z", () => { spokenLetters.text = "Z"; tileLetter = 10; }); //__________STRONG if no C

        keywords.Add("one", () => { spokenNumbers.text = "1"; tileNumber = 0; playerGuidance();  });//weak
        keywords.Add("two", () => { spokenNumbers.text = "2"; tileNumber = 1; playerGuidance(); }); 
        keywords.Add("three", () => { spokenNumbers.text = "3"; tileNumber = 2; playerGuidance(); });
        keywords.Add("four", () => { spokenNumbers.text = "4"; tileNumber = 3; playerGuidance(); });
        keywords.Add("five", () => { spokenNumbers.text = "5"; tileNumber = 4; playerGuidance(); });
        keywords.Add("six", () => { spokenNumbers.text = "6"; tileNumber = 5; playerGuidance(); });
        keywords.Add("seven", () => { spokenNumbers.text = "7"; tileNumber = 6; playerGuidance(); });
        keywords.Add("eight", () => { spokenNumbers.text = "8"; tileNumber = 7; playerGuidance(); });
        keywords.Add("nine", () => { spokenNumbers.text = "9"; tileNumber = 8; playerGuidance(); });
        keywords.Add("10", () => { spokenNumbers.text = "10"; tileNumber = 9; playerGuidance(); });

        keywords.Add("fire", () =>
        {
        fire.text = "FIRE!";


        int tileSend = (tileNumber * 10) + tileLetter;
        Debug.Log("tileSend =" + tileSend);

        //tile.transform.name = "EnemyTiles";

        tileName = "ETile (" + tileSend + ")";
        //Debug.Log("EtileName: " + tileName);
        targetTile = GameObject.Find(tileName);
        //Debug.Log("targetTile is : " + targetTile);

            TileClicked(targetTile); 
        });

        ////VoiceCommand(5);
        ; 

        /*
        //keywords.Add("ey", () => { spokenLetters.text = "A"; }); //P
        //keywords.Add("B", () => { spokenLetters.text = "B"; }); //dee
        //keywords.Add("C", () => { spokenLetters.text = "C"; }); challenges Z
        //keywords.Add("dee", () => { spokenLetters.text = "D"; }); //
        //keywords.Add("e", () => { spokenLetters.text = "E"; }); //nope
        //keywords.Add("F", () => { spokenLetters.text = "F"; }); //S
        keywords.Add("gee", () => { spokenLetters.text = "G"; }); //__________STRONG
        keywords.Add("h", () => { spokenLetters.text = "H"; });
        //keywords.Add("i", () => { spokenLetters.text = "I"; }); //difficult
        keywords.Add("jay", () => { spokenLetters.text = "J"; }); //__________STRONG
        //keywords.Add("kay", () => { spokenLetters.text = "K"; }); //
        keywords.Add("L", () => { spokenLetters.text = "L"; });
        //keywords.Add("m", () => { spokenLetters.text = "M"; }); //nope
        //keywords.Add("N", () => { spokenLetters.text = "N"; }); 10
        //keywords.Add("O", () => { spokenLetters.text = "O"; });//
        //keywords.Add("P", () => { spokenLetters.text = "P"; }); 
        keywords.Add("Q", () => { spokenLetters.text = "Q"; }); //__________STRONG
        //keywords.Add("R", () => { spokenLetters.text = "R"; });
        //keywords.Add("S", () => { spokenLetters.text = "S"; }); //________borderline
        keywords.Add("tea", () => { spokenLetters.text = "T"; });// __________MED
        //keywords.Add("U", () => { spokenLetters.text = "U"; }); //________borderline
        //keywords.Add("V", () => { spokenLetters.text = "V"; }); //B or Z
        keywords.Add("W", () => { spokenLetters.text = "W"; }); //__________STRONG
        keywords.Add("X", () => { spokenLetters.text = "X"; }); //__________STRONG
        keywords.Add("why", () => { spokenLetters.text = "Y"; }); //__________STRONG
        keywords.Add("Z", () => { spokenLetters.text = "Z"; }); //__________STRONG if no C
        */

        // OLD FOMAT:  actions.Add("forward", Forward); 

        ////https://docs.unity3d.com/ScriptReference/Windows.Speech.KeywordRecognizer.html

        /////keyword recog, and what we want to recognize:
        // ConfidenceLevel.Low); /Medium / High 
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        //keywordRecognizer.Start();

        

        //////////////////////////////////

    }

    // VOICE //////////////////////////////////

    private void playerGuidance()
    {
        if (spokenNumbers.text != " ")
        {
            instructions.text = "Say 'FIRE!' when you're ready";
        }

    }

    private void sendingTileInfo()
    {
        //send variable tileSend to server

        //wait for reply with yourTileHit (true/false)
        //show 'waiting for enemy's response' on screen

        //if receive yourTileHit (whether player's misile hit or missed)
        //  if we hit enemy ship, (if yourTileHit = true),
        //      playerComments.text = "I hit your battleship!";
        //      enemyComments.text = "argh!!";
        //      startTimer(300);
        //      tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255));
        //      tile.GetComponent<TileScript>().SwitchColors(1);

        //  else yourTileHit false,
        //      tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
        //      tile.GetComponent<TileScript>().SwitchColors(1);
        //      enemyComments.text = "Missed, no ship there.";
        //      startTimer(300);

        //if no reply within 10 seconds,  
        //      go to check hit, which will compare against enemey's pre-chosen random positions)
        //      enemyScript.NPCTurn();

    }

    private void receivingTileInfo()  // where enemy is aiming
    {
        //listen for tileReceived from server
        //if receive tileReceived (enemy's guess) from server, set new var 'received' to true
        //      go to EnemyScript (which needs if statements to check for 'received' and skip random calculations with if clause -
        //      also add line in EnemyScript to go to sendMyTileStatus()

        //if no reply within 10 seconds,  go to EnemyScript anyway run AI emeny (using random shots)
    }

    private void sendMyTileStatus(bool yourTileHit)  // whether enemy hit my tile
    {
        //send variable yourTileHit to server
        // (nothing else, it'll continue game)
    }



        /*
        private void ConvertSpeechToTile ()  //now integrated into 'fire' actions above
        {

            Debug.Log("tileNumber =" + tileNumber);
            Debug.Log("tileLetter =" + tileLetter);
            //int tileSend = ((tileNumber - 1) * 10) + tileLetter;
            Debug.Log("tileSend =" + tileSend);

            tile.name = "tile(" + tileSend + ")";
            Debug.Log("tile.name" + tile.name);
            //tile.transform.position = new Vector3(9, 15, 24.5f);  //uses tile gameobject as carrier of cordinates

        }
        */




        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs speech)
    {
        System.Action keywordAction;
        // if the keyword recognized is in our dictionary, call that Action.
        if (keywords.TryGetValue(speech.text, out keywordAction))
        {
            keywordAction.Invoke();
            Debug.Log("recognized speech 3 : " + speech.text); //best
            playerComments.text = " ";
        }
    }

   
    public void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log("recognized speech in other void : " + speech.text);
        keywords[speech.text].Invoke();
    }
    

    public void VoiceCommand(int tileNum)  //testbed for voice commands
    {
        /*
         * if (setupComplete && playerTurn)
        {
           
             Vector3 tilePos = tile.transform.position;
            
            // 2.25 per tile in each direction. 
            //set new bomb starting position.
            tilePos.y = 15 ;
            tilePos.x = tileNum ;
            tilePos.z = 25 ;
            playerTurn = false;
            
            

            Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
        }
        */
    }
    
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
            if (shipIndex <= ships.Length - 2)  //-6 to skip
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
                //timerCompleted = false;

                //pivot view around and show enemy board
                viewAngle.transform.Rotate(-42, 0, 0);
                //playerShipText.gameObject.SetActive(true);

                instructions.text = "Where would you like to fire a missile?";
                keywordRecognizer.Start();
                setupComplete = true;
                //for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);

                enemyBoard.gameObject.SetActive(true);
                scores.gameObject.SetActive(true);
            }
        }
    }
    


    
    void Update()
    {
        
        /*
        if (timerOn)
        {
            TimerText.text = " ";

            if (countdownLeft > 0)
            {
                countdownLeft -= Time.deltaTime;
                updateTimer(countdownLeft);
                
            }
            else
            {
                //time ran out, go ahead and fire missile
                countdownLeft = 0;
                timerOn = false;
                timerCompleted = true;
                TimerText.text = " ";

                //CountdownText.gameObject.SetActive(false);
                Instantiate(missilePrefab, tilePosTemp, missilePrefab.transform.rotation);
            }
        }

        */
        

    }

   

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerText.text = string.Format("{0}", seconds);

    }


    public void TileClicked(GameObject tile) //tile came from hit.collider.gameObject[.name] in TileScript OR from start, above
    {
        if (setupComplete && playerTurn)
        {
            Debug.Log("tile: " + tile); //correct :Etile(99)
            Debug.Log("coords for tile.transform.position: " + tile.transform.position);
            
          
            Vector3 tilePos = tile.transform.position;  // tilePos is created on selected tile's xyz
            Debug.Log("tile clicked tilePos" + tilePos);  //for ALL is 8,2.4,24.7
            
            tilePos.y += 10;  // player missile starting height
            float fraction = 0.5f;
            tilePos.z -= fraction;
            //tilePosTemp = tilePos;
            Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
            playerTurn = false;
            
        }
        else if (!setupComplete)  //if still positioning ships, place ship on selected tile
        {
            tileName = tile.transform.name;

            if (tileName.Contains("EnemyTile") == false)  //check chosen tile is player's 
            {
                PlaceShip(tile);
                shipScript.SetClickedTile(tile);  // script states simply:  clickedTile = tile;
                
                //Debug.Log("'tile.transform.name is____________________________" + tile.transform.name);
            }
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        //Debug.Log("__________________shipScript.GetOffsetVec(tile.transform.position) is_____" + shipScript.GetOffsetVec(tile.transform.position)); 
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);  //sends tile's xyz to shipscript to add offsets
        //Debug.Log ("________________________________________________________now neVec is_____" + newVec); //(-53.00, 20.00, -43.62) stayed at 20. = Tile 45
        ships[shipIndex].transform.localPosition = newVec; //moves ship to newVec location
        
        
    }

    void RotateClicked()  
    {
        Debug.Log("rotate clicked_______");
        shipScript.RotateShip();
    }
    
    void startTimer(float amount)
    {
       timeLeft = amount;
       while (timeLeft > 0)
       {
            //Debug.Log("counting down: " + timeLeft);
            timeLeft -= Time.deltaTime;
        }
            Debug.Log("completed countdown: " + timeLeft);
        spokenLetters.text = " ";
        spokenNumbers.text = " ";
        instructions.text = " ";
    }



    public void CheckHit(GameObject tile)  //whether player hit enemy
    {
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;
        foreach (int[] tileNumArray in enemyShips)
        {
            if (tileNumArray.Contains(tileNum))
            {
                fire.text = " ";
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

                    playerComments.text = "Yay! I totally sank your battleship!";
                    
                    enemyComments.text = "grrrrr";
                    startTimer(300);
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                else
                {
                    playerComments.text = "I hit your battleship!";
                    
                    enemyComments.text = "argh!!";
                    startTimer(300);
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
            enemyComments.text = "Missed, no ship there.";
            startTimer(300);
        }
        Invoke("EndPlayerTurn", 1.0f);
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)  //invoked if successful hit
    {
        enemyScript.MissileHit(tileNum);  //assigns an h to the tile?
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));

        if (hitObj.GetComponent<ShipScript>().HitCheckSank())  //check if fully sunk
        {
            playerShipCount--;
            playerShipText.text = playerShipCount.ToString();
            enemyScript.SunkPlayer();
        }

        enemyComments.text = "Ha ha! I got you!!";
        startTimer(300);
        //Debug.Log("Enemy just got a hit");
        Invoke("EndEnemyTurn", 2.0f);
        
    }

    private void EndPlayerTurn() //switch rotation to 0 for enemy attack view
    {
        keywordRecognizer.Stop();
        Debug.Log("____just started enemy turn");
        //for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
        foreach (GameObject fire in playerFires) fire.SetActive(true);
        //foreach (GameObject fire in enemyFires) fire.SetActive(false);
        enemyShipText.text = enemyShipCount.ToString();
        startTimer(300);
        fire.text = " ";
        playerComments.text = " ";

        enemyComments.text = "OK, my turn now...";
        startTimer(300);
        
        //Debug.Log("timerOn started");
        
        enemyScript.NPCTurn();
        //ColorAllTiles(0);
        if (playerShipCount < 1) GameOver("Ha ha, I win!!!");
    }

    public void EndEnemyTurn()  //switch rotation to 30 for player attack view
    {
        Debug.Log("____just starting player's turn");
        
        //for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
        //foreach (GameObject fire in playerFires) fire.SetActive(false);

        foreach (GameObject fire in enemyFires) fire.SetActive(true);
        playerShipText.text = playerShipCount.ToString();
        enemyComments.text = " ";
        instructions.text = "OK, where now, Commander?";

        playerTurn = true;
        keywordRecognizer.Start();
        //ColorAllTiles(1);
        if (enemyShipCount < 1) GameOver("Oh no, you won the game!!!");
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
        startTimer(300);
        enemyComments.text = "I guess that's the end for me then!" + winner;
        replayBtn.gameObject.SetActive(true);
        playerTurn = false;
    }

    void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}

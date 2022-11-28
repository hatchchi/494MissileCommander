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
    private bool firstTime;
    

    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public Button commenceBtn;
    public Button joinBtn;
    public Button declineBtn;
    public Text enemyComments;
    public Text playerComments;
    public Text spokenLetters;
    public Text spokenNumbers;
    public Text playerShipText;
    public Text enemyShipText;
    public Text instructions;
    public Text subInstructions;
    public Text subSubInstructions;
    public Text upperInstructions;
    public Text fire;
    public Text openingInstructions;
    public Button playAIBtn;
    public Button playOnlineBtn;
    public Button quitBtn;
    public GameObject gameCodeBox;
    public Text connecting;
    

    private Text gameCodeText;
    private Text receivedCode;

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
    public GameObject myTiles;
    public GameObject Etile;
    public GameObject targetTile;
    public string tileName;
    

    private bool setupComplete = false;
    private bool playerTurn = true;
    private bool onlineGame = false;
    

    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();

    private int enemyShipCount = 5;
    private int playerShipCount = 5;
    private float angle;

    //private bool onlinePlay;
    //private bool aiPlay;


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
    private bool firstShip;

    public AudioSource blast;
    public AudioSource splash;
    public AudioSource blah;
    public AudioSource grrr;
    public AudioSource argh;
    public AudioSource miss;
    public AudioSource igot;
    public AudioSource okmy;
    public AudioSource iwon;
    public AudioSource igue;
    public AudioSource plea;
    public AudioSource youc;
    public AudioSource wher;
    public AudioSource sayf;
    public AudioSource yayy;
    public AudioSource youh;
    public AudioSource okwh;
    public AudioSource ohno;
    public AudioSource shoo;
    public AudioSource nows;
    public AudioSource pres;

    
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Start()
    {
        //onlinePlay = false;
       // aiPlay = false;
        firstTime = true;
        firstShip = true;

        //activate opening buttons
        playAIBtn.onClick.AddListener(() => { onlineGame = false; startGame(); });
        playOnlineBtn.onClick.AddListener(() => chooseOnlineGame()); 
        quitBtn.onClick.AddListener(() => quitGame());
        
        voiceSetup();
    }

    private void chooseOnlineGame()
    {
        openingInstructions.text = "Game found.... would you like to start?"; 
        
        playAIBtn.gameObject.SetActive(false);
        playOnlineBtn.gameObject.SetActive(false);
        joinBtn.gameObject.SetActive(true);
        declineBtn.gameObject.SetActive(true);
        
        joinBtn.onClick.AddListener(() => { onlineGame = true; startGame(); });
        declineBtn.onClick.AddListener(() =>
        {
            playAIBtn.gameObject.SetActive(true);
            playOnlineBtn.gameObject.SetActive(true);
            joinBtn.gameObject.SetActive(false);
            declineBtn.gameObject.SetActive(false);

            openingInstructions.text = "Please choose an option to start the game";
        } );
    }

    private void startGame()
    {
        openingInstructions.text = " "; 
        playAIBtn.gameObject.SetActive(false);
        playOnlineBtn.gameObject.SetActive(false);
        joinBtn.gameObject.SetActive(false);
        declineBtn.gameObject.SetActive(false);

        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
        commenceBtn.onClick.AddListener(() => NextShipClicked());
        
        woodDock.SetActive(true);
        myTiles.SetActive(true);

        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ShipAppear();

        if(onlineGame == false)
        {
            enemyShips = enemyScript.PlaceEnemyShips();
        }
        else
        {
            //probably don't need anything here
        }

        spokenNumbers.text = " ";
        spokenLetters.text = " ";
        openingInstructions.text = " ";
        fire.text = " ";
        instructions.text = "Tap a ship then tap where to place it"; //***************need updated voice
        
        plea.Play();
        }

    private void quitGame()
    {
        openingInstructions.text = "Are you sure you'd like to quit?  Y  /   N ";
    }

    
    private void NextShipClicked()  //when 'continue' button is pressed
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
                shipIndex++;  //select next ship
                shipScript = ships[shipIndex].GetComponent<ShipScript>();
                Debug.Log("ships[shipIndex]" + ships[shipIndex]); //Cuiser v7 (gameobject)
                Debug.Log("shipIndex _____" + shipIndex); //game object from 0 ,1,2,3,4
                //shipScript.FlashColor(Color.yellow);
                shipScript.ShipAppear();
                nextBtn.gameObject.SetActive(false);
                rotateBtn.gameObject.SetActive(false);
                subInstructions.text = " ";
                upperInstructions.text = " ";
                instructions.text = "Where do you want to place the next ship?";
            }
            else  //otherwise go into game mode (first run)
            {
                commenceBtn.gameObject.SetActive(false); 
                nextBtn.gameObject.SetActive(false);
                rotateBtn.gameObject.SetActive(false);
                woodDock.SetActive(false);
                upperInstructions.text = " ";
                //pivot view around and show enemy board
                viewAngle.transform.Rotate(-42, 0, 0);

                instructions.text = "First tell me just the letter ";
                subInstructions.text = " ";  
                subSubInstructions.text = " ";
                wher.Play(); /////////////////////////////need to update the voice  to "First tell me just the letter"
                keywordRecognizer.Start();
                setupComplete = true;
                enemyBoard.gameObject.SetActive(true);
            }
        }
    }

    public void TileClicked(GameObject tile) //tile came from hit.collider.gameObject[.name] in TileScript OR from start, above
    {
        if (setupComplete && playerTurn)
        {
            
            Vector3 tilePos = tile.transform.position;  // tilePos is created on selected tile's xyz
            Debug.Log("tile clicked tilePos" + tilePos);  

            tilePos.y += 10;  // player missile starting height
            float fraction = 0.5f;
            tilePos.z -= fraction;
            //tilePosTemp = tilePos;
            Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
            playerTurn = false;
        }

        else if (!setupComplete)  //if still positioning ships, place ship on selected tile
        {
            tileName = tile.transform.name; //sets active tilename as active tile

            if (tileName.Contains("EnemyTile") == false)  //check chosen tile is player's 
            {
                PlaceShip(tile);
                splash.PlayDelayed(1);
                shipScript.SetClickedTile(tile);  // script states simply:  clickedTile = tile;
            
                rotateBtn.gameObject.SetActive(true);
                nextBtn.gameObject.SetActive(true);
                upperInstructions.text = "Press";
                instructions.text = "            or";
                subInstructions.text = "to place next ship";

                if (shipIndex >= 4)
                {
                    commenceBtn.gameObject.SetActive(true);
                    nextBtn.gameObject.SetActive(false);
                    subInstructions.text = "...commence to the battle!";
                    subSubInstructions.text = " ";
                    //no voice needed 
                    //start game
                }
                else if (firstShip == true)
                {
                    youc.Play();
                    firstShip = false;
                }
            }
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);  //sends tile's xyz to shipscript to add offsets
        ships[shipIndex].transform.localPosition = newVec; //moves ship to newVec location
    }

    void RotateClicked()
    {
        shipScript.RotateShip();
    }

    private void playerGuidance()
    {
        if (spokenLetters.text != " ")
        {
            if (firstTime == true)
            {
                if (spokenNumbers.text != " ")
                {
                    subInstructions.text = " ";
                    //voice needed /////////////////////////////////////////////voice needed: "change a letter or number"
                    //voice edit needed /////////////////////////////////////////////voice edit to: "OR say Bombs away when ready"
                    instructions.text = "change a letter or number ....or say 'BOMBS AWAY!' when ready";
                    sayf.Play();

                    firstTime = false;
                }
                else
                {
                    instructions.text = "Now say the number";
                    nows.Play();
                    subInstructions.text = " ";
                }
            }
            else
            {
                if (spokenNumbers.text != " ")
                {
                    instructions.text = "...BOMBS AWAY?";
                    //sayf.Play();
                }
            }
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

    ////////////////////////////////////////////SPEECH

    private void voiceSetup()
    {
        keywords.Add("gee", () => { spokenLetters.text = "G"; tileLetter = 1; playerGuidance(); }); //__________STRONG
        keywords.Add("h", () => { spokenLetters.text = "H"; tileLetter = 2; playerGuidance(); });
        keywords.Add("jay", () => { spokenLetters.text = "J"; tileLetter = 3; playerGuidance(); }); //__________STRONG
        keywords.Add("L", () => { spokenLetters.text = "L"; tileLetter = 4; playerGuidance(); });
        keywords.Add("Q", () => { spokenLetters.text = "Q"; tileLetter = 5; playerGuidance(); });    //__________STRONG  
        keywords.Add("tea", () => { spokenLetters.text = "T"; tileLetter = 6; playerGuidance(); });// __________MED
        keywords.Add("D", () => { spokenLetters.text = "T"; tileLetter = 6; playerGuidance(); });// 
        keywords.Add("W", () => { spokenLetters.text = "W"; tileLetter = 7; playerGuidance(); }); //__________STRONG
        keywords.Add("X", () => { spokenLetters.text = "X"; tileLetter = 8; playerGuidance(); }); //__________STRONG
        keywords.Add("why", () => { spokenLetters.text = "Y"; tileLetter = 9; playerGuidance(); }); //__________STRONG
        keywords.Add("Z", () => { spokenLetters.text = "Z"; tileLetter = 10; playerGuidance(); }); //__________STRONG if no C

        keywords.Add("one", () => { spokenNumbers.text = "1"; tileNumber = 0; playerGuidance(); });
        keywords.Add("two", () => { spokenNumbers.text = "2"; tileNumber = 1; playerGuidance(); });
        keywords.Add("three", () => { spokenNumbers.text = "3"; tileNumber = 2; playerGuidance(); });
        keywords.Add("four", () => { spokenNumbers.text = "4"; tileNumber = 3; playerGuidance(); });
        keywords.Add("five", () => { spokenNumbers.text = "5"; tileNumber = 4; playerGuidance(); });
        keywords.Add("six", () => { spokenNumbers.text = "6"; tileNumber = 5; playerGuidance(); });
        keywords.Add("seven", () => { spokenNumbers.text = "7"; tileNumber = 6; playerGuidance(); });
        keywords.Add("eight", () => { spokenNumbers.text = "8"; tileNumber = 7; playerGuidance(); });
        keywords.Add("nine", () => { spokenNumbers.text = "9"; tileNumber = 8; playerGuidance(); });
        keywords.Add("10", () => { spokenNumbers.text = "10"; tileNumber = 9; playerGuidance(); });

        keywords.Add("bombsaway", () =>
        {
            fire.text = "BOMBS AWAY!";
            instructions.text = " ";
            int tileSend = (tileNumber * 10) + tileLetter;
            Debug.Log("tileSend =" + tileSend);
            tileName = "ETile (" + tileSend + ")";
            targetTile = GameObject.Find(tileName);
            TileClicked(targetTile);
            keywordRecognizer.Stop();
        });

        ////https://docs.unity3d.com/ScriptReference/Windows.Speech.KeywordRecognizer.html
        // ConfidenceLevel.Low); /Medium / High 
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs speech)
    {
        System.Action keywordAction;
        
        // if the keyword recognized is in our dictionary, call that Action.
        if (keywords.TryGetValue(speech.text, out keywordAction))
        {
            keywordAction.Invoke();
            //Debug.Log("recognized speech 3 : " + speech.text); //best
            playerComments.text = " ";
        }
    }

    public void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        //Debug.Log("recognized speech in other void : " + speech.text);
        keywords[speech.text].Invoke();
    }
    ////////////////////////////////////////////

    void Update()
    {
        if (timerOn)
        {
            Time.timeScale = 0;
            if (timeLeft >= 0)
            {
                Debug.Log("pausing...");
                Debug.Log("timeLeft: ______" + timeLeft);
                timeLeft -= Time.unscaledDeltaTime;
            }
            else
            {
                Time.timeScale = 1;
                Debug.Log("unpaused");
                timerOn = false;
            }
        }
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
                    instructions.text = " ";
                    spokenLetters.text = " ";
                    spokenNumbers.text = " ";
                    fire.text = " ";
                    blast.Play(); 
                    playerComments.text = "Yay! You totally sank their battleship!";
                    yayy.PlayDelayed(2);
                    enemyComments.text = "argh!!";
                    argh.PlayDelayed(4);
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                    
                }
                else
                {
                    instructions.text = " ";
                    spokenLetters.text = " ";
                    spokenNumbers.text = " ";
                    fire.text = " ";
                    blast.Play();
                    playerComments.text = "You hit their battleship!";
                    youh.PlayDelayed(2);
                    enemyComments.text = "grrrrr";
                    grrr.PlayDelayed(4);
                    
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                timeLeft = 2; 
                timerOn = true;
                
                break;
            }

        }
        if (hitCount == 0)
        {
            tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileScript>().SwitchColors(1);
            splash.Play();
            enemyComments.text = "Missed!        OK, my turn now!";
            miss.PlayDelayed(1);
            okmy.PlayDelayed(2);
            timeLeft = 1;
            timerOn = true;
        }
        instructions.text = " ";
        spokenLetters.text = " ";
        spokenNumbers.text = " ";
        Invoke("EndPlayerTurn", 1.0f);

        
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)  //invoked if successful hit
    {
        enemyScript.MissileHit(tileNum);  //assigns an h to the tile?
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        blast.Play();

        if (hitObj.GetComponent<ShipScript>().HitCheckSank())  //check if fully sunk
        {
            playerShipCount--;
            //playerShipText.text = playerShipCount.ToString();
            enemyScript.SunkPlayer();
        }

        enemyComments.text = "Ha ha! I got you!!";
        igot.PlayDelayed(1);
        timeLeft = 3; 
        timerOn = true;
        //Debug.Log("Enemy just got a hit");
        Invoke("EndEnemyTurn", 2.0f);
        
    }

    private void EndPlayerTurn() 
    {
        //keywordRecognizer.Stop();

        if (playerShipCount < 1) GameOver("Ha ha, I won!!!");
        //iwon.Play();

        fire.text = " ";
        playerComments.text = " ";
        subInstructions.text = " ";

        timeLeft = 2;
        timerOn = true;

        enemyScript.NPCTurn(); //selects next tile to aim for
    }

    public void EndEnemyTurn()  
    {
        if (enemyShipCount < 1)
        {
            GameOver("Oh no, they won the game!!!");
            ohno.Play();
            enemyComments.text = "Yes! I WON!!!";
        }

        enemyComments.text = " ";
        instructions.text = "OK, where now, Commander?";
        if (firstTime == true)
        {
            okwh.PlayDelayed(1);
            timeLeft = 1;
            timerOn = true;
            firstTime = false;
        }
        
        playerTurn = true;
        timeLeft = 1;
        timerOn = true;
        keywordRecognizer.Start();
    }

    /*
    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileScript tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }
    */

    void GameOver(string winner)
    {
        timeLeft = 1;
        timerOn = true;
        enemyComments.text = "I guess that's the end for me then!" + winner;
        igue.Play();
        replayBtn.gameObject.SetActive(true);
        playerTurn = false;
    }
    /*
    void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    */

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    char[] guessGrid;
    List<int> potentialHits;
    List<int> currentHits;
    private int guess;
    public GameObject enemyMissilePrefab;
    public GameManager gameManager;
    public AudioSource splash;


    private void Start()
    {
        potentialHits = new List<int>();
        currentHits = new List<int>();
        guessGrid = Enumerable.Repeat('o', 100).ToArray();
        
    }

    public List<int[]> PlaceEnemyShips() ///asigns all five ships (noses) to a tile number, and stores under List<int>
    {
        List<int[]> enemyShips = new List<int[]>
        {
            // each ship is the nose tile and then -1 for each subsequent one. changes later to -10 if vertical
            /*
            new int[]{-1, -1, -1, -1, -1},
            new int[]{-1, -1, -1, -1},
            new int[]{-1, -1, -1},
            new int[]{-1, -1, -1},
            new int[]{-1, -1}

            */
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1},
            new int[]{-1, -1}
        };
        int[] gridNumbers = Enumerable.Range(1, 100).ToArray();
        bool taken = true;
        foreach(int[] tileNumArray in enemyShips)
        {
            taken = true;
            while(taken == true)
            {
                taken = false;
                int shipNose = UnityEngine.Random.Range(0, 99);
                int rotateBool = UnityEngine.Random.Range(0, 2);
                int minusAmount = rotateBool == 0 ? 10 : 1;  //this is where it decides wheter to go horizontal (-1) or vertical (-10)
                for(int i = 0; i < tileNumArray.Length; i++)
                {
                    // check that ship back will not go off board 
                    if((shipNose - (minusAmount * i)) < 0 || gridNumbers[shipNose - i * minusAmount] < 0)
                    {
                        taken = true;
                        break;
                    }
                    // Ship is horizontal, check ship doesnt go off the sides 0 to 10, 11 to 20
                    else if(minusAmount == 1 && shipNose /10 != ((shipNose - i * minusAmount)-1) / 10)
                    {
                        taken = true;
                        break;
                    }
                }
                // if tile is not taken, loop through tile numbers assign them to the array in the list
                if (taken == false)
                {
                    for(int j = 0; j < tileNumArray.Length; j++)
                    {
                        tileNumArray[j] = gridNumbers[shipNose - j * minusAmount];
                        gridNumbers[shipNose - j * minusAmount] = -1;
                    }
                }
            }
        }
        foreach(var x in enemyShips)
        {
            Debug.Log("Enemy ship placed on tile: " + x[0]);  //show ship nose position of ship each time per loop ( ie.  87 ...90...9...72...74 )
        }
        
        return enemyShips;
    }

    public void NPCTurn()
    {
        List<int> hitIndex = new List<int>();
        for(int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h') hitIndex.Add(i);  //(h signifies a tile already hit)  hitindex is a local tally of number of hits in this round
        }
        if(hitIndex.Count > 1)  //if there have been MORE THAN ONE prior hit in this round, (we would already know whether to choose x or y direction)
        {
            int diff = hitIndex[1] - hitIndex[0];  // 'diff' between previous 2 hits determines whether x or y ie. +/-1 or -/+ 10 .
            int posNeg = Random.Range(0, 2) * 2 - 1;  //pick a random number either up or down
            int nextIndex = hitIndex[0] + diff; //next one to aim it is the most upward/left hit one plus the difference (10 or 1)  
            while(guessGrid[nextIndex] != 'o')  //if not already sunk, (o signifies a sunken ship?)
            {
                if(guessGrid[nextIndex] == 'm' || nextIndex > 100 || nextIndex < 0)   //if not already marked missed, OR not off the board 
                {
                    diff *= -1;  //wrong way, try the other side - change the differnece to opposite value (+1 changes to -1, -10 to +10)
                }
                nextIndex += diff;  // increase target tile number by the difference. ie 88 becomes 87, 89, 98 or 78  (nextIndex is the tile number)
            }
            guess = nextIndex; //new guess is now calculated (87, etc)  'guess' is used in resolving section below the if else statements.
        }


        else if (hitIndex.Count == 1)  //if there was already ONE HIT in this round (we won't know whether to go up or down)
        {
            List<int> closeTiles = new List<int>();  //compile list of adjascent tiles
            closeTiles.Add(1); closeTiles.Add(-1); closeTiles.Add(10); closeTiles.Add(-10); //adjust each to be one over, up or down
            int index = Random.Range(0, closeTiles.Count);  // randomly pick from the four options
            int possibleGuess = hitIndex[0] + closeTiles[index];  //target will be original hit tile plus above random number
            bool onGrid = possibleGuess > -1 && possibleGuess < 100;  //check it's still on the board true/false

            while((!onGrid || guessGrid[possibleGuess] != 'o') && closeTiles.Count > 0){ //if its off the grid OR the tile marked 'already sunk(?), AND random choice was positive...
                closeTiles.RemoveAt(index);  // then delete that option from random selection
                index = Random.Range(0, closeTiles.Count);  // chose a different random selection from remaining options
                possibleGuess = hitIndex[0] + closeTiles[index]; // target will be original hit tile plus above random number
                onGrid = possibleGuess > -1 && possibleGuess < 100;  //check it's still on the board true/false 
            } // circle back until guess is valid. 

            guess = possibleGuess;  //new guess is now calculated (87, etc)  'guess' is used in resolving section below the if else statements.
        }
        else // if this is a blind attempt to find a ship in this round
        {
            int nextIndex = Random.Range(0, 100); //any number 0-100

            while(guessGrid[nextIndex] != 'o') nextIndex = Random.Range(0, 100);  //continue until choice is not already fully sunken ship
            nextIndex = GuessAgainCheck(nextIndex);  //check that its not an edge case, below
            Debug.Log(" --- need to guess twice ");
            nextIndex = GuessAgainCheck(nextIndex);  //check that its not an edge case, below
            Debug.Log(" -Checking/adjusting a second time around (will catch an issue if it was in the corner");

            guess = nextIndex; //new guess is now calculated (87, etc)  'guess' is used in resolving section below the if else statements.
        }

        GameObject tile = GameObject.Find("Tile (" + (guess + 1) + ")");  // guesses run from zero and not from one like tiles
        //if there are more than one tile with that name, which does it choose?
        Debug.Log("this is the name of the tile ememy chose:" + tile);
        guessGrid[guess] = 'm';  //mark as a miss
        
        Vector3 vec = tile.transform.position;  //get tile position
        Debug.Log("this is the tile transform position:" + tile.transform.position);
        //create new missile 25 above tile position
        vec.y += 30;  
        GameObject missile = Instantiate(enemyMissilePrefab, vec, enemyMissilePrefab.transform.rotation); 

        missile.GetComponent<EnemyMissileScript>().SetTarget(guess);  //in other script it inputs 'guess' value to the variable 'targetTile'  
        missile.GetComponent<EnemyMissileScript>().targetTileLocation = tile.transform.position;  //inputs tile position to variable 'targetTileLocation'
    }
    

    private int GuessAgainCheck(int nextIndex)  //checks if on the edges, and tries again
    {
        string str = "nx: " + nextIndex;  //turning next guess number into string such as  "nx: 15"
        int newGuess = nextIndex;  
        bool edgeCase = nextIndex < 10 || nextIndex > 89 || nextIndex % 10 == 0 || nextIndex % 10 == 9;  //edge case means 0-10 or 90-100 or a x-10-number, or a x-1-number
        bool nearGuess = false;
        //following ifs checks that the four adjascent squares are off the grid. 
        if (nextIndex + 1 < 100) nearGuess = guessGrid[nextIndex + 1] != 'o';  
        if (!nearGuess && nextIndex - 1 > 0) nearGuess = guessGrid[nextIndex - 1] != 'o';  
        if (!nearGuess && nextIndex + 10 < 100) nearGuess = guessGrid[nextIndex + 10] != 'o';
        if (!nearGuess && nextIndex - 10 > 0) nearGuess = guessGrid[nextIndex - 10] != 'o';
        if (edgeCase || nearGuess) newGuess = Random.Range(0, 100);  //if its on the edge, pick another random number
        while (guessGrid[newGuess] != 'o') newGuess = Random.Range(0, 100);  //as long as new tile number is not 'o', new number is random
        Debug.Log(str + " newGuess: " + newGuess + " e:" + edgeCase + " g:" + nearGuess);  //reports:  nx: 15 newGuess: 15 e:False g:False
        return newGuess;
    }
    public void MissileHit(int hit)
    {
        guessGrid[guess] = 'h';
        Invoke("EndTurn", 1.0f);
    }

    public void SunkPlayer()
    {
        for(int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h') guessGrid[i] = 'x';
        }
    }

    private void EndTurn()
    {
        gameManager.GetComponent<GameManager>().EndEnemyTurn();
    }

    public void PauseAndEnd(int miss)
    {
        splash.Play();
        if (currentHits.Count > 0 && currentHits[0] > miss)
        {
            foreach(int potential in potentialHits)
            {
                if(currentHits[0] > miss)
                {
                    if (potential < miss) potentialHits.Remove(potential);
                } else
                {
                    if (potential > miss) potentialHits.Remove(potential);
                }
            }
        }
        Invoke("EndTurn", 1.0f);
    }
}

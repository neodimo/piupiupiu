using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedGame : MonoBehaviour
{

    void Awake()
    {
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!GameServices.IsInitialized())
        {
            GameServices.Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    // To store the opened saved game.
    private SavedGame mySavedGame;

    // Open a saved game with automatic conflict resolution
    void OpenSavedGame()
    {
        // Open a saved game named "My_Saved_Game" and resolve conflicts automatically if any.
        //GameServices.SavedGames.OpenWithAutomaticConflictResolution("My_Saved_Game", OpenSavedGameCallback);
    }

    // Open saved game callback
    void OpenSavedGameCallback(SavedGame savedGame, string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            Debug.Log("Saved game opened successfully!");
            mySavedGame = savedGame;        // keep a reference for later operations      
        }
        else
        {
            Debug.Log("Open saved game failed with error: " + error);
        }
    }
}

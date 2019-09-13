using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedGame
{
    public int highScore;
    public int multiplier;

    public void ScoreData (GameSession gameSession)
    {
        highScore = gameSession.highScore;
        multiplier = gameSession.multiplier;
    }

}
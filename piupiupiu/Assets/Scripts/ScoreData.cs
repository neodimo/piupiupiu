using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreData
{
    public int highScore;
    public int multiplier;

    public ScoreData (GameSession gameSession)
    {
        highScore = gameSession.highScore;
        multiplier = gameSession.multiplier;
    }

}
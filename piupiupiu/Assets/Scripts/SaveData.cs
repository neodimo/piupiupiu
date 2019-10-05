using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int highScore;
    public int multiplier;
    public int level;
    public float exp;
    public float expAtStartOfNewLevel;
    public float expBarPercentage;
    public float expNeeded;

    public SaveData (GameSession gameSession)
    {
        highScore = gameSession.highScore;
        multiplier = gameSession.multiplier;
        level = gameSession.level;
        exp = gameSession.exp;
        expAtStartOfNewLevel = gameSession.expAtStartOfNewLevel;
        expBarPercentage = gameSession.expBarPercentage;
        expNeeded = gameSession.expNeeded;
    }
}
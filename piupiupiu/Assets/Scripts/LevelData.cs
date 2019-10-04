using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int level;
    public float exp;
    public float expAtStartOfNewLevel;
    public float expBarPercentage;

    public LevelData (GameSession gameSession)
    {
        level = gameSession.level;
        exp = gameSession.exp;
        expAtStartOfNewLevel = gameSession.expAtStartOfNewLevel;
        expBarPercentage = gameSession.expBarPercentage;
    }

}
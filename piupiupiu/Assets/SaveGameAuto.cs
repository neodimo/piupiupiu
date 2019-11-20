using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameAuto : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveGame.SaveGameData(GameSession.Instance);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

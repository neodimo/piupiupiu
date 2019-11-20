using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteGameSession : MonoBehaviour
{
    public void DestroyGameSession()
    {
        Destroy(GameSession.Instance.gameObject);
    }
}

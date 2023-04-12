using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class LimitFPS : MonoBehaviour
{
    void Start()
    {
        UnityEngine.Application.targetFrameRate = 60;
    }
}

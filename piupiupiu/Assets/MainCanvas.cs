using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{

    public static MainCanvas Instance;
    public static GameObject mainCanvas;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        mainCanvas = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorGridManipulate : MonoBehaviour
{
    VectorGridForce2 vgf;

    // Start is called before the first frame update
    void Start()
    {
        vgf = gameObject.GetComponent<VectorGridForce2>();
    }

    // Update is called once per frame
    void Update()
    {
        vgf.m_ForceScale = Mathf.PingPong(Time.deltaTime*3, .5f)-.1f;
        //vgf.m_Radius
    }
}

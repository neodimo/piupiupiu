using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVectorGrid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var vectorGridForce2 = gameObject.GetComponent<VectorGridForce2>();
        vectorGridForce2.m_VectorGrid = FindObjectOfType<VectorGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

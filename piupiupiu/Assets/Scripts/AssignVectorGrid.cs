using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignVectorGrid : MonoBehaviour
{
    VectorGrid vectorGrid;

    // Start is called before the first frame update
    void Start()
    {
        vectorGrid = FindObjectOfType<VectorGrid>();
        VectorGridForce2 vectorGridForce2 = gameObject.GetComponent<VectorGridForce2>();
        vectorGridForce2.m_VectorGrid = vectorGrid;
    }
}

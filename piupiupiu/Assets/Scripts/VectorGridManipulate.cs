using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorGridManipulate : MonoBehaviour
{
    VectorGridForce2 vgf;
    string forceMode;
    [SerializeField] float vacuumScale;
    [SerializeField] float vacuumRadius;
    [SerializeField] float pushScale;
    [SerializeField] float pushRadius;

    // Start is called before the first frame update
    void Start()
    {
        vgf = gameObject.GetComponent<VectorGridForce2>();
        forceMode = "vacuum";
        StartCoroutine(VectorGridForceUpAndDown());
    }

    // Update is called once per frame
    void Update()
    {
        //vgf.m_ForceScale = Mathf.PingPong(Time.deltaTime*3, .5f)-.1f;
        //vgf.m_Radius
    }

    IEnumerator VectorGridForceUpAndDown()
    {
        while (gameObject != null)
        {
            if (forceMode == "vacuum")
            {
                vgf.m_ForceScale = vacuumScale;
                vgf.m_Radius = vacuumRadius;
                forceMode = "push";
            }
            else if (forceMode == "push")
            {
                vgf.m_ForceScale = pushScale;
                vgf.m_Radius = pushRadius;
                forceMode = "vacuum";
            }
            yield return new WaitForSeconds(2f);
        }
    }
}

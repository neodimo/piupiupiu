using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightFadeOn : MonoBehaviour
{
    Light pointLight;
    float t = 0f;
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Start is called before the first frame update
    void Start()
    {
        pointLight = GetComponent<Light>();
        SetupBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        pointLight.intensity = Mathf.Lerp(.2f, .8f, t);
        pointLight.range = Mathf.Lerp(100f, 60f, t);
        t += 3f * Time.deltaTime;
    }

    private void SetupBoundaries()
    {
        xMin = -49.15f;
        xMax = 49.15f;
        yMin = -49.15f;
        yMax = 49.15f;
    }
}

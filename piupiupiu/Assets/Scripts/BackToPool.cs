using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToPool : MonoBehaviour
{
    [SerializeField] float duration = 2f;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(BackToPooling());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BackToPooling()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}

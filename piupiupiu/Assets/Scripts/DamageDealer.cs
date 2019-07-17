using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{

    [SerializeField] int damage = 100;
    
    public int GetDamage()
    {
        return damage;
    }

    public void Hit(string colliderType)
    {

        //Destroy(gameObject);
        
        if (gameObject.tag != "Player")
        {
            if (colliderType == "2d")
            {
                gameObject.SetActive(false);
            }
            else if (colliderType == "3d")
            {
                gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}

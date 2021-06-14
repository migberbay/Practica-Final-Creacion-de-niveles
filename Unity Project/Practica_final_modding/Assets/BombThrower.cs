using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombThrower : MonoBehaviour
{
    public float bomb_ammount = 9999;
    public GameObject bombPrefab, throwerLocation;

    public void createNew(){
        if(bomb_ammount > 0){
            bomb_ammount --;
            StartCoroutine(throwBomb());
        }
    }

    IEnumerator throwBomb(){
        GameObject bombInstance = GameObject.Instantiate(bombPrefab, throwerLocation.transform);
        bombInstance.transform.parent = null;

        // bombInstance.GetComponent<Rigidbody2D>().AddRelativeForce(throwerLocation.transform.right * 5);
        
        yield return null;
    }
}

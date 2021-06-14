using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeAfterSeconds : MonoBehaviour
{
    public GameObject explosionObject;
    public SpriteRenderer s_renderer;
    public float timeToExplode = 3f, timeToDelete = 0.75f;


    private void FixedUpdate() {
        timeToExplode -= Time.fixedDeltaTime;
        if(timeToExplode <= 0){
            s_renderer.enabled = false;
            explosionObject.SetActive(true);
            timeToDelete -= Time.fixedDeltaTime;
            if(timeToDelete <= 0){
                Destroy(this.gameObject);
            }
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTargeting : MonoBehaviour
{
    private Tower tower;

    private void Awake(){
        tower = GetComponent<Tower>();
    }


    private void OnTriggerEnter(Collider collision){
        
            if(collision.TryGetComponent(out Enemy enemy)){
                tower.SetTarget(enemy);
                Debug.Log("target!");
            }
        

    }
    // private void OnTriggerExit(Collider collision){
     
    //     tower.SetTarget(null);
    // }
}

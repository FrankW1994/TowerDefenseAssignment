using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private float speed = 500f;
    [SerializeField] public int damage = 80;
    //private bool hasHit = false;

    private Transform target;


    public void SetTarget(Transform _target){
        target = _target;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null){
            return;
        }
        Vector3 direction = (target.position - transform.position).normalized;

        body.velocity = direction * speed;
    }
    [PunRPC]
    private void OnTriggerEnter(Collider collision){

        if(collision.TryGetComponent(out Enemy enemy)){
            Destroy(gameObject);
            enemy.TakeDamage(damage);
            //Debug.Log("Hit");
            
        }
     

        
    }
}

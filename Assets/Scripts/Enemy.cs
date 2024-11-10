using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class Enemy : MonoBehaviourPun//, IPunObservable
{

    //public delegate void EnemyDestroyedHandler(Enemy enemy);
    //public static event EnemyDestroyedHandler OnEnemyDestroyed;
    
    public static event Action<Enemy> OnEnemyDestroyed;
    public static event Action<Enemy> OnEnemyInfiltration;

    [SerializeField] private Rigidbody rb;

    [Header("properties")] 
    public float speed;
    public int health;
    public int reward;

    //public Enemy(){
    //    float speed = 2f;
    //    int health = 100;
    //    int reward = 100;
    //}
    


    private Transform moveTarget;
    private int pathIndex;

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo onfo){
    //     throw new System.NotImplementedException();
    // }


   
    private void Start()
    {
        moveTarget = PathManager.main.path[pathIndex];
        pathIndex = 0;
    }
    
    private void Update(){

        if (PhotonNetwork.IsMasterClient)
        {
            if (Vector3.Distance(moveTarget.position, transform.position) <= 0.1f){
                    //pathIndex++;
                UpdatePathIndex();
                //Debug.Log("PathIndex: " + pathIndex + ". max is: " + PathManager.main.path.Length);
                if(pathIndex >= PathManager.main.path.Length) {

                    Infiltrate();
                    return;

                }else{
                    moveTarget = PathManager.main.path[pathIndex];
                    //Debug.Log("nextnode");
                }
            }
        }
        //CheckPath();
        //PhotonView.RPC("CheckPath", RpcTarget.AllViaServer);
        
    }

    private void UpdatePathIndex(){
        if(PhotonNetwork.IsMasterClient){
            pathIndex++;
            photonView.RPC("RPC_UpdatePathIndex", RpcTarget.Others, pathIndex);
        }
        
    }
    [PunRPC]
    private void RPC_UpdatePathIndex(int newPathIndex){
        pathIndex = newPathIndex;
    }

    //[PunRPC]
    public void CheckPath(){
        if (Vector3.Distance(moveTarget.position, transform.position) <= 0.1f){
            UpdatePathIndex();
           // Debug.Log("PathIndex: " + pathIndex + ". max is: " + PathManager.main.path.Length);
           //Debug.Log("enemy path next");
            if(pathIndex == PathManager.main.path.Length) {

                Infiltrate();
                //Debug.Log("Infiltrate");
                return;
            }else{moveTarget = PathManager.main.path[pathIndex];}
        }
    }

    private void FixedUpdate()
    {
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, moveTarget.position, step);
        //Vector3 direction = (moveTarget.position - transform.position).normalized;
        //rb.velocity = direction * speed;
    }
    
    private void Die(){

        photonView.RPC("RPC_Die", RpcTarget.All);
            //OnEnemyDestroyed?.Invoke(this); // Call event when enemy is destroyed
            //Destroy(gameObject);
        

        //Debug.Log("Died.");
    }

    [PunRPC]
    private void RPC_Die(){

        
            OnEnemyDestroyed?.Invoke(this); // Call event when enemy is destroyed
            Destroy(gameObject);
        

        //Debug.Log("Died.");
    }
    
    private void Infiltrate(){

        photonView.RPC("RPC_Infiltrate", RpcTarget.All);
        //Destroy(gameObject);
        //Debug.Log("Infiltrate.");
    }
    [PunRPC]
    private void RPC_Infiltrate(){
        
            OnEnemyInfiltration?.Invoke(this); // Call event when enemy is destroyed
            Destroy(gameObject);
        

        //Debug.Log("Infiltrate.");
    }


    public void TakeDamage(int damage){
        
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else{
            photonView.RPC("RPC_TakeDamage", RpcTarget.All, health);
        }
        
    }
    [PunRPC]
    public void RPC_TakeDamage(int damage){
        health -= damage;
    }
}
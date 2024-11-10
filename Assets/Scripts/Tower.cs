using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tower : MonoBehaviourPun
{
    // Start is called before the first frame update

    
   
    private int cost = 100;
    public Enemy targetEnemy;
    [SerializeField] private Transform turret;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireRate = 2f;
    private SphereCollider collider;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    [SerializeField] private float range = 8f;
    private bool isShooting = false;
    private bool isActive = false;
     
    
    private void OnEnable(){
        PlacementSystem.OnTowerPlaced += HandleTowerPlaced;
        GameState.OnZeroCurrency += HandleGameOver;
        
    }

    private void OnDisable()
    {
        PlacementSystem.OnTowerPlaced -= HandleTowerPlaced;
        GameState.OnZeroCurrency -= HandleGameOver;
    }

    private void Awake(){
        collider = GetComponent<SphereCollider>();
        collider.radius = range;
    }


    // Update is called once per frame
    void Update()
    {
        
        
        if (targetEnemy != null && isActive){
            //Debug.Log("isActive");
            RotateTurret();
            if (!isShooting)
            {
                StartCoroutine(ShootCoroutine());
            }
            //Debug.Log(enemiesInRange);
            CheckRange();
        }
        if (enemiesInRange.Count > 0 && targetEnemy == null){
            GetTarget();
            //Debug.Log("Target Aquired!");
        }

    }
    [PunRPC]
    private void GetTarget(){
        //Debug.Log("Got");
        targetEnemy = enemiesInRange[0];
    }

    private void CheckRange(){
        //return Vector3.Distance(targetEnemy.transform.position,transform.position ) <= range;
        float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
        //Debug.Log("distance = " + distanceToEnemy);
        if (distanceToEnemy > (range+0.6f)){
            
            
            enemiesInRange.Remove(targetEnemy);
            targetEnemy = null;
            //Debug.Log("enemy removed");
        }
    }

    private void OnTriggerEnter(Collider collision){
        
        if(collision.TryGetComponent(out Enemy enemy)){
            enemiesInRange.Add(enemy);
            //Debug.Log("target!");
        }
       //if (targetEnemy == null){
       //    if(collision.TryGetComponent(out Enemy enemy)){
       //        SetTarget(enemy);
       //        Debug.Log("target!");
       //    }
       //}
    }



    public void SetTarget(Enemy targetEnemy){
        this.targetEnemy = targetEnemy;
        //Debug.Log("Target Aquired!");
    }
    [PunRPC]
    private void RotateTurret(){
        Vector3 dir = targetEnemy.transform.position - turret.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        Vector3 rot = Quaternion.Slerp(turret.rotation, lookRot, Time.deltaTime * 100).eulerAngles;
        turret.rotation = Quaternion.Euler(0f, rot.y, 0f);
    }

    private IEnumerator ShootCoroutine()
    {
        isShooting = true;

        while (targetEnemy != null )
        {
            //Debug.Log("Target Aquired!");
            Shoot();
            yield return new WaitForSeconds(1f / fireRate); 
        }
        isShooting = false; 
        
        
        enemiesInRange.Remove(targetEnemy);
    }

    private void Shoot(){
        GameObject bulletObject = Instantiate(projectile, turret.position, Quaternion.identity);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        
        bullet.SetTarget(targetEnemy.transform);
        
    }


    //private void HandleTowerPlaced(int price){
    //    isActive = true;
    //    //Debug.Log("placed");
    //    return;
    //}

    [PunRPC]
    private void RPC_SetActive(bool active)
    {
        isActive = active;
    }

    private void HandleTowerPlaced(int price)
    {
        if (PhotonNetwork.IsMasterClient) // Or whoever owns placement control
        {
            photonView.RPC("RPC_SetActive", RpcTarget.All, true);
        }
    }

    public int GetCost(){
        return cost;
    }


    private void HandleGameOver()
    {
        
        isActive = false;
    }
}


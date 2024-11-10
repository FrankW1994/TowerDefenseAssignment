using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager Instance { get; private set;}
    [Header("Wave Settings")]
    public Wave[] waves;              // Array of waves with different settings
    [SerializeField] private float timeBetweenWaves = 5f; // Delay between waves
    public Transform spawnPoint;        // Location where enemies spawn

    private int currentWaveIndex = 0;
    private bool spawningWave = false;
    private int enemiesRemaining;
    [SerializeField] private float difficulty = 1.2f;
    private bool isSpawning = true;
    private bool isConnected = false;

    void Awake(){
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            //Debug.Log("EnemymanagerInstance");
            Instance = this; 
        }
    }
    
    private void OnEnable(){
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        Enemy.OnEnemyInfiltration += HandleEnemyDestroyed;
        GameState.OnZeroCurrency += HandleGameOver;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnEnemyKilled event to avoid memory leaks
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        Enemy.OnEnemyInfiltration -= HandleEnemyDestroyed;
        GameState.OnZeroCurrency -= HandleGameOver;
    }

    void FixedUpdate(){
        if(PhotonNetwork.IsMasterClient && isConnected == false){
            isConnected = true;
            StartCoroutine(StartNextWave());
        }
    }

    void Start()
    {
        if(PhotonNetwork.IsMasterClient){
            //StartCoroutine(StartNextWave());
        }
        
    }

    private IEnumerator StartNextWave()
    {
        while (isSpawning)
        {
            //spawningWave = true;
            currentWaveIndex++;
            Wave wave = waves[Random.Range(0,waves.Length)];
            yield return StartCoroutine(SpawnWave(wave));
            
           
            yield return new WaitForSeconds(timeBetweenWaves);

            
        }

        //spawningWave = false;
        //Debug.Log("All waves complete!");
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        //Debug.Log("Spawning Wave " + (currentWaveIndex + 1));
        int amountToSpawn = Mathf.RoundToInt(wave.spawnCount * (currentWaveIndex * difficulty));
        //Debug.Log("Wave: "+ currentWaveIndex + ". amount of enemies: " + amountToSpawn);
        enemiesRemaining = (int)amountToSpawn;/*wave.spawnCount;*/
        //Debug.Log("amount to spawn: " + amountToSpawn);
        for (int i = 0; i < amountToSpawn; i++)
        {
            //Debug.Log("spawn");
            //val Random rnd = new Random();
            SpawnEnemy(wave.enemyPrefab[Random.Range(0,wave.enemyPrefab.Length)]);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
        while (enemiesRemaining > 0) 
        {
             yield return null; // Wait until all enemies are destroyed
        }
        //Debug.Log("Wave Complete!");
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        
        enemiesRemaining--;
        //Debug.Log("enemies left this wave: " + enemiesRemaining);
    }

    private void HandleGameOver(){
        isSpawning = false;
    }

    //[SerializedField] private GameObject[] enemies;
    //
    //[SerializeField] private int baseEnemies = 8;
    //[SerializeField] private float spawnRate = 1f;
    //[SerializeField] private float wavePause = 5f;
//
    //private int currentWave = 1;

}

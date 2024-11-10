using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class GameState : MonoBehaviourPun
{
    public static GameState main;
    public static event Action OnZeroCurrency;

    public int sharedScore;
    public int kills;
    [SerializeField] private int currency;

    private void Awake(){
        main = this;
    }
    

    private void OnEnable(){
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        Enemy.OnEnemyInfiltration += HandleEnemyInfiltrated;
        PlacementSystem.OnTowerPlaced += HandleTowerPlaced;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnEnemyKilled event to avoid memory leaks
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        Enemy.OnEnemyInfiltration -= HandleEnemyInfiltrated;
        PlacementSystem.OnTowerPlaced -= HandleTowerPlaced;
    }

    private void Update(){
        if(currency < 0){
            GameOver();
        }
    }

    private void HandleEnemyDestroyed(Enemy enemy){
        //Debug.Log("EventHandler active gamestate.");
        if (PhotonNetwork.IsMasterClient) // Only the master client modifies this data
    {
        kills++;
        currency += enemy.reward;
        sharedScore += enemy.reward;
        photonView.RPC("RPC_SyncOnKill", RpcTarget.All, kills, currency, sharedScore);
    }
    }
    [PunRPC]
    private void RPC_SyncOnKill(int newKills, int newScore, int newCurrency){
        currency = newCurrency;
        kills = newKills;
        sharedScore = newScore;
    }


    private void HandleEnemyInfiltrated(Enemy enemy){
        currency -= (enemy.reward*2);
        photonView.RPC("RPC_SyncCurrency", RpcTarget.All, currency);
    }

    [PunRPC]
    private void RPC_SyncCurrency(int newCurrency){
        currency = newCurrency;
    }

    private void HandleTowerPlaced(int spendCurrency){
        currency -= spendCurrency;
        photonView.RPC("RPC_SyncCurrency", RpcTarget.All, currency);
    }

//   public void AddScore(int _points)
//   {
//       sharedScore += _points;
//
//   }
//
//   [PunRPC]
//   private void RPC_SyncScore(int newScore){
//       score = newScore;
//   }


    public int GetCurrency()
    {
        return currency;
    }

    private void GameOver(){
        photonView.RPC("RPC_GameOver", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_GameOver(){
        // Trigger event on all clients
        OnZeroCurrency?.Invoke(); // Now this will be invoked for all players
        Debug.Log("Game Over - All players are notified!");
    }

}

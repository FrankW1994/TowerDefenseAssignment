using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject gameState;
    [SerializeField] private GameObject enemyManager;
    [SerializeField] private GameObject ui;
    
    void Awake(){
        //DontDestroyOnLoad(this.gameObject);
        PhotonNetwork.Instantiate(gameState.name, new Vector3(0, 0, 0),Quaternion.identity, 0);
        PhotonNetwork.Instantiate(enemyManager.name, new Vector3(0, 0, 0),Quaternion.identity, 0);
        PhotonNetwork.Instantiate(ui.name,new Vector3(0, 0, 0),Quaternion.identity, 0);
        PhotonNetwork.Instantiate(playerPrefab.name,new Vector3(475, 265, 640),Quaternion.identity, 0);
        Debug.Log("Spawned");
    }

    void Start(){


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    
    void Start()
    {
        Debug.Log("Connecting.");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster(){
        base.OnConnectedToMaster();
        Debug.Log("Connected.");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby(){

        base.OnJoinedLobby();

        PhotonNetwork.JoinOrCreateRoom("test", null, null);
        Debug.Log("Connected to a lobby.");
        
        
        //SceneManager.LoadScene("Menu");
    }
    public override void OnJoinedRoom(){
        base.OnJoinedRoom();
        Debug.Log("Connected to a room.");
        Debug.Log("nr of Players: " + PhotonNetwork.CountOfPlayersInRooms);
        PhotonNetwork.Instantiate(playerPrefab.name,new Vector3(475, 265, 640),Quaternion.identity, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{

    [SerializeField] public TMP_InputField inputCreate;
    [SerializeField] public TMP_InputField inputJoin;
    // Start is called before the first frame update
    public void CreateRoom(){
        PhotonNetwork.CreateRoom(inputCreate.text, new RoomOptions() {MaxPlayers = 2, IsVisible = true, IsOpen = true}, TypedLobby.Default, null);
    }
    public void JoinRoom(){
        PhotonNetwork.JoinRoom(inputJoin.text);
    }
    public override void OnJoinedRoom(){
        Debug.Log("nr of Players: " + PhotonNetwork.CountOfPlayersInRooms);
        PhotonNetwork.LoadLevel("SampleScene");
    }
}

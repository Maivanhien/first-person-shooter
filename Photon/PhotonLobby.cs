using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;
    public GameObject playButton;
    public GameObject cancelButton;

    private void Awake()
    {
        lobby = this; // create singleton
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // connect to master photon server
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        playButton.GetComponent<Button>().interactable = true;
    }

    public void OnBattleButtonClicked()
    {
        playButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom(); // participate in random room
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a random game but failed. There must be no open games available");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSetting.multiplayerSetting.maxPlayers };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, there must already be a room with the same name");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSetting.multiplayerSetting.maxPlayers };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }

    public void OnCancelButtonClicked()
    {
        cancelButton.SetActive(false);
        playButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    void Update()
    {
        
    }
}

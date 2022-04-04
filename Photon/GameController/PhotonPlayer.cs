using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;
    public int myTeam;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(myAvatar == null && myTeam != 0)
        {
            if(myTeam == 1)
            {
                object[] obj = new object[] { myTeam };
                int spawnPicker = Random.Range(0, GameSetup.gameSetup.spawnPointsTeamOne.Length);
                if (PV.IsMine)
                {
                    myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.gameSetup.spawnPointsTeamOne[spawnPicker].position, GameSetup.gameSetup.spawnPointsTeamOne[spawnPicker].rotation, 0, obj);
                }
            }
            else
            {
                object[] obj = new object[] { myTeam };
                int spawnPicker = Random.Range(0, GameSetup.gameSetup.spawnpointsTeamTwo.Length);
                if (PV.IsMine)
                {
                    myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.gameSetup.spawnpointsTeamTwo[spawnPicker].position, GameSetup.gameSetup.spawnpointsTeamTwo[spawnPicker].rotation, 0, obj);
                }
            }
        }
    }

    [PunRPC]
    void RPC_GetTeam()
    {
        myTeam = GameSetup.gameSetup.nextPlayersTeam;
        GameSetup.gameSetup.UpdateTeam();
        if (PV == null)
            PV = GetComponent<PhotonView>();
        PV.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, myTeam);
    }

    [PunRPC]
    void RPC_SentTeam(int whichTeam)
    {
        myTeam = whichTeam;
    }
}

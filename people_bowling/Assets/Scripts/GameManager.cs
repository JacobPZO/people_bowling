using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerController[] players;
    public Transform[] spawnPoints;
    public int alivePlayers;

    public float postGameTime;
    
    private int playersInGame;
    
    // instance
    public static GameManager instance;

    public PlayerController GetPlayer (GameObject playerObj)
    {
        foreach(PlayerController player in players)
        {
            if(player != null && player.gameObject == playerObj)
            return player;
        }
        return null;
    }

    public PlayerController GetPlayer (int playerId)
    {
        foreach(PlayerController player in players)
        {
        if(player != null && player.id == playerId)
            return player;
        }
        return null;
    }

    public void CheckWinCondition ()
    {
        if(alivePlayers == 1)
        photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
    }
    [PunRPC]
    void WinGame (int winningPlayer)
    {
        // set the UI win text
        Invoke("GoBackToMenu", postGameTime);
    }
    void GoBackToMenu ()
    {
        NetworkManager.instance.ChangeScene("Menu");
    }

    void Awake ()
    {
        instance = this;
    }

    [PunRPC]
    void ImInGame ()
    {
        playersInGame++;
        if(PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
        photonView.RPC("SpawnPlayer", RpcTarget.All);
    }

    [PunRPC]
    void SpawnPlayer ()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        // initialize the player for all other players
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;

public class NetworkManager : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
      
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
    }

    public void ChangeSizes(string jsonSizes)
    {
        //send message to all connected players (event master client) with the random data
        photonView.RPC("ChangeSizesRPC", RpcTarget.All, jsonSizes);
    }

    //this method is going to be called automatically by Photon on all connected clients
    //and it is first called from the ChangeSizes method

    [PunRPC]
    public void ChangeSizesRPC(string jsonSizes)
    {
        print(jsonSizes);
        List<PlayerInfo> playersInfo = JsonConvert.DeserializeObject<List<PlayerInfo>>(jsonSizes);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            player.GetComponent<PlayerController>().ChangeSizeFromMaster(playersInfo);
        }
    }

    public void DestroyPlayer(int destroyPlayerActorNumber)
    {
        //send message to all connected clients with id (actorNumber) to destroy
        photonView.RPC("DestroyPlayerRPC", RpcTarget.All, destroyPlayerActorNumber);
    }

    [PunRPC]
    public void DestroyPlayerRPC(int destroyPlayerActorNumber)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if(player.GetComponent<PhotonView>().Owner.ActorNumber == destroyPlayerActorNumber)
            {
                if (player.GetComponent<PhotonView>().AmOwner)
                {
                    PhotonNetwork.Destroy(player);
                }
               
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

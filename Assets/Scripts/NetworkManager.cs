using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

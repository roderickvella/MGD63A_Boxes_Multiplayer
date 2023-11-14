using Newtonsoft.Json;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("ButtonChangeSizes").SetActive(true);
        }
        else
        {
            GameObject.Find("ButtonChangeSizes").SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnButton()
    {
        TMP_Dropdown dropdown = GameObject.Find("DropdownColour").GetComponent<TMP_Dropdown>();
        //selected colour from the dropdown
        string colour = dropdown.options[dropdown.value].text;

        //our box has to be instantiated with a random size
        float boxRandomSize = Random.Range(0.5f, 0.8f);

        //we have to instantiate our box prefab using photon so that other users
        //can see this box

        //TODO: Instantiate Box Via Photon and pass the chosen colour and random size
        object[] myCustomInitData = new object[2] { colour, boxRandomSize };
        PhotonNetwork.Instantiate("Player",
            new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f)),
            Quaternion.identity, 0, myCustomInitData);
    }

    //this method is called when the button ChangeSizes is pressed
    //this method is called by the master client (because it is visible only to the masterclient)
    public void ChangeSizesButton()
    {
        List<PlayerInfo> playerInfos = new List<PlayerInfo>();
        Photon.Realtime.Player[] allPlayers = PhotonNetwork.PlayerList;

        foreach(Photon.Realtime.Player player in allPlayers)
        {
            //generate the random size for the box
            float size = Random.Range(0.5f, 0.8f);
            //add instance to list playerInfos
            playerInfos.Add(new PlayerInfo()
            {
                actorNumber = player.ActorNumber,
                size = new Vector3(size, size, 1)
            });
        }

        //convert to json
        string json = JsonConvert.SerializeObject(playerInfos);

        //now we need to send this json data to all clients so that every client
        //can change the size of every box
        GetComponent<NetworkManager>().ChangeSizes(json);
    }

    
}

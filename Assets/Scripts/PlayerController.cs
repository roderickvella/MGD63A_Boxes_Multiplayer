using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback, IPunObservable
{
    PhotonView photonView;
    public FixedJoystick fixedJoystick;
    Rigidbody2D body;
    float horizontal;
    float vertical;

    Vector3 otherPlayerPos;



    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        string colour = (string)info.photonView.InstantiationData[0];
        float boxRandomSize = (float)info.photonView.InstantiationData[1];

        print("Loaded Colour:" + colour);
        print("Loaded Random Size:" + boxRandomSize);

        //changing sprite colour
        if (colour == "Red")
            GetComponent<SpriteRenderer>().color = Color.red;
        else if (colour == "Green")
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (colour == "Blue")
            GetComponent<SpriteRenderer>().color = Color.blue;

        //change the size of our prefab
        //transform.localScale = new Vector3(boxRandomSize, boxRandomSize);
        transform.localScale = new Vector2(boxRandomSize, boxRandomSize);

        //show player name in label
        GetComponentInChildren<TextMeshProUGUI>().text = info.photonView.Owner.NickName;

        
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);

        if (photonView.IsMine)
        {
            //this player owns (is the owner) of this object, therefore give him
            //control to the joystick
            fixedJoystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
            //now give access to the rigidbody found on the player
            body = GetComponent<Rigidbody2D>();            
        }
        else
        {
            //if player does not own this object. this means we don't need the rigidbody
            //because the object is going to move with data from the server
            Destroy(GetComponent<Rigidbody2D>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //we own this object therefore move it with the joystick
            horizontal = fixedJoystick.Horizontal;
            vertical = fixedJoystick.Vertical;
        }
        else
        {
            //if player object is not mine, then we need to manually change its position
            //with the data from the server. The stream we received from OnPhotonSerializeView()

            //we are making use of lerp for smooth animation
            transform.position = Vector3.Lerp(transform.position, otherPlayerPos
                , Time.deltaTime * 10);
            //transform.position = otherPlayerPos;
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            float runSpeed = 5.0f;
            body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //is writing means send data to other players if we own this instance
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);         
            print("Sending PlayerPos Data:"+transform.position);
        }
        else
        {
            //we are receiving data from other player
            otherPlayerPos = (Vector3) stream.ReceiveNext();          
            print("Received PlayerPos Data:" + otherPlayerPos);
        }
    }
}

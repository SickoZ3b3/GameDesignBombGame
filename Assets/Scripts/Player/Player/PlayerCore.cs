using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public interface IPlayer
{
    void StatusUpdate();
}

public class PlayerCore : NetworkBehaviour
{
    public PlayerBallController BallController;
    public PlayerMoveMent moveMent;
    public PlayerLook playerLook;
    [FormerlySerializedAs("bomb")] public GameObject bombPrefab;
    [FormerlySerializedAs("go")] public GameObject handPos;

    public bool isHand = false;

    [SyncVar]
    public GameObject handBomb;
    
    // Start is called before the first frame update
    void Start()
    {
        BallController = this.GetComponent<PlayerBallController>();
        moveMent = this.GetComponent<PlayerMoveMent>();
        playerLook.gameObject.SetActive(isLocalPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        BallController.StatusUpdate();
        moveMent.StatusUpdate();
        playerLook.StatusUpdate();
    }

    [ClientRpc]
    public void RpcShowBomb()
    {
        if (!isLocalPlayer)
            return;
        isHand = true;
    }
    
    [ClientRpc]
    public void RpcDiablBomb()
    {
        if (!isLocalPlayer)
            return;
        isHand = false;
    }
}

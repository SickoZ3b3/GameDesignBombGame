using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Experimental;
using UnityEngine;

public class PlayerBallController : NetworkBehaviour, IPlayer
{
    public float throwForce = 10f;
    public float passDistance = 20f;
    public LayerMask playerLayerMask;
    [SyncVar] private Transform handTransform;
    [SyncVar] private GameObject Ball;

    [SyncVar] public bool isHandleBall;

    public PlayerCore core;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
    }

    void Start()
    {
        handTransform = this.transform.Find("Hand");
    }

    // Update is called once per frame
    public void StatusUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //如果 炸弹在手里
        if (core.isHand)
        {
            if (Input.GetMouseButtonDown(0)) // Left click to pass
            {
                PassBomb();
            }
            else if (Input.GetMouseButtonDown(1)) // Right click to throw
            {
                CmdThrowBomb();
            }
        }
    }

    public void InitBall(GameObject go)
    {
        isHandleBall = true;
        Ball = go;
        go.transform.SetParent(handTransform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
    }

    public void RpcInit()
    {
    }

    GameObject GetBombInHand()
    {
        if (handTransform.childCount > 0)
        {
            return handTransform.GetChild(0).gameObject;
        }

        return null;
    }

    /// <summary>
    /// 传球
    /// </summary>
    public void PassBomb()
    {
        GameObject bombInstance;
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, passDistance, playerLayerMask);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (Collider player in playersInRange)
        {
            if (player.gameObject == gameObject) continue; // Skip self

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player.gameObject;
            }
        }

        if (nearestPlayer != null)
        {
            Debug.Log("Passing bomb to player: " + nearestPlayer.name);
            if (GetComponent<PlayerDeathController>().IsPlayerDead())
            {
                return;
            }
            CmdpassBall( nearestPlayer);
        }
        else
        {
            Debug.LogWarning("No player found in range to pass the bomb.");
        }
    }

    /// <summary>
    /// 传球
    /// </summary>
    [Command]
    public void CmdpassBall(GameObject nearestPlayer)
    {
        core.RpcDiablBomb();
        var targetPlayer = nearestPlayer.GetComponent<PlayerCore>();
        targetPlayer.RpcShowBomb();
        RpcPassBall(core.gameObject, nearestPlayer);
    }

    [ClientRpc]
    public void RpcPassBall(GameObject bombowner, GameObject targetPlayerGo)
    {
        var owner = bombowner.GetComponent<PlayerCore>();
        var targetPlayer = targetPlayerGo.GetComponent<PlayerCore>();
        targetPlayer.handBomb = owner.handBomb;
        targetPlayer.handBomb.transform.parent = targetPlayer.handPos.transform;
        targetPlayer.handBomb.transform.localPosition = Vector3.zero;
        owner.handBomb = null;
    }

    
    [Command]
    public void CmdThrowBomb()
    {
        //是否是本地玩家
        if (!isLocalPlayer)
            return;

        //判断是否可以丢球
        if (!core.isHand)
            return;

        GameManager.Instance.SetThrowBallTimeCold();
        RpcThrow(core.handBomb);
        core.RpcDiablBomb();
    }

    [ClientRpc]
    public void RpcThrow(GameObject bomb)
    {
        bomb.transform.SetParent(null);
        var bombrigibody = bomb.GetComponent<Rigidbody>();
        bombrigibody.isKinematic = false;
        bombrigibody.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }
}
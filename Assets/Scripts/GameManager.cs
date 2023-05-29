using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;

public class ServerBombCon
{
    private bool isStartCold = false;
    public float ExploreTime { get;  set; } = 40;
    private BombController bomb;
    private PlayerCore playerCon;


    private Action OnBombExplor;
    /// <summary>
    /// 设置炸弹 仅server 控制
    /// </summary>
    /// <param name="_bomb"></param>
    /// <param name="time"></param>
    public void SetBomb(PlayerCore player, BombController _bomb ,float time ,Action randomCallBack)
    {
        playerCon = player;
        bomb = _bomb;
        ExploreTime = time;
        OnBombExplor += randomCallBack;
    }
    /// <summary>
    /// 设置炸弹是否爆炸
    /// </summary>
    public void StartTimeCount()
    {
        isStartCold = true;
    }
    public void Update()
    {
        if (isStartCold)
        {
            ExploreTime -= Time.deltaTime;
            if (ExploreTime<=0)
            {
                bomb.ServerExplode();
                playerCon.RpcDiablBomb();
                isStartCold = false;
                
                OnBombExplor?.Invoke();
                OnBombExplor = null;
            }
        }
    }
 

    /// <summary>
    /// 修改球权控制
    /// </summary>
    /// <param name="player"></param>
    public void ChangePlayer(PlayerCore _player)
    {
        playerCon = _player;
    }

    public  GameObject GetBomb()
    {
        return bomb.gameObject;
    }
}
    
public class GameManager : NetworkBehaviour
{
    private ServerBombCon _serverBombCon ;
    public GameObject OrignBomb;
    
    public GameConfig gameConfig;

    /// <summary>
    /// 炸弹  
    /// </summary>
    public float BombExporeTime
    {
        get { return gameConfig.BombExploreTime; }
    }

    /// <summary>
    /// 炸弹扔出后爆炸的时间
    /// </summary>
    public float BombThorwExploreTime
    {
        get { return gameConfig.BombTrowExploreTime;}
    }
    public static GameManager Instance;
    
    private void Awake()
    {
        Instance = this;
        _serverBombCon = new ServerBombCon();
    }
    
    public void Update()
    {
        if (!isServer)
            return;

        if (Input.GetKeyDown(KeyCode.U))
        {
            RandomPlayer();
        }
        
        _serverBombCon.Update();
    }
    
    /// <summary>
    /// 随机给玩家发放一个球
    /// </summary>
    [Server]
    public void RandomPlayer()
    {
        int count = NetworkServer.connections.Count;
        DateTime currentTime = DateTime.UtcNow;
        long timestamp = (long)(currentTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        int seed = (int)timestamp;
        System.Random random = new System.Random(seed);
        int randomIndex = random.Next(count);
        var list = NetworkServer.connections.Keys.ToList();
        var con = list[randomIndex];
        var go = NetworkServer.connections[con].identity.gameObject;
        
        var playercore  = go.gameObject.GetComponent<PlayerCore>();
        var handpos = playercore.handPos;
        GameObject bomb = Instantiate(OrignBomb, handpos.transform.position, handpos.transform.rotation) as GameObject;
        NetworkServer.Spawn(bomb);
        var bombController = bomb.GetComponent<BombController>();
        //设置炸弹
        _serverBombCon.SetBomb(playercore,bombController,BombExporeTime,ResetBomb);
        //开始倒计时
        _serverBombCon.StartTimeCount();
        playercore.isHand = true;
        RpcCreateBall(playercore.gameObject,bomb);
    }
    
    [ClientRpc]
    public void RpcCreateBall(GameObject player,GameObject bomb)
    {
        var playercore  = player.gameObject.GetComponent<PlayerCore>();
        var handpos = playercore.handPos;
        playercore.isHand = true;
        playercore.handBomb = bomb;
        bomb.transform.parent = handpos.transform;
        bomb.transform.localPosition =Vector3.zero;
        bomb.transform.gameObject.SetActive(true);
        bomb.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void ResetBomb()
    {
        StartCoroutine(DelayToRandom());
    }

    /// <summary>
    /// 三秒后自动部署炸弹
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayToRandom()
    {
        yield return new WaitForSeconds(gameConfig.BombRebornTime);
        RandomPlayer();
    }


    /// <summary>
    /// 扔出炸弹后时间
    /// </summary>
    public void SetThrowBallTimeCold()
    {
        if ( _serverBombCon.ExploreTime>BombThorwExploreTime)
        {
            _serverBombCon.ExploreTime = BombThorwExploreTime;
        }
    }
}
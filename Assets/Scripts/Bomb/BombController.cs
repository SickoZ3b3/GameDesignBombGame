using System;
using System.Collections;
using UnityEngine;
using Mirror;
public class BombController : NetworkBehaviour
{
    
    public float countdownDuration = 40f;
    public float throwableThreshold = 1f;
    public GameObject explosionEffectPrefab;
    public float explosionRadius = 5f;
    public LayerMask playerLayerMask;

    public bool isThrowable = false;
    private float currentCountdown;
    public GameManager gameManager;


    public Transform TargetHand;
    
    void Start()
    {
        currentCountdown = countdownDuration;
    }
    
    public void SyncTransform()
    {
        if (TargetHand!=null)
            this.transform.position = TargetHand.position;
    }

    private bool isStart;

    private float ExploreTime = 40;

    [Server]
    public void ServerExplode()
    {
        GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
         // Adjust the duration based on the particle system's lifetime
        NetworkServer.Spawn(explosionEffect);
        Collider[] playersHit = Physics.OverlapSphere(transform.position, explosionRadius, playerLayerMask);

        foreach (Collider player in playersHit)
        {
            // Call the player's death and resurrection script
            PlayerDeathController playerDeathController = player.GetComponent<PlayerDeathController>();
            if (playerDeathController != null)
            {
                // Debug.Log("playerDeathController find successfully");
                // playerDeathController.DieAndRespawn();
           //     RpcExplored(playerDeathController.gameObject);
                
                var playerDeath = playerDeathController.gameObject.GetComponent<PlayerDeathController>();
                playerDeath.RpcDieAndRespawn();
            }
            else
            {
                Debug.LogError("PlayerDeathController not found on the player. Make sure the script is attached to the player prefab.");
            }
        }
        NetworkServer.Destroy(gameObject);
        // 销毁炸弹
        RpcBombDestorySelf(gameObject);
         // Destroy the bomb after the explosion
    }

    // [ClientRpc]
    // public void RpcExplored(GameObject target)
    // {
    //     GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
    //     var playerDeath = target.GetComponent<PlayerDeathController>();
    //     playerDeath.DieAndRespawn();
    // }
    
    
    [ClientRpc]
    public void RpcBombDestorySelf(GameObject bomb)
    {
        Destroy(bomb);
    }
}

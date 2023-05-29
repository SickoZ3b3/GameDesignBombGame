using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using StarterAssets;

public class PlayerDeathController : NetworkBehaviour
{
    
    public GameConfig gameConfig;

    public float respawnTime
    {
        get
        {
            return gameConfig.PlayerRebornTime;
        }
    }
    public GameObject deathEffectPrefab;
    [SyncVar]
    private bool isDead = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private List<Renderer> playerRenderers;

    void Start()
    {
        playerRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }
    void DisableRenderer()
    {
        foreach (Renderer renderer in playerRenderers)
        {
            renderer.enabled = false;
        }
    }

    void EnableRenderer()
    {
        foreach (Renderer renderer in playerRenderers)
        {
            renderer.enabled = true;
        }
    }

    [ClientRpc]
    public void RpcDieAndRespawn()
    {
        if (isDead) return; // Prevent multiple death triggers
        isDead = true;
        StartCoroutine(DeathAndRespawnRoutine());
        if (!isLocalPlayer)
        {
            return;
        }

    }

    public bool IsPlayerDead()
    {
        return isDead;
    }


    IEnumerator DeathAndRespawnRoutine()
    {
        // Disable player movement and other abilities
        // GetComponent<PlayerMovement>().enabled = false;

        // Instantiate death effect prefab
        // Adjust the duration based on the particle system's lifetime

        // Optionally, make the player invisible or show a ragdoll effect
        DisableRenderer();
        // Wait for the respawn time
        yield return new WaitForSeconds(respawnTime);

        // Resurrect the player and re-enable their abilities
        // Example: GetComponent<PlayerMovement>().enabled = true;
        EnableRenderer();
        // Reset player's position and rotation to initial values
        if (isLocalPlayer)
        {
            isDead = false;
            this.transform.position = NetworkManager.startPositions[Random.Range(0, NetworkManager.startPositions.Count)].position;
        }
    }
}
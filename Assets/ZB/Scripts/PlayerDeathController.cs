using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    public float respawnTime = 10f;
    public GameObject deathEffectPrefab;
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

    public void DieAndRespawn()
    {
        if (isDead) return; // Prevent multiple death triggers

        isDead = true;
        StartCoroutine(DeathAndRespawnRoutine());
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
        GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        // Adjust the duration based on the particle system's lifetime

        // Optionally, make the player invisible or show a ragdoll effect
        DisableRenderer();
        FirstPersonController playerController = GetComponent<FirstPersonController>();
        playerController.canMove = false;
        // Wait for the respawn time
        yield return new WaitForSeconds(respawnTime);

        // Resurrect the player and re-enable their abilities
        // Example: GetComponent<PlayerMovement>().enabled = true;
        EnableRenderer();
        playerController.canMove = true;
        // Reset player's position and rotation to initial values
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        isDead = false;


    }
}
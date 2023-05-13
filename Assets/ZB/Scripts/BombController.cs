using System;
using System.Collections;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public float countdownDuration = 40f;
    public float throwableThreshold = 1f;
    public GameObject explosionEffectPrefab;
    public float explosionRadius = 5f;
    public LayerMask playerLayerMask;

    public bool isThrowable = false;
    private float currentCountdown;
    public GameManager gameManager;
    public event Action OnBombExploded;

    void Start()
    {
        currentCountdown = countdownDuration;
    }

    void Update()
    {
        currentCountdown -= Time.deltaTime;
        Debug.Log(currentCountdown);
        if (currentCountdown <= throwableThreshold && !isThrowable)
        {
            Debug.LogWarning("can throw");
            isThrowable = true;
        }

        if (currentCountdown <= 0)
        {
            Explode();
        }
    }

    void Explode()
    {
        GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
         // Adjust the duration based on the particle system's lifetime

        Collider[] playersHit = Physics.OverlapSphere(transform.position, explosionRadius, playerLayerMask);

        foreach (Collider player in playersHit)
        {
            // Call the player's death and resurrection script
            PlayerDeathController playerDeathController = player.GetComponent<PlayerDeathController>();
            if (playerDeathController != null)
            {
                Debug.Log("playerDeathController find successfully");
                playerDeathController.DieAndRespawn();
            }
            else
            {
                Debug.LogError("PlayerDeathController not found on the player. Make sure the script is attached to the player prefab.");
            }
        }

        if (OnBombExploded != null)
        {
            OnBombExploded();
        }

        Destroy(gameObject);
         // Destroy the bomb after the explosion
    }
}

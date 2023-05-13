using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float throwForce = 10f;
    public float passDistance = 5f;
    public LayerMask playerLayerMask;
    public BombController bombController;


    private Transform handTransform;

    void Start()
    {
        handTransform = transform.Find("Hand");
    }

    void Update()
    {
        
        GameObject bombInstance = GetBombInHand();
        if (bombInstance == null) return;

        if (Input.GetMouseButtonDown(0)) // Left click to pass
        {
            PassBomb(bombInstance);
        }
        else if (Input.GetMouseButtonDown(1)) // Right click to throw
        {
            ThrowBomb(bombInstance);
        }
    }

    GameObject GetBombInHand()
    {
        if (handTransform.childCount > 0)
        {
            return handTransform.GetChild(0).gameObject;
        }
        return null;
    }

    void PassBomb(GameObject bombInstance)
    {
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

            Transform targetHand = nearestPlayer.transform.Find("Hand");
            if (targetHand != null)
            {
                bombInstance.transform.SetParent(targetHand);
                bombInstance.transform.localPosition = Vector3.zero;
                bombInstance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogError("Target player's hand not found: " + nearestPlayer.name);
            }
        }
        else
        {
            Debug.LogWarning("No player found in range to pass the bomb.");
        }
    }






    void ThrowBomb(GameObject bombInstance)
    {
        BombController bombController = bombInstance.GetComponent<BombController>();

        if (bombController.isThrowable)
        {

            bombInstance.transform.SetParent(null); // Detach the bomb from the player's hand
            Rigidbody bombRigidbody = bombInstance.AddComponent<Rigidbody>(); // Add a Rigidbody component to the bomb for physics-based movement
            bombRigidbody.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        }
     }
        // Apply force to the bomb in the forward direction
    
}
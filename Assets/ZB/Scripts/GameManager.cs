using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject bombPrefab;
    private GameObject[] players;
    private GameObject bombInstance;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(AssignBombToRandomPlayer(0f));



    }
    private void HandleBombExploded()
    {
        BombController bombController = bombInstance.GetComponent<BombController>();
        bombController.OnBombExploded -= HandleBombExploded;
        StartCoroutine(AssignBombToRandomPlayer(2f));
    }


    public IEnumerator AssignBombToRandomPlayer(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        List<GameObject> alivePlayers = new List<GameObject>();

        foreach (GameObject player in players)
        {
            PlayerDeathController playerDeathController = player.GetComponent<PlayerDeathController>();
            if (!playerDeathController.IsPlayerDead())
            {
                alivePlayers.Add(player);
            }
        }

        if (alivePlayers.Count > 0)
        {
            int randomIndex = Random.Range(0, alivePlayers.Count);
            GameObject selectedPlayer = alivePlayers[randomIndex];
            bombInstance = Instantiate(bombPrefab, selectedPlayer.transform.position, Quaternion.identity);

            // Subscribe to the OnBombExploded event
            BombController bombController = bombInstance.GetComponent<BombController>();
            bombController.OnBombExploded += HandleBombExploded;

            // Attach the bomb to the player's hand (assuming the hand is a child object with the tag "Hand")
            Transform playerHand = selectedPlayer.transform.Find("Hand");
            if (playerHand != null)
            {
                bombInstance.transform.SetParent(playerHand);
                bombInstance.transform.localPosition = Vector3.zero;
                bombInstance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogError("Hand not found in the player's hierarchy. Make sure there's a child object with the tag 'Hand'.");
            }
        }
        else
        {
            // Handle the case where all players are dead
            Debug.LogWarning("All players are dead. No bomb assigned.");
        }
    }

}

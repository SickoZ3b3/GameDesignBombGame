using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerUI : MonoBehaviour
{

    public Button GameStart;
    // Start is called before the first frame update
    void Start()
    {
        GameStart.onClick.AddListener(()=>StartGame());
    }

    public void StartGame()
    {
        GameManager.Instance.RandomPlayer();
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

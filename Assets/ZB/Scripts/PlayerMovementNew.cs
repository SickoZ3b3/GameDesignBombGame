using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNew : NetworkBehaviour
{
    // Start is called before the first frame update

    void HandleMovement(){
        if(isLocalPlayer){
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal,moveVertical,0);
            transform.position = transform.position +movement;
        }

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
}

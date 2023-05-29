using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMoveMent : NetworkBehaviour,IPlayer
{
    public float MoveSpeed;
    private float horizontal;
    private float vertical;

    public float gravity = 20f;

    public float JumpSpeed = 11f;

    public CharacterController PlayerController;
    Vector3 Player_Move;

    private void Start()
    {
        PlayerController = this.GetComponent<CharacterController>();
    }

    public void StatusUpdate()
    {
        if (!isLocalPlayer) return;
       
        if (PlayerController.isGrounded)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Player_Move = (transform.forward * vertical + transform.right * horizontal) * MoveSpeed;

            //判断玩家是否按下空格键
            if (Input.GetAxis("Jump") == 1)
            {
                Player_Move.y =JumpSpeed;
            }
        }
        Player_Move.y = Player_Move.y - gravity * Time.deltaTime;

        PlayerController.Move(Player_Move * Time.deltaTime);
    }
}
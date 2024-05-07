using System;
using System.Collections;
using System.Collections.Generic;
using MyCraftS.Player.Data;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInput : MonoBehaviour
{
    public Entity playerEntity;
    public World world;
    
 
    public Transform cam;
 
    public float jumpMaxHeight = 1.5f;
 
    public float gravity = -15f;

    public float RunSpeed = 3.370f;
    public float WalkSpeed = 1.289f;

    public float maxFallSpeed = -20f;
    private float Vertical_Target_Speed = 0;
    private float Horizontal_Target_Speed = 0;

    // Start is called before the first frame update
    private Vector3 Speed = new Vector3(0,0,0);
    private Vector3[] last3Speed;
    private int lastIndex = 0;
    
    
    private bool isRun = false;
    private bool isJump = false;
    private bool isMidAir = false;
    void Awake()
    {
        last3Speed = new Vector3[3];
        for(var i = 0; i < 3; i++)
        {
            last3Speed[i] = new Vector3(0,0,0);
        }

        world = World.DefaultGameObjectInjectionWorld;


    }

    // Update is called once per frame
    void Update()
    {
        if(playerEntity==Entity.Null)
        {
            playerEntity = PlayerDataContainer.playerEntity;
        }
        Move();
    }

    private void FixedUpdate()
    {
        Move();

    }

 
    private bool isGrounded()
    {
        throw new Exception();
        //return playerEntity.gameClientPhysicEntityCollideRes.isGrounded;
    }
    private void CaculateGravity()
    {
        if (isGrounded())
        {
            Speed.y= gravity * Time.fixedDeltaTime;
            isMidAir = false;
        }
        else
        {
            Speed.y += gravity * Time.fixedDeltaTime;
            if (Speed.y < maxFallSpeed)
            {
                Speed.y = maxFallSpeed;
            }
        }
    }

    private Vector3 CaculateAve()
    {
        Vector3 tmp = new Vector3(0,0,0);
        for(var i = 0; i < 3; i++)
        {
            tmp += last3Speed[i];
        }
        return tmp / 3f;
    }
    private void Move()
    {

        Vector3 cameraForward = cam.forward;
        Vector3 cameraRight = cam.right;
        Vector3 targetSpeed = new Vector3(0,0,0),deltaPos = new Vector3(0,0,0);
        cameraForward.y = 0; 
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        CaculateGravity();


        
        if (isJump)
        {
            Speed.y += Mathf.Sqrt(2 * -gravity * jumpMaxHeight);
            isJump = false;
            isMidAir = true;
            Vector3 ave = CaculateAve();
            Speed.x = ave.x;
            Speed.z = ave.z;
        }

        if (!isMidAir)
        {
            Speed.z = Mathf.Lerp(Speed.z, Vertical_Target_Speed, 0.5f);
            Speed.x = Mathf.Lerp(Speed.x, Horizontal_Target_Speed, 0.5f);

        }

        targetSpeed = cameraForward * Speed.z + cameraRight * Speed.x;//修正方向
        targetSpeed.y = Speed.y;
        deltaPos = targetSpeed * Time.fixedDeltaTime;

        last3Speed[lastIndex].x = Speed.x;
        last3Speed[lastIndex].z = Speed.z;
        lastIndex = (lastIndex + 1) % 3;
 
 
        //  playerEntity.ReplaceGameClientPhysicEntityVelocity(targetSpeed);
        //playerEntity.ReplaceEntityVelocity(entityVelocity);
        //characterController.Move(deltaPos);

    }


    public void onMove(InputAction.CallbackContext ctx)
    {
        Vector2 vector2 = ctx.ReadValue<Vector2>();
        
        Vertical_Target_Speed = vector2.y*(isRun?RunSpeed:WalkSpeed);
        Horizontal_Target_Speed = vector2.x*(isRun?RunSpeed:WalkSpeed);
        //Debug.Log($"{Horizontal_Speed},{Vertical_Speed}");
    }
    
    public void onRun(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Performed)
        {
            isRun = true;
            Vertical_Target_Speed = Vertical_Target_Speed * RunSpeed / WalkSpeed;
            Horizontal_Target_Speed = Horizontal_Target_Speed * RunSpeed / WalkSpeed;
        }
        else if(ctx.phase == InputActionPhase.Canceled)
        {
            isRun = false;
            Vertical_Target_Speed = Vertical_Target_Speed * WalkSpeed / RunSpeed;
            Horizontal_Target_Speed = Horizontal_Target_Speed * WalkSpeed / RunSpeed;
        }
    
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Performed)
        {
            //Debug.Log("Jump");
            if (isGrounded())
            {
                isJump = true;
                float jumpSpeed = Mathf.Sqrt(2 * -gravity * jumpMaxHeight);
                return;
            }

        }
    }

}

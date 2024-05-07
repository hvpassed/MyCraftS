using System;
using System.Collections;
using System.Collections.Generic;
using MyCraftS.Input;
using MyCraftS.Player.Data;
using MyCraftS.Setting;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class PlayerPositionUpdate : MonoBehaviour
{
    public Entity playerRelatedEntity;
    public Entity cameraRelatedEntity;
    public World world;
     

    private Vector3 _velocity;
    private void Awake()
    {
        world = World.DefaultGameObjectInjectionWorld;
    }

    
    private void LateUpdate()
    {
        if (playerRelatedEntity == Entity.Null)
        {
            playerRelatedEntity = PlayerDataContainer.playerEntity;
            
            return;
        }

        if (cameraRelatedEntity == Entity.Null)
        {
            cameraRelatedEntity = PlayerDataContainer.cameraEntity;
            return;
        }
        var entityManager = world.EntityManager;
        var playerPosition = entityManager.GetComponentData<LocalTransform>(playerRelatedEntity);
        var velocityComponent =
            entityManager.GetComponentData<PhysicsVelocity>(playerRelatedEntity);
        var xRotation =entityManager.GetComponentData<CameraStatus>(cameraRelatedEntity);
        var offset = entityManager.GetComponentData<CameraOffSet>(cameraRelatedEntity);
        _velocity = velocityComponent.Linear;
        transform.position = (Vector3)playerPosition.Position+(Vector3)SettingManager.PlayerSetting.CameraOffset;
        transform.rotation = playerPosition.Rotation;
        transform.localEulerAngles = new Vector3(xRotation.xRotation, transform.localEulerAngles.y, 0);
        entityManager.SetComponentData<CameraForward>(cameraRelatedEntity,new CameraForward()
        {

            direction= transform.forward
        });
         
    }
}

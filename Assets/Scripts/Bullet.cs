using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{

    public GameObject origin;

    public float velocity = 5f;
    public float lifeTime = 3f;
    public Vector3 direction;

    public override void OnStartServer()
    {
        base.OnStartServer();

        GetComponent<Rigidbody>().velocity = direction * velocity;
        Invoke("Server_DestroyBullet", lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {    

        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            if (origin != playerController.gameObject)
            {
                playerController.TakeDamage(playerController);
            }
            else
            {
                Server_DestroyBullet();
            }
        }
        else
        {
            Server_DestroyBullet();
        }
        

    }

    public void Server_DestroyBullet()
    {
        base.Despawn(gameObject);
    }

}

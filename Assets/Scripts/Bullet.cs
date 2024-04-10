using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{

    public float velocity = 5f;
    public float lifeTime = 3f;
    public Vector3 direction;

    
    private void Start()
    {
        
        Invoke("Server_DestroyBullet", lifeTime);
    }

    void Update()
    {
        GetComponent<Rigidbody>().velocity = direction * velocity;        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            
                Debug.Log(playerController.ClientManager.Connection.ToString());
                Debug.Log(base.Owner.ToString());
                playerController.TakeDamage(playerController);
        }

        if( base.IsOwner)
            Server_DestroyBullet();
    }

    
    public void Server_DestroyBullet()
    {
        base.Despawn(gameObject);
    }

}

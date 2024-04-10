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
        Invoke("DestroyLifetime", lifeTime);
    }

    void Update()
    {
        GetComponent<Rigidbody>().velocity = direction * velocity;        
    }

    void DestroyLifetime()
    {
        Server_DestroyBullet();
        //Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.TakeDamage(playerController);
        }

        Server_DestroyBullet();
    }

    [ServerRpc(RequireOwnership = false)]
    public void Server_DestroyBullet()
    {
        InstanceFinder.ServerManager.Despawn(gameObject);
        DestroyInstance();
    }

    [ObserversRpc]
    public void DestroyInstance()
    {
        Destroy(gameObject);
    }

}

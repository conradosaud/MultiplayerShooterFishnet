using FishNet;
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
    public int clientId;

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
        if( other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.TakeDamage();
            Server_DestroyBullet();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void Server_DestroyBullet()
    {
        Debug.Log("Destróido");
        Despawn();
    }

}

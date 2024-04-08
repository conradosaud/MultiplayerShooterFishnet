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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if( other.CompareTag("Player") ){
    //        //if( other.GetComponent<PlayerController>().IsOwner )
    //            other.GetComponent<PlayerController>().TakeDamage();
    //    }

    //    Server_DestroyBullet();
    //    //Destroy(gameObject);
    //}

    [ServerRpc(RequireOwnership = false)]
    public void Server_DestroyBullet()
    {
        Debug.Log("Destróido");
        ServerManager.Despawn(gameObject);
    }

}

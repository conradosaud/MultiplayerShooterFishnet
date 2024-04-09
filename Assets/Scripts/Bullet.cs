using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{

    public float velocity = 5f;
    public float lifeTime = 3f;
    public Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyLifetime", lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = direction * velocity;
    }

    void DestroyLifetime()
    {
        Server_DestroyBullet();
        //Destroy(gameObject);
    }

    [Command(requiresAuthority = false)]
    public void Server_DestroyBullet()
    {
        Debug.Log("Destróido");
        //NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("acertou alguem...");
        if( other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.TakeDamage();
        }

        Server_DestroyBullet();
    }

}

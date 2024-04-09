using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{


    public Transform bulletPrefab;
    public Transform bulletPoint;

    public float healthPoints;

    public float shootFrequency = 0.2f;
    bool canShoot = true;

    public float moveSpeed = 5.0f;
    public float turnSpeed = 5f;
    public float jumpForce = 5.0f;
    public float jumpTime = 1.5f;

    public float lookXLimit = 45;
    public float gravity = 9.8f;

    float rotationX = 0;

    Vector3 moveDirection = Vector3.zero;

    Camera playerCamera;
    CharacterController cc;


    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.isLocalPlayer)
        {
            cc = GetComponent<CharacterController>();
            playerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        }
        else
        {
            GetComponent<PlayerController>().enabled = false;
            transform.Find("Main Camera").gameObject.SetActive(false);
        }

    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        // Inputs
        float directionX = Input.GetAxis("Horizontal");
        float directionZ = Input.GetAxis("Vertical");
        float directionY = moveDirection.y;

        if (Input.GetKey(KeyCode.Mouse0) && canShoot)
        {
            Vector3 cameraDirection = playerCamera.transform.forward;
            canShoot = false;
            Server_Shoot(cameraDirection);
        }

        // Directions
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Player movement
        moveDirection = (forward * directionZ) + (right * directionX);
        moveDirection *= moveSpeed;

        // Player jump
        if (Input.GetKeyDown(KeyCode.Space) && cc.isGrounded)
        {
            moveDirection.y = jumpForce;
        }
        else
        {
            moveDirection.y = directionY;
        }

        // Add gravity
        moveDirection.y -= gravity * Time.deltaTime;
        // Move player
        cc.Move(moveDirection * Time.deltaTime);

        // Player rotation
        rotationX -= Input.GetAxis("Mouse Y") * turnSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * turnSpeed, 0);

    }


    [Command]
    void Server_Shoot(Vector3 cameraDirection)
    {

        Transform instantiated = Instantiate(bulletPrefab, bulletPoint.position, Quaternion.Euler(cameraDirection));
        instantiated.transform.parent = GameObject.Find("BulletPool").transform;
        instantiated.GetComponent<Bullet>().direction = cameraDirection;

        NetworkServer.Spawn(instantiated.gameObject);
        //Spawn(instantiated.gameObject);

        // Aguarda o tempo aqui pois não é possível usar IEnumerator no TargetRPC
        StartCoroutine(ResetShoot(connectionToClient));
        IEnumerator ResetShoot(NetworkConnection conn)
        {
            yield return new WaitForSeconds(shootFrequency);
            this.ResetShoot(conn);
        }

    }

    [TargetRpc]
    void ResetShoot(NetworkConnection conn)
    {
        canShoot = true;
    }

    [Command(requiresAuthority = false)]
    public void TakeDamage()
    {
        healthPoints--;
        HUDManager.lifeText.text = healthPoints.ToString();
    }

}

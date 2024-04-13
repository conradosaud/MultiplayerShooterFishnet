using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    public Transform bulletPrefab;
    public Transform bulletPoint;

    public float shootFrequency = 0.2f;
    bool canShoot = true;

    public float healthPoints = 5;

    public float moveSpeed = 5.0f;
    public float turnSpeed = 5f; 
    public float jumpForce = 5.0f;
    public float jumpTime = 1.5f;

    public float lookXLimit = 45;
    public float gravity = 9.8f;

    float rotationX = 0;

    CharacterController cc;
    Camera playerCamera;
    Vector3 moveDirection = Vector3.zero;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if( base.IsOwner)
        {
            IniciaController();
        }

    }

    void IniciaController()
    {
        cc = GetComponent<CharacterController>();
        playerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        playerCamera.gameObject.SetActive(true);
        gameObject.name += "-" + Owner.ClientId;

        // Lock camera
        // Desabilitar para facilitar o desenvolvimento
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

    }


    void Update()
    {

        if (!base.IsOwner)
            return;

        // Inputs
        float directionX = Input.GetAxis("Horizontal");
        float directionZ = Input.GetAxis("Vertical");
        float directionY = moveDirection.y;

        if( Input.GetKey(KeyCode.Mouse0) && canShoot )
        {
            Vector3 cameraDirection = playerCamera.transform.forward;
            canShoot = false;
            Server_Shoot(cameraDirection, Owner);
        }

        if( Input.GetKeyDown(KeyCode.V))
        {
            HUDController.lifeText.text = "opaa";
        }

        // Directions
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Player movement
        moveDirection = (forward * directionZ) + (right * directionX);
        moveDirection *= moveSpeed;

        // Player jump
        if( Input.GetKeyDown(KeyCode.Space) && cc.isGrounded)
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

    [TargetRpc]
    void ResetShoot(NetworkConnection conn)
    {
        canShoot = true;
    }

    [ServerRpc]
    void Server_Shoot(Vector3 cameraDirection, NetworkConnection conn)
    {

        Transform instantiated = Instantiate(bulletPrefab, bulletPoint.position, Quaternion.Euler(cameraDirection));
        instantiated.GetComponent<Bullet>().direction = cameraDirection;
        instantiated.GetComponent<Bullet>().nob = GetComponent<NetworkObject>();

        Spawn(instantiated.gameObject);

        // Aguarda o tempo aqui pois não é possível usar IEnumerator no TargetRPC
        StartCoroutine(ResetShoot(conn));
        IEnumerator ResetShoot(NetworkConnection conn)
        {
            yield return new WaitForSeconds(shootFrequency);
            this.ResetShoot(conn);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage(PlayerController script)
    {
        script.healthPoints--;
        UpdateLifeDisplay(script.Owner, script.healthPoints.ToString());
    }

    [TargetRpc]
    void UpdateLifeDisplay(NetworkConnection conn, string value)
    {
        HUDController.lifeText.text = value;
    }

}

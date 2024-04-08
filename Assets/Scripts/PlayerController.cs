using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        else
        {
            GetComponent<PlayerController>().enabled = false;
            transform.Find("Main Camera").gameObject.SetActive(false);
        }

    }

    void IniciaController()
    {
        cc = GetComponent<CharacterController>();
        playerCamera = transform.Find("Main Camera").GetComponent<Camera>();

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
            Server_Shoot(cameraDirection, LocalConnection);
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

        ServerManager.Spawn(instantiated.gameObject);

        // Aguarda o tempo aqui pois não é possível usar IEnumerator no TargetRPC
        StartCoroutine(ResetShoot(conn));
        IEnumerator ResetShoot(NetworkConnection conn)
        {
            yield return new WaitForSeconds(shootFrequency);
            this.ResetShoot(conn);
        }

        // Outra forma de realizar o tiro, com Raycast, mas sem projétil na scene
        void ShootRaycast()
        {

            Vector3 position = playerCamera.transform.position;
            Vector3 direction = playerCamera.transform.forward;

            if (Physics.Raycast(position, direction, out RaycastHit hit))
            {
                Debug.Log(hit.transform.gameObject.name);
            }
        }

    }

    [ServerRpc]
    public void TakeDamage()
    {
        Debug.Log("Reduziu a vida...");
        healthPoints--;
        HUDController.lifeText.text = healthPoints.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Bullet>(out Bullet bullet))
        {
            Debug.Log("acertô");
            TakeDamage();
            bullet.Server_DestroyBullet();
        }
    }


}

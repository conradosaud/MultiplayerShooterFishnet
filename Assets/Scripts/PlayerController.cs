using FishNet;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Utility.Performance;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{

    string username;
    TextMeshProUGUI headUsernameText;

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

    bool isDead;

    CharacterController cc;
    Camera playerCamera;
    Vector3 moveDirection = Vector3.zero;

    void Awake()
    {
        headUsernameText = transform.Find("CanvasPlayerName").transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
            return;

        InitializePlayer();

    }

    void Update()
    {

        if ( base.IsOwner == false )
            return;

        if ( isDead == true )
            return;

        // Simple player inputs
        float directionX = Input.GetAxis("Horizontal");
        float directionZ = Input.GetAxis("Vertical");
        float directionY = moveDirection.y;

        if( canShoot == true && ( Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Joystick1Button5) ) )
        {
            canShoot = false;
            Vector3 cameraDirection = playerCamera.transform.forward;
            Server_Shoot(Owner, cameraDirection);
        }

        // Directions
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Player movement
        moveDirection = (forward * directionZ) + (right * directionX);
        moveDirection *= moveSpeed;

        // Player jump
        if( cc.isGrounded == true && ( Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button2) ) )
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

    void InitializePlayer()
    {

        // --- First part: initialize player components and settings

        cc = GetComponent<CharacterController>();
        playerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        playerCamera.gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("HUD").transform.Find("Slider").GetComponent<Slider>().onValueChanged.AddListener((value) => turnSpeed = value);

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        Server_GetMyName();
        //HUDController.lifeText.text = "5";

        // --- Second part: used by respawn player after die

        transform.Find("CanvasDead").GetComponent<Canvas>().enabled = false;
        GetComponent<Renderer>().enabled = true;
        transform.Find("Visor").GetComponent<Renderer>().enabled = true;
        transform.Find("CanvasPlayerName").GetComponent<Canvas>().enabled = true;
        GetComponent<CharacterController>().enabled = true;
        isDead = false;
        healthPoints = 5;
        HUDController.lifeText.text = healthPoints.ToString();

    }

    public void DesactivatePlayer()
    {
        isDead = true;
        GetComponent<Renderer>().enabled = false;
        transform.Find("Visor").GetComponent<Renderer>().enabled = false;
        transform.Find("CanvasPlayerName").GetComponent<Canvas>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
    }

    // -------------------------------------------------------------------------------
    // Network section
    // -------------------------------------------------------------------------------

    [TargetRpc]
    public void Target_CheckAndDie(NetworkConnection conn)
    {

        if (healthPoints > 0)
        {
            return;
        }

        transform.Find("CanvasDead").GetComponent<Canvas>().enabled = true;
        Server_DesactivatePlayer();

    }

    [ServerRpc]
    void Server_DesactivatePlayer()
    {
        Observers_DesactivatePlayer();
    }
    [ObserversRpc(BufferLast = true)]
    void Observers_DesactivatePlayer()
    {
        DesactivatePlayer();
    }

    [ServerRpc]
    public void Server_RespawnPlayer()
    {
        Observers_RespawnPlayer();
    }
    [ObserversRpc(BufferLast = true)]
    void Observers_RespawnPlayer()
    {
        InitializePlayer();  
    }


    [ServerRpc]
    void Server_Shoot(NetworkConnection conn, Vector3 cameraDirection)
    {

        Transform instantiated = Instantiate(bulletPrefab, bulletPoint.position, Quaternion.Euler(cameraDirection));
        instantiated.GetComponent<Bullet>().direction = cameraDirection;
        instantiated.GetComponent<Bullet>().nob = GetComponent<NetworkObject>();

        Spawn(instantiated.gameObject);

        // Wait the time there cause isnt possible use IEnumerator on TargeRpc
        StartCoroutine(ResetShoot(conn));
        IEnumerator ResetShoot(NetworkConnection conn)
        {
            yield return new WaitForSeconds(shootFrequency);
            Target_ResetShoot(conn);
        }

    }
    [TargetRpc]
    void Target_ResetShoot(NetworkConnection conn)
    {
        canShoot = true;
    }


    [TargetRpc]
    public void Target_TakeDamage(NetworkConnection conn)
    {
        Server_TakeDamage(this);
    }
    [ServerRpc]
    public void Server_TakeDamage(PlayerController script)
    {
        Target_UpdateLifeDisplay(Owner);
        Target_CheckAndDie(script.Owner);
    }

    [TargetRpc]
    void Target_UpdateLifeDisplay(NetworkConnection conn)
    {
        healthPoints--;
        HUDController.lifeText.text = healthPoints.ToString();
    }

    [ServerRpc]
    void Server_GetMyName()
    {
        username = GameManager.instance.GetMyName(Owner);
        Oberservers_ShowMyNameOnHead(username);
    }
    [ObserversRpc(ExcludeOwner = true, BufferLast = true)]
    void Oberservers_ShowMyNameOnHead(string username)
    {
        headUsernameText.text = username;
    }
}

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

        cc = GetComponent<CharacterController>();
        GameManager.instance.defaultCamera.gameObject.SetActive(false);
        playerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        playerCamera.gameObject.SetActive(true);
        gameObject.name += "-" + Owner.ClientId;

        GetMyName();

        HUDController.lifeText.text = "5";
        //UpdateLifeDisplay(base.Owner, "5");

    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        GameManager.instance.defaultCamera.gameObject.SetActive(true);
    }

    void Update()
    {

        if (!base.IsOwner)
            return;

        if (isDead)
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

        // Directions
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Player movement
        moveDirection = (forward * directionZ) + (right * directionX);
        moveDirection *= moveSpeed;

        if( Input.GetKeyDown(KeyCode.V))
        {
            VerifyIsDead(Owner);
        }

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
    public void VerifyIsDead(NetworkConnection conn)
    {
        if (healthPoints > 0)
            return;

        Server_DesactivatePlayer();

        GameObject.Find("Canvas").transform.Find("HUD").transform.Find("Dead").GetComponent<Canvas>().enabled = true;
        GameObject.Find("Canvas").transform.Find("HUD").transform.Find("Dead").transform.Find("Respawn").GetComponent<Button>().onClick.AddListener(() =>
        {
            Server_RespawnPlayer(conn);
        });

    }

    [ServerRpc]
    void Server_DesactivatePlayer()
    {
        DesactivatePlayer();
    }
    [ObserversRpc]
    void DesactivatePlayer()
    {

        GetComponent<Renderer>().enabled = false;
        transform.Find("Visor").GetComponent<Renderer>().enabled = false;
        transform.Find("CanvasPlayerName").GetComponent<Canvas>().enabled = false;
        GetComponent<Collider>().enabled = false;
        isDead = true;
    }

    [ServerRpc]
    void Server_RespawnPlayer(NetworkConnection conn)
    {
        RespawnPlayer(conn);
    }
    [ObserversRpc]
    void RespawnPlayer(NetworkConnection conn)
    {
        GameObject.Find("Canvas").transform.Find("HUD").transform.Find("Dead").GetComponent<Canvas>().enabled = false;
        GetComponent<Renderer>().enabled = true;
        transform.Find("Visor").GetComponent<Renderer>().enabled = true;
        transform.Find("CanvasPlayerName").GetComponent<Canvas>().enabled = true;
        GetComponent<Collider>().enabled = true;
        isDead = false;
        healthPoints = 5;
        UpdateLifeDisplay(Owner, healthPoints.ToString());
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

        // Aguarda o tempo aqui pois n�o � poss�vel usar IEnumerator no TargetRPC
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
        script.VerifyIsDead(script.Owner);
    }

    [TargetRpc]
    void UpdateLifeDisplay(NetworkConnection conn, string value)
    {
        HUDController.lifeText.text = value;
    }

    [ServerRpc]
    void GetMyName()
    {
        username = GameManager.instance.GetMyName(Owner);
        ShowMyNameOnHead(username);
    }

    [ObserversRpc(ExcludeOwner = true, BufferLast = true)]
    void ShowMyNameOnHead(string username)
    {
        headUsernameText.text = username;
    }
}

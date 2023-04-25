using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    //[SerializeField]float speed= 10f;
    private float baseSpeed;
    [SerializeField] private float jumpForce = 10f;
    private float gravBase;
    [SerializeField] private Transform playerFeet;
    [SerializeField] private Animator camAnim;
    private bool isWalking = false;
    private float groundDist = 0.1f;
    public LayerMask GroundLayer;
    [SerializeField] float jumpTime;
    private bool canClimp = false;
    [SerializeField] private Firering[] gunScript;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool leftWallNearby;
    private bool rightWallNearBy;
    [SerializeField] private Transform[] wallChecks;
    public GameObject playerView;
    public float lerpTime;
    public Transform idlePos;
    public Transform reversePos;
    public Transform leftLean;
    public Transform rightLean;
    [SerializeField] private GameObject Radius;
    public bool weaponInUse = false;
    private float timePassed;
    private bool isLeaning;
    private bool useGravity = true;
    private bool reverseGravitation = false;
    public MouseLoock mL;
    [SerializeField] private float mass = 85f;
    private bool roofRun;
    private float jumpHight = 10f;
    private float jumpHightRef;
    public float speed = 6f;            // Die Geschwindigkeit, mit der sich der Charakter bewegt
    public float jumpSpeed = 8f;        // Die Geschwindigkeit, mit der der Charakter springt
    public float gravity = 20f;         // Die Gravitation, die auf den Charakter wirkt
    int jumpCounter = 0;
    public int WeaponIndex;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private bool isJumping;

    void Start()
    {
        jumpHightRef = jumpHight;
        Debug.Log("start als update lel");
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        gravBase = gravity;
        baseSpeed = speed;
    }

    void Update()
    {
        // Input abrufen
        //originalRotation = transform.localRotation;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if ((horizontal != 0 || vertical != 0) && !isWalking)
        {
            isWalking = true;
            camAnim.SetBool("isWalking", true);
        }
        else if ((vertical == 0 && horizontal == 0) && isWalking)
        {
            isWalking = false;
            camAnim.SetBool("isWalking", false);
        }

        // Charakter bewegen
        moveDirection = new Vector3(horizontal, 0f, vertical);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;

        // Sprung abrufen
        if (HandleIsGrounded())
        {
            jumpCounter = 0;
            isJumping = false;
        }
        if (Input.GetButtonDown("Jump") && (HandleIsGrounded() | jumpCounter < 2))
        {
            jumpCounter++;
            moveDirection.y += Mathf.Sqrt(jumpForce * jumpHight * gravity) * Time.fixedDeltaTime;
            Debug.Log("total jumppower: " + Mathf.Sqrt(jumpForce * jumpHight * gravity) * Time.fixedDeltaTime + "jump force: " + jumpForce + "Jump Hight" + jumpHight + "gravity: " + gravity +
            "time.deltatime: " + Time.fixedDeltaTime);
        }
        // Gravitation auf den Charakter anwenden
        if (useGravity)
        {
            if (!reverseGravitation)
            {
                moveDirection.y += -gravity * mass * Time.deltaTime;
            }
            else
            {
                moveDirection.y += gravity * mass * Time.deltaTime;
            }
        }

        // Charakter bewegen
        controller.Move(moveDirection * Time.deltaTime);
        if (Input.GetKey(KeyCode.Space) && canClimp == true)
        {
            moveDirection.y += jumpForce;
        }
        //sprint 
        if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.LeftShift) && isSprinting == false)
        {
            isSprinting = true;
            if (weaponInUse)
            {
                gunScript[WeaponIndex].turnlightoff();
                gunScript[WeaponIndex].shootAble = false;
                gunScript[WeaponIndex].WeaponAnim.SetBool("isSprinting", true);
            }
            speed *= 2.5f;
        }
        //spieler hört auf zu sprinten
        if (Input.GetKeyUp(KeyCode.LeftShift) | Input.GetButton("Fire1") | Input.GetButton("Fire2"))
        {

            isSprinting = false;
            if (weaponInUse)
            {
                gunScript[WeaponIndex].shootAble = true;
                gunScript[WeaponIndex].WeaponAnim.SetBool("isSprinting", false);
            }
            speed = baseSpeed;
        }
        //guck mal nach, dass der Spieler in der nähe einer Wand ist
        if (!useGravity)
        {
            rightWallRun(ref rightWallNearBy);
            leftWallRun(ref leftWallNearby);
        }

        //wall run
        if (isJumping && leftWallRun(ref leftWallNearby) && useGravity && jumpCounter < 2)
        {
            useGravity = false;
            playerView.transform.rotation = leftLean.rotation;
            playerView.transform.position = leftLean.position;
        }

        if (isJumping && rightWallRun(ref rightWallNearBy) && useGravity && jumpCounter < 2)
        {
            useGravity = false;
            playerView.transform.rotation = rightLean.rotation;
            playerView.transform.position = rightLean.position;
        }

        //spieler stoppt wall running
        if (!rightWallRun(ref rightWallNearBy) | !leftWallRun(ref leftWallNearby))
        {
            jumpHight = jumpHightRef;
            playerView.transform.rotation = idlePos.rotation;
            playerView.transform.position = idlePos.position;
            rightWallNearBy = false;
            leftWallNearby = false;
            useGravity = true;
        }

        //spieler soll sich ducken 
        if (Input.GetKey(KeyCode.C))
        {
            isSprinting = false;
            speed = 5f;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            if (weaponInUse)
            {
                gunScript[WeaponIndex].WeaponAnim.SetBool("isWalking", true);
            }
        }
        //spieler steht auf
        if (Input.GetKeyUp(KeyCode.C))
        {
            speed = baseSpeed;
            transform.localScale = new Vector3(1f, 1f, 1f);
            if (weaponInUse)
            {
                gunScript[WeaponIndex].WeaponAnim.SetBool("isWalking", false);
            }
        }

        //lehnen 
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            timePassed = 0;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            if (timePassed < lerpTime && !isLeaning)
            {
                playerView.transform.SetPositionAndRotation(Vector3.Slerp(playerView.transform.position, leftLean.position, timePassed / lerpTime), Quaternion.Slerp(playerView.transform.rotation, leftLean.rotation, timePassed / lerpTime));
                timePassed += Time.deltaTime;
            }

            if (timePassed >= lerpTime)
            {
                isLeaning = true;
                playerView.transform.rotation = leftLean.rotation;
            }

        }
        else if (Input.GetKey(KeyCode.E))
        {

            if (timePassed < lerpTime && !isLeaning)
            {
                playerView.transform.SetPositionAndRotation(Vector3.Slerp(playerView.transform.position, rightLean.position, timePassed / lerpTime), Quaternion.Slerp(playerView.transform.rotation, rightLean.rotation, timePassed / 1));
                timePassed += Time.deltaTime;
            }
            if (timePassed >= lerpTime)
            {
                isLeaning = true;
                playerView.transform.rotation = rightLean.rotation;
            }
        }
        //stopp lehnen 
        if (Input.GetKeyUp(KeyCode.E) | Input.GetKeyUp(KeyCode.Q))
        {
            if (timePassed < lerpTime && isLeaning)
            {
                timePassed = 0;
                playerView.transform.SetPositionAndRotation(Vector3.Slerp(playerView.transform.position, idlePos.position, timePassed / lerpTime), Quaternion.Slerp(playerView.transform.rotation, idlePos.rotation, timePassed / lerpTime));
                timePassed += Time.deltaTime;
            }
            if (timePassed >= lerpTime)
            {
                isLeaning = false;
                playerView.transform.rotation = idlePos.rotation;
            }
        }
    }
    //ground check
    private bool HandleIsGrounded()
    {
        if (Physics.CheckSphere(playerFeet.position, groundDist, GroundLayer))
        {
            isJumping = true;
            if (!reverseGravitation && jumpCounter == 2 && roofRun)
            {
                reverseGravitation = true;
                transform.Rotate(-180, transform.rotation.y - 180, 0);
            }
            Debug.Log(jumpCounter);
            return true;
        }
        return false;
    }
    //wall run check
    private bool leftWallRun(ref bool wallNearBy)
    {
        RaycastHit wall;
        if (Physics.Raycast(wallChecks[0].transform.position, wallChecks[0].TransformDirection(Vector3.left), out wall, 1f))
        {
            if (wall.transform.CompareTag("Building"))
            {
                jumpHight = 0f;
                useGravity = false;
                wallNearBy = true;
                return wallNearBy;
            }
        }
        return false;
    }
    private bool rightWallRun(ref bool wallNearBy)
    {
        RaycastHit wall;
        if (Physics.Raycast(wallChecks[1].transform.position, wallChecks[1].TransformDirection(Vector3.left), out wall, 1f))
        {
            if (wall.transform.CompareTag("Building"))
            {
                jumpHight = 0f;
                useGravity = false;
                Debug.Log("is on wall");
                wallNearBy = true;
                return wallNearBy;
            }
        }
        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        //ausserhalb des gebäudes = stopp reverse gravitation
        if (other.CompareTag("outofBuilding") && reverseGravitation)
        {
            roofRun = false;
            reverseGravitation = false;
            transform.Rotate(-180, transform.rotation.y + 180, 0);
        }
        //im gebäude erlaube reverse gravitation
        if (other.CompareTag("Building"))
        {
            roofRun = true;
        }
        //leiter check 
        if (other.CompareTag("MagnetLeiter2"))
        {
            useGravity = false;
            canClimp = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //klettern net mehr verfügbar 
        if (other.CompareTag("MagnetLeiter2"))
        {

            //gravity = gravBase;
            useGravity = true;
            canClimp = false;
        }
    }
}
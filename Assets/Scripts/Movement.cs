using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    //[SerializeField]float speed= 10f;
    private float baseSpeed;

    private float gravBase;

    private bool isWalking = false;
    private float groundDist = 0.2f;
    public LayerMask GroundLayer;

    private bool canClimp = false;

    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool leftWallNearby;
    private bool rightWallNearBy;
    private float timePassed;
    private bool isLeaning;
    private bool useGravity = true;
    private bool reverseGravitation = false;
    private bool roofRun;
    private float jumpHight = 10f;
    private float jumpHightRef;
    private int jumpCounter = 0;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private bool isJumping;
    private bool isWallRunning = false;

    [SerializeField]
    private Transform playerFeet;
    [SerializeField]
    private Animator camAnim;
    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    float jumpTime;
    [SerializeField]
    private Firering[] gunScript;
    [SerializeField]
    private GameObject Radius;
    [SerializeField]
    private float mass = 85f;
    [SerializeField]
    private Transform[] wallChecks;
    [SerializeField]
    private LayerMask wallLayer;

    public GameObject playerView;
    public float lerpTime;
    public Transform idlePos;
    public Transform reversePos;
    public Transform leftLean;
    public Transform rightLean;
    public bool weaponInUse = false;
    public MouseLoock mL;
    public float speed = 6f;            // Die Geschwindigkeit, mit der sich der Charakter bewegt
    public float jumpSpeed = 8f;        // Die Geschwindigkeit, mit der der Charakter springt
    public float gravity = 20f;         // Die Gravitation, die auf den Charakter wirkt
    public int WeaponIndex;


    private void Start()
    {
        jumpHightRef = jumpHight;
        Debug.Log("start als update lel");
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        gravBase = gravity;
        baseSpeed = speed;
    }

    private void Update()
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
        }
        if (Input.GetButtonDown("Jump") && (HandleIsGrounded()) || (Input.GetButtonDown("Jump") && jumpCounter <= 1))
        {
            Debug.Log("jump counter: " + jumpCounter);
            isJumping = true;
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
            rightWallRun();
            leftWallRun();
        }

        //wall run
        if (isJumping && leftWallRun() && useGravity && jumpCounter != 0)
        {
            /*playerView.transform.rotation = leftLean.rotation;
            playerView.transform.position = leftLean.position;*/
            isWallRunning = true;
            Debug.Log("isWallRunning: " + isWallRunning);
            useGravity = false;
        }

        if (isJumping && rightWallRun() && useGravity && jumpCounter != 0)
        {
            /*playerView.transform.rotation = rightLean.rotation;
            playerView.transform.position = rightLean.position;*/
            isWallRunning = true;
            Debug.Log("isWallRunning: " + isWallRunning);
            useGravity = false;
        }

        ////spieler stoppt wall running
            if (!rightWallRun() &&rightWallNearBy || !leftWallRun() && leftWallNearby)
            {
                Debug.Log("beginne zu fallen");
                jumpHight = jumpHightRef;
                playerView.transform.rotation = idlePos.rotation;
                playerView.transform.position = idlePos.position;
                rightWallNearBy = false;
                leftWallNearby = false;
                useGravity = true;
                isWallRunning = false;
                Debug.Log("sollte gefallen sein");
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
   
    private bool leftWallRun()
    {
        RaycastHit wall;
        if (Physics.Raycast(wallChecks[0].transform.position, wallChecks[0].TransformDirection(Vector3.left), out wall, 1f, wallLayer))
        {
            jumpHight = 3f;
            useGravity = false;
            leftWallNearby = true;
            return true;
        }
        return false;
    }
   
    private bool rightWallRun()
    {
        RaycastHit wall;
        if (Physics.Raycast(wallChecks[1].transform.position, wallChecks[1].TransformDirection(Vector3.right), out wall, 1f, wallLayer))
        {
            jumpHight = 3f;
            useGravity = false;
            Debug.Log("is on wall");
            rightWallNearBy = true;
            return true;
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
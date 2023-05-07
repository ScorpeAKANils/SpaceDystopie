using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    //[SerializeField]float speed= 10f;
    private float baseSpeed;

    private float gravBase;

    private bool isWalking = false;
    private float groundDist = 0.1f;
    public LayerMask GroundLayer;

    private bool canClimp = false;
    private float timer = 0;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool wallRunAble; 
    private float timePassed;
    private bool isLeaning;
    private bool useGravity = true;
    private bool reverseGravitation = false;
    private bool roofRun;

    private float jumpHightRef;
    private int jumpCounter = 0;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private bool isJumping;
    private bool isWallRunning = false;
    //input vars: 
    private float horizontal;
    private float vertical;
    private bool doubleJumpAble;
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
    [SerializeField, Tooltip("Emptys, wich are used for the wall the wall Check. Index 0 = left wall, index 1 = right wall")]
    private Transform[] wallChecks;
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private float jumpHight = 10f;
    [Tooltip("Object that got hit by the Raycast and is a wall")]
    private RaycastHit wall;
    private float _directionY; 

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
    private bool jumpAble;

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
        Debug.Log("JumpCounter = " + jumpCounter); 
        // Input abrufen
        checkForInput();
        //GroundCheck
        wallRunAble = WallCheck();
        jumpAble = HandleIsGrounded();

        // Sprung abrufen
        if (HandleIsGrounded())
        {
            jumpCounter = 0;
        }
        if (Input.GetButtonDown("Jump") && jumpAble)
        {
            PlayerJump();
           
        } else if (Input.GetButtonDown("Jump") && doubleJumpAble)
        {
            PlayerJump();
            doubleJumpAble = false;
        }

        // Gravitation auf den Charakter anwenden
        if (!jumpAble && wallRunAble)
        {

            if (Input.GetButtonDown("Jump"))
            {
                wallRunAble = false;
                PlayerJump();
                doubleJumpAble = false;
            }
            else
            {
                useGravity = false;
                _directionY = 0;
            }

        } else if (!wallRunAble)
        {
            useGravity = true; 
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
            startSprinting(); 
        }
        //spieler hört auf zu sprinten
        if (Input.GetKeyUp(KeyCode.LeftShift) | Input.GetButton("Fire1") | Input.GetButton("Fire2"))
        {
            stopSprinting(); 
        }


        crouching(); 

        leaning(); 
    }

    private void FixedUpdate()
    { 
        // Charakter bewegen
        PlayerMove();
        if (useGravity)
        {
            if (!reverseGravitation)
            {
                _directionY -= gravity * Time.fixedDeltaTime;
            }
            else
            {
                _directionY += gravity * Time.fixedDeltaTime;
            }
        }
        moveDirection.y = _directionY;
        controller.Move(moveDirection*Time.fixedDeltaTime); 
    }
    //ground check
    private bool HandleIsGrounded()
    {
        if (Physics.CheckSphere(playerFeet.position, groundDist, GroundLayer))
        {
            doubleJumpAble = true; 
            return true;
        }
        jumpAble = false; 
        return false;
    }
    //wall run check
   
    private void checkForInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
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

    }


    private void PlayerMove () 
    {
        moveDirection = new Vector3(horizontal, 0f, vertical);
        moveDirection = transform.TransformDirection(moveDirection * speed); 
    }

    private void PlayerJump() 
    {
        _directionY = jumpForce;
    }

    private bool WallCheck()
    {

        if (Physics.CheckSphere(wallChecks[0].position, 2f, wallLayer))
        {
            return true; 
        }

        return false; 
    }
    private void startSprinting() 
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
    private void stopSprinting() 
    {
        isSprinting = false;
        if (weaponInUse)
        {
            gunScript[WeaponIndex].shootAble = true;
            gunScript[WeaponIndex].WeaponAnim.SetBool("isSprinting", false);
        }
        speed = baseSpeed;
    }
    private void crouching() 
    {
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
    }
    private void leaning() 
    {
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
                playerView.transform.position = leftLean.position;
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
                playerView.transform.position = rightLean.position;
            }
        }
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

/* Unused code, which is obsolote, but maybe, I need him ever again
 
 
 
 **old wall checks**
     private bool leftWallRun()
    {
  
        if (Physics.Raycast(wallChecks[0].transform.position, wallChecks[0].TransformDirection(Vector3.forward), out wall, 2f, wallLayer))
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

        if (Physics.Raycast(wallChecks[1].transform.position, wallChecks[1].TransformDirection(Vector3.forward), out wall, 2f, wallLayer))
        {
            jumpHight = 3f;
            useGravity = false;
            rightWallNearBy = true;
            return true;
        }
        return false;
    }
 
 
        //wall run
        if (isJumping && leftWallRun() && useGravity && jumpCounter != 0)
        {
            /*playerView.transform.rotation = leftLean.rotation;
            playerView.transform.position = leftLean.position;
            isWallRunning = true;
            this.transform.position = Vector3.left * 1 * Time.deltaTime; 
            Debug.Log("isWallRunning: " + isWallRunning);
            useGravity = false;
            wallChecks[0].LookAt(wall.transform.position); 
        }

        if (isJumping && rightWallRun() && useGravity && jumpCounter != 0)
        {
            /*playerView.transform.rotation = rightLean.rotation;
            playerView.transform.position = rightLean.position;
            isWallRunning = true;
            this.transform.position = Vector3.left * 1 * Time.deltaTime;
            Debug.Log("isWallRunning: " + isWallRunning);
            useGravity = false;
            wallChecks[0].LookAt(wall.transform.position);
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
            foreach (Transform wallCheck in wallChecks)
            {
                wallCheck.transform.rotation = Quaternion.Euler(new Vector3(0,0,0)); 
            }
            }
 
 
 
 
 */
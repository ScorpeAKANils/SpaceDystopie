using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    //[SerializeField]float speed= 10f;
    float baseSpeed;
    [SerializeField] float JumpForce = 10f;
    float gravBase;
    [SerializeField] Transform PlayerFeet;
    [SerializeField] Animator camAnim;
    bool isWalking = false;
    float groundDist = 0.1f;
    public LayerMask GroundLayer;
    [SerializeField] float jumpTime;
    bool canClimp = false;
    [SerializeField] Firering[] gunScript;
    bool isSprinting = false;
    bool isCrouching = false;
    bool leftWallNearby;
    bool rightWallNearBy;
    [SerializeField] Transform[] wallChecks;
    public GameObject PlayerView;
    public float lerpTime;
    public Transform IdlePos;
    public Transform ReversePos;
    public Transform LeftLean;
    public Transform RightLean;
    [SerializeField] GameObject Radius;
    public bool weaponInUse = false;
    float timePassed;
    bool isLeaning;
    bool useGravity = true;
    bool reverseGravitation = false;
    public MouseLoock mL;
    [SerializeField] float mass = 85f;
    bool roofRun;
    float jumpHight = 10f;
    float jumpHightRef;
    // Start is called before the first frame update

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
            moveDirection.y += Mathf.Sqrt(JumpForce * jumpHight * gravity) * Time.fixedDeltaTime;
            Debug.Log("total jumppower: " + Mathf.Sqrt(JumpForce * jumpHight * gravity) * Time.fixedDeltaTime + "jump force: " + JumpForce + "Jump Hight" + jumpHight + "gravity: " + gravity +
            "time.deltatime: " + Time.fixedDeltaTime);
         
       
        }

        //wall run
        if (isJumping && leftWallRun(ref leftWallNearby) && useGravity && jumpCounter <2)
        {
            useGravity = false;

            //PlayerView.transform.SetPositionAndRotation(Vector3.Slerp(PlayerView.transform.position, LeftLean.position, timePassed / lerpTime), Quaternion.Slerp(PlayerView.transform.rotation, LeftLean.rotation, timePassed / lerpTime));
            //timePassed += Time.deltaTime;

            PlayerView.transform.rotation = LeftLean.rotation;
            PlayerView.transform.position = LeftLean.position;


        }

        if (isJumping && rightWallRun(ref rightWallNearBy) && useGravity && jumpCounter <2)
        {
            useGravity = false;
           
                //PlayerView.transform.SetPositionAndRotation(Vector3.Slerp(PlayerView.transform.position, RightLean.position, timePassed / lerpTime), Quaternion.Slerp(PlayerView.transform.rotation, RightLean.rotation, timePassed / 1));
                //timePassed += Time.deltaTime;
                PlayerView.transform.rotation = RightLean.rotation;
                PlayerView.transform.position = RightLean.position;
            
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
            moveDirection.y += JumpForce;
        }

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

        if (!useGravity)
        {
            rightWallRun(ref rightWallNearBy);
            leftWallRun(ref leftWallNearby);
        }

        if (!rightWallRun(ref rightWallNearBy) | !leftWallRun(ref leftWallNearby))
        {


            jumpHight = jumpHightRef;
            PlayerView.transform.rotation = IdlePos.rotation;
            PlayerView.transform.position = IdlePos.position;
            rightWallNearBy = false;
            leftWallNearby = false; 
            useGravity = true;
        }
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
        if (Input.GetKeyUp(KeyCode.C))
        {

            speed = baseSpeed;
            transform.localScale = new Vector3(1f, 1f, 1f);
            if (weaponInUse)
            {
                gunScript[WeaponIndex].WeaponAnim.SetBool("isWalking", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            timePassed = 0;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            if (timePassed < lerpTime && !isLeaning)
            {
                PlayerView.transform.SetPositionAndRotation(Vector3.Slerp(PlayerView.transform.position, LeftLean.position, timePassed / lerpTime), Quaternion.Slerp(PlayerView.transform.rotation, LeftLean.rotation, timePassed / lerpTime));
                timePassed += Time.deltaTime;
            }

            if (timePassed >= lerpTime)
            {
                isLeaning = true;
                PlayerView.transform.rotation = LeftLean.rotation;
            }

        }


        else if (Input.GetKey(KeyCode.E))
        {

            if (timePassed < lerpTime && !isLeaning)
            {
                PlayerView.transform.SetPositionAndRotation(Vector3.Slerp(PlayerView.transform.position, RightLean.position, timePassed / lerpTime), Quaternion.Slerp(PlayerView.transform.rotation, RightLean.rotation, timePassed / 1));
                timePassed += Time.deltaTime;
            }


            if (timePassed >= lerpTime)
            {
                isLeaning = true;
                PlayerView.transform.rotation = RightLean.rotation;
            }
        }

        if (Input.GetKeyUp(KeyCode.E) | Input.GetKeyUp(KeyCode.Q))
        {
            if (timePassed < lerpTime && isLeaning)
            {
                timePassed = 0;
                PlayerView.transform.SetPositionAndRotation(Vector3.Slerp(PlayerView.transform.position, IdlePos.position, timePassed / lerpTime), Quaternion.Slerp(PlayerView.transform.rotation, IdlePos.rotation, timePassed / lerpTime));
                timePassed += Time.deltaTime;
            }
            if (timePassed >= lerpTime)
            {
                isLeaning = false;
                PlayerView.transform.rotation = IdlePos.rotation;
            }
        }
    }
    bool HandleIsGrounded()
    {
        if (Physics.CheckSphere(PlayerFeet.position, groundDist, GroundLayer))
        {
            isJumping = true;
            if (!reverseGravitation && jumpCounter == 2 && roofRun)
            {
                reverseGravitation = true;
                transform.Rotate(-180, transform.rotation.y - 180, 0);
            }
            Debug.Log(jumpCounter);
            /*Collider[] col = Physics.OverlapSphere(PlayerFeet.position, 0.2f);

                if (col.CompareTag("Building"))
                {
                    roofRun = true;
                }*/

            return true;
        }
        return false;
    }




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
        if (other.CompareTag("outofBuilding") && reverseGravitation)
        {
            roofRun = false;
            reverseGravitation = false;
            transform.Rotate(-180, transform.rotation.y + 180, 0);
        }

        if (other.CompareTag("Building"))
        { 
         
            roofRun = true;
        }
        if (other.CompareTag("MagnetLeiter2"))
        {
            useGravity = false; 
            canClimp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MagnetLeiter2"))
        {

            //gravity = gravBase;
            useGravity = true; 
            canClimp = false;
        }
    }

}
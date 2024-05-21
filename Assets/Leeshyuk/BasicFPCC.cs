// ------------------------------------------
// BasicFPCC.cs
// a basic first person character controller
// with jump, crouch, run, slide
// 2020-10-04 Alucard Jay Kay
// ------------------------------------------

// source :
// https://forum.unity.com/threads/a-basic-first-person-character-controller-for-prototyping.1169491/
// Brackeys FPS controller base :
// https://www.youtube.com/watch?v=_QajrabyTJc
// smooth mouse look :
// https://forum.unity.com/threads/need-help-smoothing-out-my-mouse-look-solved.543416/#post-3583643
// ground check : (added isGrounded)
// https://gist.github.com/jawinn/f466b237c0cdc5f92d96
// run, crouch, slide : (added check for headroom before un-crouching)
// https://answers.unity.com/questions/374157/character-controller-slide-action-script.html
// interact with rigidbodies :
// https://docs.unity3d.com/2018.4/Documentation/ScriptReference/CharacterController.OnControllerColliderHit.html

// ** SETUP **
// Assign the BasicFPCC object to its own Layer
// Assign the Layer Mask to ignore the BasicFPCC object Layer
// CharacterController (component) : Center => X 0, Y 1, Z 0
// Main Camera (as child) : Transform : Position => X 0, Y 1.7, Z 0
// (optional GFX) Capsule primitive without collider (as child) : Transform : Position => X 0, Y 1, Z 0
// alternatively :
// at the end of this script is a Menu Item function to create and auto-configure a BasicFPCC object
// GameObject -> 3D Object -> BasicFPCC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR // only required if using the Menu Item function at the end of this script
using UnityEditor;
#endif

[RequireComponent(typeof(CharacterController))]
public class BasicFPCC : MonoBehaviour
{
    [Header("Layer Mask")]
    [Tooltip("Layer Mask for sphere/raycasts. Assign the Player object to a Layer, then Ignore that layer here.")]
    public LayerMask castingMask;                              // Layer mask for casts. You'll want to ignore the player.

    // - Components -
    private CharacterController controller;                    // CharacterController component
    private Transform playerTx;                                // this player object

    [Header("Main Camera")]
    [Tooltip("Drag the FPC Camera here")]
    public Transform cameraTx;                                 // Main Camera, as child of BasicFPCC object

    [Header("Optional Player Graphic")]
    [Tooltip("optional capsule to visualize player in scene view")]
    public Transform playerGFX;                                // optional capsule graphic object

    [Header("Inputs")]
    [Tooltip("Disable if sending inputs from an external script")]
    public bool useLocalInputs = true;
    [Space(5)]
    public string axisLookHorzizontal = "Mouse X";             // Mouse to Look
    public string axisLookVertical = "Mouse Y";             //
    public string axisMoveHorzizontal = "Horizontal";          // WASD to Move
    public string axisMoveVertical = "Vertical";            //
    public KeyCode keyRun = KeyCode.LeftShift;     // Left Shift to Run
    public KeyCode keyJump = KeyCode.Space;         // Space to Jump
    public KeyCode keyToggleCursor = KeyCode.BackQuote;     // ` to toggle lock cursor (aka [~] console key)

    // Input Variables that can be assigned externally
    // the cursor can also be manually locked or freed by calling the public void SetLockCursor( bool doLock )
    [HideInInspector] public float inputLookX = 0;      //
    [HideInInspector] public float inputLookY = 0;      //
    [HideInInspector] public float inputMoveX = 0;      // range -1f to +1f
    [HideInInspector] public float inputMoveY = 0;      // range -1f to +1f
    [HideInInspector] public bool inputKeyRun = false;  // is key Held
    [HideInInspector] public bool inputKeyDownJump = false;  // is key Pressed
    [HideInInspector] public bool inputKeyDownCursor = false;  // is key Pressed

    [Header("Look Settings")]
    public float mouseSensitivityX = 2f;             // speed factor of look X
    public float mouseSensitivityY = 2f;             // speed factor of look Y
    [Tooltip("larger values for less filtering, more responsiveness")]
    public float mouseSnappiness = 20f;              // default was 10f; larger values of this cause less filtering, more responsiveness
    public bool invertLookY = false;                 // toggle invert look Y
    public float clampLookY = 90f;                   // maximum look up/down angle

    [Header("Move Settings")]
    public float walkSpeed = 3f;                     // regular movement speed
    public float runSpeed = 6f;                     // run movement speed
    public float gravity = -9.81f;                   // gravity / fall rate
    public float jumpHeight = 0.4f;                  // jump height

    [Space(5)]
    public float sphereCastRadius = 0.25f;           // radius of area to detect for ground
    public float sphereCastDistance = 0.5f;         // How far spherecast moves down from origin point

    [Header("Debug Gizmos")]
    [Tooltip("Show debug gizmos and lines")]
    public bool showGizmos = false;                  // Show debug gizmos and lines

    [Header("- reference variables -")]
    public float xRotation = 0f;                     // the up/down angle the player is looking
    private float lastSpeed = 0;                     // reference for calculating speed
    private Vector3 fauxGravity = Vector3.zero;      // calculated gravity
    private float accMouseX = 0;                     // reference for mouse look smoothing
    private float accMouseY = 0;                     // reference for mouse look smoothing
    private Vector3 lastPos = Vector3.zero;          // reference for player velocity
    [Space(5)]
    public bool isGrounded = false;
    public float groundOffsetY = 0.65f;                 // calculated offset relative to height
    [Space(5)]
    public bool isCeiling = false;
    public float ceilingOffsetY = 1.25f;                // calculated offset relative to height
    [Space(5)]
    public bool cursorActive = false;                // cursor state


    void Start()
    {
        Initialize();
    }

    void Update()
    {
        ProcessInputs();
        ProcessLook();
        ProcessMovement();
    }

    void Initialize()
    {
        if (!cameraTx) { Debug.LogError("* " + gameObject.name + ": BasicFPCC has NO CAMERA ASSIGNED in the Inspector *"); }

        controller = GetComponent<CharacterController>();

        playerTx = transform;
        lastSpeed = 0;
        fauxGravity = Vector3.up * gravity;
        lastPos = playerTx.position;

        RefreshCursor();
    }

    void ProcessInputs()
    {
        if (useLocalInputs)
        {
            inputLookX = Input.GetAxis(axisLookHorzizontal);
            inputLookY = Input.GetAxis(axisLookVertical);

            inputMoveX = Input.GetAxis(axisMoveHorzizontal);
            inputMoveY = Input.GetAxis(axisMoveVertical);

            inputKeyRun = Input.GetKey(keyRun);

            inputKeyDownJump = Input.GetKeyDown(keyJump);
            inputKeyDownCursor = Input.GetKeyDown(keyToggleCursor);
        }

        if (inputKeyDownCursor)
        {
            ToggleLockCursor();
        }
    }

    void ProcessLook()
    {
        accMouseX = Mathf.Lerp(accMouseX, inputLookX, mouseSnappiness * Time.deltaTime);
        accMouseY = Mathf.Lerp(accMouseY, inputLookY, mouseSnappiness * Time.deltaTime);

        float mouseX = accMouseX * mouseSensitivityX * 100f * Time.deltaTime;
        float mouseY = accMouseY * mouseSensitivityY * 100f * Time.deltaTime;

        // rotate camera X
        xRotation += (invertLookY == true ? mouseY : -mouseY);
        xRotation = Mathf.Clamp(xRotation, -clampLookY, clampLookY);

        cameraTx.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // rotate player Y
        playerTx.Rotate(Vector3.up * mouseX);
    }

    void ProcessMovement()
    {
        // - variables -
        float nextSpeed = walkSpeed;
        Vector3 calc; // used for calculations
        Vector3 move; // direction calculation

        // player current speed
        float currSpeed = (playerTx.position - lastPos).magnitude / Time.deltaTime;
        currSpeed = (currSpeed < 0 ? 0 - currSpeed : currSpeed); // abs value

        // - Check if Grounded -
        GroundCheck();

        // - Check Ceiling above for Head Room -
        CeilingCheck();

        // - Run -

        // if grounded
        if (isGrounded && inputKeyRun)
        {
            nextSpeed = runSpeed; // to run speed
        }

        lastPos = playerTx.position; // update reference

        // - Player Move Input -
        move = (playerTx.right * inputMoveX) + (playerTx.forward * inputMoveY);

        if (move.magnitude > 1f)
        {
            move = move.normalized;
        }

        // - Slipping Jumping Gravity -

        // smooth speed
        float speed;

        if (isGrounded)
        {
                // reset angular fauxGravity movement
            fauxGravity.x = 0;
            fauxGravity.z = 0;

            if (fauxGravity.y < 0) // constant grounded gravity
            {
                fauxGravity.y = Mathf.Lerp(fauxGravity.y, -1f, 4f * Time.deltaTime);
            }

            // - Jump -
            if (inputKeyDownJump) // jump
            {
                fauxGravity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // --

            // - smooth speed -
            // take less time to slow down, more time speed up
            float lerpFactor = (lastSpeed > nextSpeed ? 4f : 2f);
            speed = Mathf.Lerp(lastSpeed, nextSpeed, lerpFactor * Time.deltaTime);
        }
        else // no friction, speed changes slower
        {
            speed = Mathf.Lerp(lastSpeed, nextSpeed, 0.125f * Time.deltaTime);
        }

        if (isCeiling && fauxGravity.y > 0)
        {
            fauxGravity.y = -1f; // 0;
        }

        lastSpeed = speed; // update reference

        // - Add Gravity -

        if (fauxGravity.y > gravity)
        {
            fauxGravity.y += gravity * Time.deltaTime;
        }

        // - Move -

        calc = speed * Time.deltaTime * move;
        calc += fauxGravity * Time.deltaTime;

        controller.Move(calc);

        // - DEBUG -

#if UNITY_EDITOR
        // slope angle and fauxGravity debug info
        if (showGizmos)
        {
            calc = playerTx.position;
            calc.y += groundOffsetY;
            Debug.DrawRay(calc, fauxGravity, Color.green);
        }
#endif
    }

    // lock/hide or show/unlock cursor
    public void SetLockCursor(bool doLock)
    {
        cursorActive = doLock;
        RefreshCursor();
    }

    void ToggleLockCursor()
    {
        cursorActive = !cursorActive;
        RefreshCursor();
    }

    void RefreshCursor()
    {
        if (!cursorActive && Cursor.lockState != CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.Locked; }
        if (cursorActive && Cursor.lockState != CursorLockMode.None) { Cursor.lockState = CursorLockMode.None; }
    }

    // check the area above, for standing from crouch
    void CeilingCheck()
    {
        Vector3 origin = new(playerTx.position.x, playerTx.position.y + ceilingOffsetY, playerTx.position.z);

        isCeiling = Physics.CheckSphere(origin, sphereCastRadius, castingMask);
    }

    // find if isGrounded, slope angle and directional vector
    void GroundCheck()
    {
        //Vector3 origin = new Vector3( transform.position.x, transform.position.y - (controller.height / 2) + startDistanceFromBottom, transform.position.z );
        Vector3 origin = new(playerTx.position.x, playerTx.position.y + groundOffsetY, playerTx.position.z);

        // Out hit point from our cast(s)

        // SPHERECAST
        // "Casts a sphere along a ray and returns detailed information on what was hit."
        if (Physics.SphereCast(origin, sphereCastRadius, Vector3.down, out _, sphereCastDistance, castingMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }   // --
    }

    // this script pushes all rigidbodies that the character touches
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.
        body.velocity = hit.moveDirection * lastSpeed;
    }

    // Debug Gizmos
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Vector3 startPoint = new(transform.position.x, transform.position.y + groundOffsetY, transform.position.z);
            Vector3 endPoint = startPoint + new Vector3(0, -sphereCastDistance, 0);
            Vector3 ceilingPoint = new(transform.position.x, transform.position.y + ceilingOffsetY, transform.position.z);

            Gizmos.color = (isGrounded == true ? Color.green : Color.white);
            Gizmos.DrawWireSphere(startPoint, sphereCastRadius);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(endPoint, sphereCastRadius);

            Gizmos.DrawLine(startPoint, endPoint);

            Gizmos.color = (isCeiling == true ? Color.red : Color.white);
            Gizmos.DrawWireSphere(ceilingPoint, sphereCastRadius);
        }
    }
#endif
}


// =======================================================================================================================================

// ** DELETE from here down, if menu item and auto configuration is NOT Required **

// this section adds create BasicFPCC object to the menu : New -> GameObject -> 3D Object
// then configures the gameobject
// demo layer used : Ignore Raycast
// also finds the main camera, attaches and sets position
// and creates capsule gfx object (for visual while editing)

// A using clause must precede all other elements defined in the namespace except extern alias declarations
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

public class BasicFPCC_Setup : MonoBehaviour
{
#if UNITY_EDITOR

    private static readonly int playerLayer = 3; // default to the Ignore Raycast Layer (to demonstrate configuration)

    [MenuItem("GameObject/3D Object/BasicFPCC", false, 0)]
    public static void CreateBasicFPCC()
    {
        GameObject go = new("Player");
        go.transform.localScale = new Vector3(0.5f, 0.7f, 0.5f);

        CharacterController controller = go.AddComponent<CharacterController>();
        controller.center = new Vector3(0, 1, 0);

        BasicFPCC basicFPCC = go.AddComponent<BasicFPCC>();

        // Layer Mask
        go.layer = playerLayer;
        basicFPCC.castingMask = ~(1 << playerLayer);

        // Main Camera
        GameObject mainCamObject = GameObject.Find("Main Camera");
        if (mainCamObject)
        {
            mainCamObject.transform.parent = go.transform;
            mainCamObject.transform.SetLocalPositionAndRotation(new Vector3(0, 1.7f, 0), Quaternion.identity);
            basicFPCC.cameraTx = mainCamObject.transform;
        }
        else // create example camera
        {
            Debug.LogWarning("** Main Camera NOT FOUND ** \nA new Camera has been created and assigned. Please replace this with the Main Camera (and associated AudioListener).");

            GameObject camGo = new("BasicFPCC Camera");
            camGo.AddComponent<Camera>();
            camGo.GetComponent<Camera>().nearClipPlane = 0.1f;
            camGo.transform.parent = go.transform;
            camGo.transform.SetLocalPositionAndRotation(new Vector3(0, 1.7f, 0), Quaternion.identity);
            basicFPCC.cameraTx = camGo.transform;
        }

        // GFX
        GameObject gfx = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        Collider cc = gfx.GetComponent<Collider>();
        DestroyImmediate(cc);
        gfx.transform.parent = go.transform;
        gfx.transform.localPosition = new Vector3(0, 1, 0);
        gfx.name = "GFX";
        gfx.layer = playerLayer;
        basicFPCC.playerGFX = gfx.transform;
        gfx.SetActive(false);
    }
#endif
}

// =======================================================================================================================================
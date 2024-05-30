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

using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;





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
    private Camera playerCamera;

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
    public KeyCode keyZoom = KeyCode.Mouse1;
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
    public float sensitivityX = 1f;             // speed factor of look X
    public float sensitivityY = 1f;             // speed factor of look y
    private float mouseSensitivityX;
    private float mouseSensitivityY;             
    public bool enableZoom = true;
    public bool holdToZoom = true;
    public float fov = 60f;
    public float zoomFOV = 10f;
    public float zoomStepTime = 5f;
    [Tooltip("larger values for less filtering, more responsiveness")]
    public float mouseSnappiness = 20f;              // default was 10f; larger values of this cause less filtering, more responsiveness
    public bool invertLookY = false;                 // toggle invert look Y
    public float clampLookY = 90f;                   // maximum look up/down angle

    [Header("Move Settings")]
    public float walkSpeed = 2f;                     // regular movement speed
    public float runSpeed = 4f;                     // run movement speed
    public float gravity = -9.81f;                   // gravity / fall rate
    public float jumpHeight = 0.2f;                  // jump height

    [Space(5)]
    public float sphereCastRadius = 0.125f;           // radius of area to detect for ground
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
    private bool isZoomed = false;
    [Space(5)]
    public bool isGrounded = false;
    public float groundOffsetY = 0.525f;                 // calculated offset relative to height
    [Space(5)]
    public bool isCeiling = false;
    public float ceilingOffsetY = 1.375f;                // calculated offset relative to height
    [Space(5)]
    public bool cursorActive = false;                // cursor state

    [Header("Audio")]
    public float velocityThreshold = 0.02f;
    Vector2 lastCharacterPosition;
    Vector2 CurrentCharacterPosition => new(transform.position.x, transform.position.z);

    public AudioSource stepAudio;
    public AudioSource runningAudio;
    public AudioSource landingAudio;
    public AudioClip[] landingSFX;
    public AudioSource jumpAudio;
    public AudioClip[] jumpSFX;
    AudioSource[] MovingAudios => new AudioSource[] { stepAudio, runningAudio };
    [Header("Running Camera")]
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    void Start()
    {
        stepAudio = GetAudioSource("Step Audio");
        runningAudio = GetAudioSource("Running Audio");
        landingAudio = GetAudioSource("Landing Audio");
        jumpAudio = GetAudioSource("Jump Audio");
        landingSFX = new AudioClip[]
        {
            Resources.Load<AudioClip>("Landing 1"),
            Resources.Load<AudioClip>("Landing 2"),
            Resources.Load<AudioClip>("Landing 3")

        };
        jumpSFX = new AudioClip[]
        {
            Resources.Load<AudioClip>("Jump 1"),
            Resources.Load<AudioClip>("Jump 2"),
            Resources.Load<AudioClip>("Jump 3")
        };
        Initialize();
    }

    void Update()
    {
        ProcessInputs();
        ProcessLook();
        ProcessMovement();
    }

    private void FixedUpdate()
    {
        float velocity = Vector3.Distance(CurrentCharacterPosition, lastCharacterPosition);
        if (velocity >= velocityThreshold && isGrounded)
        {
            if (inputKeyRun)
            {
                SetPlayingMovingAudio(runningAudio);
            }
            else
            {
                SetPlayingMovingAudio(stepAudio);
            }
        }
        else
        {
            SetPlayingMovingAudio(null);
        }
        lastCharacterPosition = CurrentCharacterPosition;
    }

    void PlayLandingAudio() => PlayRandomClip(landingAudio, landingSFX);
    void PlayJumpAudio() => PlayRandomClip(jumpAudio, jumpSFX);

    void SetPlayingMovingAudio(AudioSource audioToPlay)
    {
        // Pause all MovingAudios.
        foreach (var audio in MovingAudios.Where(audio => audio != audioToPlay && audio != null))
        {
            audio.Pause();
        }

        // Play audioToPlay if it was not playing.
        if (audioToPlay && !audioToPlay.isPlaying)
        {
            audioToPlay.Play();
        }
    }

    AudioSource GetAudioSource(string name)
    {
        // Try to get the audiosource.
        AudioSource result = System.Array.Find(GetComponentsInChildren<AudioSource>(), a => a.name == name);
        return result;
    }

    static void PlayRandomClip(AudioSource audio, AudioClip[] clips)
    {
        if (!audio || clips.Length <= 0)
            return;

        // Get a random clip. If possible, make sure that it's not the same as the clip that is already on the audiosource.
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clips.Length > 1)
            while (clip == audio.clip)
                clip = clips[Random.Range(0, clips.Length)];

        // Play the clip.
        audio.clip = clip;
        audio.Play();
    }

    void Initialize()
    {
        if (!cameraTx) { Debug.LogError("* " + gameObject.name + ": BasicFPCC has NO CAMERA ASSIGNED in the Inspector *"); }

        controller = GetComponent<CharacterController>();
        playerCamera = cameraTx.GetComponent<Camera>();
        playerCamera.fieldOfView = fov;
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
        mouseSensitivityX = sensitivityX;
        mouseSensitivityY = sensitivityY;

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

        if (enableZoom)
        {
            // Changes isZoomed when key is pressed
            // Behavior for toogle zoom
            if (Input.GetKeyDown(keyZoom) && !holdToZoom)
            {
                if (!isZoomed)
                {
                    isZoomed = true;
                }
                else
                {
                    isZoomed = false;
                }
            }

            // Changes isZoomed when key is pressed
            // Behavior for hold to zoom
            if (holdToZoom)
            {
                if (Input.GetKeyDown(keyZoom))
                {
                    isZoomed = true;
                }
                else if (Input.GetKeyUp(keyZoom))
                {
                    isZoomed = false;
                }
            }

            // Lerps camera.fieldOfView to allow for a smooth transistion
            if (isZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
                mouseSensitivityX = sensitivityX/4;
                mouseSensitivityY = sensitivityY/4;
            }
            else
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
                mouseSensitivityX = sensitivityX;
                mouseSensitivityY = sensitivityY;
            }
        }

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

        if (inputKeyRun)
        {
            isZoomed = false;
            nextSpeed = runSpeed; // to run speed
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);
        }

        lastPos = playerTx.position; // update reference

        // - Player Move Input -
        move = (playerTx.right * inputMoveX) + (playerTx.forward * inputMoveY);

        if (move.magnitude > 1f)
        {
            move = move.normalized;
        }

        // - Jumping Gravity -

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
                PlayJumpAudio();
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
        bool last = isGrounded;
        // SPHERECAST
        // "Casts a sphere along a ray and returns detailed information on what was hit."
        if (Physics.SphereCast(origin, sphereCastRadius, Vector3.down, out _, sphereCastDistance, castingMask))
        {
            isGrounded = true;
            if (last != true) PlayLandingAudio();
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
        go.transform.localScale = new Vector3(0.25f, 0.7f, 0.25f);

        CharacterController controller = go.AddComponent<CharacterController>();
        controller.center = new Vector3(0, 1, 0);

        BasicFPCC basicFPCC = go.AddComponent<BasicFPCC>();
        go.AddComponent<AudioListener>();

        // Layer Mask
        go.layer = playerLayer;
        basicFPCC.castingMask = ~(1 << playerLayer);

        GameObject camGo = new("BasicFPCC Camera");
        camGo.AddComponent<Camera>();
        camGo.AddComponent<InteractionManager>();
        camGo.GetComponent<Camera>().nearClipPlane = 0f;
        camGo.transform.parent = go.transform;
        camGo.transform.SetLocalPositionAndRotation(new Vector3(0, 1.7f, 0), Quaternion.identity);
        basicFPCC.cameraTx = camGo.transform;

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

        GameObject stepAudio = new("Step Audio");
        stepAudio.transform.parent = go.transform;
        stepAudio.transform.localPosition = Vector3.zero;
        stepAudio.AddComponent<AudioSource>();
        stepAudio.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Steps");
        stepAudio.GetComponent<AudioSource>().playOnAwake = false;
        stepAudio.GetComponent<AudioSource>().spatialBlend = 1;

        GameObject runningAudio = new("Running Audio");
        runningAudio.transform.parent = go.transform;
        runningAudio.transform.localPosition = Vector3.zero;
        runningAudio.AddComponent<AudioSource>();
        runningAudio.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Steps");
        runningAudio.GetComponent<AudioSource>().playOnAwake = false;
        runningAudio.GetComponent<AudioSource>().pitch = 1.3f;
        runningAudio.GetComponent<AudioSource>().spatialBlend = 1;

        GameObject landingAudio = new("Landing Audio");
        landingAudio.transform.parent = go.transform;
        landingAudio.transform.localPosition = Vector3.zero;
        landingAudio.AddComponent<AudioSource>();
        landingAudio.GetComponent<AudioSource>().playOnAwake = false;
        landingAudio.GetComponent<AudioSource>().spatialBlend = 1;

        GameObject jumpAudio = new("Jump Audio");
        jumpAudio.transform.parent = go.transform;
        jumpAudio.transform.localPosition = Vector3.zero;
        jumpAudio.AddComponent<AudioSource>();
        jumpAudio.GetComponent<AudioSource>().playOnAwake = false;
        jumpAudio.GetComponent<AudioSource>().spatialBlend = 1;


        GameObject crosshairCanvas = new("Crosshair Canvas");
        crosshairCanvas.transform.SetParent(go.transform, false);
        crosshairCanvas.AddComponent<Canvas>();
        crosshairCanvas.AddComponent<CanvasScaler>();
        crosshairCanvas.AddComponent<GraphicRaycaster>();
        crosshairCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        crosshairCanvas.GetComponent<Canvas>().vertexColorAlwaysGammaSpace = true;

        GameObject crosshair = new("Crosshair");
        crosshair.transform.SetParent(crosshairCanvas.transform, false);
        crosshair.AddComponent<Image>();
        crosshair.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        crosshair.GetComponent<RectTransform>().sizeDelta = new Vector2(3f, 3f);
    }
#endif
}

// =======================================================================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ActionState { NoAction, Jumping, Wallrunning_GetUp, Wallrunning, Wallrunning_GetDown, Sliding, Sliding_GetUp, Stumble }
public class FPSRunnerMotor : MonoBehaviour {

    public static FPSRunnerMotor Instance { get; private set; }
    [SerializeField]
    public float forwardSpeed = 10f;
    [SerializeField]
    private float actionSpeed = 3f;
    [SerializeField]
    private float verticalSpeed = 5f;
    [SerializeField]
    private float horizontalSpeed = 2f;
    [SerializeField]
    private float wallrunStep = 5f;
    [SerializeField]
    private float wallrunVerticalSpeed = 3f;
    [SerializeField]
    private float slidingStep = 5f;
    [SerializeField]
    private float shootDistance = 100f;
    [SerializeField]
    private Texture2D cursorTexture;
    [SerializeField]
    private GameObject bulletPrefab;

	[SerializeField]
	private GameObject GameOverCanvas;
    [SerializeField]
    private GameObject Pause;
    [SerializeField]
    private InputField nameForHS;

	private PauseMenuManager PauseManager;

    private Rigidbody playerRigidbody;
    private Animator playerAnimator;

    private float currentSpeed;
    private Vector3 velocity;
    private float currentActionStartPositionZ;
    private float distanceFromWall;
    private bool canWallrunLeft;
    private bool canWallrunRight;
    private ActionState actionState;
    private bool isDead;

    private MobileInput currentMobileInput;
    private Vector2 tapPosition;
    private FPSRunnerAudioManager audioManager;
    public bool onGround { get { return (groundObjects.Count > 0); } }
    public bool canShoot;
    private List<Collider> groundObjects;


    public void SetCustomCursor()
    {
        Cursor.SetCursor(this.cursorTexture, new Vector2(-16, 16), CursorMode.Auto);
    }
    void Awake()
    {
        Instance = this;
        canShoot = true;
        audioManager = GetComponent<FPSRunnerAudioManager>();
		GameOverCanvas.gameObject.SetActive (false); // Game over canvas defaultne vypnuty
                                                     // tady mui to padalo a myslim ze ot neni treba	PauseManager = GameObject.FindGameObjectWithTag("PauseManager").GetComponent<PauseMenuManager>();

        Pause.SetActive(true);
#if UNITY_STANDALONE
        SetCustomCursor();
#endif
#if MOBILE_INPUT
        var eventManager = new GameObject("EventManager");
        eventManager.AddComponent<EventManager>();
        var mobileInputManager = new GameObject("MobileInputManager");
        mobileInputManager.AddComponent<MobileInputManager>();
#endif
        // setting default speed
    }

    // Use this for initialization
    void Start ()
    {
        currentMobileInput = MobileInput.NoInput;
        groundObjects = new List<Collider>();
        actionState = ActionState.NoAction;
        distanceFromWall = GetComponent<Collider>().bounds.extents.x + 1f;
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

    }

    private void FixedUpdate()
    {

        if (actionState == ActionState.NoAction && onGround)
        {
            canWallrunRight = Physics.Raycast(transform.position, Vector3.right, distanceFromWall);
            canWallrunLeft = Physics.Raycast(transform.position, Vector3.left, distanceFromWall);          
        }

    }
    // Update is called once per frame
    void Update ()
    {
       // Debug.Log("mobile Input: " + currentMobileInput);
       // Debug.Log("ground objects: "+ groundObjects.Count);
        if(PauseMenuManager.Paused == false) {
            velocity = playerRigidbody.velocity;
            if (!isDead)
            {
                StartAction();
                CheckActionState();
                MoveForward();
                MoveVertical();
                Shoot();
                playerRigidbody.velocity = velocity;
            }
            else
            {
                velocity.z -= (velocity.z <= 0) ? 0 : 0.5f;
                playerRigidbody.velocity = velocity;
                Time.timeScale = 0;
                /* asi nepouzijeme - bude se pokračovat přes tlačítko
                if (Input.anyKeyDown) {
                    SceneManager.LoadScene (0); //Neviem, ci to pojde na androide
                    Time.timeScale = 1;
                }*/
            }
        }
        else
        {
            AudioListener.pause = true;
        }


        //print("velocity: " + velocity);

    }

    private void SetDeath(Vector3 deathDirection)
    {
        isDead = true;
        print("DEAD!!!!!!!!!!!!!!!!!!!!!");
        playerAnimator.SetFloat("runningSpeed", 0);
        GameOverCanvas.gameObject.SetActive(true); // Aktivacia Game over canvasu
        AudioListener.pause = true;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Pause.SetActive(false);
        if (LevelManager.isScoreTop(GameManager.completeScore))
        {
            nameForHS.gameObject.SetActive(true);
        }
    }


    private void Shoot()
    {

        if(canShoot)
        {
#if UNITY_STANDALONE
            if (Input.GetButtonDown("Fire1"))
            {

                Vector3 position = new Vector3(Input.mousePosition.x + 32f, Input.mousePosition.y, shootDistance);
                position = Camera.main.ScreenToWorldPoint(position);
                GameObject go = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
                go.transform.LookAt(position);
                // Debug.Log(position);
                audioManager.PlayShoot();
                go.GetComponent<Rigidbody>().AddForce(go.transform.forward * 200, ForceMode.Impulse);
            }
#endif
#if MOBILE_INPUT
        if ((currentMobileInput & MobileInput.Tap) == MobileInput.Tap)
        {
            Vector3 position = new Vector3(tapPosition.x, tapPosition.y, shootDistance);
            position = Camera.main.ScreenToWorldPoint(position);
            GameObject go = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
            go.transform.LookAt(position);
            // Debug.Log(position);
            audioManager.PlayShoot();
            go.GetComponent<Rigidbody>().AddForce(go.transform.forward * 200, ForceMode.Impulse);
            currentMobileInput &= ~MobileInput.Tap;
           // Debug.Log("mobileInput changed to: " + currentMobileInput);

        }
#endif
        }

    }


    private void MoveVertical()
    {
#if UNITY_STANDALONE
        float hAxis = Input.GetAxis("Horizontal");
        if(actionState == ActionState.NoAction)
        {
            velocity.x = hAxis * horizontalSpeed;
        }
        else
        {
            velocity.x = 0;
        }
#endif
#if MOBILE_INPUT
        if (actionState == ActionState.NoAction)
        {
            velocity.x = 0;
            if (( currentMobileInput & MobileInput.HoldLeft) == MobileInput.HoldLeft)
            {
                velocity.x = -horizontalSpeed;
            }
            if ((currentMobileInput & MobileInput.HoldRight) == MobileInput.HoldRight)
            {
                velocity.x = horizontalSpeed;
            }
            if ((currentMobileInput & (MobileInput.HoldRight | MobileInput.HoldLeft)) == (MobileInput.HoldRight | MobileInput.HoldLeft))
            {
                velocity.x = 0;
            }
        }
#endif
    }

    private void MoveForward()
    {
        if (actionState != ActionState.NoAction)
        {
           // Debug.Log("velocity: " + velocity);

            if (actionState == ActionState.Jumping)
            {
                currentSpeed = actionSpeed/2;
            } else
            {
                currentSpeed = actionSpeed;
            }
            

        }
        else
        {
            if (currentSpeed < forwardSpeed)
            {
                currentSpeed += 0.2f;
            }
        }
        if(!onGround)
        {
            playerAnimator.SetFloat("runningSpeed", 0);
        }
        else
        {
            playerAnimator.SetFloat("runningSpeed", (currentSpeed / (float)forwardSpeed)+0.5f);
            //Debug.Log("runningSpeed: " + (currentSpeed / (float)forwardSpeed) + "curspeed: " + currentSpeed + ", forwardSpeed: " + forwardSpeed);
        }

        velocity.z = currentSpeed;
    }


    private void StartAction()
    {
#if UNITY_STANDALONE
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");
        if (onGround)
        {
           // print("ON GROUND!!!!!!!!!!!!!!!!!!!!!!!!!");
            if (actionState == ActionState.NoAction)
            {
                if (vAxis > 0.2)
                {
                    if ((hAxis < -0.2f) && canWallrunLeft)
                    {
                        print("WALLRUNNING LEFT!!!!!!!!!!!!!!!");
                        actionState = ActionState.Wallrunning_GetUp;
                        playerAnimator.SetTrigger("startWallrunLeft");
                        velocity.y = wallrunVerticalSpeed;

                    }
                    else if ((hAxis > 0.2f) && canWallrunRight)
                    {
                        print("WALLRUNNING RIGHT!!!!!!!!!!!!!!!");
                        actionState = ActionState.Wallrunning_GetUp;
                        playerAnimator.SetTrigger("startWallrunRight");
                        velocity.y = wallrunVerticalSpeed;
                    }
                    else if((hAxis < 0.2f) && (hAxis > -0.2f))
                    {
                        print("JUMPING!!!!!!!!!!!!!!!");
                        actionState = ActionState.Jumping;
                        velocity.y = verticalSpeed;
                        playerAnimator.SetTrigger("startJump");
                    }

                }
                else if (vAxis < -0.2)
                {
                    print("SLIDING!!!!!!!!!!!!!!!!!");
                    actionState = ActionState.Sliding;
                    playerAnimator.SetTrigger("startSlide");
                    currentActionStartPositionZ = transform.position.z;
                }
            }
        }
#endif
#if MOBILE_INPUT
        if (onGround)
        {
            if (actionState == ActionState.NoAction)
            {
                switch (currentMobileInput)
                {

                    case MobileInput.SwipeDown:
                        print("SLIDING!!!!!!!!!!!!!!!!!");
                        actionState = ActionState.Sliding;
                        playerAnimator.SetTrigger("startSlide");
                        currentActionStartPositionZ = transform.position.z;
                        currentMobileInput &= ~MobileInput.SwipeDown;
                        //Debug.Log("mobileInput changed to: " + currentMobileInput);
                        break;
                    case MobileInput.SwipeUp:
                        print("JUMPING!!!!!!!!!!!!!!!");
                        actionState = ActionState.Jumping;
                        velocity.y = verticalSpeed;
                        playerAnimator.SetTrigger("startJump");
                        currentMobileInput &= ~MobileInput.SwipeUp;
                        //Debug.Log("mobileInput changed to: " + currentMobileInput);
                        break;
                    case MobileInput.SwipeLeft:
                        if (canWallrunLeft)
                        {
                            print("WALLRUNNING LEFT!!!!!!!!!!!!!!!");
                            actionState = ActionState.Wallrunning_GetUp;
                            playerAnimator.SetTrigger("startWallrunLeft");
                            velocity.y = wallrunVerticalSpeed;
                        }
                        currentMobileInput &= ~MobileInput.SwipeLeft;
                        //Debug.Log("mobileInput changed to: " + currentMobileInput);
                        break;
                    case MobileInput.SwipeRight:

                        if (canWallrunRight)
                        {
                            print("WALLRUNNING RIGHT!!!!!!!!!!!!!!!");
                            actionState = ActionState.Wallrunning_GetUp;
                            playerAnimator.SetTrigger("startWallrunRight");
                            velocity.y = wallrunVerticalSpeed;
                        }
                        currentMobileInput &= ~MobileInput.SwipeRight;
                       // Debug.Log("mobileInput changed to: " + currentMobileInput);
                        break;
                }

            } else
            {
                currentMobileInput &= ~(MobileInput.SwipeUp | MobileInput.SwipeRight | MobileInput.SwipeLeft | MobileInput.SwipeRight);
                //Debug.Log("mobileInput changed to: " + currentMobileInput);

            }
        }
#endif
    }

    private void CheckActionState()
    {

        switch (actionState)
        {
            case ActionState.Wallrunning_GetUp:
                if (velocity.y < 0.2)
                {
                    actionState = ActionState.Wallrunning;
                    currentActionStartPositionZ = transform.position.z;
                    playerAnimator.SetTrigger("wallrun");
                    print("actionState switched to: " + actionState);
                    playerRigidbody.useGravity = false;

                }
                break;
            case ActionState.Wallrunning:
                velocity.y = 0;
                if ((transform.position.z - currentActionStartPositionZ) > wallrunStep)
                {

                    actionState = ActionState.Wallrunning_GetDown;
                    playerAnimator.SetTrigger("wallrunGetDown");
                    playerRigidbody.useGravity = true;
                    print("actionState switched to: " + actionState);

                }
                break;
            case ActionState.Wallrunning_GetDown:
                if (onGround)
                {
                    actionState = ActionState.Stumble;
                    playerAnimator.SetTrigger("stumble");
                    print("actionState switched to: " + actionState);

                }
                break;
            case ActionState.Jumping:
                if (onGround && (velocity.y <= 0))
                {
                    actionState = ActionState.Stumble;
                    playerAnimator.SetTrigger("stumble");
                    print("actionState switched to: " + actionState);

                }
                break;
            case ActionState.Sliding:
                if ((transform.position.z - currentActionStartPositionZ) > slidingStep)
                {
                    actionState = ActionState.Sliding_GetUp;
                    playerAnimator.SetTrigger("slideGetUp");
                    print("actionState switched to: " + actionState);

                }
                break;
        }
    }

    private void FinishAction()
    {
        actionState = ActionState.NoAction;
        
        print("ACTION ENDED!!!!!!!!!!!!!!!!!");
    }
     private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            groundObjects.Add(other);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Hurdle"))
        {
            Vector3 direction = (other.ClosestPointOnBounds(transform.position) - transform.position).normalized;
            if (direction.y < -0.1f)
            {
                groundObjects.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
       
        groundObjects.Remove(other);
    }

    private void OnCollisionEnter(Collision other)
    {
       // Debug.Log("Collision enter: " + other.gameObject.name + " contacts: " + -other.contacts[0].normal);

        Vector3 direction = -other.contacts[0].normal;
        if (other.gameObject.tag == "HurdleDanger")
        {
            SetDeath(direction);
        }
        if (other.gameObject.tag == "Hurdle")
        {
            if ((direction.z > 0.8f) || (direction.y > 0.8f))
            {
                SetDeath(direction);
            }
        }
    }

#if MOBILE_INPUT
    void OnEnable()
    {
        EventManager.StartListening(MobileInputManager.e_OnHoldLeftEnter, new UnityAction<object>(OnHoldLeftEnter));
        EventManager.StartListening(MobileInputManager.e_OnHoldLeftLeave, new UnityAction<object>(OnHoldLeftLeave));
        EventManager.StartListening(MobileInputManager.e_OnHoldRightEnter, new UnityAction<object>(OnHoldRightEnter));
        EventManager.StartListening(MobileInputManager.e_OnHoldRightLeave, new UnityAction<object>(OnHoldRightLeave));
        EventManager.StartListening(MobileInputManager.e_OnTap, new UnityAction<object>(OnTap));
        EventManager.StartListening(MobileInputManager.e_OnSwipeRight, new UnityAction<object>(OnSwipeRight));
        EventManager.StartListening(MobileInputManager.e_OnSwipeLeft, new UnityAction<object>(OnSwipeLeft));
        EventManager.StartListening(MobileInputManager.e_OnSwipeUp, new UnityAction<object>(OnSwipeUp));
        EventManager.StartListening(MobileInputManager.e_OnSwipeDown, new UnityAction<object>(OnSwipeDown));

    }

    void OnDisable()
    {

        EventManager.StopListening(MobileInputManager.e_OnHoldLeftEnter, new UnityAction<object>(OnHoldLeftEnter));
        EventManager.StopListening(MobileInputManager.e_OnHoldLeftLeave, new UnityAction<object>(OnHoldLeftLeave));
        EventManager.StopListening(MobileInputManager.e_OnHoldRightEnter, new UnityAction<object>(OnHoldRightEnter));
        EventManager.StopListening(MobileInputManager.e_OnHoldRightLeave, new UnityAction<object>(OnHoldRightLeave));
        EventManager.StopListening(MobileInputManager.e_OnTap, new UnityAction<object>(OnTap));
        EventManager.StopListening(MobileInputManager.e_OnSwipeRight, new UnityAction<object>(OnSwipeRight));
        EventManager.StopListening(MobileInputManager.e_OnSwipeLeft, new UnityAction<object>(OnSwipeLeft));
        EventManager.StopListening(MobileInputManager.e_OnSwipeUp, new UnityAction<object>(OnSwipeUp));
        EventManager.StopListening(MobileInputManager.e_OnSwipeDown, new UnityAction<object>(OnSwipeDown));
    }


    void OnHoldLeftEnter(object value)
    {
        currentMobileInput |= MobileInput.HoldLeft;
        //Debug.Log("mobileInput changed to: " + currentMobileInput);

    }

    void OnHoldRightEnter(object value)
    {
        currentMobileInput |= MobileInput.HoldRight;
        //Debug.Log("mobileInput changed to: " + currentMobileInput);

    }

    void OnHoldLeftLeave(object value)
    {
        currentMobileInput &= ~MobileInput.HoldLeft;
        //Debug.Log("mobileInput changed to: " + currentMobileInput);

    }

    void OnHoldRightLeave(object value)
    {
        currentMobileInput &= ~MobileInput.HoldRight;
        //Debug.Log("mobileInput changed to: " + currentMobileInput);


    }

    void OnTap(object value)
    {
        tapPosition = (Vector2)value;
        currentMobileInput |= MobileInput.Tap;
        //Debug.Log("mobileInput changed to: " + currentMobileInput);

    }

    void OnSwipeRight(object value)
    {
        currentMobileInput |= MobileInput.SwipeRight;
       // Debug.Log("mobileInput changed to: " + currentMobileInput);

    }
    void OnSwipeLeft(object value)
    {
        currentMobileInput |= MobileInput.SwipeLeft;
        //Debug.Log("mobileInput changed to: " + currentMobileInput);

    }
    void OnSwipeUp(object value)
    {
        currentMobileInput |= MobileInput.SwipeUp;
       // Debug.Log("mobileInput changed to: " + currentMobileInput);

    }
    void OnSwipeDown(object value)
    {
        currentMobileInput |= MobileInput.SwipeDown;
        //Debug.Log("mobileInput changed to: " + currentMobileInput);

    }

#endif

}

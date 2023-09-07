using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement and velocity")]
    // Movemento e velocidade
    [SerializeField] private float movementSpeed;
    [SerializeField] private bool smoothActivated;
    [Range(0,0.3f)][SerializeField] private float movementSmooth;
    private Rigidbody2D rb;
    private float horizontalMovement;
    public float HorizontalMovement => horizontalMovement;
    private Vector3 velocity = Vector3.zero;
    private bool lookingRight = true;
    private bool isRunning = true;
    public bool IsRunning { 
        get { return isRunning; } 
        set { isRunning = value; } 
    }

    private bool isBeingAbducted;
    
    [Header ("Jump and groundcheck")]
    // Salto e suelo
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform groundController;
    [SerializeField] private Vector3 dimensionBox;
    [SerializeField] private float fallGravityScale;
    [SerializeField] private float coyoteTime;
    
    private float defaultGravityScale;
    private float coyoteTimeCounter;
    private bool isGrounded;
    public bool IsGrounded => isGrounded;
    private bool jump;
    public bool Jump => jump;

    // Dashing
    [Header ("Dashing")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    
    private bool canDash = true;
    private bool isDashing;

    [Header ("Particle Effects")]
    // Partículas
    [SerializeField] private ParticleSystem footstepsEffect;
    [SerializeField] private ParticleSystem jumpEffect;
    [SerializeField] private ParticleSystem dashEffect;
    private ParticleSystem.EmissionModule footEmission;
    private ParticleSystem.MinMaxCurve initialFootEmissionRot;

    // Cámara e animacións
    [Header ("Camera")]
    [SerializeField] private float leftCamLimit = -10f;
    private CinemachineVirtualCamera vCam;
    private CinemachineBrain cinemachineBrain;
    private bool cinemachineBrainEnabled = true;
    private CinemachineFramingTransposer cinemachineFramingTransposer;

    private Animator animator;
    private static readonly int Movement = Animator.StringToHash("horizontalMovement");
    private static readonly int VerticalMovement = Animator.StringToHash("verticalMovement");
    private static readonly int Running = Animator.StringToHash("isRunning");
    private static readonly int Grounded = Animator.StringToHash("isGrounded");
    private static readonly int IsDashing = Animator.StringToHash("isDashing");

    // Singleton
    public static PlayerMovement Instance { get; private set; }
    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        vCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        cinemachineFramingTransposer = vCam.GetComponentInChildren<CinemachineFramingTransposer>();
        cinemachineBrain = GameObject.Find("MainCamera").GetComponent<CinemachineBrain>();
        footEmission = footstepsEffect.emission;
        initialFootEmissionRot = footEmission.rateOverTime;
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        UpdateAnimations();
        
        if (isDashing || isBeingAbducted) return;
        
        horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed * (isRunning ? 1.5f : 1f);
        
        if (Input.GetButtonDown("Jump")) jump = true;
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) StartCoroutine(Dash()); // TODO: Key to Button
    
        if (isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDashing || isBeingAbducted) return;
        // TODO: Health?
        isGrounded = Physics2D.OverlapBox(groundController.position, dimensionBox, 0f, groundLayers);
        Move(horizontalMovement * Time.fixedDeltaTime);
        CheckLimitsOnCam();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ufo"))
        {
            StartCoroutine(StartAbduction(other.gameObject.GetComponent<Abducting>()));
        }
    }

    private void UpdateAnimations() {
        animator.SetFloat(Movement, Mathf.Abs(horizontalMovement));
        animator.SetFloat(VerticalMovement, isGrounded ? 0 : rb.velocity.y);
        animator.SetBool(Running, isRunning && horizontalMovement != 0);
        animator.SetBool(Grounded, isGrounded);
        animator.SetBool(IsDashing, isDashing);
    }
    
    private void Move(float moving) {
        // Desplazamento
        Vector3 targetVelocity = new Vector2(moving, rb.velocity.y);
        rb.velocity = smoothActivated ? Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmooth) : targetVelocity;

        // Xiro
        if (moving > 0 && !lookingRight) Turn();
        else if (moving < 0 && lookingRight) Turn();

        // efectos de partículas
        if (moving != 0 && isGrounded && !isDashing) footEmission.rateOverTime = initialFootEmissionRot;
        else footEmission.rateOverTime = 0f;

        // Salto
        if (jump && (isGrounded || coyoteTimeCounter > 0f)) {
            isGrounded = false;
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            coyoteTimeCounter = 0f;
            jumpEffect.Play();
        }
        jump = false;
    }

    private void Turn() {
        lookingRight = !lookingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        cinemachineFramingTransposer.m_TrackedObjectOffset.x *= -1f;
    }
    
    private void CheckLimitsOnCam() {
        leftCamLimit = -10f;
        if (transform.position.x < leftCamLimit && cinemachineBrainEnabled)
        {
            cinemachineBrain.enabled = false;
            cinemachineBrainEnabled = false;
        } else if (transform.position.x >= leftCamLimit && !cinemachineBrainEnabled)
        {
            cinemachineBrain.enabled = true;
            cinemachineBrainEnabled = true;
        }
        
        if (rb.velocity.y < -0.1f) {
            rb.gravityScale = fallGravityScale;
            if (cinemachineFramingTransposer.m_TrackedObjectOffset.y > 0f)
                cinemachineFramingTransposer.m_TrackedObjectOffset.y *= -1f;
        } else {
            rb.gravityScale = defaultGravityScale;
            cinemachineFramingTransposer.m_TrackedObjectOffset.y = Mathf.Abs(cinemachineFramingTransposer.m_TrackedObjectOffset.y);
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        dashEffect.Play();
        rb.gravityScale = 0f;
        rb.velocity = new(transform.localScale.x * dashingPower, 0f);
        
        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = defaultGravityScale;
        isDashing = false;
        
        yield return new WaitForSeconds(0.1f);
        dashEffect.Stop();

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private IEnumerator StartAbduction(Abducting abduct)
    {
        yield return new WaitForSeconds(0.2f);
        
        rb.velocity = Vector2.zero;
        isBeingAbducted = true;
        abduct.AbductPlayer(gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundController.position, dimensionBox);
    }
}

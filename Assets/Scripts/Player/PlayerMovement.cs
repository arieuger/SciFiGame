using System.Collections;
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

    private bool shouldMove = true;
    public bool ShouldMove
    {
        get { return shouldMove;  }
        set { shouldMove = value; }
    }
    
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

    // Animacións
    private Animator animator;
    private static readonly int Movement = Animator.StringToHash("horizontalMovement");
    private static readonly int VerticalMovement = Animator.StringToHash("verticalMovement");
    private static readonly int Running = Animator.StringToHash("isRunning");
    private static readonly int Grounded = Animator.StringToHash("isGrounded");
    private static readonly int IsDashing = Animator.StringToHash("isDashing");

    // Singleton
    public static PlayerMovement Instance { get; private set; }
    private void Awake() 
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        footEmission = footstepsEffect.emission;
        initialFootEmissionRot = footEmission.rateOverTime;
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        UpdateAnimations();
        
        if (isDashing || PlayerAbduction.Instance.IsBeingAbducted) return;

        if (shouldMove) horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed * (isRunning ? 1.5f : 1f);
        else horizontalMovement = 0;
        
        if (Input.GetButtonDown("Jump") && shouldMove) jump = true;
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) StartCoroutine(Dash()); // TODO: Key to Button
    
        if (isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDashing || PlayerAbduction.Instance.IsBeingAbducted) return;
        // TODO: Health?
        isGrounded = Physics2D.OverlapBox(groundController.position, dimensionBox, 0f, groundLayers);
        Move(horizontalMovement * Time.fixedDeltaTime);
        CheckGravityScale();
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
        CameraFollow.Instance.TurnCamera();
    }
    
    private void CheckGravityScale()
    {
        rb.gravityScale = rb.velocity.y < -0.1f ? fallGravityScale : defaultGravityScale;
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundController.position, dimensionBox);
    }
}

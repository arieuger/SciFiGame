using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalMovement;
    private Vector3 velocity = Vector3.zero;
    private bool lookingRight = true;
    private bool isRunning;
    private bool isGrounded;
    private bool jump = false;

    [SerializeField] private float movementSpeed;
    [SerializeField] private bool smoothActivated = false;
    [Range(0,0.3f)][SerializeField] private float movementSmooth;

    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform groundController;
    [SerializeField] private Vector3 dimensionBox;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        isRunning = Input.GetKey(KeyCode.LeftShift);
        horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed * (isRunning ? 1.5f : 1f);
        Debug.Log(isRunning + " - " + horizontalMovement);
        animator.SetFloat("horizontalMovement", Mathf.Abs(horizontalMovement));
        animator.SetBool("isRunning", isRunning && horizontalMovement != 0);

        if (Input.GetButtonDown("Jump")) {
            jump = true;
        }
    }

    private void FixedUpdate() {

        isGrounded = Physics2D.OverlapBox(groundController.position, dimensionBox, 0f, groundLayers);
        animator.SetBool("isGrounded", isGrounded);

        Move(horizontalMovement * Time.fixedDeltaTime, jump);    

        jump = false;
    }

    private void Move(float moving, bool jumping) {
        Vector3 targetVelocity = new Vector2(moving, rb.velocity.y);
        rb.velocity = smoothActivated ? Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmooth) : targetVelocity;

        if (moving > 0 && !lookingRight) Turn();
        else if (moving < 0 && lookingRight) Turn();

        if (isGrounded && jumping) {
            isGrounded = false;
            rb.AddForce(new Vector2(0f, jumpForce));
        }
    }

    private void Turn() {
        lookingRight = !lookingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundController.position, dimensionBox);
    }
}

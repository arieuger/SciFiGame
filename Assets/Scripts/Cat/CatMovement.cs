using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    [SerializeField] private Vector3 surpriseBox;
    [SerializeField] private Vector2 surpriseBoxOffset;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform startAbductionPosition;
    [SerializeField] private float speed = 8f;
    
    private Animator animator;
    private static readonly int TurnHead = Animator.StringToHash("turnHead");
    private static readonly int Surprise = Animator.StringToHash("surprise");
    private static readonly int Walking = Animator.StringToHash("walking");
    
    private bool isIdle = true;
    private Vector3 surpriseBoxPosition;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        surpriseBoxPosition = new Vector3(transform.position.x + surpriseBoxOffset.x, transform.position.y + surpriseBoxOffset.y);
        StartCoroutine(TurnCatHead());
    }

    void Update()
    {
        if (isIdle && Physics2D.OverlapBox(surpriseBoxPosition, surpriseBox, 0f, playerLayer))
        {
            isIdle = false;
            StartCoroutine(WalkAway());
        }
    }

    private IEnumerator TurnCatHead()
    {
        while (isIdle)
        {
            animator.SetTrigger(TurnHead);
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }

    private IEnumerator WalkAway()
    {
        animator.SetTrigger(Surprise);
        yield return new WaitForSeconds(0.3f);
        
        PlayerMovement.Instance.ShouldMove = false;
        yield return new WaitForSeconds(0.7f);
        
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
        animator.SetTrigger(Walking);
        
        while (transform.position.x < startAbductionPosition.position.x)
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                startAbductionPosition.position, speed * Time.deltaTime);
            yield return null;
        }
        
        List<Collider2D> colliders = new List<Collider2D>();
        rb.OverlapCollider(new ContactFilter2D().NoFilter(), colliders);
        colliders.Find(c => c.CompareTag("Ufo")).GetComponent<Abducting>().AbductObject(transform, true, false);
        rb.gravityScale = 0;

    }

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
        PlayerMovement.Instance.ShouldMove = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(transform.position.x + surpriseBoxOffset.x, transform.position.y + surpriseBoxOffset.y), surpriseBox);
    }
}

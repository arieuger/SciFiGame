using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerAbduction : MonoBehaviour
{
    private Rigidbody2D rb;
    private CinemachineBrain cinemachineBrain;
    private Animator animator;
    private static readonly int Abduct = Animator.StringToHash("abduct");

    private bool isBeingAbducted;
    public bool IsBeingAbducted => isBeingAbducted;

    private bool isInUfo;
    public bool IsInUfo => isInUfo;

    public static PlayerAbduction Instance { get; private set; }
    private void Awake() 
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cinemachineBrain = GameObject.Find("MainCamera").GetComponent<CinemachineBrain>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ufo")) return;
        
        isInUfo = true;
        if (!PlayerInvisibility.Instance.IsInvisible)
            StartCoroutine(StartAbductionCoroutine(other.gameObject.GetComponent<Abducting>()));
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ufo"))
        {
            isInUfo = false;
        }
    }
    
    private IEnumerator StartAbductionCoroutine(Abducting abduct)
    {
        yield return new WaitForSeconds(0.2f);

        StartAbduction(abduct);
    }

    public void StartAbduction(Abducting abduct)
    {
        rb.velocity = Vector2.zero;
        isBeingAbducted = true;
        rb.gravityScale = 0;
        cinemachineBrain.enabled = false;
        animator.SetTrigger(Abduct);
        abduct.AbductObject(gameObject.transform);
    }
}

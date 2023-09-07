using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    private Animator animator;
    private static readonly int TurnHead = Animator.StringToHash("turnHead");
    private bool isIdle = true;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(TurnCatHead());
    }

    void Update()
    {
        
    }

    private IEnumerator TurnCatHead()
    {
        while (true)
        {
            if (!isIdle) continue;
            
            animator.SetTrigger(TurnHead);
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }
}

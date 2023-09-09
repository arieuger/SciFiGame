using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class PlayerInvisibility : MonoBehaviour {

    [SerializeField] private GameObject invisibilityBar;

    private Material material;
    private LocalKeyword shouldShowOutlineKeyword;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isInvisible;
    public bool IsInvisible => isInvisible;
    
    public static PlayerInvisibility Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        material = sr.material;
        shouldShowOutlineKeyword = new LocalKeyword(material.shader, "_SHOULDSHOWOUTLINE");
        StartCoroutine(LerpAlpha());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.S) && !PlayerAbduction.Instance.IsBeingAbducted) StartCoroutine(TwinklePlayerVisibility());
    }

    public void SwitchPlayerVisibility()
    {
        StartCoroutine(TwinklePlayerVisibility());
    }

    private IEnumerator TwinklePlayerVisibility()
    {
        isInvisible = !isInvisible;
        invisibilityBar.SetActive(isInvisible);
        PlayerMovement.Instance.IsRunning = !isInvisible;
        if (!isInvisible) CheckShouldBeAbducted();

        for (int i = 0; i < Random.Range(2, 4); i++)
        {
            material.SetFloat("_DissolveAmount", isInvisible ? 0f : 1f);
            material.SetKeyword(shouldShowOutlineKeyword, isInvisible);
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
            
            material.SetFloat("_DissolveAmount", isInvisible ? 1f : 0f);
            material.SetKeyword(shouldShowOutlineKeyword, !isInvisible);
            yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
            
            material.SetFloat("_DissolveAmount", isInvisible ? 0f : 1f);
            material.SetKeyword(shouldShowOutlineKeyword, isInvisible);
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
        }
    }

    private void CheckShouldBeAbducted()
    {
        if (!PlayerAbduction.Instance.IsInUfo) return;
        
        List<Collider2D> colliders = new List<Collider2D>();
        rb.OverlapCollider(new ContactFilter2D().NoFilter(), colliders);
        colliders.ForEach(c =>
        {
            if (c.CompareTag("Ufo")) PlayerAbduction.Instance.StartAbduction(c.GetComponent<Abducting>());
        });
    }

    private IEnumerator LerpAlpha() {
        
        float duration = 1.5f;
        float lerpTimer = 0f;
        float minAlpha = 0.1f;

        while (true) {
            Color color = sr.color;
            if (isInvisible) {
                lerpTimer += Time.deltaTime;
                float lerp = Mathf.PingPong(lerpTimer, duration) / duration;
                color.a = Mathf.Lerp(minAlpha, 1f, Mathf.SmoothStep(minAlpha, 1f, lerp));
            } else {
                lerpTimer = 0;
                color.a = 1f;
            }

            sr.color = color;
            yield return null;
        }        
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Dissolve : MonoBehaviour {

    [SerializeField] private GameObject invisibilityBar;

    private Material material;
    private LocalKeyword shouldShowOutlineKeyword;
    private SpriteRenderer sr;
    private bool isInvisible;
    private bool shouldShowOutline = false;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        material = sr.material;
        shouldShowOutlineKeyword = new LocalKeyword(material.shader, "_SHOULDSHOWOUTLINE");
        StartCoroutine(LerpAlpha());
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.S)) isInvisible = !isInvisible;
        
        material.SetFloat("_DissolveAmount", isInvisible ? 0f : 1f);
        material.SetKeyword(shouldShowOutlineKeyword, isInvisible);
        invisibilityBar.SetActive(isInvisible);
        PlayerMovement.Instance.IsRunning = !isInvisible;
        
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

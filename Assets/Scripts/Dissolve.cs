using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Dissolve : MonoBehaviour {

    private Material material;
    private LocalKeyword shouldShowOutlineKeyword;
    private SpriteRenderer sr;
    private PlayerMovement pMov;

    private bool isDissolving = false;
    private float fade = 1f;
    private bool isInvisible = false;
    private bool shouldShowOutline = false;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        material = sr.material;
        shouldShowOutlineKeyword = new LocalKeyword(material.shader, "_SHOULDSHOWOUTLINE");
        pMov = GetComponent<PlayerMovement>();
        StartCoroutine(LerpAlpha());
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.S) && !isDissolving) {
            isDissolving = true;
            isInvisible = fade == 0f;
        }

        if (!isDissolving) return;
        
        if (!isInvisible) {
            fade -= Time.deltaTime;
            pMov.IsRunning = false;
            if (fade <= 0f) {
                fade = 0f;
                shouldShowOutline = true;
                isDissolving = false;
            }

        } else {
            shouldShowOutline = false;
            fade += Time.deltaTime;
            pMov.IsRunning = true;
            if (fade >= 1f) {
                fade = 1f;
                isDissolving = false;
            }
        }

        material.SetFloat("_DissolveAmount", fade);
        material.SetKeyword(shouldShowOutlineKeyword, shouldShowOutline);
        
    }

    private IEnumerator LerpAlpha() {
        Color color = sr.color;
        float duration = 1.5f;
        float lerpTimer = 0f;
        float minAlpha = 0.1f;

        while (true) {
            if (shouldShowOutline) {
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

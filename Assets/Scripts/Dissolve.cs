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
        
        // TODO: Fade inicial/final
        if (!isInvisible) {
            fade -= Time.deltaTime;
            pMov.IsRunning = false;
            if (fade <= 0f) {
                shouldShowOutline = true;
                fade = 0f;
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
        float duration = 2f;
        float minAlpha = 0.2f;

        while (true) {
            if (shouldShowOutline) {
                float lerp = Mathf.PingPong(Time.time, duration) / duration;
                color.a = Mathf.Lerp(minAlpha, 1f, Mathf.SmoothStep(minAlpha, 1f, lerp));
            } else {
                color.a = 1f;
            }

            sr.color = color;
            yield return null;
        }        
    }
    
}

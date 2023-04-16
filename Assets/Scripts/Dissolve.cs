using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour {

    private Material material;
    private bool isDissolving = false;
    private float fade = 1f;
    private bool isInvisible = false;
    private bool shouldShowOutline = false;

    void Start() {
        material = GetComponent<SpriteRenderer>().material;
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.S) && !isDissolving) {
            isDissolving = true;
            isInvisible = fade == 0f;
        }

        if (!isDissolving) return;

        if (!isInvisible) {
            // TODO: Corrutina para slide
            fade -= Time.deltaTime;
            if (fade <= 0f) {
                shouldShowOutline = true;
                fade = 0f;
                isDissolving = false;
            }

        } else {
            shouldShowOutline = false;
            fade += Time.deltaTime;
            if (fade >= 1f) {
                fade = 1f;
                isDissolving = false;
            }
        }

        material.SetFloat("_DissolveAmount", fade);
        material.SetInteger("_ShouldShowOutline", shouldShowOutline ? 1 : 0);
        
    }
}

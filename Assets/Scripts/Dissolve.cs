using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour {

    private Material material;
    private bool isDissolving = false;
    private float fade = 1f;

    void Start() {
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.S)) isDissolving = true;

        if (isDissolving) {
            fade -= Time.deltaTime;

            if (fade <= 0f) {
                fade = 0f;
                isDissolving = false;
            }

            material.SetFloat("_DissolveAmount", fade);
        }
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPlatform : MonoBehaviour
{

    [SerializeField] private Sprite transparentPlatform;
    [SerializeField] private Sprite opaquePlatform;
    [SerializeField] private Color transparentColor;
    [SerializeField] private Color opaqueColor;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ufo"))
        {
            Debug.Log("Plataforma iluminada");
            sr.sprite = opaquePlatform;
            sr.color = opaqueColor;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ufo"))
        {
            Debug.Log("Plataforma en sombra");
            sr.sprite = transparentPlatform;
            sr.color = transparentColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

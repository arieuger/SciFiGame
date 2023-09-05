using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenericPlatform : MonoBehaviour
{

    [SerializeField] private Sprite transparentPlatform;
    [SerializeField] private Sprite opaquePlatform;
    [SerializeField] private Color transparentColor;
    [SerializeField] private Color opaqueColor;

    private SpriteRenderer sr;
    private BoxCollider2D bc;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ufo"))
        {
            StartCoroutine(TwinkleChangingPlatform(false));
            bc.isTrigger = false;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ufo"))
        {
            StartCoroutine(TwinkleChangingPlatform(true));
            bc.isTrigger = true;
        }
    }

    private IEnumerator TwinkleChangingPlatform(bool toTransparent)
    {
        Color changedColor = toTransparent ? transparentColor : opaqueColor;
        Sprite changedSprite = toTransparent ? transparentPlatform : opaquePlatform;
        Color originalColor = toTransparent ? opaqueColor : transparentColor;
        Sprite originalSprite = toTransparent ? opaquePlatform : transparentPlatform;

        for (int i = 0; i < Random.Range(2,4); i++) {
            sr.sprite = changedSprite;
            sr.color = changedColor;
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));

            sr.sprite = originalSprite;
            sr.color = originalColor;
            yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
            
            sr.sprite = changedSprite;
            sr.color = changedColor;
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
        }
    }
    
}

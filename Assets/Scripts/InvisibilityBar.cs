using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvisibilityBar : MonoBehaviour {

    [SerializeField] private float maxTime = 15f;

    private Slider slider;
    private float timeRemaining;
    private bool shouldBeCounting = true;
    private Image fillImage;
    private bool isPlayerMoving;

    void Update() {
        isPlayerMoving = PlayerMovement.Instance.HorizontalMovement != 0;
        if (shouldBeCounting && timeRemaining >= 0) {
            timeRemaining -= Time.deltaTime;
            if (isPlayerMoving) AccelerateOnWalk();
            slider.value = timeRemaining / maxTime;
        } else if (timeRemaining <= 0) {
            gameObject.SetActive(false);
        }
    }

    public void OnEnable() {
        if (slider == null) slider = GetComponent<Slider>();
        if (fillImage == null) fillImage = gameObject.transform.Find("Fill").GetComponent<Image>();
        
        slider.value = 1f;
        timeRemaining = maxTime;
        shouldBeCounting = true;
    }

    public void OnDisable() {
        shouldBeCounting = false;
    }

    private void AccelerateOnWalk() {
        timeRemaining -= Time.deltaTime / 1.5f;
        StartCoroutine(LerpColor());
    }

    private IEnumerator LerpColor() {
        
        Color originalColor = fillImage.color;
        Color alertColor = new Color32(226,109,86,255);
        float duration = 1f;
        float lerpTimer = 0f;

        while (true) {
            if (isPlayerMoving) {
                lerpTimer += Time.deltaTime;
                float lerp = Mathf.PingPong(lerpTimer, duration) / duration;
                fillImage.color = Color.Lerp(originalColor, alertColor, lerp);
            } else {
                fillImage.color = originalColor;
                yield break;
            }
            
            yield return null;
        }

        
    }
    
}

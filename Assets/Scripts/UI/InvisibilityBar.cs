using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvisibilityBar : MonoBehaviour {

    [SerializeField] private float maxTime = 15f;
    [SerializeField] private Dissolve playerDissolve;
    private float jumpTime = 1.5f;

    private Slider slider;
    private float timeRemaining;
    private bool shouldBeCounting = true;
    private Image fillImage;
    private bool isPlayerMoving;
    private bool isPlayerJumping;
    [SerializeField] private Color originalColor/* = new Color32(46,93,82,255)*/;
    private readonly Color alertColor = new Color32(226,109,86,255);

    void Update() {
        isPlayerMoving = PlayerMovement.Instance.HorizontalMovement != 0;
        isPlayerJumping = PlayerMovement.Instance.Jump && PlayerMovement.Instance.IsGrounded;
        
        if (!isPlayerMoving && fillImage.color != originalColor) fillImage.color = originalColor;

        if (shouldBeCounting && timeRemaining >= 0) {
            timeRemaining -= Time.deltaTime;
            if (isPlayerMoving) AccelerateOnWalk();
            slider.value = timeRemaining / maxTime;
        } else if (timeRemaining <= 0) {
            gameObject.SetActive(false);
            playerDissolve.SwitchPlayerVisibility();
        }
    }

    private void FixedUpdate() {
        if (isPlayerJumping) {
            timeRemaining -= jumpTime;
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

        float duration = 1f;
        float lerpTimer = 0f;

        while (true) {
            
            if (!isPlayerMoving) yield break;
            
            lerpTimer += Time.deltaTime;
            float lerp = Mathf.PingPong(lerpTimer, duration) / duration;
            fillImage.color = Color.Lerp(originalColor, alertColor, lerp);
            yield return null;
        
        }
    }
    
}

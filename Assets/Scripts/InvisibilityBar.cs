using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvisibilityBar : MonoBehaviour {

    [SerializeField] private float maxTime = 15f;

    private Slider slider;
    private float timeRemaining;
    private bool shouldBeCounting = true;

    void Update() {
        if (shouldBeCounting && timeRemaining >= 0) {
            timeRemaining -= Time.deltaTime;
            if (PlayerMovement.Instance.HorizontalMovement != 0) AccelerateOnWalk();
            
            slider.value = timeRemaining / maxTime;
        } else if (timeRemaining <= 0) {
            gameObject.SetActive(false);
        }
    }

    public void OnEnable() {
        if (slider == null) slider = GetComponent<Slider>();
        slider.value = 1f;
        timeRemaining = maxTime;
        shouldBeCounting = true;
    }

    public void OnDisable() {
        shouldBeCounting = false;
    }

    private void AccelerateOnWalk() {
        timeRemaining -= Time.deltaTime / 1.5f;
    }
    
}

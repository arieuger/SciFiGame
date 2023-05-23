using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class Twinkling : MonoBehaviour {

    private float maxIntensity;
    [SerializeField] private float minIntensity;
    [SerializeField] bool twinklingActive = true;

    private Light2D lightComponent;
    
    void Start() {
        lightComponent = GetComponent<Light2D>();
        maxIntensity = lightComponent.intensity;
        StartCoroutine(Twinkle());
    }

    private IEnumerator Twinkle() {
        while (true) {            

            if (twinklingActive) {
                
                // Tintileo
                for (int i = 0; i < Random.Range(2,8); i++) {
                    lightComponent.intensity = Random.Range(minIntensity, minIntensity + 0.3f);
                    yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));

                    lightComponent.intensity = maxIntensity;
                    yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
                }

                // Espera                
                yield return new WaitForSeconds(Random.Range(0.5f, 5f));
            
            } else yield return null;
            
        }

    }
}

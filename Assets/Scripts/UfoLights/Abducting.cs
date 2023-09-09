using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Abducting : MonoBehaviour
{
    [SerializeField] private Transform endAbductionPoint;
    [SerializeField] private float speed = 1f;
    
    public void AbductObject(Transform abducted, bool shouldDestroy = false, bool shouldReload = true)
    {
        StartCoroutine(AbductionCo(abducted, shouldDestroy, shouldReload));
    }

    private IEnumerator AbductionCo(Transform abducted, bool shouldDestroy, bool shouldRelaod)
    {
        while (abducted.position.y < endAbductionPoint.position.y)
        {
            abducted.position = Vector2.MoveTowards(abducted.position, endAbductionPoint.position, speed * Time.deltaTime);
            yield return null;
        }

        if (shouldDestroy) Destroy(abducted.gameObject);
        
        yield return new WaitForSeconds(1f);
        if (shouldRelaod) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

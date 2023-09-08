using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Abducting : MonoBehaviour
{
    [SerializeField] private Transform endAbductionPoint;
    [SerializeField] private float speed = 1f;
    
    public void AbductObject(Transform abducted)
    {
        StartCoroutine(AbductionCo(abducted));
    }

    private IEnumerator AbductionCo(Transform abducted)
    {
        while (abducted.position.y < endAbductionPoint.position.y)
        {
            abducted.position = Vector2.MoveTowards(abducted.position, endAbductionPoint.position, speed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

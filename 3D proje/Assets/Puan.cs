using System.Collections;
using UnityEngine;

public class Puan : MonoBehaviour
{
    private Renderer rend;
    private Collider col;

    private void Start()
    {
        rend= GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ReSpawn());
            PointManager.OnPointCollected.Invoke();
            Debug.Log("Puan toplandı");
        }
    }

    private IEnumerator ReSpawn()
    {
        rend.enabled = false;
        col.enabled = false;
        yield return new WaitForSeconds(5f);
        rend.enabled = true;
        col.enabled = true;
    }
}

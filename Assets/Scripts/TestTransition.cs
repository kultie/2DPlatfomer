using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransition : MonoBehaviour
{
    private Material _material;
    [SerializeField] private float transitionTime = 0.5f;

    private void OnEnable()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        _material = GetComponent<Renderer>().material;
        float time = transitionTime;
        while (time >= 0)
        {
            time -= Time.deltaTime;
            _material.SetFloat("_Cutoff", 1 - (time / transitionTime));
            _material.SetFloat("_NoiseOffset", Random.Range(0, 5));
            yield return null;
        }
    }

    private void OnDisable()
    {
        _material.SetFloat("_Cutoff", 0);
        _material.SetFloat("_NoiseOffset", 0);
    }
}
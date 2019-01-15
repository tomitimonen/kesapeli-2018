using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialBlinker : MonoBehaviour {

    [SerializeField] private string parameterName = "";
    [SerializeField] private GameObject model;
    [SerializeField] private float blinkTime = 1f;

    IList<Renderer> renderers;

    // Use this for initialization
    void Start () {
        renderers = new List<Renderer>();
        foreach (Transform child in model.transform)
        {
            Renderer rend = child.gameObject.GetComponent<Renderer>();
            if (rend != null) renderers.Add(rend);
        }
        foreach (Renderer rend in renderers)
        {
            rend.material.SetFloat(parameterName, 0f);
        }
    }

    public void Blink()
    {
        StartCoroutine(BlinkCoroutine());
    }

    void SetParameter(float value)
    {
        foreach (Renderer rend in renderers)
        {
            rend.material.SetFloat(parameterName, value);
        }
    }
	
    IEnumerator BlinkCoroutine()
    {
        SetParameter(1f);
        yield return new WaitForSeconds(blinkTime);
        SetParameter(0f);
        yield return new WaitForSeconds(blinkTime);
    }
}

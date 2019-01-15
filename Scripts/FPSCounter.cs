using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

    public Text fpsText;
    public float currentFps;

	// Use this for initialization
	void Start () {
        currentFps = 0f;

    }
	
	// Update is called once per frame
	void Update () {
        float fps = 1f / Time.deltaTime;
        currentFps = Mathf.Lerp(currentFps, fps, .2f);
        fpsText.text = Mathf.Round(currentFps).ToString();
	}
}

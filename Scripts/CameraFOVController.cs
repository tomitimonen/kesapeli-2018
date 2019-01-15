using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFOVController : MonoBehaviour {

    CinemachineVirtualCamera cam;
    public float hFov = 90f;

	// Use this for initialization
	void Start () {
        cam = GetComponent<CinemachineVirtualCamera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        //pitää horizontal FOVin vakiona näytön asennosta (portrait/landscape) riippumatta
        //lähde: https://answers.unity.com/questions/604164/is-there-a-way-to-set-the-horizontal-field-of-view.html
        float hFOVrad = hFov * Mathf.Deg2Rad;
        float camH = Mathf.Tan(hFOVrad * .5f) / cam.m_Lens.Aspect;
        float vFOVrad = Mathf.Atan(camH) * 2f;
        cam.m_Lens.FieldOfView = vFOVrad * Mathf.Rad2Deg;



    }
}

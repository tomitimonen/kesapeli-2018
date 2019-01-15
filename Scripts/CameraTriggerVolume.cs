using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTriggerVolume : MonoBehaviour {

    public CinemachineVirtualCamera volumeCamera;
    CinemachineVirtualCamera defaultCamera;

    // Use this for initialization
    void Start () {
        defaultCamera = GameObject.FindGameObjectWithTag("CMDefaultCamera").GetComponent<CinemachineVirtualCamera>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            defaultCamera.enabled = false;
            volumeCamera.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            defaultCamera.enabled = true;
            volumeCamera.enabled = false;
        }
    }
}

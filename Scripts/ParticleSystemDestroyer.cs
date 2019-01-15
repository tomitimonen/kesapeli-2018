using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroyer : MonoBehaviour {

    public ParticleSystem particleSystem;

	// Use this for initialization
	void Start () {
        //ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {

        //tuhoa gameobject kun partikkeliefekti on päättynyt
		if (!particleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
	}
}

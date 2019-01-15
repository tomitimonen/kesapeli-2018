using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepHandler : MonoBehaviour {

    public delegate void Callback();
    public Callback FootStepHandler;
	
    public void Footstep()
    {
        FootStepHandler();
    }
}

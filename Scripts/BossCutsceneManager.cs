using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class BossCutsceneManager : MonoBehaviour {

    [SerializeField] GameObject playerStartPoint;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject arena;
    [SerializeField] BezierSpline bossPath;
    [SerializeField] GameObject backgroundMusicObject;
    [SerializeField] Canvas storyCanvas;
    PlayerController playerController;
    GameObject player;
    PathKeeper playerKeeper;
    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        playerKeeper = player.GetComponent<PathKeeper>();

        playerController = GameObject.Find("TouchControlHandler").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void StartCutscene()
    {
        storyCanvas.enabled = true;
        playerController.SetCutscene(true);
        //StartBattle();
    }

    public void StartBattle()
    {
        storyCanvas.enabled = false;
        playerController.SetCutscene(false);

        playerKeeper.currentSpline = bossPath;
        player.transform.position = playerStartPoint.transform.position;
        boss.SetActive(true);
        if (arena != null) arena.SetActive(true);
        backgroundMusicObject.SetActive(false);
        HealthSystem healthSystem = player.GetComponent<HealthSystem>();
        healthSystem.currentHealth = healthSystem.maxHealth;
        playerController.setSpellState(0, true, 1);

        //siirrä pelaajan kamerat bossiareenan luo
        player.GetComponent<PlayerControls>().MoveCameras();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCutscene();
        }
    }
}

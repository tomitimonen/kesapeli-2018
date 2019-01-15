using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginningCutsceneManager : MonoBehaviour {

    //[SerializeField] GameObject backgroundMusicObject;
    [SerializeField] Canvas storyCanvas;
    [SerializeField] TutorialManager tutorialManager;
    PlayerController playerController;
    GameObject player;
    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        playerController = GameObject.Find("TouchControlHandler").GetComponent<PlayerController>();

        if (!GameDataManager.Instance.GameData.player.firstCheckPointReached) StartCoroutine(StartCutsceneCoroutine());
    }
	
    void StartCutscene()
    {
        storyCanvas.enabled = true;
        playerController.SetCutscene(true);
    }

    IEnumerator StartCutsceneCoroutine()
    {
        yield return new WaitForSeconds(1f);
        StartCutscene();
        
    }

    public void StartGame()
    {
        
        storyCanvas.enabled = false;
        playerController.SetCutscene(false);
        tutorialManager.StartTutorial();
        //backgroundMusicObject.SetActive(false);
    }
}

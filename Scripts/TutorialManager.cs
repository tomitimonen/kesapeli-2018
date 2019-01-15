using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Canvas tutorialCanvas;
    [SerializeField] private Image[] tutorialGuideImages;
    [SerializeField] private Image tutorialSpellOpenImage;
    [SerializeField] private Image tutorialSpellDrawImage;
    [SerializeField] private int spellTriggerVolumeIndex;

    bool spellTutorialPhase = false;
    bool completed;
    HealthSystem playerHealthSystem;

    // Use this for initialization
    void Start () {

        playerHealthSystem = GameObject.Find("Player").GetComponent<HealthSystem>();

        completed = GameDataManager.Instance.GameData.tutorial.completed;
        if (completed)
        {
            disableAllImages();
            tutorialCanvas.enabled = false;
        }
    }

    public void StartTutorial()
    {
        if (!completed) tutorialCanvas.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
		
        if (spellTutorialPhase)
        {
            if (playerHealthSystem.currentHealth >= playerHealthSystem.maxHealth)
            {
                spellTutorialPhase = false;
                EndTutorial();
            }

            if (playerController.spellMenuState != PlayerController.SpellMenuState.DrawGesture)
            {
                tutorialSpellOpenImage.enabled = true;
                tutorialSpellDrawImage.enabled = false;
            }
            else
            {
                tutorialSpellOpenImage.enabled = false;
                tutorialSpellDrawImage.enabled = true;
            }
        }
	}

    void disableAllImages()
    {
        foreach (Image image in tutorialGuideImages)
        {
            image.enabled = false;
        }
    }

    public void OnEnterTutorialVolume(int index)
    {
        if (completed) return;

        disableAllImages();
        if (index >= 0)
        {
            //loitsun opastus käsitellään eri tavalla
            if (index != spellTriggerVolumeIndex)
            {
                if (spellTutorialPhase) ExitSpellVolume();
                tutorialGuideImages[index].enabled = true;
            }
            else
            {
                if (!spellTutorialPhase) EnterSpellVolume();
            }
        }
        else EndTutorial();

    }

    void EndTutorial()
    {
        tutorialCanvas.enabled = false;
        completed = true;
        GameDataManager.Instance.GameData.tutorial.completed = true;
    }

    void EnterSpellVolume()
    {
        spellTutorialPhase = true;
        playerController.SpellMenuEnabled = true;
    }
    void ExitSpellVolume()
    {
        spellTutorialPhase = false;
    }

    public void RestoreSpell()
    {
        playerController.setSpellState(0, true, 1);
    }


}

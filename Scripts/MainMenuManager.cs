using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    [SerializeField]private AudioSource clickSound;
    [SerializeField]private AudioSource rejectSound;
    [SerializeField] private AudioSource startGameSound;

    public Canvas mainCanvas;
    public Canvas levelSelectCanvas;
    public Canvas settingsCanvas;
    public Canvas loadingCanvas;
    public Image backgroundImage;
    public Image loadingBarImage; 
    public string startGameLevel;
    public GameObject playButton;
    public GameObject continueButton;

    enum MenuState
    {
        Main,
        LevelSelect,
        Settings
    }

    MenuState openedMenu = MenuState.Main;

	// Use this for initialization
	void Start () {
        loadingCanvas.enabled = false;
        //rajaton FPS suorituskyvyn testausta varten
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 999;

        bool hasStartedGame = GameDataManager.Instance.GameData.player.firstCheckPointReached;
        playButton.SetActive(!hasStartedGame);
        continueButton.SetActive(hasStartedGame);
    }
	
	// Update is called once per frame
	void Update () {
        //escape on paluunappi mobiilissa
		if (Input.GetKey(KeyCode.Escape))
        {
            if (openedMenu == MenuState.LevelSelect || openedMenu == MenuState.Settings)
            {
                OpenMainMenu();
            }
        }
	}

    public void PlayGame()
    {
        startGameSound.Play();
        mainCanvas.enabled = false;
        levelSelectCanvas.enabled = false;
        settingsCanvas.enabled = false;
        backgroundImage.enabled = false;
        loadingCanvas.enabled = true;
        StartCoroutine(LoadLevel());
    }
    public void ContinueGame()
    {
        PlayGame();
    }
    public void StartLevel(int level)
    {
        if (level == 0) //onko taso avattu
        {
            GameDataManager.Instance.CreateNewGame();
            PlayGame();
        }
        else
        {
            rejectSound.Play();
        }
    }
    public void ReturnToMainMenu()
    {
        clickSound.Play();
        OpenMainMenu();
    }
    public void OpenMainMenu()
    {
        openedMenu = MenuState.Main;
        mainCanvas.enabled = true;
        levelSelectCanvas.enabled = false;
        settingsCanvas.enabled = false;
        backgroundImage.enabled = true;
    }
    public void OpenLevelSelect()
    {
        clickSound.Play();
        openedMenu = MenuState.LevelSelect;
        mainCanvas.enabled = false;
        levelSelectCanvas.enabled = true;
        settingsCanvas.enabled = false;
        backgroundImage.enabled = false;
    }
    public void OpenSettings()
    {
        clickSound.Play();
        openedMenu = MenuState.LevelSelect;
        mainCanvas.enabled = false;
        levelSelectCanvas.enabled = false;
        settingsCanvas.enabled = true;
        backgroundImage.enabled = true;
    }
    public void OpenExtras()
    {
        rejectSound.Play();
        /*clickSound.Play();
        openedMenu = MenuState.LevelSelect;
        mainCanvas.enabled = false;
        levelSelectCanvas.enabled = false;
        settingsCanvas.enabled = true;
        backgroundImage.enabled = true;*/
    }


    IEnumerator LoadLevel()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(startGameLevel);
        while (!asyncLoad.isDone)
        {
            //Debug.Log(asyncLoad.progress);
            loadingBarImage.fillAmount = asyncLoad.progress;
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using UnityEngine.UI;
using PDollarGestureRecognizer;
using System;

public class PlayerController : MonoBehaviour {

    [SerializeField] private float timeSlowDownFactor = .3f;
    [SerializeField] private Canvas spellMenuCanvas;
    [SerializeField] private GameObject spellMenuButtons;
    [SerializeField] private Image spellDrawBackground;
    [SerializeField] private Image spellDrawGuide;
    [SerializeField] private GameObject spellMenuButtonPrefab;
    [SerializeField] private Spell[] spells;
    [SerializeField] private AudioSource toggleSpellMenuSound;

    public bool SpellMenuEnabled { get; set; }
    public bool EnableInput { get; set; }
    public bool[] spellOwned;
    public int[] spellCount;
    GameObject[] spellButtons;

    float maxTapLength = .5f;
    float minTapStrength = .4f;
    float gestureAcceptMinScore = .7f;
    private List<Gesture> storedGestures = new List<Gesture>();
    PlayerControls playerControls;
    float defaultDeltaTime;
    float defaultMaximumDeltaTime;
    int selectedSpell = 0;

    bool cutscene = false;
    bool paused = false;

    public void setSpellState(int index, bool owned, int count)
    {
        spellCount[index] = count;
        spellOwned[index] = owned;
        spellButtons[index].SetActive(owned);
        spellButtons[index].GetComponent<Button>().interactable = count > 0;
    }

    public enum SpellMenuState
    {
        Closed,
        SelectSpell,
        DrawGesture
    }

    public SpellMenuState spellMenuState = SpellMenuState.Closed;

    void Start()
    {
        spellOwned = new bool[spells.Length];
        spellCount = new int[spells.Length];
        spellButtons = new GameObject[spells.Length];
        //rakenna loitsumenun valintanapit
        for (int i = 0; i < spells.Length; i++)
        {

            Spell spell = spells[i];
            storedGestures.Add(GestureIO.ReadGestureFromXML(spell.gestureXml.text));
            GameObject spellButtonObject = Instantiate(spellMenuButtonPrefab);
            spellButtons[i] = spellButtonObject;
            spellButtonObject.transform.SetParent(spellMenuButtons.transform, false);
            spellButtonObject.GetComponent<Image>().sprite = spell.icon;
            int index = i; //delegaatin muuttujan pitää olla samassa scopessa kuin delegaatti
            spellButtonObject.GetComponent<Button>().onClick.AddListener(delegate { StartSpellGestureDrawMode(index); });
        }

        //lataa loitsujen tilanne tallennuksesta
        GameData.Spells spellData = GameDataManager.Instance.GameData.spells;
        GameData.Player playerData = GameDataManager.Instance.GameData.player;
        if (playerData.firstCheckPointReached)
        {
            Debug.Log("PlayerController - First Checkpoint Reached");
            Debug.Log(spellData.count[0]);
            Array.Copy(spellData.owned, spellOwned, spells.Length);
            Array.Copy(spellData.count, spellCount, spells.Length);
            for (int i = 0; i < spells.Length; i++) setSpellState(i, spellOwned[i], spellCount[i]);
        }
        else
        {
            for (int i = 0; i < spells.Length; i++) setSpellState(i, false, 0);
            setSpellState(0, true, 1); //parannusloitsu on käytössä heti pelin alussa
        }

        EnableInput = true;
        //loitsumenu avataan tutoriaalin suorittamisen jälkeen
        SpellMenuEnabled = playerData.firstCheckPointReached;

        defaultDeltaTime = Time.fixedUnscaledDeltaTime;//Time.fixedDeltaTime;
        defaultMaximumDeltaTime = 1f;//Time.maximumDeltaTime;
        SetSpellMenuSlowDown(false);

        playerControls = GameObject.Find("Player").GetComponent<PlayerControls>();

        

    }

    public void SwipeLeft()
    {
        if (EnableInput && spellMenuState == SpellMenuState.Closed) playerControls.SwipeLeft();
    }
    public void SwipeRight()
    {
        if (EnableInput && spellMenuState == SpellMenuState.Closed) playerControls.SwipeRight();
    }

    public void SetCutscene(bool enabled)
    {
        cutscene = enabled;
        EnableInput = !cutscene;
        //SetSlowdown();
        Time.timeScale = cutscene || spellMenuState != SpellMenuState.Closed ? 0.000001f : 1f;
        //Time.timeScale = paused || spellMenuState != SpellMenuState.Closed ? 0f : 1f;
    }

    public void SetPaused(bool enabled)
    {
        paused = enabled;
        EnableInput = !paused && !cutscene;
        SetSlowdown();
        //Time.timeScale = enabled || spellMenuState != SpellMenuState.Closed ? 0f : 1f;
    }
    public void SetSlowdown()
    {
        
        Time.timeScale = paused || cutscene || spellMenuState != SpellMenuState.Closed ? 0.000001f : 1f;
    }


    void SetSpellMenuSlowDown(bool enabled)
    {
        SetSlowdown();
        //Time.timeScale = enabled ? 0f : 1f;
        //Time.fixedDeltaTime = defaultDeltaTime * Time.timeScale;
        //Time.maximumDeltaTime = defaultMaximumDeltaTime * Time.timeScale;
        /*Time.timeScale = enabled ? timeSlowDownFactor : 1f;
        Time.fixedDeltaTime = defaultDeltaTime * Time.timeScale;
        Time.maximumDeltaTime = defaultMaximumDeltaTime * Time.timeScale;*/
    }
    public void Tap(LeanFinger f)
    {
        if (EnableInput)
        {
            if (spellMenuState == SpellMenuState.Closed)
            {
                float tapStrength = Mathf.Min(f.Age, maxTapLength) / maxTapLength;
                float jumpPower = Mathf.Min(tapStrength + minTapStrength, 1f);
                playerControls.Tap(/*jumpPower*/1f);
            }
            //loitsumenun sulkeminen näpäyttämällä
            else if (spellMenuState == SpellMenuState.SelectSpell)
            {
                ToggleSpellMenu();
            }
        }
    }
    public void SwipeUp()
    {
        if (EnableInput && SpellMenuEnabled) ToggleSpellMenu();
    }
    public void SwipeDown()
    {
        if (EnableInput && spellMenuState == SpellMenuState.SelectSpell)
        {
            toggleSpellMenuSound.Play();
            CloseSpellMenu();
        }
    }

    public void ToggleSpellMenu()
    {
        if (spellMenuState == SpellMenuState.Closed || spellMenuState == SpellMenuState.SelectSpell)
        {
            toggleSpellMenuSound.Play();
            spellMenuCanvas.enabled = !spellMenuCanvas.enabled;
            spellMenuButtons.SetActive(true);
            spellDrawBackground.enabled = false;
            spellDrawGuide.enabled = false;
            
            spellMenuState = spellMenuCanvas.enabled ? SpellMenuState.SelectSpell : SpellMenuState.Closed;
            SetSpellMenuSlowDown(spellMenuCanvas.enabled);
        }
    }

    public void StartSpellGestureDrawMode(int spellNumber)
    {
        //siirry loitsun piirtotilaan
        spellMenuState = SpellMenuState.DrawGesture;
        selectedSpell = spellNumber;
        spellMenuButtons.SetActive(false);
        spellDrawBackground.enabled = true;
        spellDrawGuide.sprite = spells[spellNumber].guideSprite;
        spellDrawGuide.enabled = true;
    }
    void CloseSpellMenu()
    {
        spellMenuState = SpellMenuState.Closed;
        spellMenuCanvas.enabled = false;
        SetSpellMenuSlowDown(false);
    }

    public void CastSpell(int spellNumber)
    {
        playerControls.CastSpell(spellNumber, spells[spellNumber]);
    }
    public void FingerDown()
    {
        //Debug.Log("Finger down");
    }
    public void FingerUp(LeanFinger f)
    {
        if (spellMenuState == SpellMenuState.DrawGesture)
        {
            //pelaajan piirtämän spell gesturen tunnistus
            IList<LeanSnapshot> leanSnapshots = f.Snapshots;
            Debug.Log(leanSnapshots.Count);
            List<Point> points = new List<Point>();
            //siirrä leantouchin pistelista sormen kosketuspisteistä pdollarin käytettäväksi
            foreach (LeanSnapshot snapshot in leanSnapshots)
            {
                Point newPoint = new Point(snapshot.ScreenPosition.x, snapshot.ScreenPosition.y, 0);
                points.Add(newPoint);
            }
            if (leanSnapshots.Count > 1)
            {
                Gesture spellGesture = new Gesture(points.ToArray());
                Result gestureResult = PointCloudRecognizer.Classify(spellGesture, storedGestures.ToArray());

                //Debug.Log(gestureResult.GestureClass + ": " + gestureResult.Score);
                //Debug.Log("Selected Spell: "+spellNames[selectedSpell]);

                if (gestureResult.Score > gestureAcceptMinScore)
                {
                    if (gestureResult.GestureClass.Equals(spells[selectedSpell].gestureName))
                    {
                        if (spellOwned[selectedSpell] && spellCount[selectedSpell] > 0)
                        {
                            CastSpell(selectedSpell);
                            setSpellState(selectedSpell, true, spellCount[selectedSpell] - 1);
                        }
                    }
                }
            }

            CloseSpellMenu();

        }
    }
}

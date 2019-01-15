using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckPoint : MonoBehaviour {

    public GameObject flag;
    public ParticleSystem activationEffect;

    bool activated;
    Animator anim;
    PersistentID id;
    GameObject spawnPoint;
    AudioSource activationSound;

	// Use this for initialization
	void Start () {
        anim = flag.GetComponent<Animator>();
        id = GetComponent<PersistentID>();
        activationSound = GetComponent<AudioSource>();
        spawnPoint = transform.Find("SpawnPoint").gameObject;
        activated = false;
        //tarkista ja päivitä checkpointin tiedot pelin tallennuksessa
        GameData data = GameDataManager.Instance.GameData;
        if (!data.checkPoints.ContainsKey(id.id))
        {
            data.checkPoints[id.id] = new GameData.CheckPoint();
        }
        else
        {
            if (data.checkPoints[id.id].activated)
            {
                Debug.Log("CheckPoint - " + id.id + " already activated");
                activated = true;
                anim.SetBool("Raised", true);
            }
        }
    }
	
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;
            activationEffect.Play();
            anim.SetTrigger("Raise");
            activationSound.Play();
            //pelin tallennus
            GameObject player = GameObject.Find("Player");
            GameData data = GameDataManager.Instance.GameData;
            data.checkPoints[id.id].activated = true;
            //paikka, suunta, polku, checkpointin saavutus
            data.player.position = new GameData.SerializableVector3(spawnPoint.transform.position);
            data.player.rotation = new GameData.SerializableQuaternion(spawnPoint.transform.rotation);
            data.player.firstCheckPointReached = true;
            data.player.pathName = player.GetComponent<PathKeeper>().currentSpline.name;
            //health
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            data.player.currentHealth = healthSystem.currentHealth;
            data.player.maxHealth = healthSystem.maxHealth;
            //loitsut
            PlayerController playerController = GameObject.Find("TouchControlHandler").GetComponent<PlayerController>();
            GameData.Spells spellData = data.spells;
            spellData.owned = new bool[playerController.spellOwned.Length];
            spellData.count = new int[playerController.spellCount.Length];
            Array.Copy(playerController.spellOwned, spellData.owned, playerController.spellOwned.Length);
            Array.Copy(playerController.spellCount, spellData.count, playerController.spellCount.Length);
            //tallenna pelin tila tiedostoon
            GameDataManager.Instance.SaveGame();
        }
    }
}

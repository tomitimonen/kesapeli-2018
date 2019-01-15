#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class IDUpdater : ScriptableWizard {

    //aseta uniikit ID:t kaikille kentässä oleville vihollisille
    [MenuItem ("Custom/Update IDs")]
    static void UpdateIDs()
    {
        int numCheckedIDs = 0;
        int numUpdatedIDs = 0;
        IDictionary<int, int> duplicateIDCounts = new Dictionary<int, int>();
        IList<PersistentID> idObjects = new List<PersistentID>();
        //etsi kaikki scenen GameObjecteissa olevat PersistentID-komponentit, myös epäaktiivisista GameObjecteista
        GameObject[] gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject obj in gameObjects)
        {
            //jätä objekti pois listalta jos se on prefab-originaali (ei scenessä)
            if (PrefabUtility.GetPrefabParent(obj) == null && PrefabUtility.GetPrefabObject(obj) != null) continue;

            PersistentID idObj = obj.GetComponent<PersistentID>();
            if (idObj != null)
            {
                idObjects.Add(idObj);
            }
        }
        //tarkista, onko id-arvoissa duplikaatteja ja anna virheilmoitus, mikäli on
        foreach (PersistentID id in idObjects)
        {
            if (id.id > 0)
            {
                if (duplicateIDCounts.ContainsKey(id.id)) duplicateIDCounts[id.id]++;
                else duplicateIDCounts.Add(id.id, 0);
            }
        }
        foreach(int id in duplicateIDCounts.Keys)
        {
            int dupcount = duplicateIDCounts[id];
            if (dupcount > 0) Debug.LogError("IDUpdater - " + dupcount + " duplicate IDs for ID " + id);
        }
        //etsi suurin olemassa oleva ID:n arvo, uudet arvot ovat tätä seuraavasta arvosta alkaen
        int maxId = 0;
        foreach(PersistentID id in idObjects)
        {
            if (id.id > maxId) maxId = id.id;         
        }
        //aseta ID:t kaikille ID-objekteille, joilta puuttuu arvo
        foreach (PersistentID id in idObjects)
        {
            if (id.id < 0) //negatiivinen id on oletusarvo, joka tarkoittaa että ID:tä ei ole asetettu
            {
                int newId = maxId + 1;
                maxId = newId;
                //id pitää asettaa näin, jotta uusi arvo säilyy play modeen siirtymisen jälkeen
                SerializedObject obj = new SerializedObject(id);
                obj.FindProperty("id").intValue = newId;
                obj.ApplyModifiedProperties();
                numUpdatedIDs++;
            }
        }
        numCheckedIDs = idObjects.Count;
        Debug.Log("IDUpdater:\n"+ "Checked " + numCheckedIDs + " IDs\n"+ "Updated " + numUpdatedIDs + " IDs");
    }
}

#endif
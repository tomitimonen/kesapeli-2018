#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LightMapScaler : ScriptableWizard {

    //pienentää kaikkien staattisten objektien lightmap scalea
    [MenuItem ("Custom/Set lightmap scales")]
    static void SetLightmapScales()
    {
        int numChangedObjects = 0;
        IList<Renderer> renderers = new List<Renderer>();
        //etsi kaikki scenen GameObjecteissa olevat PersistentID-komponentit, myös epäaktiivisista GameObjecteista
        GameObject[] gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject obj in gameObjects)
        {
            //jätä objekti pois listalta jos se on prefab-originaali (ei scenessä)
            if (PrefabUtility.GetPrefabParent(obj) == null && PrefabUtility.GetPrefabObject(obj) != null) continue;
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                renderers.Add(rend);
            }
        }
        //aseta lightmap scale kaikille objekteille
        foreach (Renderer rend in renderers)
        {
            SerializedObject obj = new SerializedObject(rend);
            obj.FindProperty("m_ScaleInLightmap").floatValue = .1f;
            obj.ApplyModifiedProperties();
            numChangedObjects++;
        }

        Debug.Log("LightMapScaler:\n" + "Set lightmap scale for " + numChangedObjects+" objects");

    }
}

#endif
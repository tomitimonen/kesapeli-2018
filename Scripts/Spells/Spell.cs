using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Spell", menuName = "SpellMenu/Spell")]//WHAT!??!
public class Spell : ScriptableObject
{
    new public string name = "Spell";
    public TextAsset gestureXml;
    public string gestureName;
    public Sprite icon = null;
    public Sprite guideSprite;
    public UnityEvent useAction;
    public AudioClip castSound;

}

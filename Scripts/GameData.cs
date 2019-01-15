using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {

    [System.Serializable]
    public class SerializableVector3
    {
        public float x, y, z;
        public SerializableVector3(Vector3 pos)
        {
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }
        public Vector3 toVector3()
        {
            return new Vector3(x, y, z);
        }
    }
    [System.Serializable]
    public class SerializableQuaternion
    {
        public float x, y, z, w;
        public SerializableQuaternion(Quaternion rot)
        {
            x = rot.x;
            y = rot.y;
            z = rot.z;
            w = rot.w;
        }
        public Quaternion toQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }

    [System.Serializable]
    public class Enemy
    {
        public bool isAlive;
        public int health;
        public Enemy()
        {
            isAlive = true;
            health = 0;
        }
    }
    public IDictionary<int, Enemy> enemies;
    [System.Serializable]
    public class CheckPoint
    {
        public bool activated;
        public CheckPoint()
        {
            activated = false;
        }
    }
    public IDictionary<int, CheckPoint> checkPoints;
    [System.Serializable]
    public class Player
    {
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public int currentHealth;
        public int maxHealth;
        public bool firstCheckPointReached;
        public string pathName;
        public Player()
        {
            position = new SerializableVector3(Vector3.zero);
            rotation = new SerializableQuaternion(Quaternion.identity);
            firstCheckPointReached = false;
            pathName = "";
            currentHealth = 0;
            maxHealth = 0;
        }
    }
    public Player player;
    [System.Serializable]
    public class Spells
    {
        public bool[] owned;
        public int[] count;
    }
    public Spells spells;

    [System.Serializable]
    public class Tutorial
    {
        public bool completed;
        public Tutorial()
        {
            completed = false;
        }
    }
    public Tutorial tutorial;
    public GameData()
    {
        enemies = new Dictionary<int, Enemy>();
        checkPoints = new Dictionary<int, CheckPoint>();
        player = new Player();
        spells = new Spells();
        tutorial = new Tutorial();
    }

}

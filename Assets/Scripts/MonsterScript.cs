using UnityEngine;
using System.Collections;

public class MonsterScript : MonoBehaviour
{
    public float lifetime = 12f;
    public GameObject spawnEffect;
    public GameObject deathEffect;
    private float monsterTimer = 0f;
    private GameObject rootGameObj;

    void Start()
    {
        rootGameObj = transform.GetChild(1).gameObject;
        Instantiate(spawnEffect, rootGameObj.transform);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if(monsterTimer >= (lifetime - 3f))
        {
            Instantiate(deathEffect, rootGameObj.transform);
            monsterTimer = 0;
        }
        else
        {
            monsterTimer += Time.deltaTime;
        }
    }
}
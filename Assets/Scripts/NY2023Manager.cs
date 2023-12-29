using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

public class NY2023Manager : MonoBehaviour
{
    public GameObject scorpyObj;
    public GameObject scorpyPlane;
    public GameObject scorpySpawner;
    public GameObject scorpyProjectileSpawner;

    public GameObject wayneObj;
    public GameObject waynePlane;
    public GameObject wayneSpawner;
    public GameObject wayneProjectileSpawner;

    public GameObject baaulpObj;
    public GameObject baaulpBed;

    public GameObject pipeSpawner;
    public GameObject stageSpawner;
    public GameObject aboveStageSpawner;
    public GameObject scorbSpawner;

    public GameObject rock;
    public GameObject[] effects;
    public GameObject endingEffect;
    public GameObject projectileEffect;
    public GameObject defaultProjectileTarget;
    public GameObject spawnEffect;
    public GameObject teleportEffect;

    public GameObject endingScorb;
    public GameObject demon;

    public GameObject[] gems;


    public GameObject[] bandInstruments;

    private bool endingSequence = false;
    private float endingTimer;

    private void Start()
    {
     
    }

    private void Update()
    {
        if (endingSequence)
        {
            if (endingTimer >= 10f)
            {
                Instantiate(endingEffect, stageSpawner.transform.position, new Quaternion(0, 0, 0, 0));
                Instantiate(endingEffect, stageSpawner.transform.position, new Quaternion(0, 0, 0, 0));
                endingSequence = false;
            }
            else
            {
                endingTimer += Time.deltaTime;
            }
        }

        //spawn a rock effect thing to test
        if (Input.GetKeyDown(KeyCode.T))
        {
            var effect = Instantiate(rock, scorpySpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }

        //spawn a random effect out the pipe
        if (Input.GetKeyDown(KeyCode.Y))
        {
            var effect = Instantiate(effects[RandomNumber(0, 9)], pipeSpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }

        //toggle instrument animation
        if (Input.GetKeyDown(KeyCode.I))
        {
            foreach(var instrument in bandInstruments)
            {
                var instrumentAnimator = instrument.GetComponent<Animator>();
                instrumentAnimator.SetBool("isPlaying", !instrumentAnimator.GetBool("isPlaying"));
            }
        }

        //spawn a demon
        if (Input.GetKeyDown(KeyCode.P))
        {
            var glassb = Instantiate(demon);
        }

        //ending sequence
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            endingSequence = true;
            endingScorb.GetComponent<Animator>().SetTrigger("ending");
            Instantiate(rock, scorbSpawner.transform.position, new Quaternion(0, 100, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            var demon = GameObject.FindGameObjectWithTag("Enemy");

            if (scorpyObj.transform.GetChild(0).gameObject.activeSelf)
            {
                var projectile = Instantiate(projectileEffect, scorpyProjectileSpawner.transform.position, new Quaternion(180, 100, 180, 180));
                projectile.transform.LookAt(defaultProjectileTarget.transform);

                if (demon != null)
                    projectile.transform.LookAt(demon.transform.GetChild(2));
            }

            if (wayneObj.transform.GetChild(0).gameObject.activeSelf)
            {
                var projectileW = Instantiate(projectileEffect, wayneProjectileSpawner.transform.position, new Quaternion(180, 100, 180, 180));
                projectileW.transform.LookAt(defaultProjectileTarget.transform);

                if (demon != null)
                    projectileW.transform.LookAt(demon.transform.GetChild(2));
            }
        }

        //toggle visibility of scorpy
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            scorpyPlane.SetActive(!scorpyPlane.activeSelf);
            Instantiate(spawnEffect, scorpySpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }

        //toggle visibility of wayne
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            waynePlane.SetActive(!waynePlane.activeSelf);
            Instantiate(spawnEffect, wayneSpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }

        //toggle scorpy entrance
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            scorpyObj.GetComponent<Animator>().SetTrigger("Enter");
        }

        //toggle scorpy teleport 1
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Instantiate(teleportEffect, scorpySpawner.transform.position, new Quaternion(0, 0, 0, 0));
            scorpyObj.GetComponent<Animator>().SetBool("Teleport1", true);
        }

        //toggle scorpy idle position
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Instantiate(teleportEffect, scorpySpawner.transform.position, new Quaternion(0, 0, 0, 0));
            scorpyObj.GetComponent<Animator>().SetBool("Teleport1", false);
            scorpyObj.GetComponent<Animator>().SetBool("Teleport2", false);
        }

        //toggle scorpy teleport 1
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Instantiate(teleportEffect, scorpySpawner.transform.position, new Quaternion(0, 0, 0, 0));
            scorpyObj.GetComponent<Animator>().SetBool("Teleport2", true);
        }

        //toggle baaulp entrance
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            baaulpObj.GetComponent<Animator>().SetTrigger("Enter");
        }

        //toggle baaulp exit
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            baaulpObj.GetComponent<Animator>().SetTrigger("Exit");
            Instantiate(baaulpBed);
        }

        //spawn a random effect out the pipe
        if (Input.GetKeyDown(KeyCode.M))
        {
            var effect = Instantiate(gems[RandomNumber(0, 3)], pipeSpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }
    }

    private int RandomNumber(int lowerNum = 1, int higherNum = 2000)
    {
        System.Random rand = new System.Random();
        return rand.Next(lowerNum, higherNum);
    }
}

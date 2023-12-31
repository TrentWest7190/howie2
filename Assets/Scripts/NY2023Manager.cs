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
    public GameObject gemSpawner;

    public GameObject electricityEffect;
    public GameObject[] effects;
    public GameObject endingEffect;
    public GameObject projectileEffect;
    public GameObject defaultProjectileTarget;
    public GameObject spawnEffect;
    public GameObject teleportEffect;

    public GameObject endingScorb;
    public GameObject demon;

    public GameObject[] gems;
    public GameObject trophy;

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

        //spawn an electricity effect on me and wayne
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Instantiate(electricityEffect, scorpySpawner.transform.position, new Quaternion(0, 0, 0, 0));
            Instantiate(electricityEffect, wayneSpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }

        //spawn a random effect out the pipe
        if (Input.GetKeyDown(KeyCode.F15))
        {
            Instantiate(effects[RandomNumber(0, 2)], pipeSpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }

        //toggle instrument animation
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            foreach(var instrument in bandInstruments)
            {
                var instrumentAnimator = instrument.GetComponent<Animator>();
                instrumentAnimator.SetBool("isPlaying", !instrumentAnimator.GetBool("isPlaying"));
            }
        }

        //spawn a demon
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            var glassb = Instantiate(demon);
        }

        //ending sequence
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            endingSequence = true;
            endingScorb.GetComponent<Animator>().SetTrigger("ending");
            Instantiate(electricityEffect, scorbSpawner.transform.position, new Quaternion(0, 100, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
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
            Instantiate(teleportEffect, scorpySpawner.transform);
            scorpyObj.GetComponent<Animator>().SetBool("Teleport1", true);
            scorpyObj.GetComponent<Animator>().SetBool("Teleport2", false);
        }

        //toggle scorpy idle position
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Instantiate(teleportEffect, scorpySpawner.transform);
            scorpyObj.GetComponent<Animator>().SetBool("Teleport1", false);
            scorpyObj.GetComponent<Animator>().SetBool("Teleport2", false);
        }

        //toggle scorpy teleport 2
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Instantiate(teleportEffect, scorpySpawner.transform);
            scorpyObj.GetComponent<Animator>().SetBool("Teleport2", true);
            scorpyObj.GetComponent<Animator>().SetBool("Teleport1", false);
        }

        //toggle scorpy animation 1
        if (Input.GetKeyDown(KeyCode.F13))
        {
            scorpyObj.GetComponent<Animator>().SetTrigger("Anim1");
        }

        //toggle scorpy animation 2
        if (Input.GetKeyDown(KeyCode.F14))
        {
            scorpyObj.GetComponent<Animator>().SetTrigger("Anim2");
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
            Destroy(baaulpObj, 10f);
            Destroy(baaulpBed, 10f);
        }

        //toggle wayne entrance
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            wayneObj.GetComponent<Animator>().SetTrigger("Enter");
        }

        //toggle wayne animation 1
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            wayneObj.GetComponent<Animator>().SetTrigger("Anim1");
        }

        //toggle wayne animation 2
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            wayneObj.GetComponent<Animator>().SetTrigger("Anim2");
        }

        //spawn a random gem
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            var effect = Instantiate(gems[RandomNumber(0, 3)], gemSpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }

        //spawn trophy
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            var trophyStatus = trophy.GetComponent<Animator>().GetBool("Appear");
            trophy.GetComponent<Animator>().SetBool("Appear", !trophyStatus);
        }

    }

    private int RandomNumber(int lowerNum = 1, int higherNum = 2000)
    {
        System.Random rand = new System.Random();
        return rand.Next(lowerNum, higherNum);
    }
}

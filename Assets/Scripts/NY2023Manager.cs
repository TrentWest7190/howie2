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
    public GameObject scorpySpawner;
    public GameObject scorpyProjectileSpawner;

    public GameObject wayneObj;
    public GameObject wayneSpawner;
    public GameObject wayneProjectileSpawner;

    public GameObject pipeSpawner;
    public GameObject stageSpawner;
    public GameObject aboveStageSpawner;
    public GameObject scorbSpawner;
    public GameObject rock;
    public GameObject[] effects;
    public GameObject endingEffect;
    public GameObject endingScorb;
    public GameObject demon;
    public GameObject projectileEffect;
    public GameObject defaultProjectileTarget;

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
            //we can improve this eventually by deleting previous instances of this before spawning one but w/e for now
            var glassb = Instantiate(rock, scorpySpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }
        //spawn something out the pipe
        if (Input.GetKeyDown(KeyCode.Y))
        {
            var glassb = Instantiate(effects[RandomNumber(0, 9)], pipeSpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }
        //spawn something atop the stage
        if (Input.GetKeyDown(KeyCode.U))
        {
            var glassb = Instantiate(effects[RandomNumber(0, 9)], pipeSpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }
        //spawn something atop the stage
        if (Input.GetKeyDown(KeyCode.I))
        {
            foreach(var instrument in bandInstruments)
            {
                var instrumentAnimator = instrument.GetComponent<Animator>();
                instrumentAnimator.SetBool("isPlaying", !instrumentAnimator.GetBool("isPlaying"));
            }
        }

        //spawn something atop the stage
        if (Input.GetKeyDown(KeyCode.P))
        {
            var glassb = Instantiate(demon);
        }

        //ending sequence
        if (Input.GetKeyDown(KeyCode.O))
        {
            endingSequence = true;
            endingScorb.GetComponent<Animator>().SetTrigger("ending");
            Instantiate(rock, scorbSpawner.transform.position, new Quaternion(0, 100, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            var demon = GameObject.FindGameObjectWithTag("Enemy");

            if (scorpyObj.activeSelf)
            {
                var projectile = Instantiate(projectileEffect, scorpyProjectileSpawner.transform.position, new Quaternion(180, 100, 180, 180));
                projectile.transform.LookAt(defaultProjectileTarget.transform);

                if (demon != null)
                    projectile.transform.LookAt(demon.transform.GetChild(2));
            }

            if (wayneObj.activeSelf)
            {
                var projectileW = Instantiate(projectileEffect, wayneProjectileSpawner.transform.position, new Quaternion(180, 100, 180, 180));
                projectileW.transform.LookAt(defaultProjectileTarget.transform);

                if (demon != null)
                    projectileW.transform.LookAt(demon.transform.GetChild(2));
            }
        }
    }

    private int RandomNumber(int lowerNum = 1, int higherNum = 2000)
    {
        System.Random rand = new System.Random();
        return rand.Next(lowerNum, higherNum);
    }
}

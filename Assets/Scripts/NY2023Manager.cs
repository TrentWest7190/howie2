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
    public GameObject wayneObj;
    public GameObject wayneSpawner;
    public GameObject pipeSpawner;
    public GameObject stageSpawner;
    public GameObject aboveStageSpawner;
    public GameObject rock;
    public GameObject[] effects;

    public GameObject[] bandInstruments;

    private void Start()
    {
     
    }

    private void Update()
    {
        //spawn a rock effect thing to test
        if (Input.GetKeyDown(KeyCode.T))
        {
            //we can improve this eventually by deleting previous instances of this before spawning one but w/e for now
            var glassb = Instantiate(rock, scorpySpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }
        //spawn something out the pipe
        if (Input.GetKeyDown(KeyCode.Y))
        {
            var glassb = Instantiate(effects[0], pipeSpawner.transform.position, new Quaternion(0, 0, 0, 0));
        }
        //spawn something atop the stage
        if (Input.GetKeyDown(KeyCode.U))
        {
            var glassb = Instantiate(effects[0], pipeSpawner.transform.position, new Quaternion(0, 0, 0, 0));
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
    }

    private string RandomNumber()
    {
        System.Random rand = new System.Random();
        return rand.Next(1, 2000).ToString();
    }
}

using UnityEngine;

namespace EasyCharacterMovement
{
    /// <summary>
    /// Helper class to make sure Physics Auto Simulation is enabled in Physics Project Settings, otherwise ECM2 demos wont work.
    /// </summary>

    public class AutoSimulationEnabler : MonoBehaviour
    {
        void Start()
        {
            if (Physics.autoSimulation == false)
            {
                Debug.LogWarning("Phsyics Autos Simulation is Disabled in Project Settings (Physics).\n" +
                    " Please make sure to enable it.");

                Physics.autoSimulation = true;
            }
        }
    }
}

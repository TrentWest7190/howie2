using System.Collections.Generic;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementExamples
{
    [DefaultExecutionOrder(-1000)]
    public class SimulationManager : MonoBehaviour
    {
        public enum SimulationMethod
        {
            Fixed,
            SemiFixed,
            Substepping
        }

        public SimulationMethod simulationMethod = SimulationMethod.Fixed;

        [Header("Substepping")]

        public float MaxPhysicsDeltaTime = 0.016667f;
        public float MaxSubstepDeltaTime = 0.019231f;
        
        public int MaxSubsteps = 4;

        private static readonly List<ISimulatable> _simulatables = new List<ISimulatable>(16);

        private static float _accumulator;

        public static void AddSimulatable(ISimulatable simulatable)
        {
            if (simulatable == null)
                return;

            if (!_simulatables.Contains(simulatable))
                _simulatables.Add(simulatable);
        }

        public static void RemoveSimulatable(ISimulatable simulatable)
        {
            if (simulatable == null)
                return;

            if (!_simulatables.Contains(simulatable))
                _simulatables.Add(simulatable);
        }

        public static void FlushSimulatables()
        {
            _simulatables.Clear();
        }

        public static void Simulate(float deltaTime)
        {
            // Call pre-physics simulatables

            for (int i = 0, c = _simulatables.Count; i < c; i++)
            {
                ISimulatable simulatable = _simulatables[i];

                simulatable.PrePhysicsUpdate(deltaTime);
            }

            // Simulate physics

            Physics.Simulate(deltaTime);

            // Call post-physics simulatables

            for (int i = 0, c = _simulatables.Count; i < c; i++)
            {
                ISimulatable simulatable = _simulatables[i];

                simulatable.PostPhysicsUpdate(deltaTime);
            }
        }

        public static void Interpolate(float interpolationFactor)
        {
            for (int i = 0, c = _simulatables.Count; i < c; i++)
            {
                ISimulatable simulatable = _simulatables[i];

                simulatable.Interpolate(interpolationFactor);
            }
        }

        private void Awake()
        {
            // Disable auto simulation

            Physics.autoSimulation = false;
            Physics.autoSyncTransforms = false;
        }

        private void Update()
        {
            if (simulationMethod == SimulationMethod.Fixed)
            {
                float deltaTime = Time.deltaTime;
                if (deltaTime > Time.maximumDeltaTime)
                    deltaTime = Time.maximumDeltaTime;

                _accumulator += deltaTime;

                while (_accumulator >= MaxPhysicsDeltaTime)
                {
                    _accumulator -= MaxPhysicsDeltaTime;

                    Simulate(MaxPhysicsDeltaTime);
                }

                float interpolationFactor = _accumulator / MaxPhysicsDeltaTime;

                Interpolate(interpolationFactor);

            } else if (simulationMethod == SimulationMethod.SemiFixed)
            {
                float deltaTime = Time.deltaTime;
                if (deltaTime > Time.maximumDeltaTime)
                    deltaTime = Time.maximumDeltaTime;

                float fixedDeltaTime = Time.fixedDeltaTime;

                while (deltaTime >= fixedDeltaTime)
                {
                    deltaTime -= fixedDeltaTime;
                    Simulate(fixedDeltaTime);
                }

                if (deltaTime > 0.0f)
                    Simulate(deltaTime);
            }
            else
            {
                // Simulate using Substepping

                float deltaTime = Time.deltaTime;
                if (deltaTime > Time.maximumDeltaTime)
                    deltaTime = Time.maximumDeltaTime;

                int substeps = Mathf.CeilToInt(deltaTime / MaxPhysicsDeltaTime);
                if (substeps > MaxSubsteps)
                    substeps = MaxSubsteps;

                float substepDeltaTime = deltaTime / substeps;
                if (substepDeltaTime > MaxSubstepDeltaTime)
                    substepDeltaTime = MaxSubstepDeltaTime;

                for (int i = 0; i < substeps; i++)
                    Simulate(substepDeltaTime);
            }
        }
    }
}

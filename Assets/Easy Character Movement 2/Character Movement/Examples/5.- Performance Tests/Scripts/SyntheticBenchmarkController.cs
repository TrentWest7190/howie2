using System.Collections.Generic;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementExamples
{
    public class SyntheticBenchmarkController : MonoBehaviour
    {
        [Range(0, 5000)]
        public int spawnCount = 1000;

        public CharacterMovement characterPrefab;

        private readonly List<CharacterMovement> _characters = new List<CharacterMovement>(1024);

        private Vector3 _movementDirection;

        private void Spawn(int spawnCount)
        {
            for (int i = _characters.Count - 1; i >= 0; i--)
            {
                CharacterMovement movement = _characters[i];
                Destroy(movement.gameObject);
            }

            _characters.Clear();

            int charsPerRow = Mathf.CeilToInt(Mathf.Sqrt(spawnCount));
            Vector3 firstPos = charsPerRow * 2f * 0.5f * -Vector3.one;
            firstPos.y = 0f;

            for (int i = 0; i < spawnCount; i++)
            {
                int row = i / charsPerRow;
                int col = i % charsPerRow;

                Vector3 pos = firstPos + Vector3.right * row * 2f + Vector3.forward * col * 2f;

                CharacterMovement newChar = Instantiate(characterPrefab);
                newChar.SetPosition(pos);

                _characters.Add(newChar);
            }
        }
        
        private void Simulate(float deltaTime)
        {
            Vector3 newVelocity = _movementDirection * 5.0f;

            for (int i = _characters.Count - 1; i >= 0; i--)
            {
                CharacterMovement movement = _characters[i];
                
                movement.RotateTowards(_movementDirection, 540.0f * deltaTime);
                movement.Move(newVelocity, deltaTime);
            }
        }

        private void Start()
        {
            Spawn(spawnCount);
        }
        
        private void Update()
        {
            // Update move direction

            _movementDirection = Mathf.Sin(Time.time * 2f) * Vector3.forward;

            // Simulate characters

            Simulate(Time.deltaTime);
        }
    }
}
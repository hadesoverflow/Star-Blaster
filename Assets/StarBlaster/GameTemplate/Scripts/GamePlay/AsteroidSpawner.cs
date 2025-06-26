using System.Collections.Generic;
using DenkKits.UIManager.Scripts.Base;
using UnityEngine;
using DG.Tweening;
using StarBlaster.GameTemplate.Scripts.Controllers;

namespace StarBlaster.GameTemplate.Scripts.GamePlay
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [Header("Asteroid Settings")] 
        public List<GameObject> asteroidPrefabs;

        [Header("Spawn Settings")] 
        public float spawnInterval = 2f;
        public float spawnHeightOffset = 1f;
        public float moveDuration = 5f;
        public float moveDistance = 15f;
        public float maxAngle = 30f;

        private float timer;
        private Camera mainCamera;

        // Stage management
        private float stageTimer = 0f;
        private float stageDuration = 10f;

        private int currentSpawnCount = 1;  // số thiên thạch spawn cùng lúc
        private int maxSpawnCount = 10;

        private float minMoveDuration = 1f; // tốc độ nhanh nhất (thời gian di chuyển nhỏ nhất)

        private List<GameObject> activeAsteroids = new List<GameObject>();

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (GameController.Instance.isGamePaused) return;

            timer += Time.deltaTime;
            stageTimer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                for (int i = 0; i < currentSpawnCount; i++)
                {
                    SpawnAsteroid();
                }
                timer = 0f;
            }

            if (stageTimer >= stageDuration)
            {
                stageTimer = 0f;
                IncreaseDifficultyRandomly();
            }

        }

        private void SpawnAsteroid()
        {
            if (asteroidPrefabs == null || asteroidPrefabs.Count == 0)
            {
                Debug.LogWarning("Asteroid prefab list is empty.");
                return;
            }

            Vector3 topLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
            Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            float spawnX = Random.Range(topLeft.x, topRight.x);
            float spawnY = topLeft.y + spawnHeightOffset;
            Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

            GameObject selectedPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Count)];
            GameObject asteroid = SimplePool.Spawn(selectedPrefab, spawnPos, Quaternion.identity);
            activeAsteroids.Add(asteroid);

            float angle = Random.Range(-maxAngle, maxAngle);
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.down;
            Vector3 targetPos = spawnPos + direction * moveDistance;

            asteroid.transform.rotation = Quaternion.identity;

            asteroid.transform.DOMove(targetPos, moveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    activeAsteroids.Remove(asteroid);
                    SimplePool.Despawn(asteroid);
                });
        }

        private void IncreaseDifficultyRandomly()
        {
            bool increaseSpeed = Random.value > 0.5f;

            if (increaseSpeed)
            {
                // Giảm thời gian moveDuration (tăng tốc độ bay)
                float newDuration = Mathf.Max(minMoveDuration, moveDuration - 1f);
                if (newDuration < moveDuration)
                {
                    moveDuration = newDuration;
                    ShowNotice("Asteroids are moving faster!");
                }
                else
                {
                    // Nếu không thể giảm nữa thì tăng spawn
                    IncreaseSpawnCount();
                }
            }
            else
            {
                IncreaseSpawnCount();
            }
        }

        private void IncreaseSpawnCount()
        {
            if (currentSpawnCount < maxSpawnCount)
            {
                currentSpawnCount++;
                ShowNotice("Asteroids are spawning more densely!");
            }
            else
            {
                // Nếu đã max spawn rồi thì thử tăng tốc
                float newDuration = Mathf.Max(minMoveDuration, moveDuration - 1f);
                if (newDuration < moveDuration)
                {
                    moveDuration = newDuration;
                    ShowNotice("Asteroids are moving faster!");
                }
            }
        }

        public void PauseAllAsteroids()
        {
            foreach (var asteroid in activeAsteroids)
            {
                asteroid.transform.DOPause();
            }
        }

        public void ResumeAllAsteroids()
        {
            foreach (var asteroid in activeAsteroids)
            {
                asteroid.transform.DOPlay();
            }
        }

        public void ShowNotice(string notice)
        {
            UIManager.Instance.AlertManager.ShowAlertMessage(notice);
        }
    }
}

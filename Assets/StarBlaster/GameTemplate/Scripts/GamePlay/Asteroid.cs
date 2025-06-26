using DenkKits.AudioManager.Scripts;
using DG.Tweening;
using StarBlaster.GameTemplate.Scripts.Controllers;
using UnityEngine;

namespace StarBlaster.GameTemplate.Scripts.GamePlay
{
    public class Asteroid : MonoBehaviour
    {
        public enum AsteroidSize
        {
            Small,
            Medium,
            Large
        }

        public AsteroidSize size;
        public GameObject explosionEffect;
        public GameObject star;

        private int _hitCount = 0;
        private int _requiredHits;

        private void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            switch (size)
            {
                case AsteroidSize.Small:
                    _requiredHits = 1;
                    break;
                case AsteroidSize.Medium:
                    _requiredHits = 2;
                    break;
                case AsteroidSize.Large:
                    _requiredHits = 3;
                    break;
            }
        }

        public void OnHit()
        {
            _hitCount++;

            if (_hitCount >= _requiredHits)
            {
                Explode();
            }
        }

        void Explode()
        {
            if (explosionEffect)
            {
                SimplePool.Spawn(explosionEffect, transform.position, Quaternion.identity);
            }

            AudioManager.Instance.PlaySfx(AudioName.Gameplay_AsteroidExplo);
            GameController.Instance.asteroidDestroyedCount++;

            if (GameController.Instance.asteroidDestroyedCount % 10 == 0)
            {
                GameController.Instance.AddSuperWeapon();
            }

            int starCount = 0;
            switch (size)
            {
                case AsteroidSize.Small:
                    starCount = 1;
                    break;
                case AsteroidSize.Medium:
                    starCount = 3;
                    break;
                case AsteroidSize.Large:
                    starCount = 5;
                    break;
            }

            for (int i = 0; i < starCount; i++)
            {
                Vector3 spawnPos = transform.position;
                GameObject spawnedStar = SimplePool.Spawn(star, spawnPos, Quaternion.identity);

                // Scale từ nhỏ tới lớn
                spawnedStar.transform.localScale = Vector3.zero;
                spawnedStar.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

                // Random hướng và khoảng cách (giảm lại)
                Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
                float distance = UnityEngine.Random.Range(0.3f, 0.7f); // trước là 0.5f - 1.5f
                Vector3 targetPos = spawnPos + (Vector3)(direction * distance);

                spawnedStar.transform.DOMove(targetPos, 0.4f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => { });
            }

            SimplePool.Despawn(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                AudioManager.Instance.PlaySfx(AudioName.Gameplay_PlayerLose);
                if (GameController.Instance.PlayerHit())
                {
                    SimplePool.Despawn(gameObject);
                }
            }
        }

        private void OnDisable()
        {
            transform.DOKill();
        }
    }
}
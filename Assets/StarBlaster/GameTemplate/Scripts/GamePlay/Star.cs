using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using StarBlaster.GameTemplate.Scripts.Controllers;

namespace StarBlaster.GameTemplate.Scripts.GamePlay
{
    public class Star : MonoBehaviour
    {
        private float lifeTime = 7f;
        private float blinkStartTime = 5f;
        private SpriteRenderer spriteRenderer;
        private Coroutine lifeCoroutine;
        private bool isBlinking = false;

        private float elapsedTime = 0f;
        private bool isPaused = false;

        private void Awake()
        {
            GameController.Instance.onPauseGame.AddListener(OnPause);
            GameController.Instance.onResumeGame.AddListener(OnResume);
        }

        private void OnPause()
        {
            isPaused = true;
            DOTween.Pause(gameObject);
        }

        private void OnResume()
        {
            isPaused = false;
            DOTween.Play(gameObject);
        }

        void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            elapsedTime = 0f;
            isPaused = false;
            isBlinking = false;
            lifeCoroutine = StartCoroutine(HandleLifetime());
        }


        IEnumerator HandleLifetime()
        {
            while (elapsedTime < lifeTime)
            {
                if (!isPaused)
                {
                    elapsedTime += Time.deltaTime;

                    if (elapsedTime >= blinkStartTime && !isBlinking)
                    {
                        Blink();
                        isBlinking = true;
                    }
                }
                yield return null;
            }

            SimplePool.Despawn(gameObject);
        }

        void Blink()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.DOFade(0f, 0.2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.Linear)
                    .SetId(gameObject); // ID để DOKill dễ hơn
            }
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject); // Dừng mọi tween có ID là gameObject

            if (lifeCoroutine != null)
            {
                StopCoroutine(lifeCoroutine);
                lifeCoroutine = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameController.Instance.AddScore(1);

                // Option: Hiệu ứng thu thập
                transform.DOKill(); // stop blink tween
                transform.DOScale(1.5f, 0.2f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        SimplePool.Despawn(gameObject);
                    });
            }
        }
    }
}

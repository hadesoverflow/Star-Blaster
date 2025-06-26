using System.Collections;
using StarBlaster.GameTemplate.Scripts.Controllers;
using UnityEngine;

namespace StarBlaster.GameTemplate.Scripts.GamePlay
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 10f;
        public float lifeTime = 5f;
        public LayerMask hitMask; // chọn layer Asteroid ở đây

        private Coroutine despawnCoroutine;
        private Vector3 lastPosition;

        private void OnEnable()
        {
            lastPosition = transform.position;
            despawnCoroutine = StartCoroutine(AutoDespawnAfterDelay());
        }

        private void OnDisable()
        {
            if (despawnCoroutine != null)
            {
                StopCoroutine(despawnCoroutine);
                despawnCoroutine = null;
            }
        }

        private IEnumerator AutoDespawnAfterDelay()
        {
            yield return new WaitForSeconds(lifeTime);
            SimplePool.Despawn(gameObject);
        }

        private void Update()
        {
            if (GameController.Instance.isGamePaused)
            {
                return;
            }
            Vector3 currentPosition = transform.position;

            Vector3 direction = (currentPosition + Vector3.up * speed * Time.deltaTime) - lastPosition;
            float distance = direction.magnitude;
            direction.Normalize();

            RaycastHit2D hit = Physics2D.Raycast(lastPosition, direction, distance, hitMask);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                Asteroid asteroid = hit.collider.GetComponent<Asteroid>();
                if (asteroid != null)
                {
                    asteroid.OnHit();
                }

                SimplePool.Despawn(gameObject);
                return;
            }

            transform.Translate(Vector3.up * speed * Time.deltaTime);

            lastPosition = currentPosition;
        }
    }
}
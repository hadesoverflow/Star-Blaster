using System;
using DenkKits.AudioManager.Scripts;
using DG.Tweening;
using StarBlaster.GameTemplate.Scripts.Controllers;
using UnityEngine;

namespace StarBlaster.GameTemplate.Scripts.GamePlay
{
    public class Player : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float delay = 0.2f;
        public GameObject bulletPrefab;
        public Transform firePoint;
        public SpriteRenderer superWeapon;
        public SpriteRenderer spriteRenderer;
        [SerializeField] private float boundsOffset = 0.5f; // khoảng cách lề an toàn

        public float superLaserDuration = 1f; // thời gian tồn tại của laser
        public float superLaserCooldown = 3f; // thời gian chờ mới được bắn lại

        private bool isFiring;
        private bool canFireLaser = true;
        public BoxCollider2D laserCollider;
        
        
        public bool isInvulnerable = false;
        public GameObject shieldEffect; 
        
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            shieldEffect.SetActive(false);

        }
        public void StartInvulnerability()
        {
            isInvulnerable = true;
            shieldEffect.SetActive(true);

            // Blink effect
            float duration = 2f;
            float blinkInterval = 0.1f;
            int blinkTimes = Mathf.RoundToInt(duration / (blinkInterval * 2));

            Sequence blinkSeq = DOTween.Sequence();
            for (int i = 0; i < blinkTimes; i++)
            {
                blinkSeq.Append(spriteRenderer.DOFade(0f, blinkInterval));
                blinkSeq.Append(spriteRenderer.DOFade(1f, blinkInterval));
            }

            blinkSeq.OnComplete(() =>
            {
                shieldEffect.SetActive(false);
                isInvulnerable = false;
            });
        }
      
        private void Start()
        {
            superWeapon.SetActive(false);
        }
        void Update()
        {
            HandleMouseFollow();
        }

        void HandleMouseFollow()
        {
            if (GameController.Instance.isGamePaused) return;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; 
            Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(mousePos);

            float followSpeed = 5f;
            transform.position = Vector3.Lerp(transform.position, worldMousePos, Time.deltaTime * followSpeed);
            ClampToScreen();
        }

        public void FireSuperWeapon()
        {
            if (!canFireLaser || GameController.Instance.weaponCoin <= 0) return;
            GameController.Instance.MinusSuperWeapon();
            AudioManager.Instance.PlaySfx(AudioName.Gameplay_Leaser);
            canFireLaser = false;
            superWeapon.SetActive(true);

            // Tính chiều cao từ đáy màn đến đỉnh màn (theo viewport Y: 0 → 1)
            Vector3 screenBottom = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0));
            Vector3 screenTop = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0));
            float laserHeight = screenTop.y - screenBottom.y;

            // Vì xoay 90 độ, ta tween size.x theo laserHeight
            superWeapon.size = new Vector2(0f, superWeapon.size.y); // bắt đầu từ 0 width

            // Tween tăng dần size.x
            float currentWidth = 0f;
            DOTween.To(() => currentWidth, x =>
                {
                    currentWidth = x;
                    superWeapon.size = new Vector2(currentWidth, superWeapon.size.y);

                    // Đồng bộ collider theo chiều dài laser
                    if (laserCollider != null)
                    {
                        laserCollider.size = new Vector2(currentWidth, superWeapon.size.y);
                        laserCollider.offset = new Vector2(currentWidth / 2f, 0f);
                    }

                }, laserHeight, 0.3f)
                .SetEase(Ease.OutCubic)
                .SetId(superWeapon); // để có thể Kill

            Invoke(nameof(DisableSuperWeapon), superLaserDuration);
            Invoke(nameof(ResetSuperWeaponCooldown), superLaserCooldown);
        }


        void DisableSuperWeapon()
        {
            DOTween.Kill(superWeapon);
            superWeapon.SetActive(false);
        }


        void ResetSuperWeaponCooldown()
        {
            canFireLaser = true;
        }

        public void HandleInput(Vector2 moveDir, bool firePressed)
        {
            MoveHorizontal(moveDir.x);
            MoveForward(moveDir.y);
            if (firePressed && !isFiring)
            {
                Fire();
            }
        }
        void MoveForward(float dirY)
        {
            transform.Translate(Vector3.up * dirY * moveSpeed * Time.deltaTime);
            ClampToScreen();
        }

        void MoveHorizontal(float dirX)
        {
            transform.Translate(Vector3.right * dirX * moveSpeed * Time.deltaTime);
            ClampToScreen();
        }
        void ClampToScreen()
        {
            Vector3 pos = transform.position;

            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

            pos.x = Mathf.Clamp(pos.x, bottomLeft.x + boundsOffset, topRight.x - boundsOffset);
            pos.y = Mathf.Clamp(pos.y, bottomLeft.y + boundsOffset, topRight.y - boundsOffset);

            transform.position = pos;
        }

        void Fire()
        {
            isFiring = true;
            var buttelt = SimplePool.Spawn(bulletPrefab, firePoint.position, Quaternion.identity);
            AudioManager.Instance.PlaySfx(AudioName.Gameplay_Shoot);
            Invoke(nameof(ResetFire), delay); // Delay nhỏ để không bắn quá nhanh
        }

        void ResetFire()
        {
            isFiring = false;
        }
    }
}
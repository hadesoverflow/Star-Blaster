using _GAME.Scripts;
using _GAME.Scripts.Popup;
using _GAME.Scripts.Views;
using DenkKits.AudioManager.Scripts;
using DenkKits.UIManager.Scripts.Base;
using DenkKits.UIManager.Scripts.UIAlert;
using DenkKits.UIManager.Scripts.UIPopup;
using DenkKits.UIManager.Scripts.UIView;
using DG.Tweening;
using Imba.Utils;
using StarBlaster.GameTemplate.Scripts.GamePlay;
using UnityEngine;
using UnityEngine.Events;

namespace StarBlaster.GameTemplate.Scripts.Controllers
{
    public class GameController : ManualSingletonMono<GameController>
    {
        public bool isGamePaused = false;
        public int score;
        public int weaponCoin;
        public Player player;
        public AsteroidSpawner asteroidSpawner;
        public BgSpawnner bgSpawnner;
        public int asteroidDestroyedCount; // thêm biến này


        public UnityEvent onPauseGame;
        public UnityEvent onResumeGame;
        private GameView _gameView;
        public int playerLives = 3; // thêm vào

        void Start()
        {
            UIManager.Instance.ViewManager.ShowView(UIViewName.GameView);
            UIManager.Instance.HideTransition(() => { });
            _gameView = UIManager.Instance.ViewManager.GetViewByName<GameView>(UIViewName.GameView);
            _gameView.UpdateScore(score);
            _gameView.UpdateSUPER(score);
            _gameView.UpdateHeart(playerLives);
            Cursor.visible = false;

        }

        void Update()
        {
            if (isGamePaused) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.SettingPopup, new SettingPopupParam
                {
                    showGroupBtn = true
                });
                return;
            }

            Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            bool firePressed = Input.GetMouseButtonDown(0);

            if (player != null)
            {
                player.HandleInput(moveDir, firePressed);
            }

            if (Input.GetMouseButtonDown(1))
            {
                player.FireSuperWeapon();
            }
        }

        public void AddSuperWeapon()
        {
            weaponCoin++;
            AudioManager.Instance.PlaySfx(AudioName.Gameplay_EarnSuperWeapon);
            UIManager.Instance.AlertManager.ShowAlertMessage("Super weapon +1");
            _gameView.UpdateSUPER(weaponCoin);
        }

        public void MinusSuperWeapon()
        {
            weaponCoin--;
            _gameView.UpdateSUPER(weaponCoin);
        }

        public void PauseGame()
        {
            isGamePaused = true;
            bgSpawnner.Pause();
            Cursor.visible = true;
            asteroidSpawner.PauseAllAsteroids();
        }

        public void ResumeGame()
        {
            Cursor.visible = false;
            isGamePaused = false;
            bgSpawnner.Resume();
            asteroidSpawner.ResumeAllAsteroids();
        }


        public void AddScore(int amount)
        {
            AudioManager.Instance.PlaySfx(AudioName.Gameplay_PlayerScore);

            score += amount;
            _gameView.UpdateScore(score);
        }

        public bool PlayerHit()
        {
            if (playerLives <= 0|| player.isInvulnerable) return false;

            playerLives--;
            _gameView.UpdateHeart(playerLives);
            ShakeCamera();
            if (playerLives <= 0)
            {
                ShowEndGame();
            }
            else
            {
                player.StartInvulnerability(); // gọi xử lý bất tử
            }

            return true;
        }
        private void ShakeCamera(float duration = 0.2f, float strength = 0.3f)
        {
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.transform.DOShakePosition(duration, strength, vibrato: 10, randomness: 90, snapping: false, fadeOut: true);
            }
        }
        public void ShowEndGame()
        {
            PauseGame();

            EndGamePopupParam param = new EndGamePopupParam
            {
                score = score,
                isNewHighScore = score > PlayerPrefs.GetInt(GameConstants.PlayerPrefsHighScore, 0)
            };

            PlayerPrefs.SetInt(GameConstants.PlayerPrefsHighScore,
                Mathf.Max(score, PlayerPrefs.GetInt(GameConstants.PlayerPrefsHighScore, 0)));
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.EndGamePopup, param);
        }
    }
}
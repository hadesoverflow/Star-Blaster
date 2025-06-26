using _GAME.Scripts.Popup;
using DenkKits.AudioManager.Scripts;
using DenkKits.UIManager.Scripts.Base;
using DenkKits.UIManager.Scripts.UIPopup;
using DenkKits.UIManager.Scripts.UIView;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _GAME.Scripts.Views
{
    public class GameView : UIView
    {
        [SerializeField] private TextMeshProUGUI currentQuestion;
        [SerializeField] private TextMeshProUGUI timer;
        [SerializeField] private TextMeshProUGUI currentAnwserChoosen;
        [SerializeField] private TextMeshProUGUI currentScore;
        [SerializeField] private TextMeshProUGUI currentSUPER;
        [SerializeField] private TextMeshProUGUI Combostreak;
        [SerializeField] private TextMeshProUGUI getScoreEf;
        [SerializeField] private TextMeshProUGUI textHeart;

        public override void Awake()
        {
            base.Awake();
            _initialPosition = getScoreEf.transform.position;
        }

        public void UpdateQuestionIndex(int index)
        {
            currentQuestion.text = $"{index}";
        }   
        public void UpdateHeart(int index)
        {
            textHeart.text = $"{index}";
        }

        public void UpdateTimer(float time)
        {
            timer.text = time.ToString("0.00");
        }

        public void UpdateAnswerChoosen(string answer)
        {
            currentAnwserChoosen.text = answer;
        }

        public void ShowCombostreak(int streak)
        {
            if (streak == 1)
            {
                return;
            }

            Combostreak.gameObject.SetActive(true); // Bật lên
            Combostreak.text = $"Combo Streak: {streak}";

            Combostreak.transform.localScale = Vector3.one * 0.5f;
            Combostreak.color = new Color(Combostreak.color.r, Combostreak.color.g, Combostreak.color.b, 1f);

            // Hiệu ứng phóng to rồi thu nhỏ nhẹ
            Combostreak.transform
                .DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack);

            // Làm mờ sau 1 giây
            Combostreak.DOFade(0, 1f)
                .SetDelay(1f)
                .OnComplete(() => { Combostreak.gameObject.SetActive(false); });
        }

        public void UpdateScore(int score)
        {
            currentScore.text = $"{score}";
            AnimateText(currentScore);
        }

        public GameObject effectIcon;

        public void UpdateSUPER(int score)
        {
            // effectIcon.SetActive(true);
            // RectTransform fxRect = effectIcon.GetComponent<RectTransform>();
            // RectTransform targetRect = currentSUPER.GetComponent<RectTransform>();
            //
            // fxRect.anchoredPosition = Vector2.zero; 
            // fxRect.localScale = Vector3.one * 1.2f;
            //
            // Vector2 targetPos = targetRect.anchoredPosition;
            //
            // fxRect
            //     .DOAnchorPos(targetPos, 0.6f)
            //     .SetEase(Ease.OutCubic)
            //     .OnComplete(() =>
            //     {
            //         currentSUPER.text = $"{score}";
            //
            //         effectIcon.SetActive(false);
            //     });
            //
            // fxRect
            //     .DOScale(0.5f, 0.6f)
            //     .SetEase(Ease.OutQuad);
            currentSUPER.text = $"{score}";
            AnimateText(currentSUPER);
            PlayEffectFlyToUI();
        }

        private void PlayEffectFlyToUI()
        {
            if (effectIcon == null || currentSUPER == null) return;
        }


        private void AnimateText(TextMeshProUGUI text)
        {
            text.transform.DOKill(); // Hủy tween cũ nếu có
            text.transform.localScale = Vector3.one; // Reset scale

            text.transform.DOScale(1.2f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => { text.transform.DOScale(1f, 0.1f).SetEase(Ease.InQuad); });
        }

        public void OpenSetting()
        {
            var param = new SettingPopupParam
            {
                showGroupBtn = true
            };
            Debug.Log("OPEN SETING WITH PARAM: " + param.showGroupBtn);
            AudioManager.Instance.PlaySfx(AudioName.UI_Click);
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.SettingPopup, param);
        }

        private Vector3 _initialPosition;

        public void ShowScoreEffect(int addedScore)
        {
            // Show the score effect
            getScoreEf.gameObject.SetActive(true);
            getScoreEf.text = $"+{addedScore}"; // Display the score added
            getScoreEf.transform.localScale = Vector3.one; // Reset scale
            getScoreEf.color =
                new Color(getScoreEf.color.r, getScoreEf.color.g, getScoreEf.color.b, 1f); // Reset alpha to full

            getScoreEf.transform.position = _initialPosition;

            // Shrink and then grow the effect
            getScoreEf.transform
                .DOScale(0.5f, 0.2f) // Shrink to 50%
                .SetEase(Ease.InBack) // Smooth shrinking
                .OnComplete(() =>
                {
                    // Grow the effect after shrinking
                    getScoreEf.transform
                        .DOScale(1.5f, 0.3f) // Grow to 150%
                        .SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            // Get the position of currentScore for the animation
                            Vector3 targetPosition = currentScore.transform.position;

                            // Animate movement towards the currentScore
                            getScoreEf.transform
                                .DOMove(targetPosition, 1f) // Move towards currentScore
                                .SetEase(Ease.OutQuad)
                                .OnComplete(() =>
                                {
                                    // Once the effect has moved to currentScore, update the score
                                    UpdateScore(addedScore); // Update the actual score
                                    getScoreEf.DOFade(0, 0.5f) // Fade out the effect
                                        .OnComplete(() =>
                                            getScoreEf.gameObject.SetActive(false)); // Hide it after fading out
                                });
                        });
                });

            // Optionally, you can add a scaling effect to the currentScore as well
            currentScore.transform
                .DOScale(1.1f, 0.3f)
                .SetLoops(2, LoopType.Yoyo); // Slightly scale up and back down to emphasize score change
        }
    }
}
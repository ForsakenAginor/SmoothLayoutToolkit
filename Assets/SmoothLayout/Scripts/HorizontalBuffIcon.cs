using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothLayoutToolkit
{
    public class HorizontalBuffIcon : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerTextField;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _targetTransform;

        [Header("Animation Settings")]
        [SerializeField] private AnimationCurve _fadeOutCurve;
        [SerializeField] private float _fadeOutDuration = 2f;

        private float _timer;
        private bool _isDisplaying;

        public event Action Disappeared;

        public bool IsDisplaying => _isDisplaying;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if (_isDisplaying == false)
                return;

            _timer -= Time.deltaTime;
            _timerTextField.text = _timer.ToString("0.0");
        }

        public void Show(Sprite sprite, float duration)
        {
            _isDisplaying = true;
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _timer = duration;
            StartCoroutine(ShowCoroutine(sprite, duration));
        }

        private IEnumerator ShowCoroutine(Sprite sprite, float duration)
        {
            _image.sprite = sprite;
            _canvasGroup.alpha = 1f;
            float fadeOutDuration;

            if (_fadeOutDuration < duration)
            {
                fadeOutDuration = _fadeOutDuration;
            }
            else
            {
                fadeOutDuration = 0.5f * duration;
            }
            
            WaitForSeconds fadeOutDelay = new WaitForSeconds(fadeOutDuration);
            WaitForSeconds displayDelay = new WaitForSeconds(duration - fadeOutDuration);
            
            yield return displayDelay;
            _canvasGroup.DOFade(0f, fadeOutDuration).SetEase(_fadeOutCurve);
            yield return fadeOutDelay;

            transform.SetAsLastSibling();
            _isDisplaying = false;
            gameObject.SetActive(false);
            Disappeared?.Invoke();
        }
    }
}
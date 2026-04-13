using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothLayoutToolkit
{
    public class BuffFeedIcon : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerTextField;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _targetTransform;

        [Header("Animation Settings")]
        [SerializeField] private AnimationCurve _fadeOutCurve;
        [SerializeField] private float _fadeInDuration = 0.5f;
        [SerializeField] private float _fadeOutDuration = 2f;

        [SerializeField] private Vector2 _targetMinAnchor = Vector2.zero;
        [SerializeField] private Vector2 _targetMaxAnchor = new Vector2(0.5f, 1f);

        private Vector2 _originMinAnchor;
        private Vector2 _originMaxAnchor;
        private bool _isDisplaying;
        private float _timer;
    
        public event Action Disappeared;
    
        public bool IsDisplaying => _isDisplaying;

        private void Awake()
        {
            _originMinAnchor = _targetTransform.anchorMin;
            _originMaxAnchor = _targetTransform.anchorMax;
        
            _canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if(_isDisplaying == false)
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
            float fadeInDuration, fadeOutDuration;

            if (_fadeOutDuration + _fadeInDuration < duration)
            {
                fadeInDuration = _fadeInDuration;
                fadeOutDuration = _fadeOutDuration;
            }
            else
            {
                fadeInDuration = 0.1f * duration;
                fadeOutDuration = 0.4f * duration;
            }
        
            WaitForSeconds fadeInDelay = new WaitForSeconds(fadeInDuration);
            WaitForSeconds fadeOutDelay = new WaitForSeconds(fadeOutDuration);
            WaitForSeconds displayDelay = new WaitForSeconds(duration - fadeInDuration - fadeOutDuration);
        
            _targetTransform.anchorMin = _originMinAnchor;
            _targetTransform.anchorMax = _originMaxAnchor;
        
            _canvasGroup.DOFade(1f, fadeInDuration);
            _targetTransform.DOAnchorMax(_targetMaxAnchor, fadeInDuration).SetEase(Ease.OutQuad);
            _targetTransform.DOAnchorMin(_targetMinAnchor, fadeInDuration).SetEase(Ease.OutQuad);
        
            yield return fadeInDelay;
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
using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothLayoutToolkit
{
    public class FeedMessage : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _targetTransform;

        [Header("Animation Settings")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private float _fadeOutDuration = 0.5f;

        [SerializeField] private Vector2 _targetMinAnchor = Vector2.zero;
        [SerializeField] private Vector2 _targetMaxAnchor = new Vector2(0.5f, 1f);

        private Vector2 _originMinAnchor;
        private Vector2 _originMaxAnchor;
    
        private WaitForSeconds _fadeInDelay;
        private WaitForSeconds _fadeOutDelay;
        private WaitForSeconds _displayDelay;
    
        private bool _isDisplaying;
    
        public event Action Disappeared;
    
        public bool IsDisplaying => _isDisplaying;

        private void Awake()
        {
            _originMinAnchor = _targetTransform.anchorMin;
            _originMaxAnchor = _targetTransform.anchorMax;
        
            _canvasGroup.alpha = 0f;
            _fadeInDelay = new WaitForSeconds(_fadeInDuration);
            _fadeOutDelay = new WaitForSeconds(_fadeOutDuration);
            _displayDelay = new WaitForSeconds(_displayDuration);
        }

        public void Show(Sprite sprite, string text)
        {
            _isDisplaying = true;
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            StartCoroutine(ShowCoroutine(sprite, text));
        }

        private IEnumerator ShowCoroutine(Sprite sprite, string text)
        {
            _image.sprite = sprite;
            _text.text = text;

            _targetTransform.anchorMin = _originMinAnchor;
            _targetTransform.anchorMax = _originMaxAnchor;
        
            _canvasGroup.DOFade(1f, _fadeInDuration);
            _targetTransform.DOAnchorMax(_targetMaxAnchor, _fadeInDuration).SetEase(Ease.OutQuad);
            _targetTransform.DOAnchorMin(_targetMinAnchor, _fadeInDuration).SetEase(Ease.OutQuad);
        
            yield return _fadeInDelay;
            yield return _displayDelay;
            _canvasGroup.DOFade(0f, _fadeOutDuration).SetEase(Ease.InQuad);
            yield return _fadeOutDelay;
        
            transform.SetAsLastSibling();
            _isDisplaying = false;
            gameObject.SetActive(false);
            Disappeared?.Invoke();
        }
    }
}

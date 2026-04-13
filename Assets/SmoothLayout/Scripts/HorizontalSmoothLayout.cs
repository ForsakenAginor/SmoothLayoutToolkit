using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothLayoutToolkit
{
    public class HorizontalSmoothLayout : SmoothLayout, ISmoothLayout
    {
        [Header("Settings")]
        [SerializeField] private float _animationSpeed = 10f;
        [SerializeField] private float _spacing = 10f;
        [SerializeField] private bool _startFromLeft = true;
    
        [Header("Padding")]
        [SerializeField] private float _leftPadding = 0f;
        [SerializeField] private float _rightPadding = 0f;
    
        private readonly List<RectTransform> _currentActive = new List<RectTransform>();
        private readonly List<RectTransform> _activeChildren = new List<RectTransform>();
    
        private Dictionary<RectTransform, float> _targetXPositions = new Dictionary<RectTransform, float>();
        private Dictionary<RectTransform, float> _currentXPositions = new Dictionary<RectTransform, float>();
    
        private bool _needsUpdate;
        private float _lastCheckTime;
    
        private void OnEnable()
        {
            UpdateChildrenList();
            CalculateTargetPositions();
        }
    
        private void Start()
        {
            UpdateChildrenList();
            CalculateTargetPositions();
            SetInitialPositions();
        }
    
        private void Update()
        {
            AnimatePositions();
        }
    
        public override void UpdateLayout()
        {
            List<RectTransform> currentActive = GetCurrentActiveChildren();
            bool isChanged = IsActiveChildrenListChanged(currentActive);
        
            if (isChanged)
            {
                UpdateChildrenList();
                CalculateTargetPositions();
                _needsUpdate = true;
            }
        }
    
        private void AnimatePositions()
        {
            if (_needsUpdate == false)
            {
                return;
            }
        
            bool allReachedTarget = true;
        
            foreach (RectTransform child in _activeChildren)
            {
                if (child == null)
                {
                    continue;
                }
            
                if (!_currentXPositions.ContainsKey(child) || !_targetXPositions.ContainsKey(child))
                {
                    continue;
                }
            
                float newX = Mathf.Lerp(_currentXPositions[child], _targetXPositions[child], Time.deltaTime * _animationSpeed);
                _currentXPositions[child] = newX;
            
                Vector3 position = child.anchoredPosition;
                position.x = newX;
                child.anchoredPosition = position;
            
                if (Mathf.Abs(newX - _targetXPositions[child]) > 0.01f)
                {
                    allReachedTarget = false;
                }
            }
        
            if (allReachedTarget)
            {
                FinishAnimation();
            }
        }
    
        private void FinishAnimation()
        {
            _needsUpdate = false;
        
            foreach (RectTransform child in _activeChildren)
            {
                if (child != null && _targetXPositions.ContainsKey(child))
                {
                    Vector3 position = child.anchoredPosition;
                    position.x = _targetXPositions[child];
                    child.anchoredPosition = position;
                    _currentXPositions[child] = _targetXPositions[child];
                }
            }
        }
    
        private List<RectTransform> GetCurrentActiveChildren()
        {
            _currentActive.Clear();
        
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
            
                if (child.gameObject.activeInHierarchy)
                {
                    RectTransform rectTransform = child as RectTransform;
                
                    if (rectTransform != null)
                    {
                        _currentActive.Add(rectTransform);
                    }
                }
            }
        
            return _currentActive;
        }
    
        private bool IsActiveChildrenListChanged(List<RectTransform> currentActive)
        {
            if (currentActive.Count != _activeChildren.Count)
            {
                return true;
            }
        
            for (int i = 0; i < currentActive.Count; i++)
            {
                if (currentActive[i] != _activeChildren[i])
                {
                    return true;
                }
            }
        
            return false;
        }
    
        private void UpdateChildrenList()
        {
            _activeChildren.Clear();
        
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
            
                if (child.gameObject.activeInHierarchy)
                {
                    RectTransform rectTransform = child as RectTransform;
                
                    if (rectTransform != null)
                    {
                        _activeChildren.Add(rectTransform);
                    }
                }
            }
        }
    
        private void CalculateTargetPositions()
        {
            _targetXPositions.Clear();
        
            float currentX = _startFromLeft ? _leftPadding : -_leftPadding;
        
            for (int i = 0; i < _activeChildren.Count; i++)
            {
                RectTransform child = _activeChildren[i];
                float childWidth = GetChildWidth(child);
            
                _targetXPositions[child] = currentX;

                if (_startFromLeft)
                {
                    if (child.anchoredPosition.x < currentX)
                        child.anchoredPosition = new Vector2(currentX, child.anchoredPosition.y);
                }
                else
                {
                    if (child.anchoredPosition.x > currentX)
                        child.anchoredPosition = new Vector2(currentX, child.anchoredPosition.y);
                }
            
                _currentXPositions[child] = child.anchoredPosition.x;
            
                if (_startFromLeft)
                {
                    currentX += childWidth + _spacing;
                }
                else
                {
                    currentX -= childWidth + _spacing;
                }
            }
        }
    
        private void SetInitialPositions()
        {
            foreach (RectTransform child in _activeChildren)
            {
                if (_targetXPositions.ContainsKey(child))
                {
                    Vector3 position = child.anchoredPosition;
                    position.x = _targetXPositions[child];
                    child.anchoredPosition = position;
                    _currentXPositions[child] = _targetXPositions[child];
                }
            }
        
            _needsUpdate = false;
        }
    
        private float GetChildWidth(RectTransform child)
        {
            if (child == null)
            {
                return 0f;
            }
        
            float width = child.rect.width;
        
            LayoutElement layoutElement = child.GetComponent<LayoutElement>();
        
            if (layoutElement != null && layoutElement.preferredWidth > 0)
            {
                width = layoutElement.preferredWidth;
            }
        
            return width;
        }
    }
}
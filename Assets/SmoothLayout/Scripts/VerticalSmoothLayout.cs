using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothLayoutToolkit
{
    public class VerticalSmoothLayout : SmoothLayout, ISmoothLayout
    {
        [Header("Settings")]
        [SerializeField] private float _animationSpeed = 10f;
        [SerializeField] private float _spacing = 10f;
        [SerializeField] private bool _startFromTop = true;
    
        [Header("Padding")]
        [SerializeField] private float _topPadding = 0f;
        [SerializeField] private float _bottomPadding = 0f;
    
        private readonly List<RectTransform> _currentActive = new List<RectTransform>();
        private readonly List<RectTransform> _activeChildren = new List<RectTransform>();
    
        private Dictionary<RectTransform, float> _targetYPositions = new Dictionary<RectTransform, float>();
        private Dictionary<RectTransform, float> _currentYPositions = new Dictionary<RectTransform, float>();
    
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
            
                if (!_currentYPositions.ContainsKey(child) || !_targetYPositions.ContainsKey(child))
                {
                    continue;
                }
            
                float newY = Mathf.Lerp(_currentYPositions[child], _targetYPositions[child], Time.deltaTime * _animationSpeed);
                _currentYPositions[child] = newY;
            
                Vector3 position = child.anchoredPosition;
                position.y = newY;
                child.anchoredPosition = position;
            
                if (Mathf.Abs(newY - _targetYPositions[child]) > 0.01f)
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
                if (child != null && _targetYPositions.ContainsKey(child))
                {
                    Vector3 position = child.anchoredPosition;
                    position.y = _targetYPositions[child];
                    child.anchoredPosition = position;
                    _currentYPositions[child] = _targetYPositions[child];
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
            _targetYPositions.Clear();
        
            float currentY = _startFromTop ? -_topPadding : _topPadding;
        
            for (int i = 0; i < _activeChildren.Count; i++)
            {
                RectTransform child = _activeChildren[i];
                float childHeight = GetChildHeight(child);
            
                _targetYPositions[child] = currentY;

                if (_startFromTop)
                {
                    if (child.anchoredPosition.y > currentY)
                        child.anchoredPosition = new Vector2(child.anchoredPosition.x, currentY);
                }
                else
                {
                    if (child.anchoredPosition.y < currentY)
                        child.anchoredPosition = new Vector2(child.anchoredPosition.x, currentY);
                }
            
                _currentYPositions[child] = child.anchoredPosition.y;
            
                if (_startFromTop)
                {
                    currentY -= childHeight + _spacing;
                }
                else
                {
                    currentY += childHeight + _spacing;
                }
            }
        }
    
        private void SetInitialPositions()
        {
            foreach (RectTransform child in _activeChildren)
            {
                if (_targetYPositions.ContainsKey(child))
                {
                    Vector3 position = child.anchoredPosition;
                    position.y = _targetYPositions[child];
                    child.anchoredPosition = position;
                    _currentYPositions[child] = _targetYPositions[child];
                }
            }
        
            _needsUpdate = false;
        }
    
        private float GetChildHeight(RectTransform child)
        {
            if (child == null)
            {
                return 0f;
            }
        
            float height = child.rect.height;
        
            LayoutElement layoutElement = child.GetComponent<LayoutElement>();
        
            if (layoutElement != null && layoutElement.preferredHeight > 0)
            {
                height = layoutElement.preferredHeight;
            }
        
            return height;
        }
    }
}
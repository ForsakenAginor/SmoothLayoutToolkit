using System.Collections.Generic;
using UnityEngine;

namespace SmoothLayoutToolkit
{
    public abstract class AbstractedFeed<T> : MonoBehaviour where T : MonoBehaviour
    {
        private readonly List<T> _messages = new List<T>();
    
        [SerializeField] private T _prefab;
        [SerializeField] private RectTransform _parent;
        [SerializeField] private SmoothLayout _layout;
    
        protected IEnumerable<T> Messages => _messages;
    
        protected ISmoothLayout Layout => _layout;

        protected virtual T CreateMessage()
        {
            T newMessage = Instantiate(_prefab, _parent);
            _messages.Add(newMessage);
            return newMessage;
        }
    }
}
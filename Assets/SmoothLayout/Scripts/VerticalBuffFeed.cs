using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SmoothLayoutToolkit
{
    public interface IBuffFeed
    {
        public void Show(string buffType, float duration);
    }

    public class VerticalBuffFeed : AbstractedFeed<BuffFeedIcon>, IBuffFeed
    {
        private Dictionary<string, Sprite> _feedData = new Dictionary<string, Sprite>();

        [SerializeField] private BuffData[] _buffTypes;
    
        private void Awake()
        {
            foreach (BuffData buffType in _buffTypes)
            {
                _feedData.Add(buffType.Id, buffType.Icon);
            }
        }

        private void OnDestroy()
        {
            foreach (var message in Messages)
            {
                message.Disappeared -= OnMessageDisappeared;
            }
        }

        public void Show(string buffType, float duration)
        {
            if (_feedData.ContainsKey(buffType) == false)
                throw new KeyNotFoundException("No buff found for buff type: " + buffType);
        
            if(duration < 0)
                throw new ArgumentOutOfRangeException("duration", "Duration must be greater than zero");
        
        
            BuffFeedIcon inactiveBuff = Messages.FirstOrDefault(message => message.IsDisplaying == false);
            var data = _feedData[buffType];
        
            if (inactiveBuff != null)
            {
                inactiveBuff.Show(data, duration);
            }
            else
            {
                BuffFeedIcon newBuff = CreateMessage();
                newBuff.Show(data, duration);
                newBuff.Disappeared += OnMessageDisappeared;
            }
        
            Layout.UpdateLayout();
        }

        private void OnMessageDisappeared()
        {
            Layout.UpdateLayout();
        }
    
    
        [Serializable]
        private class BuffData
        {
            public string Id;
            public Sprite Icon;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SmoothLayoutToolkit
{
    public interface IMessageFeed
    {
        public void Show(string id);
    }

    public class MessageFeed : AbstractedFeed<FeedMessage>, IMessageFeed
    {
        private readonly Dictionary<string, FeedData> _feedData = new Dictionary<string, FeedData>();
        
        [SerializeField] private FeedData[] _feedsIds;

        private void Awake()
        {
            foreach (var idContainer in _feedsIds)
            {
                _feedData.Add(idContainer.Id, idContainer);
            }
        }

        private void OnDestroy()
        {
            foreach (FeedMessage message in Messages)
            {
                message.Disappeared -= OnMessageDisappeared;
            }
        }


        public void Show(string id)
        {
            if(_feedData.ContainsKey(id) == false)
                throw new KeyNotFoundException("No feed with id " + id + " exists.");
            
            FeedMessage inactiveMessage = Messages.FirstOrDefault(message => message.IsDisplaying == false);
            var data = _feedData[id];
        
            if (inactiveMessage != null)
            {
                inactiveMessage.Show(data.Sprite, data.Text);
            }
            else
            {
                var newMessage = CreateMessage();
                newMessage.Show(data.Sprite, data.Text);
                newMessage.Disappeared += OnMessageDisappeared;
            }
        
            Layout.UpdateLayout();
        }

        private void OnMessageDisappeared()
        {
            Layout.UpdateLayout();
        }

        [Serializable]
        private class FeedData
        {
            public string Id;
            public Sprite Sprite;
            public string Text;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace SmoothLayoutToolkit
{
    public class DemoLogic : MonoBehaviour
    {
        [SerializeField] private Button _showFeedButton;
        [SerializeField] private Button _showLeftBuffButton;
        [SerializeField] private Button _showTopBuffButton;
        
        [SerializeField] private MessageFeed _messageFeed;
        [SerializeField] private VerticalBuffFeed _verticalBuffFeed;
        [SerializeField] private HorizontalBuffFeed _horizontalBuffFeed;

        private void Awake()
        {
            _showFeedButton.onClick.AddListener(ShowFeed);
            _showLeftBuffButton.onClick.AddListener(ShowLeftBuff);
            _showTopBuffButton.onClick.AddListener(ShowTopBuff);
        }

        private void OnDestroy()
        {
            _showFeedButton.onClick.RemoveListener(ShowFeed);
            _showLeftBuffButton.onClick.RemoveListener(ShowLeftBuff);
            _showTopBuffButton.onClick.RemoveListener(ShowTopBuff);
        }

        private void ShowTopBuff()
        {
            _horizontalBuffFeed.Show("Heal", 10f);
        }

        private void ShowFeed()
        {
            _messageFeed.Show("Heal");
        }

        private void ShowLeftBuff()
        {
            _verticalBuffFeed.Show("Heal", 10f);
        }
    }
}
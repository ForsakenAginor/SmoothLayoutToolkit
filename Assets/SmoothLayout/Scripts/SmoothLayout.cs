using UnityEngine;

namespace SmoothLayoutToolkit
{
    public abstract class SmoothLayout : MonoBehaviour, ISmoothLayout
    {
        public abstract void UpdateLayout();
    }
}
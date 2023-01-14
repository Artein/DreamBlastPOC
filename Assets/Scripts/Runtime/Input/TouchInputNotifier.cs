using Game.Utils;
using UnityEngine;
using Zenject;

namespace Game.Input
{
    [ZenjectBound]
    public class TouchInputNotifier : ITickable
    {
        public event TouchBeganHandler TouchBegan;
        
        void ITickable.Tick()
        {
            Vector2 position = Vector2.zero;

            if (TryGetTouchPosition_Mobile(ref position) || 
                TryGetTouchPosition_Desktop(ref position))
            {
                TouchBegan?.Invoke(position);
            }
        }

        private static bool TryGetTouchPosition_Mobile(ref Vector2 position)
        {
            if (Application.isMobilePlatform && UnityEngine.Input.touchCount > 0)
            {
                foreach (var touch in UnityEngine.Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        position = touch.position;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryGetTouchPosition_Desktop(ref Vector2 position)
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                position = UnityEngine.Input.mousePosition;
                // Note: Input.mousePosition reports the position of the mouse even when it is not inside the Game View, such as when Cursor.lockState
                // is set to CursorLockMode.None. When running in windowed mode with an unconfined cursor, position values smaller than 0 or greater
                // than the screen dimensions (Screen.width,Screen.height) indicate that the mouse cursor is outside of the game window.
                if (position is { x: >= 0, y: >= 0 } && position.x <= Screen.width && position.y <= Screen.height)
                {
                    return true;
                }
            }

            return false;
        }
        
        public delegate void TouchBeganHandler(Vector2 pixelCoordinates);
    }
}
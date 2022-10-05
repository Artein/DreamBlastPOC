using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Input
{
    [UsedImplicitly]
    public class TouchInputNotifier : ITickable
    {
        public event TouchBeganHandler TouchBegan;
        
        void ITickable.Tick()
        {
            bool receivedTouch = false;
            Vector2 position = Vector2.zero;
            
            if (Application.isMobilePlatform && UnityEngine.Input.touchCount > 0)
            {
                foreach (var touch in UnityEngine.Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        receivedTouch = true;
                        position = touch.position;
                        break;
                    }
                }
            }
            else if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                position = UnityEngine.Input.mousePosition;
                // Note: Input.mousePosition reports the position of the mouse even when it is not inside the Game View, such as when Cursor.lockState
                // is set to CursorLockMode.None. When running in windowed mode with an unconfined cursor, position values smaller than 0 or greater
                // than the screen dimensions (Screen.width,Screen.height) indicate that the mouse cursor is outside of the game window.
                if (position.x >= 0 && position.y >= 0 && position.x <= Screen.width && position.y <= Screen.height)
                {
                    receivedTouch = true;
                }
            }

            if (receivedTouch)
            {
                TouchBegan?.Invoke(position);
            }
        }

        public delegate void TouchBeganHandler(Vector2 pixelCoordinates);
    }
}
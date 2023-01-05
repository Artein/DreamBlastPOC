using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityUtils.Invocation;
using Zenject;

namespace Game.Input
{
    [UsedImplicitly]
    public class GameObjectInputNotifier : IInitializable, IDisposable
    {
        [Inject] private TouchInputNotifier _inputNotifier;
        private readonly List<RaycastResult> _raycastResults = new();
        private PointerEventData _pointerEventData;
        private readonly Dictionary<int, List<ObjectTouchedHandler>> _subscribers = new();

        void IInitializable.Initialize()
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
            _inputNotifier.TouchBegan += OnTouchBeganNotified;
        }

        void IDisposable.Dispose()
        {
            _inputNotifier.TouchBegan -= OnTouchBeganNotified;
        }

        public IDisposable Subscribe(int layer, ObjectTouchedHandler handler)
        {
            List<ObjectTouchedHandler> subscribers;
            if (_subscribers.ContainsKey(layer))
            {
                subscribers = _subscribers[layer];
            }
            else
            {
                subscribers = new List<ObjectTouchedHandler>();
                _subscribers.Add(layer, subscribers);
            }
            
            subscribers.Add(handler);
            return new DisposableAction(() =>
            {
                bool removed = subscribers.Remove(handler);
                Assert.IsTrue(removed);
            });
        }

        private void OnTouchBeganNotified(Vector2 pixelCoordinates)
        {
            // TODO: As an optimization might be a mix of all GraphicRaycasters + Physics2D.Raycast
            _pointerEventData.position = pixelCoordinates;
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

            if (_raycastResults.Count > 0)
            {
                // we need only the very first object. It is a blocking one for other
                var raycastResult = _raycastResults[0];
                if (_subscribers.TryGetValue(raycastResult.gameObject.layer, out var subscribers))
                {
                    for (int i = 0; i < subscribers.Count; i++)
                    {
                        var subscriber = subscribers[i];
                        subscriber?.Invoke(raycastResult.gameObject);
                    }
                }
            }
        }

        public delegate void ObjectTouchedHandler(GameObject gameObject);
    }
}
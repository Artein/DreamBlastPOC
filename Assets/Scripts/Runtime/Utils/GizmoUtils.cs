using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityUtils.Invocation;

namespace Game.Utils
{
    public static class GizmoUtils
    {
        [MustUseReturnValue]
        public static IDisposable SetColorWithHandle(Color color)
        {
            var prevColor = Gizmos.color;
            Gizmos.color = color;
            return new DisposableAction(() => Gizmos.color = prevColor);
        }
    }
}
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Platform
{
    [UsedImplicitly]
    public class TargetFPSController : IInitializable
    {
        private const int ProductionTargetFPS = 60;

        // indicates that the game should render at the platform's default frame rate. This default rate depends on the platform
        // https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html
        public const int DefaultPlatformFPS = -1;

        void IInitializable.Initialize()
        {
            Application.targetFrameRate = Application.isEditor ? DefaultPlatformFPS : ProductionTargetFPS;
        }
    }
}
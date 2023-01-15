using System;
using System.ComponentModel;
using Eflatun.SceneReference;
using Game.Utils;
using SRDebugger.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    [ZenjectBound]
    public class ProjectOptions : IInitializable, IDisposable
    {
        [Inject] private IDebugService _debugService;
        [Inject(Id = InjectionIds.SceneReference.Core)] private SceneReference _coreSceneRef;
        
        void IInitializable.Initialize()
        {
            _debugService.AddOptionContainer(this);
        }

        void IDisposable.Dispose()
        {
            _debugService.RemoveOptionContainer(this);
        }

        [Category("Application")]
        public void RestartApplication()
        {
            Debug.unityLogger.Log(nameof(ProjectOptions), $"Clicked '{nameof(RestartApplication)}' button");
            _debugService.HideDebugPanel();
            SceneManager.LoadScene(_coreSceneRef.Name, LoadSceneMode.Single);
        }
    }
}
using System;
using Game.Utils;
using SRDebugger.Services;
using Zenject;

namespace Game
{
    [ZenjectBound]
    public class ProjectOptions : IInitializable, IDisposable
    {
        [Inject] private IDebugService _debugService;
        
        void IInitializable.Initialize()
        {
            _debugService.AddOptionContainer(this);
        }

        void IDisposable.Dispose()
        {
            _debugService.RemoveOptionContainer(this);
        }
    }
}
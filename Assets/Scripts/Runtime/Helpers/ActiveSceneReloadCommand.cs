using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Game.Helpers
{
    [UsedImplicitly]
    public class ActiveSceneReloadCommand
    {
        public async UniTask ExecuteAsync()
        {
            // TODO: Won't work in multi-scene setup
            var activeSceneName = SceneManager.GetActiveScene().name;
            
            await SceneManager.LoadSceneAsync(activeSceneName);
        }
    }
}
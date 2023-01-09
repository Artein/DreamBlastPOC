namespace Game
{
    public static class InjectionIds
    {
        public enum Int : byte
        {
            ChipsLayer,
            IgnoreRaycastsLayer,
        }

        public enum Transform : byte
        {
            ChipsContainer,
            LevelContainer,
            CameraRig,
        }

        public enum AssetReferenceScene : byte
        {
            Level,
        }

        public enum SceneReference : byte
        {
            Bootstrap,
            Loading,
        }
    }
}
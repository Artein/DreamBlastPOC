using System.Diagnostics;
using Unity.Profiling;
using UnityEngine;

namespace Game.Physics
{
    [DisallowMultipleComponent]
    public class PhysicsSimulationExecutor : MonoBehaviour
    {
        public long LastSimulationDurationMs { get; private set; }
        public long MaxSimulationDurationMs { get; private set; }
        public long LastSimulationDurationTicks { get; private set; }
        public long MaxSimulationDurationTicks { get; private set; }
        
        private ProfilerMarker _updateProfilerMarker = new($"{nameof(PhysicsSimulationExecutor)}.{nameof(Update)}");
        private readonly Stopwatch _stopwatch = new();
        private readonly GUIStyle _labelStyle = new(); //create a new variable
        private readonly Rect _msLabelRect = new(50, 120, 250, 30);
        private readonly Rect _ticksLabelRect = new(50, 150, 250, 30);
        private SimulationMode2D _originalSimulationMode;

        private void Awake()
        {
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.fontSize = 30;
            enabled = Physics2D.simulationMode == SimulationMode2D.Script;
        }

        private void Update()
        {
            using var profileScopeHandle = _updateProfilerMarker.Auto();
            
            _stopwatch.Restart();
            {
                Physics2D.Simulate(Time.deltaTime);
            }
            _stopwatch.Stop();
            LastSimulationDurationMs = _stopwatch.ElapsedMilliseconds;
            if (LastSimulationDurationMs > MaxSimulationDurationMs)
            {
                MaxSimulationDurationMs = LastSimulationDurationMs;
            }
            
            LastSimulationDurationTicks = _stopwatch.ElapsedTicks;
            if (LastSimulationDurationTicks > MaxSimulationDurationTicks)
            {
                MaxSimulationDurationTicks = LastSimulationDurationTicks;
            }
        }

        private void OnGUI()
        {
            GUI.Label(_msLabelRect, $"Physics {LastSimulationDurationMs}ms (max: {MaxSimulationDurationMs})", _labelStyle);
            GUI.Label(_ticksLabelRect, $"Physics {LastSimulationDurationTicks} ticks (max: {MaxSimulationDurationTicks})", _labelStyle);
        }
    }
}
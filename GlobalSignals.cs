using Godot;
using System;

// Deprecated. Will remove at some point.
namespace GlobalSignal
{
    public partial class GlobalSignals : Node
    {
        private static GlobalSignals _instance;
        public static GlobalSignals Instance => _instance;

        public override void _Ready()
        {
            if (_instance == null)
                _instance = this;
            else
                QueueFree();
        }

        [Signal]
        public delegate void PlayerDiedEventHandler();
    }
}
using System;

namespace _project.scripts.Utils
{
    public class Timer
    {
        private readonly float _duration;
        private readonly Action _onComplete;

        public bool IsTicking { get; private set; }
        public float RemainingTime { get; private set; }

        public Timer(float duration, Action onComplete = null)
        {
            _duration = duration;
            RemainingTime = duration;
            IsTicking = false;
            _onComplete = onComplete;
        }
        public void Start()
        {
            IsTicking = true;
        }

        public void Stop()
        {
            IsTicking = false;
        }
        public void Reset()
        {
            RemainingTime = _duration;
            IsTicking = true;
        }

        public void Tick(float deltaTime)
        {
            if (!IsTicking) return;
            RemainingTime -= deltaTime;
            
            if (RemainingTime > 0) return;
            
            IsTicking = false;
            RemainingTime = 0;
            _onComplete?.Invoke();
        }
    }
}
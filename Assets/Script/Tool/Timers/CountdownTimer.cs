using System;

namespace Script.Tool.Timers
{
    public abstract class Timer
    {
        protected float InitialTime;
        public float Time;

        public abstract float Process { get; }
        public bool IsRunning { get; private set; }
        public event Action OnStart = delegate {};
        public event Action OnStop = delegate {};
        protected Timer(float initialTime)
        {
            InitialTime = initialTime;
            IsRunning = false;
        }
        public void Start()
        {
            Time = InitialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                OnStart?.Invoke();
            }
            
        }
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnStop?.Invoke();
            }
        }

        public void Pause() => IsRunning = false;
        public void Resume() => IsRunning = true;

        public void Reset()
        {
            Time = InitialTime;
            IsRunning = false;
        }
        public void Reset(float newTime)
        {
            InitialTime = newTime;
            Reset();
        }

        public abstract void Tick(float deltaTime);
    }

    public class CountdownTimer : Timer
    {
        public CountdownTimer(float initialTime) : base(initialTime) { }
        public override float Process => 1f - Time / InitialTime;
        public bool IsFinished => Time <= 0;
        public override void Tick(float deltaTime)
        {
            if (IsRunning)
            {
                Time -= deltaTime; 
                if (Time <= 0) Stop();
            }
        }

    }
}
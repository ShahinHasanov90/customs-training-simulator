using UnityEngine;

namespace CustomsSim.Core
{
    /// <summary>Root coordinator for the current training session.</summary>
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private string startingSceneName = "InspectionHall";
        [SerializeField] private bool autoStart = true;

        public SessionState State { get; private set; } = SessionState.Idle;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            if (autoStart)
            {
                BeginSession();
            }
        }

        public void BeginSession()
        {
            State = SessionState.Loading;
            EventBus.Raise(new SessionStarted());
            SceneLoader.Load(startingSceneName, onLoaded: () =>
            {
                State = SessionState.Running;
                EventBus.Raise(new SessionReady());
            });
        }

        public void EndSession(float finalScore)
        {
            if (State == SessionState.Ended) return;
            State = SessionState.Ended;
            EventBus.Raise(new SessionEnded(finalScore));
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }

    public enum SessionState
    {
        Idle,
        Loading,
        Running,
        Paused,
        Ended
    }

    public readonly struct SessionStarted { }

    public readonly struct SessionReady { }

    public readonly struct SessionEnded
    {
        public readonly float FinalScore;
        public SessionEnded(float finalScore) => FinalScore = finalScore;
    }
}

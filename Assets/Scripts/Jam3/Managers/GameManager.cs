using UnityEngine;
using Jam3.Util;

namespace Jam3
{
    public class GameManager : Singleton<GameManager>
    {
        public delegate void GameOverEvent(bool isGameOver);
        public event GameOverEvent OnGameOver;

        public delegate void PlayingEvent(bool isPlaying);
        public event PlayingEvent OnPlay;

        public ParticleSystem GameOverParticles = null;

        [SerializeField]
        private GameObject objectsCounter = null;

        public float TotalDistance { get; set; }
        public float MaxSpeed { get; set; }
        public float TotalObjects { get; set; }
        public bool TapEnabled { get; set; }
        public bool IsPlaying { get { return isPlaying; } }

        public bool IsGameOver { get => isGameOver; }

        private bool isGameOver = false;
        private bool isPlaying = false;

        public void Prepare()
        {
            isPlaying = true;

            if (ObjectManager.Instance.BallObject != null)
                ObjectManager.Instance.BallObject.HoldSpring();
        }

        public void Play()
        {
            isPlaying = true;

            if (ObjectManager.Instance.BallObject != null)
            {
                OnPlay?.Invoke(true);
                ObjectManager.Instance.BallObject.ReleaseSpring();
            }
        }

        public void Replay()
        {
            isPlaying = false;

            OnPlay?.Invoke(false);
            GameOver(false);
            ObjectManager.Instance.ResetBall();
        }

        public void Reset()
        {
            isPlaying = false;

            OnPlay?.Invoke(false);
            ObjectManager.Instance.Reset();
        }

        public void RestartGame()
        {
            isPlaying = false;

            OnPlay?.Invoke(false);
            GameOver(false);
            objectsCounter.SetActive(false);

            ObjectManager.Instance.Reset();
            ObjectManager.Instance.Restart();

            SelectionManager.Instance.UnselectObject();

            PlacementManager.Instance.Cancel();
            PlacementManager.Instance.IsBallPlaced = false;
            PlacementManager.Instance.InSceneObjectsCount = 0;

            PopUpManager.Instance.CloseAll();
        }

        public void GameOver(bool isGameOver)
        {
            OnPlay?.Invoke(false);
            OnGameOver?.Invoke(isGameOver);
        }

        public string[] GetResults()
        {
            string[] result = new string[3];

            if (ObjectManager.Instance.BallObject != null)
            {
                result[0] = ObjectManager.Instance.BallObject.MaxSpeed.ToString("F") + "MPH";
                result[1] = UnitUtil.UnitToInches(ObjectManager.Instance.BallObject.TraveledDistance) + "";
                result[2] = PlacementManager.Instance.InSceneObjectsCount.ToString();
            }

            AudioManager.Instance.PlayAudioClip("Applause");

            if (GameOverParticles != null)
            {
                if (GameOverParticles.isPlaying)
                    GameOverParticles.Stop();

                GameOverParticles.Play();
            }

            return result;
        }
    }
}


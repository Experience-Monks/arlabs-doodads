//-----------------------------------------------------------------------
// <copyright file="GameManager.cs" company="Jam3 Inc">
//
// Copyright 2021 Jam3 Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using Jam3.Util;

namespace Jam3
{
    /// <summary>
    /// Game manager.
    /// </summary>
    /// <seealso cref="Singleton<GameManager>" />
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

        // Runtime variables
        private bool isGameOver = false;
        private bool isPlaying = false;

        /// <summary>
        /// Prepare.
        /// </summary>
        public void Prepare()
        {
            isPlaying = true;

            if (ObjectManager.Instance.BallObject != null)
                ObjectManager.Instance.BallObject.HoldSpring();
        }

        /// <summary>
        /// Play.
        /// </summary>
        public void Play()
        {
            isPlaying = true;

            if (ObjectManager.Instance.BallObject != null)
            {
                OnPlay?.Invoke(true);
                ObjectManager.Instance.BallObject.ReleaseSpring();
            }
        }

        /// <summary>
        /// Replay.
        /// </summary>
        public void Replay()
        {
            isPlaying = false;

            OnPlay?.Invoke(false);
            GameOver(false);
            ObjectManager.Instance.ResetBall();
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public void Reset()
        {
            isPlaying = false;

            OnPlay?.Invoke(false);
            ObjectManager.Instance.Reset();
        }


        /// <summary>
        /// Restarts game.
        /// </summary>
        public void RestartGame()
        {
            isPlaying = false;

            OnPlay?.Invoke(false);
            GameOver(false);
            objectsCounter.SetActive(false);

            ObjectManager.Instance.Reset();
            ObjectManager.Instance.Restart();

            SelectionManager.Instance.UnselectObject();

            PlacementManager.Instance.Stop();
            PlacementManager.Instance.IsBallPlaced = false;
            PlacementManager.Instance.InSceneObjectsCount = 0;

            PopUpManager.Instance.CloseAll();
        }

        /// <summary>
        /// Games over.
        /// </summary>
        /// <param name="isGameOver">The is game over.</param>
        public void GameOver(bool isGameOver)
        {
            OnPlay?.Invoke(false);
            OnGameOver?.Invoke(isGameOver);
        }

        /// <summary>
        /// Gets results.
        /// </summary>
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


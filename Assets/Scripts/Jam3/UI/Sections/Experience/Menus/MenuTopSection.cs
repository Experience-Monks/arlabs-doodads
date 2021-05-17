using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Jam3
{
    public class MenuTopSection : MenuSectionUI
    {
        [SerializeField]
        private ARSession arSession = null;

        public GameObject clearButton = null;
        public GameObject resetButton = null;

        private bool clearButtonLastState;

        private void Awake() { }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            RegisterCallbacks();

            clearButton.SetActive(false);
            resetButton.SetActive(true);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            UnregisterCallbacks();
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterCallbacks();
        }

        private void HandleSelectionActive(bool isSelecting, ARObject selectedObject)
        {
            if (PlacementManager.Instance.InSceneObjectsCount < 1)
            {
                clearButton.SetActive(false);
            }
            else
            {
                clearButton.SetActive(!isSelecting);
                resetButton.SetActive(true);
            }

        }

        private void HandlePlay(bool isPlaying)
        {
            if (PlacementManager.Instance.InSceneObjectsCount < 1)
            {
                clearButton.SetActive(false);
            }
            else
            {
                clearButton.SetActive(!isPlaying);
            }

            if (isPlaying)
                PopUpManager.Instance.CloseAll();
        }

        private void HandleGameOver(bool isGameOver)
        {
            if (PlacementManager.Instance.InSceneObjectsCount < 1)
            {
                clearButton.SetActive(false);
            }
            else
            {
                clearButton.SetActive(!isGameOver);
            }
        }

        public void PromptClearConfirmation()
        {
            AudioManager.Instance.PlayAudioClip("ObjectSelection");

            clearButtonLastState = clearButton.activeSelf;

            PopUpManager.Instance.ShowClearConfirmation();
            clearButton.SetActive(false);
            resetButton.SetActive(false);
        }

        public void PromptResetConfirmation()
        {
            AudioManager.Instance.PlayAudioClip("ObjectSelection");

            clearButtonLastState = clearButton.activeSelf;

            PopUpManager.Instance.ShowResetConfirmation();
            clearButton.SetActive(false);
            resetButton.SetActive(false);
        }

        public void OnClearGameClick()
        {
            AudioManager.Instance.PlayAudioClip("Reset");

            GameManager.Instance.RestartGame();

            if (MenuControllerUI != null)
                MenuControllerUI.SetMenuList();

            clearButton.SetActive(false);
            resetButton.SetActive(true);
        }

        public void CancelRestart()
        {
            AudioManager.Instance.PlayAudioClip("HelpClose");

            PopUpManager.Instance.CloseAll();

            if (PlacementManager.Instance.InSceneObjectsCount > 0)
            {
                clearButton.SetActive(clearButtonLastState);
                resetButton.SetActive(true);
            }

        }

        public void OnRestartGameClick()
        {
            AudioManager.Instance.PlayAudioClip("Reset");

            GameManager.Instance.RestartGame();
            arSession.Reset();

            UXManager.Instance.GoToSection(SectionType.Scan);
        }

        /// <summary>
        /// Registers the callback.
        /// </summary>
        private void RegisterCallbacks()
        {
            SelectionManager.Instance.OnSelection += HandleSelectionActive;
            GameManager.Instance.OnPlay += HandlePlay;
            GameManager.Instance.OnGameOver += HandleGameOver;
        }

        /// <summary>
        /// Unregisters the callback.
        /// </summary>
        private void UnregisterCallbacks()
        {
            SelectionManager.Instance.OnSelection -= HandleSelectionActive;
            GameManager.Instance.OnPlay -= HandlePlay;
            GameManager.Instance.OnGameOver -= HandleGameOver;
        }

    }
}

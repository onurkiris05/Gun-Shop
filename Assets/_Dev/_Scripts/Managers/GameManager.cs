using System;
using UnityEngine;

namespace Game.Managers
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        public static event Action<GameState> OnBeforeStateChanged;
        public static event Action<GameState> OnAfterStateChanged;
        public static event Action OnLevelCompleted;
        public static event Action<int> OnCardUpdate;
        public static event Action<int> OnCardShopping;
        public GameState State { get; private set; }


        #region UNITY EVENTS

        protected override void Awake()
        {
            base.Awake();

            if (!IsNewGame() && !IsLevelLoaded())
            {
                Debug.Log("Loading level");
                SceneController.Instance.LoadScene(PlayerPrefs.GetInt("CurrentLevel", 0));
            }

            State = GameState.Start;
        }

        #endregion

        #region PUBLIC METHODS

        public void ChangeState(GameState newState)
        {
            if (newState == State) return;

            OnBeforeStateChanged?.Invoke(newState);

            State = newState;
            switch (newState)
            {
                case GameState.Start:
                    break;
                case GameState.Running:
                    break;
                case GameState.EndGame:
                    break;
                case GameState.MinigameRunning:
                    break;
                case GameState.MinigameShopping:
                    break;
                case GameState.MinigameEnd:
                    break;
            }

            OnAfterStateChanged?.Invoke(newState);
            Debug.Log($"New state: {newState}");
        }

        public void StartGame()
        {
            ChangeState(GameState.Running);
            SetNewGame();
        }

        public bool IsNewGame() => PlayerPrefs.GetInt("IsNewGame", 1) == 1;

        public void SetNewGame() => PlayerPrefs.SetInt("IsNewGame", 0);

        public int GetLevel() => PlayerPrefs.GetInt("CurrentLevel", 0) + 1;

        public void InvokeOnLevelCompleted() => OnLevelCompleted?.Invoke();
        public void InvokeOnCardUpdate(int minigameWallet) => OnCardUpdate?.Invoke(minigameWallet);
        public void InvokeOnCardShopping(int minigameWallet) => OnCardShopping?.Invoke(minigameWallet);

        #endregion

        #region PRIVATE METHODS

        private bool IsLevelLoaded() => SceneController.Instance.CheckIsSceneLoaded();

        #endregion
    }

    public enum GameState
    {
        Start,
        Running,
        EndGame,
        MinigameRunning,
        MinigameShopping,
        MinigameEnd
    }
}
namespace Script.Core
{
    public class GameState
    {
        private static GameState instance;
        public static GameState Instance => instance ??= new GameState();
        
        private struct GameStateData
        {
            public bool isFinished;
        }
        
        private GameStateData _gameStateData;
        public bool IsFinished => _gameStateData.isFinished;
        
        public void Finish()
        {
            _gameStateData.isFinished = true;
        }

        public void Reset()
        {
            _gameStateData = default;
        }
    }
}
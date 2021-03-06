using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private GameStateManager _gameStateManager = null;
    [SerializeField] private GameLoopManager _gameLoopManager = null;
    [SerializeField] private AudioManager _audioManager = null;

    [SerializeField] private EGameState _sceneToLoadFirst = EGameState.LEVEL1;

    private void Start()
    {
        _gameStateManager.Initialize();
        _gameLoopManager.Initialize();
        _audioManager.Initialize();

GameStateManager.Instance.LaunchTransition(_sceneToLoadFirst);
    }


}

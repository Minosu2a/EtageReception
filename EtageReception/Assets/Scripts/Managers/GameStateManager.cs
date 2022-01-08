using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    #region Fields
    private EGameState _currentState = EGameState.NONE;
    private Dictionary<EGameState, AGameState> _states = null;

    private EGameState _nextState = EGameState.NONE;
    private EGameState _previousState = EGameState.NONE;
    #endregion Fields

    #region Properties
    public AGameState CurrentState => _states[_currentState];
    public EGameState NextState => _nextState;
    public EGameState PreviousState => _previousState;
    #endregion Properties

    #region Methods
    #region Mono
    public void Initialize()
    {
        _states = new Dictionary<EGameState, AGameState>();

        InitializeState initializeState = new InitializeState();
        initializeState.Initialize(EGameState.INITIALIZE);
        _states.Add(EGameState.INITIALIZE, initializeState);

        LoadingState loadingState = new LoadingState();
        loadingState.Initialize(EGameState.LOADING);
        _states.Add(EGameState.LOADING, loadingState);

        MainMenuState mainMenuState = new MainMenuState();
        mainMenuState.Initialize(EGameState.MAINMENU);
        _states.Add(EGameState.MAINMENU, mainMenuState);

        GameState gameState = new GameState();
        gameState.Initialize(EGameState.GAME);
        _states.Add(EGameState.GAME, gameState);

        LEVEL1State level1State = new LEVEL1State();
        level1State.Initialize(EGameState.LEVEL1);
        _states.Add(EGameState.LEVEL1, level1State);

        _currentState = EGameState.INITIALIZE;
       //    CurrentState.EnterState();
    }

    protected override void Update()
    {
        CurrentState.UpdateState();
    }
    #endregion Mono

    #region StateMachine
    public void ChangeState(EGameState newState)
    {
        Debug.Log("Transition from " + _currentState + " to " + newState);

        CurrentState.ExitState();
        _previousState = _currentState;

        _currentState = newState;
        CurrentState.EnterState();
    }

    public void LaunchTransition(EGameState newState)
    {
        _previousState = _currentState;
        _nextState = newState;
        ChangeState(EGameState.LOADING);
    }
    #endregion StateMachine
    #endregion Methods
}

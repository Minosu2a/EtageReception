using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingState : AGameState
{
    #region Fields
    private AsyncOperation _loadingOp = null;
    private AsyncOperation _nextOp = null;
    private AsyncOperation _prevOp = null;

    private bool _isLoadFinish = false;
    private bool _isUnloadFinish = false;
    #endregion Fields

    #region Properties
    #endregion Properties

    #region Methods
    public override void EnterState()
    {
        _loadingOp = SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
    }

    public override void UpdateState()
    {
        if(_loadingOp.isDone && !_isLoadFinish)
        {
            _isLoadFinish = true;
            string sceneName = string.Empty;
            sceneName = GameStateHelper.SetSceneName(GameStateManager.Instance.NextState);
            _nextOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            sceneName = GameStateHelper.SetSceneName(GameStateManager.Instance.PreviousState);
            _prevOp = SceneManager.UnloadSceneAsync(sceneName);
        }

        if(_nextOp != null && _prevOp != null && _isLoadFinish && !_isUnloadFinish)
        {
            if(_prevOp.isDone && _nextOp.isDone)
            {
                _loadingOp = SceneManager.UnloadSceneAsync("LoadingScreen");
                _isUnloadFinish = true;
            }
        }
        
        if (_loadingOp.isDone && _isLoadFinish && _isUnloadFinish)
        {
            GameStateManager.Instance.ChangeState(GameStateManager.Instance.NextState);
        }
    }

    public override void ExitState()
    {
        ResetOps();
    }

    private void ResetOps()
    {
        _isLoadFinish = false;
        _isUnloadFinish = false;
        _loadingOp = null;
        _nextOp = null;
        _prevOp = null;
}
    #endregion Methods
}

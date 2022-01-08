using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStateHelper
{
    public static string SetSceneName(EGameState state)
    {
        string sceneName = string.Empty;
        switch (state)
        {
            case EGameState.INITIALIZE:
                sceneName = "PersistentManagers";
                break;
            case EGameState.LOADING:
                sceneName = "LoadingScreen";
                break;
            case EGameState.MAINMENU:
                sceneName = "MainMenu";
                break;
            case EGameState.GAME:
                sceneName = "LysianeLD";
                break;
            case EGameState.BATTLE:
                sceneName = "BattleScene";
                break;
            case EGameState.LEVEL1:
                sceneName = "AdrienLD";
                break;
            case EGameState.LEVEL2:
                sceneName = "LysianeLD";
                break;
        }
        return sceneName;
    }
}

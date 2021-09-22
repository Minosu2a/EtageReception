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
                sceneName = "Game";
                break;
            case EGameState.BATTLE:
                sceneName = "BattleScene";
                break;
        }
        return sceneName;
    }
}

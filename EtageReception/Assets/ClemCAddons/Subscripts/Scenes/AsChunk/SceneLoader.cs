using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClemCAddons;
using UnityEngine.SceneManagement;
using System;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Transform _positionTarget;
    [SerializeField] private Scenes _scenesAreas;
    private string[] _lockedScenes = new string[] { };
    private string _currentScene;
    private AsyncOperation _currentLoading;

    public Scenes ScenesAreas { get => _scenesAreas; set => _scenesAreas = value; }

    [Serializable]
    public class Scenes
    {
        public string[] Names;
        public Transform[] Areas;
        public Scenes(string[] scenes, Transform[] areas)
        {
            Names = scenes;
            Areas = areas;
        }
    }

    void Update()
    {
        if (ClemCAddons.Utilities.Timer.MinimumDelay(777, 0.1f))
        {
            for (int i = 0; i < _scenesAreas.Names.Length; i++)
            {
                var pos1 = _scenesAreas.Areas[i].position - (_scenesAreas.Areas[i].lossyScale / 2);
                var pos2 = _scenesAreas.Areas[i].position + (_scenesAreas.Areas[i].lossyScale / 2);
                if (_positionTarget.position.IsBetween(Vector3.Min(pos1, pos2), Vector3.Max(pos1, pos2)))
                {
                    bool valid = true;
                    for (int t = 0; t < SceneManager.sceneCount; t++)
                    {
                        if (SceneManager.GetSceneAt(t).name == _scenesAreas.Names[i])
                            valid = false;
                    }
                    if (valid)
                    {
                        Debug.Log("Loading " + _scenesAreas.Names[i]);
                        SceneManager.LoadSceneAsync(_scenesAreas.Names[i], LoadSceneMode.Additive);
                    }
                }
                else
                {
                    for (int t = 0; t < SceneManager.sceneCount; t++)
                    {
                        if (SceneManager.GetSceneAt(t).name == _scenesAreas.Names[i] && _lockedScenes.FindIndex(_scenesAreas.Names[i]) == -1)
                        {
                            Debug.Log("Unloading " + SceneManager.GetSceneAt(t).name);
                            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(t).name);
                            return;
                        }
                    }
                }
            }
        }
    }


    public void UberTravelPreLoad(string fromLvl, string toLvl)
    {
        _currentScene = fromLvl;
        _lockedScenes = _lockedScenes.Add(toLvl).Add(fromLvl);
        bool valid = true;
        for (int t = 0; t < SceneManager.sceneCount; t++)
        {
            if (SceneManager.GetSceneAt(t).name == toLvl)
                valid = false;
        }
        if (valid)
        {
            Debug.Log("Loading " + toLvl);
            _currentLoading = SceneManager.LoadSceneAsync(toLvl, LoadSceneMode.Additive);
        }
    }

    public void ExecuteUberTravel(Transform player, Vector3 targetPosition)
    {
        StartCoroutine(UberLoading(player, targetPosition));
    }

    IEnumerator UberLoading(Transform player, Vector3 targetPosition)
    {
        while (_currentLoading != null && !_currentLoading.isDone)
            yield return new WaitForSeconds(0.1f);
        player.position = targetPosition;
        _lockedScenes = new string[] { };
    }
}

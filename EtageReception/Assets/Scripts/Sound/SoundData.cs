using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Database/SoundData")]
public class SoundData : ScriptableObject
{
    [Tooltip("The name of the Sound Data (Make sure it's this one you are using when starting Sounds or Musics)")]
    [SerializeField] private string _key = string.Empty;

    [Tooltip("Drop the Audio File in here, the name of the audio file does not matter")]
    [SerializeField] private AudioClip _clip = null;

    [TextArea]
    [Tooltip("A Description of the Sounds to place any details needed for your team")]
    [SerializeField] private string _description = string.Empty;

    [Space]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _volume = 1;

    [Range(-3.0f, 3.0f)]
    [SerializeField] private float _pitch = 1;

    [Space]
    [SerializeField] private bool _loop = false;


    public string Key => _key;

    public AudioClip Clip => _clip;

    public float Volume  => _volume;

    public float Pitch => _pitch;

    public bool Loop => _loop;

}

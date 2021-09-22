using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    #region Fields
    [Header("Sound Datas")]
    [Tooltip("Place Every Sound Data you want to use in here ! (Watch out to not let a slot empty)")]
    [SerializeField] private SoundData[] _soundDatas = null; //THE SOUND DATA TABLE, 

    [Header("Sound & Music Sources")]
    [Tooltip("2D Sound Source is the prefab model used for all 2D non-looped 'Sound Datas' (Find it in the 'Prefabs/Audio' folder)")]
    [SerializeField] private AudioSource _2DSoundSource = null; 
    [Tooltip("3D Sound Source is the prefab model used for all 3D non-looped 'Sound Datas' (Find it in the 'Prefabs/Audio' folder)")]
    [SerializeField] private AudioSource _3DSoundSource = null;

    [Tooltip("2D Repetitive Sound Source is the prefab model used for all 2D looped 'Sound Datas' (Find it in the 'Prefabs/Audio' folder)")]
    [SerializeField] private AudioSource _2DRepetitiveSoundSource = null;
    [Tooltip("3D Repetitive Sound Source is the prefab model used for all 3D looped 'Sound Datas' (Find it in the 'Prefabs/Audio' folder)")]
    [SerializeField] private AudioSource _3DRepetitiveSoundSource = null; 
    [Space]

    [Tooltip("Music Source is a unique 2D AudioSource, it's children of the AudioManager")]
    [SerializeField] private AudioSource _musicSource = null;
    [Tooltip("Transition Source is a 2D Audiosource used to do fades and transition. It's also children of the AudioManager")]
    [SerializeField] private AudioSource _transitionSource = null; 

    [Header("Volumes")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _soundsVolume = 1f;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _musicsVolume = 1f;
    [Space]

    private Dictionary<string, SoundData> _soundData = null;



    private Dictionary<string, AudioSource> _2DSources = null;
    private Dictionary<string, AudioSource> _2DRepetitiveSources = null;

    private Dictionary<string, AudioSource> _3DSources = null;
    private Dictionary<string, AudioSource> _3DRepetitiveSources = null;




    //Timer Réference/Attribut
    [Header("Default Timer Value")]
    [Tooltip("Default value and suggested value is 0.1f")]
    [SerializeField] private float _fadeTick = 0.1f;

    //--- FADE IN
    private float _timeToFadeIn = 3f;

    private Timer _timerFadeInTick = null;
    private float _volumeFadeInTarget = 1f;
    private float _fadeInTickVolumeValue = 0;


    //--- FADE OUT
    private float _timeToFadeOut = 3f;

    private Timer _timerFadeOutTick = null;
    private float _volumeFadeOutTarget = 1f;
    private float _fadeOutTickVolumeValue = 0;


    [Header("Optional")]
    [Tooltip("Experimental Method that destroy the sound after it played. Set to true if you want it to be active")]
    [SerializeField] private bool _activateSoundDestroyer = true;

    [Space]
    [Tooltip("True = the 'Music Data On Start' will start playing when the game launch")]
    [SerializeField] private bool _playMusicOnStart = false;

    [Tooltip("Place the SoundData of the music you want to play on start (If 'Play Music On Start' is set to false nothing will happen)")]
    [SerializeField] private SoundData _musicDataOnStart = null;


    #endregion Fields

    #region Property

    public float SoundsVolume
    {
        get
        {
            return _soundsVolume;
        }
        set
        {
            _soundsVolume = Mathf.Clamp(value, 0, 1);
            MainSoundVolumeUpdate();
        }
    }

    public float MusicsVolume
    {
        get
        {
            return _musicsVolume;
        }
        set
        {
            _musicsVolume = Mathf.Clamp(value, 0, 1);
            MusicVolumeUpdate();
        }
    }


    #endregion Property

    #region Methods



    #region Start
    public void Initialize()
    {

        _soundData = new Dictionary<string, SoundData>();

        if (_soundDatas.Length >= 1)    //CHECK IF ANY SOUND DATAS EXIST
        {
            for (int i = 0; i < _soundDatas.Length; i++)    //SETUP THE DICTIONARY CORRECTLY
            {
                _soundData.Add(_soundDatas[i].Key, _soundDatas[i]);
            }

            _2DSources = new Dictionary<string, AudioSource>();
            _2DRepetitiveSources = new Dictionary<string, AudioSource>();

            _3DSources = new Dictionary<string, AudioSource>();
            _3DRepetitiveSources = new Dictionary<string, AudioSource>();


            _timerFadeInTick = new Timer(); //SET UP THE TIMER CORRECTLY (USED FOR MUSIC TRANSITIONS)
            _timerFadeInTick.OnTick += FadeInTick;

            _timerFadeOutTick = new Timer();
            _timerFadeOutTick.OnTick += FadeOutTick;

        }

        if(_playMusicOnStart == true)
        {
            if(_musicDataOnStart == null)  
            {
                Debug.LogWarning("You have to setup the 'Music Data On Start' with the corresponding SoundData of the Music");
            }
            else
            {
                AudioSource source = _musicSource;

                AudioClip clipToPlay = _musicDataOnStart.Clip;

                source.volume = (_musicDataOnStart.Volume * _soundsVolume);

                source.pitch = _musicDataOnStart.Pitch;

                source.loop = _musicDataOnStart.Loop;

                source.clip = clipToPlay;

                source.Play();
            }

        }


    }
    #endregion Start

    #region Volume Manager
    private void MainSoundVolumeUpdate()
    {
        _2DSoundSource.volume = _soundsVolume;
        _3DSoundSource.volume = _soundsVolume;
        _2DRepetitiveSoundSource.volume = _soundsVolume;
        _3DRepetitiveSoundSource.volume = _soundsVolume;
    }

    private void MusicVolumeUpdate()
    {
        _musicSource.volume = _musicsVolume;
        _transitionSource.volume = _musicsVolume;
    }
    #endregion Volume Manager


    #region Music
    public void PlayMusic(string key)
    {

        if (_soundData.ContainsKey(key) == false)
        {
            Debug.LogError("Fnct Play Music : Specified key not found for the audio file");
        }
        else
        {

            if (_soundData[key].Loop == false)   //Verify if the Soundata is set to loop, if it's not the case use PlayAudioOneShot instead
            {
                Debug.LogWarning("Fnct PlayMusic : The Soundata is not set as loop");
            }
            PlayAudio(_musicSource, key);
        }

    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void PlayMusicWithFadeIn(string key, float speed)
    {

        if (_soundData.ContainsKey(key) == false)
        {
            Debug.LogError("Fnct Play Music with Fade In : Specified key not found for the audio file");
        }
        else
        {

            if (_soundData[key].Loop == false)   //Verify if the Soundata is set to loop, if it's not the case use PlayAudioOneShot instead
            {
                Debug.LogWarning("Fnct PlayMusicWithFadeIn : The Soundata is not set as loop");
            }

            _timeToFadeIn = speed;

            float numberofTick = _timeToFadeIn / _fadeTick;  //Calcul du nombre de tick en fonction de speed et la valeur fadeTick

            _fadeInTickVolumeValue = (_soundData[key].Volume * _musicsVolume) / numberofTick;  //Calcul du volume à incrémenter à chaque tick
            _volumeFadeInTarget = _soundData[key].Volume * _musicsVolume; //Calcul du volume que la source va avoir (Valeur Max)

            PlayAudio(_musicSource, key);

            _musicSource.volume = 0; //A faire plus propre (comment?)

            _timerFadeInTick.StartTimer(_fadeTick);
        }


    }


    private void FadeInTick()
    {
        if (_musicSource.volume >= _volumeFadeInTarget)
        {
            _timerFadeInTick.StopTimer();
        }
        else
        {
            _musicSource.volume += _fadeInTickVolumeValue;

        }
    }


    public void StopMusicWithFadeOut(float speed)
    {

        if (!_musicSource.isPlaying)
        {
            Debug.LogWarning("Fnct Stop Music with Fade Out : There is no Audio playing to fade Out");
        }
        else
        {

            SwitchAudioSource(_musicSource, _transitionSource); //Switch du clip vers l'audio source transition

            _musicSource.Stop();


            _timeToFadeOut = speed;

            float numberofTick = _timeToFadeOut / _fadeTick;  //Calcul du nombre de tick en fonction de speed et la valeur fadeTick

            _fadeOutTickVolumeValue = _transitionSource.volume / numberofTick;  //Calcul du volume à incrémenter à chaque tick
            _volumeFadeOutTarget = 0; //Calcul du volume que la source va avoir (Valeur Max)

            _transitionSource.Play();

            _timerFadeOutTick.StartTimer(_fadeTick);
        }

    }
    private void SwitchAudioSource(AudioSource audioToChange, AudioSource audioTarget)
    {
        AudioClip clipToSwitch = audioToChange.clip;

        audioTarget.volume = audioToChange.volume;

        audioTarget.pitch = audioToChange.pitch;

        audioTarget.clip = clipToSwitch;

        audioTarget.time = audioToChange.time; //Démare la musique sur une nouvelle source âu même moment T que sur l'audio à changer
    }

    private void FadeOutTick()
    {
        if (_transitionSource.volume <= _volumeFadeOutTarget)
        {
            _timerFadeOutTick.StopTimer();
            _transitionSource.Stop();
        }
        else
        {
            _transitionSource.volume -= _fadeOutTickVolumeValue;

        }
    }


    public void SwitchMusicTransition(string key, float fadeInSpeed, float fadeOutSpeed)
    {

        if (_soundData.ContainsKey(key) == false)
        {
            Debug.LogError("Fnct Switch Music : Specified key not found for the audio file");

        }
        else
        {

            if (!_musicSource.isPlaying)
            {
                Debug.LogWarning("Fnct Switch Music : There is no Audio already playing to fade Out");
            }


            #region Audio Source Switch + Fade Out

            SwitchAudioSource(_musicSource, _transitionSource); //Switch du clip vers l'audio source transition


            _transitionSource.Play();

            #endregion Audio Source Switch



            #region Fade Out

            _timeToFadeOut = fadeOutSpeed;

            float numberofTickFadeOut = _timeToFadeOut / _fadeTick;  //Calcul du nombre de tick en fonction de speed et la valeur fadeTick

            _fadeOutTickVolumeValue = _transitionSource.volume / numberofTickFadeOut;  //Calcul du volume à incrémenter à chaque tick
            _volumeFadeOutTarget = 0; //Calcul du volume que la source va avoir (Valeur Max)

            _transitionSource.Play();

            _timerFadeOutTick.StartTimer(_fadeTick);

            #endregion Fade Out



            #region New Music Play + Fade In



            _timeToFadeIn = fadeInSpeed;

            float numberofTickFadeIn = _timeToFadeIn / _fadeTick;  //Calcul du nombre de tick en fonction de speed et la valeur fadeTick

            _fadeInTickVolumeValue = (_soundData[key].Volume * _musicsVolume) / numberofTickFadeIn;  //Calcul du volume à incrémenter à chaque tick
            _volumeFadeInTarget = _soundData[key].Volume * _musicsVolume; //Calcul du volume que la source va avoir (Valeur Max)

            PlayAudio(_musicSource, key);

            _musicSource.volume = 0; //A faire plus propre (comment?)

            _timerFadeInTick.StartTimer(_fadeTick);

            #endregion New Music Play + Fade In

        }


    }
    #endregion Music




    #region 2DSound

    /// <summary>
    /// Start a 2D Sound
    /// </summary>
    /// <param name="key">Parameter value to pass.</param>
    /// <returns>Returns an integer based on the passed value.</returns>
    public void Start2DSound(string key)
    {
        if (_soundData.ContainsKey(key) == false)   
        {
            Debug.LogError("Fnct StartSound2D : Specified key not found for the audio file");
        }
        else if (_soundData[key].Loop == true) //Verify if the Soundata is set to loop, if it's not the case use PlayAudioOneShot instead
        {
            AudioSource repSource2D = Instantiate(_2DRepetitiveSoundSource, transform);
            _2DRepetitiveSources.Add(key, repSource2D);

            PlayAudio(repSource2D, key);
        }
        else
        {
            AudioSource oneShotSource2D = Instantiate(_2DSoundSource, transform);
           // _2DSources.Add(key, oneShotSource2D);

            PlayAudio(oneShotSource2D, key);
            StartCoroutine(SoundDestroyer(_soundData[key].Clip.length, oneShotSource2D));

        }
    }

    #endregion 2DSound



    #region 3DSound
    public void Start3DSound(string key, Transform position)
    {

        if (_soundData.ContainsKey(key) == false)   //Verify if the Soundata is set to loop, if it's not the case use PlayAudioOneShot instead
        {
            Debug.LogError("Fnct Start3DSound : Specified key not found for the audio file");
        }
        else if (_soundData[key].Loop == true)
        {
            AudioSource repSource3D = Instantiate(_3DRepetitiveSoundSource, position);
            _3DRepetitiveSources.Add(key, repSource3D);

            PlayAudio(repSource3D, key);
        }
        else
        {
            AudioSource oneShotSource3D = Instantiate(_3DSoundSource, position);
            //_3DSources.Add(key, oneShotSource3D);

            PlayAudio(oneShotSource3D, key);
        }
    }
    #endregion 3DSound



    #region Common Sounds

    private void PlayAudio(AudioSource source, string key) //USED BY OTHER METHOD IN THE CLASS
    {

        AudioClip clipToPlay = _soundData[key].Clip;

        source.volume = (_soundData[key].Volume * _soundsVolume);

        source.pitch = _soundData[key].Pitch;

        source.loop = _soundData[key].Loop;

        source.clip = clipToPlay;

        source.Play();
    } 

    public void StopSound(ESoundType soundType, string audioSourceName)
    {
        GameObject audioSourceToDestoy;

        if (_soundData.ContainsKey(audioSourceName) == false)
        {
            Debug.LogError("Fnct StopSound : Specified key not found for the audio file");
        }

        switch (soundType)
        {
            case ESoundType.ONESHOT2D:

                if (_2DSources.Count == 0)
                {
                    Debug.LogWarning("Fnct StopSound : There is no audio source currently playing, the type of sound you are trying to stop is probably incorrect");
                }
                else
                {


                    audioSourceToDestoy = _2DSources[audioSourceName].gameObject;

                    _2DSources.Remove(audioSourceName);
                    Destroy(audioSourceToDestoy);
                }
                break;


            case ESoundType.REPETITIVE2D:

                if (_2DRepetitiveSources.Count == 0)
                {
                    Debug.LogWarning("Fnct StopSound : There is no audio source currently playing, the type of sound you are trying to stop is probably incorrect");
                }
                else
                {

                    audioSourceToDestoy = _2DRepetitiveSources[audioSourceName].gameObject;

                    _2DRepetitiveSources.Remove(audioSourceName);
                    Destroy(audioSourceToDestoy);
                }
                break;


            case ESoundType.ONESHOT3D:

                if (_3DSources.Count == 0)
                {
                    Debug.LogWarning("Fnct StopSound : There is no audio source currently playing, the type of sound you are trying to stop is probably incorrect");
                }
                else
                {
                    audioSourceToDestoy = _3DSources[audioSourceName].gameObject;

                    _3DSources.Remove(audioSourceName);
                    Destroy(audioSourceToDestoy);
                }
                break;


            case ESoundType.REPETITIVE3D:

                if (_3DRepetitiveSources.Count == 0)
                {
                    Debug.LogWarning("Fnct StopSound : There is no audio source currently playing, the type of sound you are trying to stop is probably incorrect");
                }
                else
                {
                    audioSourceToDestoy = _3DRepetitiveSources[audioSourceName].gameObject;

                    _3DRepetitiveSources.Remove(audioSourceName);
                    Destroy(audioSourceToDestoy);
                }
                break;

            default:
                Debug.LogWarning("Fnct StopSound : soundType is not correct");
                break;
        }


    }


    IEnumerator SoundDestroyer(float soundLength, AudioSource sourceToDestroy) //SOUND DESTROYER, WAIT FOR THE LENGTH OF THE SOUND AND WHEN THE SOUND IS DONE PLAYING IT WILL DESTROY IT
    {
        yield return new WaitForSeconds(soundLength);
        Destroy(sourceToDestroy.gameObject);
    }
    #endregion Common Sounds



    #region Repetitive Sounds

    public void ChangeRepetitiveSoundRate(string audioSourceName, float newRate)
    {

    }

    #endregion Repetitive Sounds



    #endregion Methods
}
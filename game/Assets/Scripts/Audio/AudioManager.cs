using ElementalAlchemist.Dialogue;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Progression;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ElementalAlchemist.Audio
{
    /// <summary>Central audio singleton: plays UI/gameplay one-shots in response to game events and drives the
    /// looping background track. Persists across scene loads like the other managers; duplicates self-destroy.</summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Volume")]
        [Range(0f, 1f)] [SerializeField] private float _musicVolume = 0.5f;
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1f;

        [Header("Music")]
        [SerializeField] private AudioClip _menuMusic;
        [SerializeField] private AudioClip _explorationMusic;
        [SerializeField] private AudioClip _cutsceneMusic;
        [SerializeField] private string _menuSceneName = "MainMenu";

        [Header("SFX")]
        [SerializeField] private AudioClip _uiClick;
        [SerializeField] private AudioClip _pauseMenuOpen;
        [SerializeField] private AudioClip _playerMenuOpen;
        [SerializeField] private AudioClip _fusionMenuOpen;
        [SerializeField] private AudioClip _sanctuaryMenuOpen;
        [SerializeField] private AudioClip _pickup;
        [SerializeField] private AudioClip _dialogueLine;
        [SerializeField] private AudioClip _stepChime;

        private AudioSource _musicSource;
        private AudioSource _sfxSource;
        private AudioClip _baseMusic;
        private bool _inCutscene;

        /// <summary>Plays the shared UI click sound (every button and menu close). No-op if no AudioManager is alive.</summary>
        public static void Click()
        {
            if (Instance)
            {
                Instance.PlayOneShot(Instance._uiClick);
            }
        }

        /// <summary>Plays the pause-menu open sound.</summary>
        public static void PauseOpen()
        {
            if (Instance)
            {
                Instance.PlayOneShot(Instance._pauseMenuOpen);
            }
        }

        /// <summary>Plays the player-menu open sound.</summary>
        public static void PlayerMenuOpen()
        {
            if (Instance)
            {
                Instance.PlayOneShot(Instance._playerMenuOpen);
            }
        }

        /// <summary>Plays the fusion-menu open sound.</summary>
        public static void FusionMenuOpen()
        {
            if (Instance)
            {
                Instance.PlayOneShot(Instance._fusionMenuOpen);
            }
        }

        /// <summary>Plays the sanctuary-menu open sound.</summary>
        public static void SanctuaryMenuOpen()
        {
            if (Instance)
            {
                Instance.PlayOneShot(Instance._sanctuaryMenuOpen);
            }
        }

        /// <summary>Plays the element-pickup sound. No-op if no AudioManager is alive yet.</summary>
        public static void Pickup()
        {
            if (Instance)
            {
                Instance.PlayOneShot(Instance._pickup);
            }
        }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.volume = _musicVolume;

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.volume = _sfxVolume;

            DialogueManager.LineAdvanced += OnDialogueLine;
            StoryDirector.StepCompleted += OnStepCompleted;
            ActionMapController.Changed += OnActionMapChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            // sceneLoaded does not fire for the scene we were created in, so pick the opening track here.
            if (Instance == this)
            {
                SelectBaseMusic(SceneManager.GetActiveScene());
            }
        }

        private void OnValidate()
        {
            // Lets the volume sliders be tuned live in Play mode.
            if (_musicSource)
            {
                _musicSource.volume = _musicVolume;
            }

            if (_sfxSource)
            {
                _sfxSource.volume = _sfxVolume;
            }
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            DialogueManager.LineAdvanced -= OnDialogueLine;
            StoryDirector.StepCompleted -= OnStepCompleted;
            ActionMapController.Changed -= OnActionMapChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }

        private void OnDialogueLine(string _) => PlayOneShot(_dialogueLine);

        private void OnStepCompleted(StoryStep _) => PlayOneShot(_stepChime);

        // A cutscene is a single locked moment (the Grand Master walking in + speaking), bounded by the Cutscene
        // action map. Dialogue keeps whatever is playing, so cutscene music carries through his lines but a plain NPC
        // chat (which never enters Cutscene) stays on exploration music.
        private void OnActionMapChanged(string map)
        {
            if (map == ActionMaps.Cutscene)
            {
                _inCutscene = true;
                PlayMusic(_cutsceneMusic);
            }
            else if (map == ActionMaps.Player && _inCutscene)
            {
                _inCutscene = false;
                PlayMusic(_baseMusic);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => SelectBaseMusic(scene);

        private void SelectBaseMusic(Scene scene)
        {
            _baseMusic = scene.name == _menuSceneName ? _menuMusic : _explorationMusic;

            // Don't stomp cutscene music if a scene loads mid-cutscene; OnActionMapChanged restores the base track.
            if (!_inCutscene)
            {
                PlayMusic(_baseMusic);
            }
        }

        private void PlayMusic(AudioClip clip)
        {
            if (!_musicSource || !clip || _musicSource.clip == clip)
            {
                return;
            }

            _musicSource.clip = clip;
            _musicSource.Play();
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (clip && _sfxSource)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }
    }
}

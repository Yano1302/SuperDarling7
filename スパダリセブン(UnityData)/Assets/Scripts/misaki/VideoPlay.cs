using UnityEngine;
using UnityEngine.Video;

namespace Art.Sample
{
    /// <summary>VideoPlayer�ɂ�铮��Đ��N���X</summary>
    public class VideoPlay : MonoBehaviour
    {
        public enum PlayState
        {
            Stoped = 0,
            Loading,
            Loaded,
            Playing,
            Paused
        }

        /// <summary>�G���[�ʒm</summary>
        public System.Action<string> OnErrorReceived = null;
        /// <summary>���[�h�����ʒm</summary>
        public System.Action OnPrepareCompleted = null;
        /// <summary>�Đ��J�n�ʒm</summary>
        public System.Action OnStarted = null;
        /// <summary>�Đ������ʒm</summary>
        public System.Action OnEnd = null;

        /// <summary>�X�e�[�^�X</summary>
        public PlayState State
        {
            get { return _playState; }
        }

        /// <summary>�C���X�^���X�̐���</summary>
        public static VideoPlay Create(Camera camera = null)
        {
            // �V�[�������f����ꍇ��DontDestroyOnLoad�ɂ���
            GameObject movieGameObject = new GameObject(kMovieGameObjectName);
            VideoPlay movieBehaviour = movieGameObject.AddComponent<VideoPlay>();
            movieBehaviour.Init(movieGameObject, camera);

            return movieBehaviour;
        }

        /// <summary>�C���X�^���X�̔j��</summary>
        public void Dispose()
        {
            // �C�x���g�ݒ����
            _videoPlayer.errorReceived -= ErrorReceived;
            _videoPlayer.prepareCompleted -= PrepareCompleted;
            _videoPlayer.started -= PlayStart;
            _videoPlayer.loopPointReached -= PlayEnd;
            OnErrorReceived = null;
            OnPrepareCompleted = null;
            OnStarted = null;
            OnEnd = null;

            GameObject.Destroy(_movieGameObject);
        }

        /// <summary>���O���[�f�B���O</summary>
        public void Preload(string filePath)
        {
            _playState = PlayState.Loading;
#if WITH_DEVELOP
            _movieGameObject.name = kMovieGameObjectName + System.IO.Path.GetFileName(filePath);
#endif
            _videoPlayer.url = filePath;
            _videoPlayer.Prepare();
        }

        /// <summary>���O���[�h��������̍Đ��J�n</summary>
        public void PlayPrepared(bool loop = false, bool tapSkip = true)
        {
            if (_playState != PlayState.Loaded
                && _playState != PlayState.Stoped)
            {
                const string message = "not Prepared";
                ErrorReceived(_videoPlayer, message);
                return;
            }

            _tapSkip = tapSkip;
            _videoPlayer.isLooping = loop;
            _playState = PlayState.Playing;
            _videoPlayer.Play();
        }

        /// <summary>����̍Đ��J�n(�p�X�w��)</summary>
        public void Play(string filePath, bool loop = false, bool tapSkip = true)
        {
            if (_playState == PlayState.Playing || _playState == PlayState.Paused)
            {
                const string message = "already playing";
                ErrorReceived(_videoPlayer, message);
                return;
            }

            if (_playState == PlayState.Loading || _playState == PlayState.Loaded)
            {
                const string message = "already Prepared";
                ErrorReceived(_videoPlayer, message);
                return;
            }
#if WITH_DEVELOP
            _movieGameObject.name = kMovieGameObjectName + System.IO.Path.GetFileName(filePath);
#endif
            _tapSkip = tapSkip;
            _videoPlayer.url = filePath;
            _videoPlayer.isLooping = loop;

            _playState = PlayState.Playing;
            _videoPlayer.Play();
        }

        /// <summary>�Đ���~</summary>
        public void Stop()
        {
            if (_videoPlayer.isPlaying)
            {
                _videoPlayer.Stop();
            }

            PlayEnd(_videoPlayer);
        }

        /// <summary>�Đ��ꎞ��~</summary>
        public void Pause()
        {
            if (_playState != PlayState.Playing)
            {
                return;
            }

            _playState = PlayState.Paused;
            _videoPlayer.Pause();
        }

        /// <summary>�Đ��ꎞ��~�ĊJ</summary>
        public void Resume()
        {
            if (_playState != PlayState.Paused)
            {
                return;
            }

            _playState = PlayState.Playing;
            _videoPlayer.Play();
        }

        /// <summary>�\���̗L�������ݒ�</summary>
        public void SetEnabled(bool enabled)
        {
            _videoPlayer.enabled = enabled;
        }

        private void Init(GameObject gameObject, Camera camera)
        {
            _movieGameObject = gameObject;

            _audioSource = gameObject.AddComponent<AudioSource>();
            _videoPlayer = gameObject.AddComponent<VideoPlayer>();

            // �\���ݒ�
            _videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
            _videoPlayer.targetCamera = camera;
            _videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
            _videoPlayer.playOnAwake = false;

            // �T�E���h�ݒ�
            //TODO: �G�f�B�^���Ɖ������o�͂���Ȃ�(Unity 2017.2.1f1)
            //      PrepareCompleted��ɐݒ肷���2��ڈȍ~�͍Đ������c
            _audioSource.playOnAwake = false;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            _videoPlayer.controlledAudioTrackCount = 1;
            _videoPlayer.EnableAudioTrack(0, true);
            _videoPlayer.SetTargetAudioSource(0, _audioSource);

            // �C�x���g�ݒ�
            _videoPlayer.errorReceived += ErrorReceived;
            _videoPlayer.prepareCompleted += PrepareCompleted;
            _videoPlayer.started += PlayStart;
            _videoPlayer.loopPointReached += PlayEnd;
        }

        private void ErrorReceived(VideoPlayer player, string message)
        {
            if (OnErrorReceived != null)
            {
                OnErrorReceived(message);
            }
        }

        private void PrepareCompleted(VideoPlayer player)
        {
            _playState = PlayState.Loaded;
            if (OnPrepareCompleted != null)
            {
                OnPrepareCompleted();
            }
        }

        private void PlayStart(VideoPlayer player)
        {
            if (OnStarted != null)
            {
                OnStarted();
            }
        }

        private void PlayEnd(VideoPlayer player)
        {
            if (_playState == PlayState.Stoped)
            {
                return;
            }
            _playState = PlayState.Stoped;

            if (OnEnd != null)
            {
                OnEnd();
            }
        }

        void Update()
        {
            if (_playState != PlayState.Playing || !_tapSkip)
            {
                return;
            }

            // �^�b�v�m�F
            if (Input.GetMouseButtonUp(0))
            {
                UnityEngine.Debug.Log("push");
                Stop();
            }
        }

        private const string kMovieGameObjectName = "Movie_";
        private PlayState _playState = PlayState.Stoped;
        private GameObject _movieGameObject = null;
        private VideoPlayer _videoPlayer = null;
        private AudioSource _audioSource = null;
        private bool _tapSkip = false;

    } // class VideoPlay
} // namespace Art.Sample

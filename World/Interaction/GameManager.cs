using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


namespace CevarnsOfEvil
{

    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        [SerializeField] AudioMixer audioMixer;
        [SerializeField] GameObject FPSCounter;


        public static GameManager Instance => instance;
        public Level Dungeon => Level.Instance;


        private void Awake()
        {
          instance = this;  
        }


        public void Start()
        {
            instance = this;
            SetupAudio();
        }


        public void SetupAudio()
        {
            audioMixer.SetFloat("Volume", Options.AudioVolume);
            audioMixer.SetFloat("Game", Options.GameVolume);
            audioMixer.SetFloat("Music", Options.MusicVolume);
        }


        [Command]
        public void ShowFPS() {
            FPSCounter.SetActive(!FPSCounter.activeSelf);
        }


#region Level Transitions


        public void NextLevel()
        {
            StartCoroutine(LoadLevel());
        }


        private IEnumerator LoadLevel()
        {
            Time.timeScale = 0.0f;
            SceneManager.UnloadSceneAsync(GameConstants.DUNGEON_SCENE);
            yield return null;
            SceneManager.LoadScene(GameConstants.DUNGEON_SCENE, LoadSceneMode.Additive);
            yield return null;
            UIManager.Instance.EnterPlayMode();
            yield return null;
            while(!Level.levelReady) yield return null;
            yield return null;
            Player.PC.EnableInput();
            UIManager.Instance.SoundManager.UnMuteGame();
            Time.timeScale = 1.0f;
        }


        public void ReloadLevel()
        {
            StartCoroutine(DoReloadLevel());
        }


        private IEnumerator DoReloadLevel()
        {
            SceneManager.UnloadSceneAsync(GameConstants.DUNGEON_SCENE);
            yield return null;
            SceneManager.LoadScene(GameConstants.DUNGEON_SCENE, LoadSceneMode.Additive);
        }

#endregion

    }

}

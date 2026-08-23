using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using LeastSquares;

namespace CevarnsOfEvil

{

    public class GameStartingScreen : MonoBehaviour
    {
        [SerializeField] TMP_Text levelText;
        [SerializeField] TMP_Text hintText;

        [SerializeField] string[] hints;
        private static List<string> shuffledHints = new List<string>();


        // Start is called before the first frame update
        void Start()
        {
            Time.timeScale = 0.0f; 
            if(GameData.resuming) GameData.LoadGame();
            levelText.text = LocalizationManager.GetTranslation("UIStrings", "LevelN", GameData.Level.ToString());
            UIManager.Instance.SoundManager.Stop();
            UIManager.Instance.SoundManager.MuteGame();
            Player.PC.DisableInput();
            StartCoroutine(LoadLevel());            
        }


        private IEnumerator LoadLevel()
        {
            Time.timeScale = 0.0f; 
            yield return null;
            if(GameData.resuming) GameData.LoadGame();
            yield return null;
            SceneManager.LoadScene(GameConstants.DUNGEON_SCENE, LoadSceneMode.Additive);
            yield return new WaitForSecondsRealtime(1.0f);
            ShowHint();            
            yield return new WaitForSecondsRealtime(2.0f);
            while(!Level.levelReady) yield return null;
            yield return null;
            UIManager.Instance.EnterPlayMode();
            yield return null;
            gameObject.SetActive(false);
            Player.PC.EnableInput();
            if(GameData.resuming) Player.PC.Actor.FixSword();
            UIManager.Instance.SoundManager.UnMuteGame();
            Time.timeScale = 1.0f;
        }


        private void ShowHint() {
            hintText.text = LocalizationManager.GetTranslation("Hints",
                    hints[Random.Range(0, hints.Length)]);
        }
    }


}
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


namespace CevarnsOfEvil
{

    public class Options : MonoBehaviour
    {
        private const float SQRT10 = 3.1622776601683793319988935444327f;

        [SerializeField] AudioMixer audioMixer;

        [SerializeField] GameObject parentMenu;

        [SerializeField] Slider lookSlider;
        [SerializeField] Slider moveSlider;
        [SerializeField] Slider volumeSlider;
        [SerializeField] Slider SFXSlider;
        [SerializeField] Slider musicSlider;
        [SerializeField] Toggle fullscreenToggle;
        [SerializeField] TMP_Dropdown qualityDropdown;

        public static float lookSensitivity = 0.5f;
        public static float moveSensitivity = 1.0f;
        public static float audioVolume = 0;
        public static float gameVolume = 0;
        public static float musicVolume = 0;
        public static bool isFullscreen;
        public static int graphicsQuality;


        private void Awake()
        {
            
        }


        private void OnDisable()
        {
            // Input Variables
            PlayerPrefs.SetFloat("LookSensitivity", lookSensitivity);
            PlayerPrefs.SetFloat("MoveSensitivity", moveSensitivity);
            // Graphics Variables
            if (isFullscreen) PlayerPrefs.SetInt("Fullscreen", 1);
            else PlayerPrefs.SetInt("Fullscreen", 0);
            PlayerPrefs.SetInt("GraphicsQuality", graphicsQuality);
            // Sound Variables
            PlayerPrefs.SetFloat("MasterVolume", audioVolume);
            PlayerPrefs.SetFloat("GameVolume", gameVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            // Misc Variables
            // ...TODO
        }

        private void Start()
        {
            // Input Variables
            lookSensitivity = PlayerPrefs.GetFloat("LookSensitivity", 0.5f);
            moveSensitivity = PlayerPrefs.GetFloat("MoveSensitivity", 1.0f);
            lookSlider.value = Mathf.Log(lookSensitivity * 2, SQRT10);
            moveSlider.value = Mathf.Log(moveSensitivity, SQRT10);
            // Graphics Variables
            isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) > 0;
            graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", 3);
            Screen.fullScreen = isFullscreen;
            QualitySettings.SetQualityLevel(graphicsQuality);
            fullscreenToggle.isOn = isFullscreen;
            qualityDropdown.value = graphicsQuality;
            // Sound Variables
            audioVolume = PlayerPrefs.GetFloat("MasterVolume", 0);
            gameVolume = PlayerPrefs.GetFloat("GameVolume", 0);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
            volumeSlider.value = Mathf.Pow(2, audioVolume / 10);
            SFXSlider.value = Mathf.Pow(2, gameVolume / 10);
            musicSlider.value = Mathf.Pow(2, musicVolume / 10);
            // Misc Variables
            // ...TODO
        }


        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = isFullscreen = fullscreen;
        }


        public void SetGraphicsQuality(int quality)
        {
            QualitySettings.SetQualityLevel(graphicsQuality = quality);
        }

        /*
         * A Brief Lession on Psychoaccustics
         * 
         * Intensity vs. Loudness: Intensity is a physical measure of 
         * the energy of sound, and may be measured in terms of power 
         * or sound preasure, with 10 dB being 10 times the power while 
         * 20 dB is 10 times the sound preasure.  In contrast, loudness 
         * is a subjective measure of how a sound sounds; research has 
         * found 10 times the power (+10 dB) results in twice the 
         * subjective loudness to tested listeners.  Thus:
         * 
         * +10 Db is
         * * 10 times the power
         * * 3.1622776... times the sound preasure, and
         * * 2 times the loudness
         * 
         * Based on this sound adjustment code is based on base 2 logarithms 
         * with twice the input value converted to +10 db, to accurately 
         * model human auditory perception.
         */

        public void SetAudioVolume(float loudness)
        {
            audioVolume = Mathf.Log(loudness, 2) * 10;
            audioMixer.SetFloat("Volume", audioVolume);
        }


        public void SetGameVolume(float loudness)
        {
            gameVolume = Mathf.Log(loudness, 2) * 10;
            audioMixer.SetFloat("Game", gameVolume);
        }


        public void SetMusicVolume(float loudness)
        {
            musicVolume = Mathf.Log(loudness, 2) * 10;
            audioMixer.SetFloat("Music", musicVolume);
        }


        /*
         * Input sensitity is set for a range of -1 to +1 and converted 
         * to based on powers of the square root of 10, so that the total 
         * range represents tuning the sensitivity such that the maximum 
         * is 10 time as sensative as the minimum, a good range.  The 
         * default value is 0, and converted to the sensitivity found best 
         * for play with keyboard and mouse during play testing.
         * 
         * This is valuable both because different players may prefer different 
         * levels of sensitivity and because different systems and drivers 
         * may respond differently.  Notably I have found a look sensitivity 
         * that seems responsive with an XBox controller in Windows is very 
         * sluggish with the same controller in Linux.
         */


        public void SetLookSensitivity(float rawValue)
        {
            lookSensitivity = 0.5f * Mathf.Pow(SQRT10, rawValue);
        }


        public void SetMoveSensitivity(float rawValue)
        {
            moveSensitivity = Mathf.Pow(SQRT10, rawValue);
        }


        public void DefaultSensitivity()
        {
            lookSensitivity = 0.5f;
            moveSensitivity = 1.0f;
            lookSlider.value = 0;
            moveSlider.value = 0;
        }


        public void Back()
        {
            parentMenu.SetActive(true);
            gameObject.SetActive(false);
        }


    }
}
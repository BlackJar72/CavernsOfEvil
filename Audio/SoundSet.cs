using UnityEngine;


namespace DLD
{
    [CreateAssetMenu(menuName = "DLD/Audio/SoundSet", fileName = "SoundSet", order = 101)]
    public class SoundSet : ScriptableObject
    {
        [SerializeField] Sound[] sounds;


        public void PlaySound(AudioSource source, int sound)
        {
            sounds[sound].Play(source);
        }


        public void PlayRandom(AudioSource source)
        {
            sounds[Random.Range(0, sounds.Length)].Play(source);
        }


        public void PlayRandomized(AudioSource source)
        {
            sounds[Random.Range(0, sounds.Length)].PlayRandomized(source);
        }


        public void PlayRandomized(AudioSource source, int sound)
        {
            sounds[sound].PlayRandomized(source);
        }
    }

}
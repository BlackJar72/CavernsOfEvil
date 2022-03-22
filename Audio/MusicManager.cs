using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace CevarnsOfEvil
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] AudioClip[] allTracks;
        private AudioSource music;
        private static int lastPlayed;
        private List<AudioClip> shuffler;


        // Start is called before the first frame update
        void Start()
        {
            music = GetComponent<AudioSource>();
            shuffler = new List<AudioClip>();
            if (GameData.Level == 1)
            {
                lastPlayed = 0;
                shuffler.Add(allTracks[0]);
            }
            else
            {
                for(int i = 0; i < allTracks.Length; i++)
                {
                    if(i != lastPlayed) shuffler.Add(allTracks[i]);
                }
                lastPlayed = Random.Range(0, shuffler.Count);
            }
            PlayTrack(lastPlayed);
        }


        private void PlayTrack(int track)
        {
            music.clip = shuffler[track];
            music.Play();
        }
    }

}
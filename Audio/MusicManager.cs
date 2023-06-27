using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace CevarnsOfEvil
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] AudioClip levelOneMusic;
        [SerializeField] List<AudioClip> allTracks;
        private AudioSource music;

        private static int index;
        private static List<AudioClip> usedTracks;


        private void Awake()
        {
            if (usedTracks == null)
            {
                usedTracks = new List<AudioClip>();
                foreach (AudioClip track in allTracks)
                {
                    usedTracks.Add(track);
                }
                Init();
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            music = GetComponent<AudioSource>();
            if (GameData.Level == 1)
            {
                usedTracks.Shuffle();
                music.clip = levelOneMusic;
                music.Play();
            }
            else
            {
                PlayTrack(index % usedTracks.Count);
                index++;
                if(index >= usedTracks.Count)
                {
                    index = 0;
                    usedTracks.Shuffle();
                }
            }
        }


        private void PlayTrack(int track)
        {
            music.clip = usedTracks[track];
            music.Play();
        }


        public static void Init()
        {
            index = 0;
        }
    }

}
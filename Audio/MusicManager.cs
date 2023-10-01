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
                if((GameData.Level == 2) && (usedTracks[index % usedTracks.Count] == levelOneMusic)) {
                    index++;
                    // This should not happen, but just in case....
                    if(index >= usedTracks.Count) NewShuffle();
                }
                PlayTrack(index % usedTracks.Count);
                index++;
                if(index >= usedTracks.Count) NewShuffle();
            }
        }


        private void PlayTrack(int track)
        {
            music.clip = usedTracks[track];
            music.Play();
        }


        private void NewShuffle() {
            index = 0;
            usedTracks.Shuffle();
        }


        public static void Init()
        {
            index = 0;
        }
    }

}

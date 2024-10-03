using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace EmployeeClasses.Util
{
    internal class Sounds
    {

        public static AudioClip beaconSound;

        public static AudioClip reelSound;
        public static AudioClip swingSound;
        public static AudioClip kickSound;

        public static AudioClip harvestSound;
        public static AudioClip synthSound;
        public static AudioClip stimSound;

        public static AudioClip switchSound;

        public static AudioClip hackSound;
        public static AudioClip failSound;

        public static Dictionary<int, AudioClip> library;

        public static void Awake()
        {
            beaconSound = LoadClip("Beacon");
            reelSound = LoadClip("KickReel");
            swingSound = LoadClip("KickSwing");
            kickSound = LoadClip("KickHit");
            harvestSound = LoadClip("Harvest");
            synthSound = LoadClip("Synthesize");
            stimSound = LoadClip("Stim");
            switchSound = LoadClip("Switch");
            hackSound = LoadClip("Hacking");
            failSound = LoadClip("HackFail");

            library = new Dictionary<int, AudioClip>() {
            { 0, beaconSound }, { 1, reelSound }, { 2, swingSound }, { 3, kickSound }, {4, harvestSound }, {5, synthSound }, {6, stimSound }, {7, switchSound}, {8, hackSound}, {9, failSound} };
        }

        public static AudioClip LoadClip(string clip) {
            var item = ModBase.assets.LoadAsset<AudioClip>(clip);

            if (item == null)
                ModBase.als.LogError("Trouble loading Beacon clip");

            return item;
        }

        public static void PlayClipFromSource(int clip, AudioSource source, float volume)
        {
            source.PlayOneShot(library[clip], volume);
        }

    }
}

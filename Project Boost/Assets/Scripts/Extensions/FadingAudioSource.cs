﻿using UnityEngine;

namespace Extensions
{
    public class FadingAudioSource
    {
        protected AudioSource Original { get; private set; }
        public FadingState FadingState { get; set; }
        public float PlayVolume { get; set; }
        public float FadeSpeed { get; set; }

        internal bool IsPlaying
        {
            get
            {
                return Original.isPlaying;
            }
        }

        public float CurrentVolume
        {
            get
            {
                return currentVolume;
            }

            protected set
            {
                currentVolume = value;
                Original.volume = value;
            }
        }

        private float currentVolume;
        private const float fadeSpeedLow = 0.3f;
        private const float lowerThresholdVolume = 0.05f;
        private const float upperThresholdVolume = 0.1f;


        public FadingAudioSource(AudioSource original)
        {
            Original = original;
            PlayVolume = original.volume;
            CurrentVolume = original.volume;
            FadeSpeed = 1f;
        }

        public void FadeOut()
        {
            FadingState = FadingState.FadingOut;
            if (CurrentVolume > upperThresholdVolume)
            {
                CurrentVolume -= FadeSpeed * Time.deltaTime;
            }
            else if (CurrentVolume > lowerThresholdVolume)
            {
                CurrentVolume -= fadeSpeedLow * Time.deltaTime;
            }
            else if (IsPlaying)
            {
                Original.Stop();
                FadingState = FadingState.None;
            }
        }

        public void Play()
        {
            if (IsPlaying)
            {
                Original.Stop();
            }
            FadingState = FadingState.None;
            CurrentVolume = PlayVolume;
            Original.Play();
        }
    }
}

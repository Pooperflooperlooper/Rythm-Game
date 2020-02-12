﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Visualizer : MonoBehaviour
{
    AudioSource _audioSource;
    public static float[] _samples = new float[512];
    public static float[] _freqBand = new float[8];

    float lastTimeSpawned;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        lastTimeSpawned = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        GetSpectrumAudioSource();
        MakeFreqBands();

        for (int i = 0; i < 8; i++) {
            if (_freqBand[i] > .8f && Time.timeSinceLevelLoad - lastTimeSpawned > .5f)
            {
                Debug.Log("Spawned");
                Vector3 spawnPos = new Vector3(i * 2, 10, 5);
                Instantiate(Resources.Load("Sphere"), spawnPos, Quaternion.identity);

                lastTimeSpawned = Time.timeSinceLevelLoad;
            }
        }

    }

    void GetSpectrumAudioSource() {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void MakeFreqBands() {
        /*
         * 22050 / 512 = 43
         * 
         * 20 - 60
         * 250 - 500
         * 500 - 2000
         * 2000 - 4000
         * 4000 - 6000
         * 
         * 0 - 2
         * 1 - 4
         * 2 - 8
         * 3 - 16
         * 4 - 32
         * 5 - 64
         * 6 - 128
         * 7 - 256
         */

        int count = 0;
        for (int i = 0; i < 8; i++) {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) *  2;

            if (i == 7) {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++) {
                if (count < 512) {
                    average += _samples[count] * (count++);
                    count++;
                }
            }

            average /= count;

            _freqBand[i] = average * 10;

        }

    }

}

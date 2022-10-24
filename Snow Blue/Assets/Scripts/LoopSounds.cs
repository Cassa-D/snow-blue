using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LoopSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] loopableSounds;
    [SerializeField] private AudioSource soundSource;
        
    private int _currSoundIndex;

    private int GetRandomIndex(int excludeIndex)
    {
        while (true)
        {
            if (loopableSounds.Length == 1)
            {
                return 0;
            }
            
            var randomIndex = Random.Range(0, loopableSounds.Length - 1);

            if (randomIndex == excludeIndex)
            {
                continue;
            }

            return randomIndex;
        }
    }

    private void Update()
    {
        if (!soundSource.isPlaying)
        {
            _currSoundIndex = GetRandomIndex(_currSoundIndex);

            if (_currSoundIndex >= loopableSounds.Length)
            {
                _currSoundIndex = 0;
            }
            
            soundSource.clip = loopableSounds[_currSoundIndex];
            soundSource.Play();
        }
    }

    public void SwitchMusic(int index, float volume)
    {
        if (index >= loopableSounds.Length)
        {
            index = 0;
        }
        
        _currSoundIndex = index;
        
        soundSource.clip = loopableSounds[_currSoundIndex];
        
        soundSource.volume = volume;
        soundSource.Play();
    }
}

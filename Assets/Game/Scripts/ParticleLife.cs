using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLife : MonoBehaviour
{
    public float timeToDestroy = 2f;

    private AudioSource particleSound;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
        particleSound = GetComponent<AudioSource>();

        if (particleSound)
        {
            particleSound.volume = PlayerPrefs.GetFloat("SoundEffects");
            particleSound.Play();
        }
    }
}
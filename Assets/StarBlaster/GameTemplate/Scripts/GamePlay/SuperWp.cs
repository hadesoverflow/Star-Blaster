using System;
using System.Collections;
using StarBlaster.GameTemplate.Scripts.GamePlay;
using UnityEngine;

public class SuperWp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            other.GetComponent<Asteroid>().OnHit();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            other.GetComponent<Asteroid>().OnHit();
        }
    }
}

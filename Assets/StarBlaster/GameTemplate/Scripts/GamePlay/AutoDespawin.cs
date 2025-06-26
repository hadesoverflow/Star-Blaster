using System;
using UnityEngine;

namespace StarBlaster.GameTemplate.Scripts.GamePlay
{
    public class AutoDespawin : MonoBehaviour
    {
        public float lifeTime;

        private void OnEnable()
        {
            Invoke("Despawn", lifeTime);
        }

        public void Despawn()
        {
            SimplePool.Despawn(this.gameObject);
        }
    }
}
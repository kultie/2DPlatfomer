using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class HurtBox : MonoBehaviour
    {
        private BoxCollider2D _collider;
        private IEntity _owner;

        private void Awake()
        {
            _owner = GetComponentInParent<IEntity>();
        }

        public void Hit(HitBox hitBox)
        {
            Debug.Log("Ouch");
        }
    }
}
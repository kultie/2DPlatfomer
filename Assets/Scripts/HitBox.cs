using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class HitBox : MonoBehaviour
    {
        private BoxCollider2D _collider;
        private IEntity _owner;
        [SerializeField] private int maxTargetHit = 5;
        [SerializeField] private int hitLagFrame = 5;
        [SerializeField] private bool multiHit = false;
        [SerializeField] private float hitInterval = 0.03f;

        private void OnEnable()
        {
            _owner = GetComponentInParent<IEntity>();
            _collider = GetComponent<BoxCollider2D>();


            CollisionCheck();

            if (multiHit)
            {
                StartCoroutine(MultiCheck());
            }
        }

        IEnumerator MultiCheck()
        {
            while (true)
            {
                yield return new WaitForSeconds(hitInterval);
                CollisionCheck();
            }
        }

        void CollisionCheck()
        {
            Collider2D[] hitSubjects = new Collider2D[maxTargetHit];
            _collider.OverlapCollider(new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("HurtBox"),
                useTriggers = true
            }, hitSubjects);
            var subjects = hitSubjects.Where(FilterTarget).Select(t => t.GetComponent<HurtBox>()).ToArray();

            foreach (var a in subjects)
            {
                a.Hit(this);
            }

            if (subjects.Length > 0)
            {
                StartCoroutine(HitStopSequence());
            }
        }

        IEnumerator HitStopSequence()
        {
            yield return null;
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(hitLagFrame / 60f);
            Time.timeScale = 1;
        }

        bool FilterTarget(Collider2D collider)
        {
            if (collider == null) return false;
            var entity = collider.GetComponent<HurtBox>();
            if (entity == null) return false;
            if (entity == _owner) return false;
            return true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Kultie.Platformer2DSystem
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineGroundAttack : EntityAction
    {
        private PlayableDirector _director;

        public override IEnumerator Process(IEntity caster)
        {
            _director = GetComponent<PlayableDirector>();
            // _director.timeUpdateMode = DirectorUpdateMode.Manual;
            _director.Play();
            float currentTime = 0;
            while (currentTime <= _director.duration)
            {
                currentTime += Time.deltaTime;
                _director.time = currentTime;
                _director.Evaluate();
                yield return null;
            }
        }
    }
}
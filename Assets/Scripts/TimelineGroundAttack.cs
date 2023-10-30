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
        [SerializeField] [Range(0.02f, 5)] private float speed = 1;

        public override IEnumerator Process(IEntity caster)
        {
            _director = GetComponent<PlayableDirector>();
            // _director.timeUpdateMode = DirectorUpdateMode.Manual;
            _director.Play();
            // float currentTime = 0;
            while (_director.playableGraph.IsValid() && _director.playableGraph.IsPlaying())
            {
                _director.playableGraph.GetRootPlayable(0).SetSpeed(speed);
                // currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
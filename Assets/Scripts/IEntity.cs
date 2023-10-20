using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    public interface IEntity
    {
        State CurrentState { set; get; }
        public float Facing { get; }
        public PhysicEntity Physic { get; }
        public GravitationalEntity Gravitation { get; }
        public Animator Animator { get; }
        public SpriteRenderer Renderer { get; }
    }    
}


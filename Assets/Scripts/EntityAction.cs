using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    public abstract class EntityAction : ScriptableObject
    {
        public abstract IEnumerator Process(IEntity caster);
    }
    
}

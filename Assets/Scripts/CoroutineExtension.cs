using System.Collections;
using System.Linq;
using UnityEngine;

namespace Kultie.Extensions
{
    public static class CoroutineExtension
    {
        public static CoroutineReference Schedule(this MonoBehaviour monoBehaviour,
            IEnumerator routine)
        {
            return new CoroutineReference(monoBehaviour, monoBehaviour.StartCoroutine(routine));
        }

        public static void Stop(this MonoBehaviour monoBehaviour, IEnumerator routine)
        {
            monoBehaviour.StopCoroutine(routine);
        }

        public static IEnumerator WaitAll(this MonoBehaviour monoBehaviour, params IEnumerator[] coroutines)
        {
            return new All(monoBehaviour, coroutines);
        }

        public static IEnumerator WaitAny(this MonoBehaviour monoBehaviour, params IEnumerator[] coroutines)
        {
            return new Any(monoBehaviour, coroutines);
        }


        private class All : WaitBase
        {
            public override bool keepWaiting => _wait.Any(t => t);

            public All(MonoBehaviour monoBehaviour, params IEnumerator[] coroutines) : base(monoBehaviour, coroutines)
            {
            }
        }

        private class Any : WaitBase
        {
            public override bool keepWaiting => _wait.All(t => t);

            public Any(MonoBehaviour monoBehaviour, params IEnumerator[] coroutines) : base(monoBehaviour, coroutines)
            {
            }
        }

        private abstract class WaitBase : CustomYieldInstruction
        {
            protected readonly bool[] _wait;

            protected WaitBase(MonoBehaviour monoBehaviour, params IEnumerator[] coroutines)
            {
                _wait = new bool[coroutines.Length];
                for (int i = 0; i < coroutines.Length; i++)
                {
                    monoBehaviour.StartCoroutine(Wrapper(coroutines[i], i));
                }
            }

            private IEnumerator Wrapper(IEnumerator e, int index)
            {
                while (true)
                {
                    if (e != null && e.MoveNext())
                    {
                        _wait[index] = true;
                        yield return e.Current;
                    }
                    else
                    {
                        _wait[index] = false;
                        break;
                    }
                }
            }
        }
    }

    public struct CoroutineReference
    {
        public Coroutine Coroutine { get; private set; }

        public MonoBehaviour MonoBehaviour { get; private set; }

        public CoroutineReference(MonoBehaviour monoBehaviour, Coroutine coroutine)
        {
            MonoBehaviour = monoBehaviour;
            Coroutine = coroutine;
        }

        public void Stop()
        {
            if (Coroutine == null || !MonoBehaviour)
                return;
            MonoBehaviour.StopCoroutine(this.Coroutine);
            Coroutine = null;
        }

        public void Clear() => Coroutine = null;
    }
}
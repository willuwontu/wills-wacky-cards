using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WillsWackyCards.MonoBehaviours
{
    public abstract class Hooked_Mono : MonoBehaviour
    {
        public virtual void OnRoundStart()
        {

        }
        public virtual void OnRoundEnd()
        {

        }
        public virtual void OnPointStart()
        {

        }
        public virtual void OnPointEnd()
        {

        }
        public virtual void OnGameStart()
        {

        }
        public virtual void OnGameEnd()
        {

        }
        public virtual void OnBattleStart()
        {

        }
        public virtual void OnPickStart()
        {

        }
        public virtual void OnPickEnd()
        {

        }
        public virtual void OnPlayerPickStart()
        {

        }
        public virtual void OnPlayerPickEnd()
        {

        }
    }
    public class HookedMonoManager : MonoBehaviour
    {
        internal List<Hooked_Mono> hookedMonos = new List<Hooked_Mono>();

        public static HookedMonoManager instance;

        private void Start()
        {
            instance = this;
        }
    }
}

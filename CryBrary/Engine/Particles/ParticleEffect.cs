﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CryEngine.Engine.Particles.Native;

namespace CryEngine
{
    public class ParticleEffect
    {
        internal ParticleEffect(IntPtr ptr)
        {
#if !(RELEASE && RELEASE_DISABLE_CHECKS)
            if (ptr == IntPtr.Zero)
                throw new NullPointerException();
#endif

            Handle = ptr;
        }

        internal IntPtr Handle { get; set; }

        #region Statics
        /// <summary>
        /// </summary>
        /// <param name="effectName"></param>
        /// <param name="loadResources">Load all required resources?</param>
        /// <returns>The specified particle effect, or null if failed.</returns>
        public static ParticleEffect Get(string effectName, bool loadResources = true)
        {
            return TryGet(NativeParticleEffectMethods.FindEffect(effectName, loadResources));
        }

        private static ParticleEffect TryGet(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                var particleEffect = ParticleEffects.FirstOrDefault(x => x.Handle == handle);
                if (particleEffect == null)
                {
                    particleEffect = new ParticleEffect(handle);
                    ParticleEffects.Add(particleEffect);
                }

                return particleEffect;
            }

            return null;
        }

        private static List<ParticleEffect> ParticleEffects = new List<ParticleEffect>();
        #endregion

        /// <summary>
        /// Spawns this effect
        /// </summary>
        /// <param name="independent"></param>
        /// <param name="pos">World location to place emitter at.</param>
        /// <param name="dir">World rotation of emitter, set to Vec3.Up if null.</param>
        /// <param name="scale">Scale of the emitter.</param>
        public void Spawn(Vec3 pos, Vec3? dir = null, float scale = 1f, bool independent = true)
        {
            NativeParticleEffectMethods.Spawn(Handle, independent, pos, dir ?? Vec3.Up, scale);
        }

        public void Remove()
        {
            NativeParticleEffectMethods.Remove(Handle);
        }

        public void LoadResources()
        {
            NativeParticleEffectMethods.LoadResoruces(Handle);
        }

        public ParticleEffect GetChild(int index)
        {
            var childHandle = NativeParticleEffectMethods.GetChild(Handle, index);

            return TryGet(childHandle);
        }

        /// <summary>
        /// Gets the number of sub-particles assigned to this effect.
        /// </summary>
        public int ChildCount { get { return NativeParticleEffectMethods.GetChildCount(Handle); } }

        public string Name { get { return NativeParticleEffectMethods.GetName(Handle); } }
        public string FullName { get { return NativeParticleEffectMethods.GetFullName(Handle); } }

        public bool Enabled { get { return NativeParticleEffectMethods.IsEnabled(Handle); } set { NativeParticleEffectMethods.Enable(Handle, value); } }

        #region Operator overloads
        /// <summary>
        /// Gets sub-effect by index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public ParticleEffect this[int i]
        {
            get { return GetChild(i); }
        }

        /// <summary>
        /// Gets sub-effect by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ParticleEffect this[string name]
        {
            get 
            {
                for (var i = 0; i < ChildCount; i++)
                {
                    var child =  GetChild(i);
                    if (child.Name == name)
                        return child;
                }

                return null;
            }
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj != null && obj is ParticleEffect)
                return this == obj;

            return false;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;

                hash = hash * 29 + Handle.GetHashCode();

                return hash;
            }
        }
        #endregion
    }
}
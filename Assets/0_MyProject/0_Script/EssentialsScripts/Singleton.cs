using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace WeplayVR.Base
{
    public class Singleton<T> where T : class
    {
        protected static volatile T s_tInstance = null;

        class LockObject
        {
            public bool IsInstanceBeingCreated { get; set; }
        };

        //this is the lock object for thread safety. Refer MSDN Singleton implementation
        private static LockObject s_lockobj = new LockObject();

        //Empty Constructor.. refer MSDN Singleton implementation
        static Singleton() { }

        public static bool IsInstanceBeingCreated()
        {
            return s_lockobj.IsInstanceBeingCreated;
        }

        //Getter for the Instance. This function should be thread safe and must check if the instance has been created

        public static T Instance
        {
            get
            {
                lock (s_lockobj)
                {
                    if (s_tInstance == null) // instance not yet created
                    {
                        //create the instance.
                        Type t = typeof(T);
                        Debug.Assert(!s_lockobj.IsInstanceBeingCreated, "Invalid recursive call to " + t.Name + ".Instance");

                        // Ensure there are no PUBLIC constructors...
                        ConstructorInfo[] ctors = t.GetConstructors();
                        if (ctors.Length > 0)
                        {
                            throw new InvalidOperationException(String.Format("{0} has at least one accesible ctor making it impossible to enforce singleton behaviour", t.Name));
                        }

                        // Create an instance via the private constructor
                        s_lockobj.IsInstanceBeingCreated = true;
                        s_tInstance = (T)Activator.CreateInstance(t, true);
                        s_lockobj.IsInstanceBeingCreated = false;

                        //If the singleton derives from Interfaces that have Listeners for the Instance created inform them.
                        ISingletonCreatedListener cbSingleCreated = s_tInstance as ISingletonCreatedListener;
                        if (cbSingleCreated != null)
                            cbSingleCreated.OnInstanceCreated();

                        //similiarily if any other listeners need to be informed do that here
                    }
                }

                return s_tInstance;
            }
        }

        //Just by calling this function on the Instance we are forcing the Instance to get created through the getter function
        public virtual void Initialize()
        {
        }
    }

    /// <summary>
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class MBSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                     "' already destroyed on application quit." +
                                     " Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        Type t = typeof(T);

                        _instance = (T)FindObjectOfType(t);

                        /*
                        // Ensure there are no PUBLIC constructors...
                        ConstructorInfo[] ctors = t.GetConstructors();
                        if (ctors.Length > 0)
                        {
                            throw new InvalidOperationException(String.Format("{0} has at least one accesible ctor making it impossible to enforce singleton behaviour", t.Name));
                        }
                        */

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                           " - there should never be more than 1 singleton!" +
                                           " Reopenning the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);

                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                      " is needed in the scene, so '" + singleton +
                                      "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                      _instance.gameObject.name);
                        }

                        ISingletonCreatedListener cbSingleCreated = _instance as ISingletonCreatedListener;
                        if (cbSingleCreated != null)
                            cbSingleCreated.OnInstanceCreated();
                    }

                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;
        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public virtual void OnDestroy()
        {
            applicationIsQuitting = true;
        }

        //Just by calling this function on the Instance we are forcing the Instance to get created through the getter function
        public virtual void Initialize()
        {
        }
    }

}
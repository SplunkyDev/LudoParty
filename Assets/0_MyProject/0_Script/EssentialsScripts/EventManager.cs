using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace WeplayVR.Base
{
   
    /// <summary>
    /// A singleton class that has the holds the delegate dictionary for events callback.
    /// It can be extended for any event. 
    /// Classes can trigger events and any other class that has registered for the event will be notified (delegate will be called)
    /// </summary>
    public class EventManager : MBSingleton<EventManager>, ISingletonCreatedListener
    {

		private bool m_bEnableLogs = false;
		public bool EnableLogs
		{
			get
			{
				return m_bEnableLogs;
			}
			set
			{
				m_bEnableLogs = value;
			}
		}

        /// <summary>
        /// Default Constructor. Initializes the dictionary
        /// </summary>
        private EventManager()
        {

        }

        /// <summary>
        /// Initializes the dictionary as soon as the instance is created
        /// </summary>
        public void OnInstanceCreated()
        {
            
            m_lstEventRegistry = new Dictionary<Type, AppEventDelegate>();

			if(m_bEnableLogs)
				Debug.Log("[EventManager] Instance Create");
        }

        /*
        public void OnInstanceCreated()
        {
            Console.WriteLine("Event Manager Instance created");
        }
        */

        /// <summary>
        /// Delegate definition for events
        /// </summary>
        /// <param name="args"></param>
        public delegate void AppEventDelegate(IEventBase a_refEventBase);

        // Dictionary<eAppEvents, AppEventDelegate> m_dicEventRegistry;
        Dictionary<Type, AppEventDelegate> m_lstEventRegistry;

        /// <summary>
        /// Registers a callback delegate to the specified event
        /// </summary>
        /// <param name="a_eEvent"> Enumerated event name</param>
        /// <param name="a_delListener"> Callback that is to be called when the event is triggered </param>
        public void RegisterEvent<T>(AppEventDelegate a_delListener) where T : IEventBase
        {

            Type t = typeof(T);
            if (!m_lstEventRegistry.ContainsKey(t))
            {
                m_lstEventRegistry.Add(typeof(T), a_delListener);
                return;
            }

            //AppEventDelegate exsisting = m_lstEventRegistry[t];
            m_lstEventRegistry[t] += a_delListener;
			if (m_bEnableLogs)
				Debug.Log("[EventManager] Register Event " + t.ToString());
        }

        /// <summary>
        /// deregisters the event delegate from the dictionary.
        /// The delegate will not be called when the event is fired after this call
        /// </summary>
        /// <param name="a_eEvent">  Enumerated event name </param>
        /// <param name="a_delListener">Callback that is to be called when the event is triggered</param>
        public void DeRegisterEvent<T>(AppEventDelegate a_delListener) where T : IEventBase
        {
            Type t = typeof(T);
            if (!m_lstEventRegistry.ContainsKey(t))
                return;

            m_lstEventRegistry[t] -= a_delListener;
			if (m_bEnableLogs)
				Debug.Log("[EventManager] DeRegister Event " + t.ToString());
        }

        /// <summary>
        /// Triggers the event causing the delegates to be called
        /// </summary>
        /// <param name="a_eEvent"> Enumerated event name</param>
        /// <param name="args">array of arguments that are passed to the delegates</param>
        public void TriggerEvent<T>(IEventBase a_refEvent) where T : IEventBase
        {
            Type t = typeof(T);
			if (m_bEnableLogs)
				Debug.Log("[EventManager] trigger Event " + t.ToString());
            if (m_lstEventRegistry.ContainsKey(t))
            {
                AppEventDelegate d = m_lstEventRegistry[t];
                if (d != null)
                {
                    d(a_refEvent);
					if (m_bEnableLogs)
						Debug.Log("[EventManager] trigger Event " + t.ToString());
                }
                else
                {
                    //This an error. 
                    Console.WriteLine("Could not trigger event: ");
                }
            }

        }


    }


}

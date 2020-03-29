using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

namespace GameUtility.Base
{


    public interface ISingletonCreatedListener
    {
        void OnInstanceCreated();
    }

    public interface IEventBase
    {

    }

    public interface IInputEvent : IEventBase
    {

    }

    public interface IEventListener
    {
        void RegisterForEvents();
        void DeRegisterForEvents();
    }


}


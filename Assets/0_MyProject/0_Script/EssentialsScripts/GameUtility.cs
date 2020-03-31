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

	public enum PathTileType
	{
		None =0,
		BlueStart =1,
		BlueEnd = 2,
		YellowStart =3,
		YellowEnd = 4,
		RedStart =5,
		RedEnd =6,
		GreenStart =7,
		GreenEnd =8

	}
}


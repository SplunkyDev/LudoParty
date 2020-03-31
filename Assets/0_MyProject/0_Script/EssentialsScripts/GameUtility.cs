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

	public enum ePlayerTurn
	{
		None =0,
		Blue =1,
		Yellow =2,
		red =3,
		Green =4
	}

	public enum ePathTileType
	{
		None =0,
		BlueStart =1,
		BlueEnd = 2,
		YellowStart =3,
		YellowEnd = 4,
		RedStart =5,
		RedEnd =6,
		GreenStart =7,
		GreenEnd =8,
		BlueSafePath =9,
		BlueSafeZone =10,
		YellowSafePath =11,
		YellowSafeZone =12,
		RedSafePath =13,
		RedSafeZone =14,
		GreenSafePath =15,
		GreenSafeZone =16,
		Special =17
	}

	public enum eTokenState
	{
		House =0,
		InRoute =1,
		InHideOut =2,
		InStairwayToHeaven =3,
		InHeaven =4
	}

	public enum eSafePathType
	{
		None =0,
		Blue =1,
		Yellow =2,
		Red =3,
		Green =4
	}

	public enum eTokenType
	{
		None =0,
		Blue =1,
		Yellow =2,
		Red =3,
		Green =4
	}


}


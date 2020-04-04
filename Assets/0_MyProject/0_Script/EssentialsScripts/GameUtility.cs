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

	public enum ePlayerState
	{
		PlayerRollDice =0,
		PlayerMoveToken =1
	}

	public enum ePlayerTurn
	{
		PlayerOne = 0,
		PlayerTwo = 1,
		PlayerThree = 2,
		PlayerFour = 3
	}

	public enum ePlayerToken
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
		InHeaven =4,
		EntryToStairway =5
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

	public enum eScaleType
	{
		None =0,
		TokenType = 1,
		SharedTile =2
	}


	public class PlayerData
	{
		public ePlayerTurn m_enumPlayerTurn;
		public ePlayerToken m_enumPlayerToken;
		public ePlayerState m_ePlayerState;
		public bool m_bPlayAgain;
		public int m_iRollSixIn;
		public bool m_gameComplete;
	}


	public enum eGameState
	{
		None = 0,
		Initialize = 1,
		LevelSelection = 2,
		InGame = 3,
		GameComplete = 4,
		Menu = 6,
		WaitingForOpponent = 7
	}

	public enum eGameUIState
	{
		None = 0,
		InitializeUI = 1,
		MenuUI = 2,
		InGameUI = 3,
		GameComplete = 4,
	}

	public enum eUITweenMoveState
	{
		None = 0,
		Horizontal_Left = 1,
		Horizontal_Right = 2,
		Vertical_Up = 3,
		vertical_Down = 4
	}

	public enum eMessageType
	{
		None = 0,
		PlayerTurn = 1,
		StartAcknowledgement = 2,
		GameEnded =3
	}
}


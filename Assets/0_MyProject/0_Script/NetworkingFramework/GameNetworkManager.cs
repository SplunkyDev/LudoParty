﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

public class GameNetworkManager : MBSingleton<GameNetworkManager>
{
	protected GameNetworkManager() { }

	private eGameState enum_GameState;
	public eGameState GameState
	{
		get
		{
			return enum_GameState;
		}

		set
		{
			enum_GameState = value;
		}
	}


	private ePlayerTurn m_enumPlayerTurn;
	public ePlayerTurn EnumPlayerTurn { get => m_enumPlayerTurn;}

	protected List<eMessageType> m_lstMessageType;

	private void RegisterEvents()
	{

		EventManager.Instance.RegisterEvent<EventOpponentLeftRoom>(OpponentLeftRoom);
	}

	private void DeregisterEvents()
	{
		if (EventManager.Instance == null)
			return;
		EventManager.Instance.DeRegisterEvent<EventOpponentLeftRoom>(OpponentLeftRoom);

	}

	private void OnEnable()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		RegisterEvents();
	}

	private void OnDisable()
	{
		DeregisterEvents();
	}

	private void Awake()
	{
		//Enable EventManager Debug Logs
		if(EventManager.Instance)
			EventManager.Instance.EnableLogs = false;	
	}

	private void Start()
    {
		m_lstMessageType = new List<eMessageType>();

	}


    private void Update()
    {

	}



	public void SendAcknowledgement()
	{
		m_lstMessageType.Clear();
		m_lstMessageType.Add(eMessageType.StartAcknowledgement);

		if (EssentialDataManager.Instance.BOnlineMode)
		{
			EventManager.Instance.TriggerEvent<EventInsertInGameMessage>(new EventInsertInGameMessage(m_lstMessageType.ToArray()));
		}
	}


	private void InitializeGame()
	{
		Debug.Log("[GameNetworkManager] InitializeGame");
		EventManager.Instance.TriggerEvent<EventStartGameSession>(new EventStartGameSession());
	}

	public void PlayerGoalScored()
	{
		
	}


	//This event is triggered when an opponent player has left/disconnected from room
	private void OpponentLeftRoom(IEventBase a_Event)
	{
		EventOpponentLeftRoom data = a_Event as EventOpponentLeftRoom;
		if(data == null)
		{
			Debug.LogError("[GameNetworkManager] EventOpponentLeftRoom Error");
			return;
		}

		//TODO: HANDLE OPPONENT LEFT

	}



	//TODO: Think about how to manage sleep 
	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{

		}
		else
		{

		}
	
	}


}

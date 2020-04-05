using System.Collections;
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
		EventManager.Instance.RegisterEvent<EventFirstPlayerEntered>(GenerateFirstPlayer);
		EventManager.Instance.RegisterEvent<EventGenerateNextPlayer>(GenerateNextPlayer);
	}

	private void DeregisterEvents()
	{
		if (EventManager.Instance == null)
			return;
		EventManager.Instance.DeRegisterEvent<EventOpponentLeftRoom>(OpponentLeftRoom);
		EventManager.Instance.DeRegisterEvent<EventFirstPlayerEntered>(GenerateFirstPlayer);
		EventManager.Instance.DeRegisterEvent<EventGenerateNextPlayer>(GenerateNextPlayer);

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


	public void InitializeGame()
	{
		Debug.Log("[GameNetworkManager] InitializeGame");
		if (GameManager.Instance.EnumMyPlayerTurn == ePlayerTurn.PlayerOne)
		{
			m_lstMessageType.Clear();
			m_lstMessageType.Add(eMessageType.GameStart);
			EventManager.Instance.TriggerEvent<EventInsertInGameMessage>(new EventInsertInGameMessage(m_lstMessageType.ToArray()));
		}
		EventManager.Instance.TriggerEvent<EventStartGameSession>(new EventStartGameSession());
	}

	public void PlayerGoalScored()
	{
		
	}

	private void GenerateFirstPlayer(IEventBase a_Event)
	{
		EventFirstPlayerEntered data = a_Event as EventFirstPlayerEntered;
		if (data == null)
		{
			Debug.LogError("[GameNetworkManager] EventFirstPlayerEntered Error");
			return;
		}

	
		PlayerData playerData = new PlayerData();
		playerData.m_enumPlayerTurn = ePlayerTurn.PlayerOne;
		playerData.m_enumPlayerToken = ePlayerToken.Blue;
		playerData.m_strUserName = data.StrUsername;

		GameManager.Instance.SetPlayerData(playerData);

		EventManager.Instance.TriggerEvent<EventDevicePlayerTurn>(new EventDevicePlayerTurn(playerData.m_enumPlayerTurn));
		Debug.Log("[GameNetworkManager] Player One Generated");
	}

	private void GenerateNextPlayer(IEventBase a_Event)
	{
		EventGenerateNextPlayer data = a_Event as EventGenerateNextPlayer;
		if (data == null)
		{
			Debug.LogError("[GameNetworkManager] EventGenerateNextPlayer Error");
			return;
		}

		Debug.Log("[GameNetworkManager] Generated Next Player");
		GameManager.Instance.UpdatePlayersInGame(data.StrUsername);

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

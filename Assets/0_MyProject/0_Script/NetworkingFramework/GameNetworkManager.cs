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

	private int m_iRandomSeed = 0;
	public int GameRandomSeed
	{
		get
		{
			return m_iRandomSeed;
		}

		set
		{
			m_iRandomSeed = value;
		}
	}

	private float  m_fPlayerScore = 0f, m_fOpponentScore = 0f;
	public float PlayerScore
	{
		get
		{
			return m_fPlayerScore;
		}

        set
        {
            m_fPlayerScore = value;
        }
	}

	public float OpponentScore
	{
		get
		{
			return m_fOpponentScore;
		}
	}

	public float m_fRematchWaitTime;
	protected float m_fRemainingRematchTime;
	public float RemainingRematchSession
	{
		get
		{
			return m_fRemainingRematchTime;
		}
	}

	private bool m_bPlayerWon = false, m_bRematch = false;
	public bool RematchResponse
	{
		get
		{
			return m_bRematch;
		}

		set
		{
			m_bRematch = value;
		}
	}

	private bool m_bOpponentFinishedGame;
	public bool BOpponentFinishedGame
	{
		get
		{
			return m_bOpponentFinishedGame;
		}

	}

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

		//TODO: This will start the network feature, call by game when needed
		//InitializeNetworkOrOffline();
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
		m_fRemainingRematchTime = m_fRematchWaitTime;
		m_bOpponentFinishedGame = false;
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

using System.Collections;
using System.Collections.Generic;
using GameUtility.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemRandom = System.Random;

public class GameManager : MBSingleton<GameManager>
{

	private const int MAXTRY = 3;

	private PlayerData m_RefCurrentPlayer;
	public PlayerData CurrentPlayer { get => m_RefCurrentPlayer; }

	private int m_iCurrentDiceValue = 0, m_iTotalRolls = MAXTRY;
	public int ICurrentDiceValue { get => m_iCurrentDiceValue;}

	private List<PlayerData> m_lstPlayerData = new List<PlayerData>();

	private eGameState m_enumGameState;
	public eGameState EnumGameState { get => m_enumGameState; set => m_enumGameState = value; }

	//This is the Player Turn state for current Device if online mode
	private ePlayerTurn m_enumMyPlayerTurn = ePlayerTurn.PlayerOne;
	public ePlayerTurn EnumMyPlayerTurn { get => m_enumMyPlayerTurn;}

	private ePlayerTurn m_enumCurrentPlayerTurn = ePlayerTurn.PlayerOne;
	//USed to check if the player turn has changed
	private ePlayerTurn m_enumPlayerTurn = ePlayerTurn.PlayerOne;
	public ePlayerTurn EnumPlayerTurn { get => m_enumPlayerTurn; }

	//This holds the current player index of the PlayerData List
	private int m_iCurrentPlayerIndex = 0;
	private List<PlayerData> m_lstPlayerCompleted = new List<PlayerData>();
	private bool m_bOnlineMultiplayer;
	public bool BOnlineMultiplayer { get => m_bOnlineMultiplayer; set => m_bOnlineMultiplayer = value; }

	public ePlayerToken EnumPlayerToken
	{
		get
		{
			for (int i = 0; i < m_lstPlayerData.Count; i++)
			{
				if (m_lstPlayerData[i].m_enumPlayerTurn == m_enumCurrentPlayerTurn)
				{
					return m_lstPlayerData[i].m_enumPlayerToken;
				}				
			}
			return ePlayerToken.None;
		}
	}

	private List<eMessageType> m_lstMessageType = new List<eMessageType>();
	private int m_iRandSeed;
	public int IRandomSeed { get => m_iRandSeed; }
	private SystemRandom m_gameRandom;
	private const int IMINVALUE = 3, IMAXVALUE = 10;
	private Coroutine m_coConnectiontimeout;

	void RegisterToEVents()
	{
		EventManager.Instance.RegisterEvent<EventPlayerFinished>(PlayerFinishedGame);
		EventManager.Instance.RegisterEvent<EventDevicePlayerTurn>(SetDevicePlayerTurn);
		EventManager.Instance.RegisterEvent<EventOpponentDiceRoll>(DiceRollFromOpponent);
		EventManager.Instance.RegisterEvent<EventStartGameSession>(StartOnlineGame);
		EventManager.Instance.RegisterEvent<EventFirstPlayerEntered>(GenerateRandomSeed);
		EventManager.Instance.RegisterEvent<EventSetRandomSeedGotFromNetwork>(SetRandomSeedGotFromNetwork);
	}

	void DeregisterToEvents()
	{
		if (EventManager.Instance == null) return;

		EventManager.Instance.DeRegisterEvent<EventPlayerFinished>(PlayerFinishedGame);
		EventManager.Instance.DeRegisterEvent<EventDevicePlayerTurn>(SetDevicePlayerTurn);
		EventManager.Instance.DeRegisterEvent<EventOpponentDiceRoll>(DiceRollFromOpponent);
		EventManager.Instance.DeRegisterEvent<EventStartGameSession>(StartOnlineGame);
		EventManager.Instance.DeRegisterEvent<EventFirstPlayerEntered>(GenerateRandomSeed);
		EventManager.Instance.DeRegisterEvent<EventSetRandomSeedGotFromNetwork>(SetRandomSeedGotFromNetwork);
	}


	private void OnEnable()
	{
		RegisterToEVents();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		DeregisterToEvents();
		SceneManager.sceneLoaded -= OnSceneLoaded;	
	}


	private void OnSceneLoaded(Scene a_scene, LoadSceneMode a_loadMode)
	{
		switch(a_scene.buildIndex)
		{
			case 0:
				Debug.Log("[GameManager] Menu Scene Loaded");

				if(BOnlineMultiplayer)
				{
					WarpNetworkManager.Instance.ConnectionEstablished = false;					
					m_bOnlineMultiplayer = false;
				}
				EventManager.Instance.TriggerEvent<EventShowMenuUI>(new EventShowMenuUI(true, eGameState.Menu));
				break;
			case 1:
				Debug.Log("[GameManager] Game Scene Loaded");
			

				if(!BOnlineMultiplayer)
				{
					for (int i = 0; i < 2; i++)
					{
						PlayerData playerData = new PlayerData();
						playerData.m_enumPlayerTurn = (ePlayerTurn)i;
						playerData.m_enumPlayerToken = (ePlayerToken)(i + 1);

						SetPlayerData(playerData);
						playerData = null;
					}

					EventManager.Instance.TriggerEvent<EventShowInGameUI>(new EventShowInGameUI(true, eGameState.InGame));
					StartCoroutine(InitializeGame(1f));
				}
				else
				{
					EventManager.Instance.TriggerEvent<EventInitializeNetworkApi>(new EventInitializeNetworkApi());
					EventManager.Instance.TriggerEvent<EventShowWaitingForPlayersUI>(new EventShowWaitingForPlayersUI(true,eGameState.WaitingForOpponent));

					m_coConnectiontimeout = StartCoroutine(ConnectionTimeout(10));
				}
				
				break;
		}
	}

	private IEnumerator ConnectionTimeout(float a_fDelay)
	{
		yield return new WaitForSeconds(a_fDelay);
		EventManager.Instance.TriggerEvent<EventDisonnectFromServer>(new EventDisonnectFromServer());
		EventManager.Instance.TriggerEvent<EventShowWaitingForPlayersUI>(new EventShowWaitingForPlayersUI(false,eGameState.None));
		EventManager.Instance.TriggerEvent<EventErrorInConnectionMessage>(new EventErrorInConnectionMessage("Connection Timed Out (15 sec waiting), Please try again."));
		EventManager.Instance.TriggerEvent<EventShowConnectionErrorUI>(new EventShowConnectionErrorUI(true,eGameState.ErrorInConnection));

	}

	private void StartOnlineGame(IEventBase a_Event)
	{
		EventStartGameSession data = a_Event as EventStartGameSession;
		if(data == null)
		{
			Debug.LogError("[GameManager] EventStartGameSession is null");
			return;
		}

		//Stopping the time out coroutine if connection has been established
		if(m_coConnectiontimeout != null)
		{
			StopCoroutine(m_coConnectiontimeout);
			m_coConnectiontimeout = null;
		}

		EventManager.Instance.TriggerEvent<EventShowWaitingForPlayersUI>(new EventShowWaitingForPlayersUI(false, eGameState.None));
		EventManager.Instance.TriggerEvent<EventShowInGameUI>(new EventShowInGameUI(true, eGameState.InGame));
		StartCoroutine(InitializeGame(1f));

	}

	private void SetDevicePlayerTurn(IEventBase a_Event)
	{
		EventDevicePlayerTurn data = a_Event as EventDevicePlayerTurn;
		if(data == null)
		{
			Debug.LogError("[GameManager][SetDevicePlayerTurn] Device Player turn trigger null");
			return;
		}

		m_enumMyPlayerTurn = data.EnumPlayerTurn;
	}

	private void GenerateRandomSeed(IEventBase a_Event)
	{
		EventFirstPlayerEntered data = a_Event as EventFirstPlayerEntered;
		if (data == null)
		{
			Debug.LogError("[GameManager] EventFirstPlayerEntered null");
			return;
		}

		m_iRandSeed = Random.Range(6, 18);
		m_gameRandom = new SystemRandom(m_iRandSeed);
		Debug.Log("Random Seed: " + m_iRandSeed);
		
	}

	private void SetRandomSeedGotFromNetwork(IEventBase a_Event)
	{
		EventSetRandomSeedGotFromNetwork data = a_Event as EventSetRandomSeedGotFromNetwork;
		if (data == null)
		{
			Debug.LogError("[GameManager] EventSetRandomSeedGotFromNetwork null");
			return;
		}

		m_iRandSeed = data.IRandomSeed;
		m_gameRandom = new SystemRandom(m_iRandSeed);
		Debug.Log("Random Seed set from network: " + m_iRandSeed);
	}

	public void SetPlayerData(PlayerData a_PlayerData)
	{
		//Setting the value of how many turns until the player gets a forced six if not got until then
		a_PlayerData.m_iRollSixIn = Random.Range(3, 10); ;

		Debug.Log("[GameManager] PlayerData PlayerTurn: "+a_PlayerData.m_enumPlayerTurn);
		Debug.Log("[GameManager] PlayerData PlayerToken: "+a_PlayerData.m_enumPlayerToken);
		Debug.Log("[GameManager] PlayerData PlayerToken: "+a_PlayerData.m_iRollSixIn);
		m_lstPlayerData.Add(a_PlayerData);
	}

	//Giving player turn and token base on the order of connecting to game
	public void UpdatePlayersInGame(string a_strUsername)
	{

		if (m_enumMyPlayerTurn != ePlayerTurn.PlayerOne)
			return;

		PlayerData playerData = new PlayerData();
		//Create Next Player
		switch (m_lstPlayerData[(m_lstPlayerData.Count - 1)].m_enumPlayerTurn)
		{
			case ePlayerTurn.PlayerOne:
				playerData.m_enumPlayerTurn = ePlayerTurn.PlayerTwo;
				playerData.m_enumPlayerToken = ePlayerToken.Yellow;
				playerData.m_strUserName = a_strUsername;
				SetPlayerData(playerData);
				break;
			case ePlayerTurn.PlayerTwo:
				playerData.m_enumPlayerTurn = ePlayerTurn.PlayerThree;
				playerData.m_enumPlayerToken = ePlayerToken.red;
				playerData.m_strUserName = a_strUsername;
				SetPlayerData(playerData);
				break;
			case ePlayerTurn.PlayerThree:
				playerData.m_enumPlayerTurn = ePlayerTurn.PlayerFour;
				playerData.m_enumPlayerToken = ePlayerToken.Green;
				playerData.m_strUserName = a_strUsername;
				SetPlayerData(playerData);
				break;
			case ePlayerTurn.PlayerFour:
			
				break;
		}

		m_lstMessageType.Clear();
		m_lstMessageType.Add(eMessageType.PlayerTurn);

		EventManager.Instance.TriggerEvent<EventInsertInGameMessage>(new EventInsertInGameMessage(m_lstMessageType.ToArray()));

		if(m_lstPlayerData.Count >= EssentialDataManager.Instance.MaxPlayers)
		{
			m_lstMessageType.Clear();
			m_lstMessageType.Add(eMessageType.StartAcknowledgement);
			EventManager.Instance.TriggerEvent<EventInsertInGameMessage>(new EventInsertInGameMessage(m_lstMessageType.ToArray()));
		}
	}

	public void DeserializePlayersData(string m_strPlayerData)
	{
		PlayerContainer playerContainer = JsonUtility.FromJson<PlayerContainer>(m_strPlayerData);
		m_lstPlayerData.Clear();
		m_lstPlayerData = playerContainer.LstPlayerDataContainer;

		for (int i = 0; i < m_lstPlayerData.Count; i++)
		{
			if (string.Compare(WarpNetworkManager.Instance.PlayerName, m_lstPlayerData[i].m_strUserName) == 0)
			{
				m_enumMyPlayerTurn = m_lstPlayerData[i].m_enumPlayerTurn;
			}
		}

		Debug.Log("[GameManager] My Device Player: "+m_enumMyPlayerTurn.ToString());
	}

	public bool CheckIfAllPlayersHaveBeenAccountedFor()
	{
		bool bValid = false;
		if(m_lstPlayerData.Count == EssentialDataManager.Instance.MaxPlayers)
		{
			Debug.Log("[GameManager] All Player in Game");
			bValid = true;
		}
		else
		{
			Debug.Log("[GameManager] Missing Player in Game");
		}

		return bValid;
	}

	public void SendMessageToStartGame()
	{
		m_lstMessageType.Clear();
		m_lstMessageType.Add(eMessageType.GameStart);

		EventManager.Instance.TriggerEvent<EventInsertInGameMessage>(new EventInsertInGameMessage(m_lstMessageType.ToArray()));

	}

	public string  GetAllPlayerData()
	{
		PlayerContainer playerContainer = new PlayerContainer(m_lstPlayerData);
		string strJson = JsonUtility.ToJson(playerContainer);
		Debug.Log("[GameManager] All Player serialized: "+strJson);
		return strJson;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

	private void PlayerFinishedGame(IEventBase a_Event)
	{
		EventPlayerFinished data = a_Event as EventPlayerFinished;
		if(data == null)
		{
			Debug.LogError("[GamaManager] Event PlayerFinished null");
			return;
		}

		Debug.Log("[GameManager] Processing Game Complete");
		for (int i = 0; i < m_lstPlayerData.Count; i++)
		{
			if ((int)m_lstPlayerData[i].m_enumPlayerToken == (int)data.RefTokenData.EnumTokenType)
			{
				m_lstPlayerData[i].m_gameComplete = true;
				m_lstPlayerCompleted.Add(m_lstPlayerData[i]);
				Debug.Log("[GameManager] Yes, player has completed the game");
			}
		}

		if((m_lstPlayerData.Count - m_lstPlayerCompleted.Count) == 1)
		{
			//TODO: TODO END GAME HERE
			Debug.Log("[GameManager] End the game");
			GameEnded();
		}
		
	}

	private void GameEnded()
	{
		InGameUIManager.Instance.ShowGameResults(m_lstPlayerCompleted);
	}

	private IEnumerator InitializeGame(float a_fDelay)
	{
		m_lstPlayerCompleted.Clear();

		yield return new WaitForSeconds(a_fDelay);
		EventManager.Instance.TriggerEvent<EventHighlightCurrentPlayer>(new EventHighlightCurrentPlayer(EnumPlayerToken));
		//Get Reference to PlayerData, dont need to loop through players when ever update in data is required
		CurrentPlayerData();
		CurrentPlayer.m_ePlayerState = ePlayerState.PlayerRollDice;
		//This is used to check whether the palyer get 6 continuously 3 times
		m_iTotalRolls = MAXTRY;
		InGameUIManager.Instance.Instructions("Roll Dice");
	}


	public void LoadToGame(int a_iBuildIndex)
	{
		//TODO: Load Game and start the game session
		switch (a_iBuildIndex)
		{
			case 0:
				SceneManager.LoadScene(a_iBuildIndex, LoadSceneMode.Single);
				break;
			case 1:
				SceneManager.LoadScene(a_iBuildIndex, LoadSceneMode.Single);
				break;
		}

		
	}

	private void DiceRollFromOpponent(IEventBase a_Event)
	{
		EventOpponentDiceRoll data = a_Event as EventOpponentDiceRoll;
		if (data == null)
		{
			Debug.LogError("[GamaManager] Event Oppoennt dice roll null");
			return;
		}

		m_iCurrentDiceValue = data.IDiceRoll;
	}

	public void RollTheDice()
	{
		//Let's Roll a D20, ;-P
		m_iCurrentDiceValue = UnityEngine.Random.Range(1, 7);
		m_iCurrentDiceValue = m_iCurrentDiceValue > 6?6:m_iCurrentDiceValue;

		if (m_bOnlineMultiplayer)
		{
			m_lstMessageType.Clear();
			m_lstMessageType.Add(eMessageType.PlayerDiceRoll);
			EventManager.Instance.TriggerEvent<EventInsertInGameMessage>(new EventInsertInGameMessage(m_lstMessageType.ToArray()));
		}
	}

	public void CheckResult()
	{
		UpdateBaseOnGameRules();
	}

	public void CheckPlayerChangeCondtion()
	{
		if(!m_RefCurrentPlayer.m_bPlayAgain || m_RefCurrentPlayer.m_gameComplete)
		{
			ChangePlayerTurn();
		}
		else
		{
			CurrentPlayer.m_ePlayerState = ePlayerState.PlayerRollDice;
		}

		switch (CurrentPlayer.m_ePlayerState)
		{
			case ePlayerState.PlayerRollDice:
				InGameUIManager.Instance.Instructions("Roll Dice");
				break;
			case ePlayerState.PlayerMoveToken:
				InGameUIManager.Instance.Instructions("Select Token");
				break;
		}
	}

	private void UpdateBaseOnGameRules()
	{
		Debug.Log("[GameManager][CheckPlayerPlayCondition]");
		Debug.Log("[GameManager][CheckPlayerPlayCondition] Total Rolls: " + m_iTotalRolls);
	
		//Checking how many 6 dice value has been got by player
		if (m_iCurrentDiceValue == 6)
		{
			CurrentPlayer.m_bPlayAgain = true;

			m_iTotalRolls--;
			Debug.Log("[GameManager][CheckPlayerPlayCondition] Total Rolls Remaining: " + m_iTotalRolls);
			UpdatePlayerSixPossiblity();

			
			//If the total changes of getting 6 is over or no taken can be moved
			if (m_iTotalRolls <= 0 || !TokenManager.Instance.CheckValidTokenMovement(m_iCurrentDiceValue))
			{
				CurrentPlayer.m_ePlayerState = ePlayerState.PlayerRollDice;				
				ChangePlayerTurn();
			}	
			else
			{
				CurrentPlayer.m_ePlayerState = ePlayerState.PlayerMoveToken;
			}
			Debug.Log("[GameManager][CheckPlayerPlayCondition] 6 got now");
		}
		else 
		{
			Debug.Log("[GameManager][CheckPlayerPlayCondition] Dice value not : ");
			//This is used to check whether the palyer get 6 continuously 3 times
			m_iTotalRolls = MAXTRY;
			//If the player hasnt got a 6 for a while let the player get 6
			if (CurrentPlayer.m_iRollSixIn <= 0)
			{
				m_iCurrentDiceValue = 6;
				m_iTotalRolls--;
				Debug.Log("[GameManager][CheckPlayerPlayCondition] Total Rolls Remaining: " + m_iTotalRolls);
				UpdatePlayerSixPossiblity();
				CurrentPlayer.m_bPlayAgain = true;
			}
			else
			{
				CurrentPlayer.m_bPlayAgain = false;
			}

			//If SIX has not been got by the player 
			IncreasePossiblityofGettingSix();

			if (!TokenManager.Instance.CheckValidTokenMovement(m_iCurrentDiceValue))
			{
				CurrentPlayer.m_ePlayerState = ePlayerState.PlayerRollDice;
				ChangePlayerTurn();
			}
			else
			{
				CurrentPlayer.m_ePlayerState = ePlayerState.PlayerMoveToken;
			}
			
		}
	}

	private void ChangePlayerTurn()
	{
		Debug.Log("[GameManager][ChangePlayerTurn]");	
		
		//Check if nect player has finished his game, if so go to next player who has not completed the game
		do
		{
			m_iCurrentPlayerIndex++;
			if (m_iCurrentPlayerIndex >= m_lstPlayerData.Count)
			{
				m_iCurrentPlayerIndex = 0;
			}
		}
		while (m_lstPlayerData[m_iCurrentPlayerIndex].m_gameComplete);
		m_enumCurrentPlayerTurn = m_lstPlayerData[m_iCurrentPlayerIndex].m_enumPlayerTurn;

		if (m_enumCurrentPlayerTurn != EnumPlayerTurn)
		{
			m_enumPlayerTurn = m_enumCurrentPlayerTurn;
			//Get Reference to PlayerData, dont need to loop through players when ever update in data is required
			CurrentPlayerData();
			m_iTotalRolls = MAXTRY;
		}

		CurrentPlayer.m_ePlayerState = ePlayerState.PlayerRollDice;

		Debug.Log("[GamaManager] Player Turn Updated: " + EnumPlayerTurn);
		Debug.Log("[GamaManager] Player Token Updated: " + EnumPlayerToken);
		EventManager.Instance.TriggerEvent<EventHighlightCurrentPlayer>(new EventHighlightCurrentPlayer(EnumPlayerToken));
	}



	#region Modify_PlayerData
	private void UpdatePlayerSixPossiblity()
	{
		Debug.Log("[GameManager]Updated Possiblity of getting 6");

		if(BOnlineMultiplayer)
		{			
			CurrentPlayer.m_iRollSixIn = m_gameRandom.Next(IMINVALUE, IMAXVALUE);
			Debug.Log("[GameManager]Updated Possiblity of getting 6 using random seed: "+ CurrentPlayer.m_iRollSixIn);

		}
		else
		{
			CurrentPlayer.m_iRollSixIn = Random.Range(IMINVALUE, IMAXVALUE);
			Debug.Log("[GameManager]Updated Possiblity of getting 6 locally: " + CurrentPlayer.m_iRollSixIn);
		}
		
	}

	private void IncreasePossiblityofGettingSix()
	{
		CurrentPlayer.m_iRollSixIn--;
	}

	private void CurrentPlayerData()
	{
		for (int i = 0; i < m_lstPlayerData.Count; i++)
		{
			if (m_lstPlayerData[i].m_enumPlayerTurn == m_enumCurrentPlayerTurn)
			{
				m_RefCurrentPlayer = m_lstPlayerData[i];
			}
		}
	}
	#endregion
	
}
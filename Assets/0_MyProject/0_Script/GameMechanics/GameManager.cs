using System.Collections;
using System.Collections.Generic;
using GameUtility.Base;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MBSingleton<GameManager>
{

	private const int MAXTRY = 3;

	private PlayerData m_RefCurrentPlayer;
	public PlayerData CurrentPlayer { get => m_RefCurrentPlayer; set => m_RefCurrentPlayer = value; }

	private int m_iCurrentDiceValue = 0, m_iTotalRolls = MAXTRY;
	public int ICurrentDiceValue { get => m_iCurrentDiceValue;}

	private List<PlayerData> m_lstPlayerData = new List<PlayerData>();

	private eGameState m_enumGameState;
	public eGameState EnumGameState { get => m_enumGameState; set => m_enumGameState = value; }

	private ePlayerTurn m_enumMyPlayerTurn = ePlayerTurn.PlayerOne;

	private ePlayerTurn m_enumCurrentPlayerTurn = ePlayerTurn.PlayerOne;
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


	void RegisterToEVents()
	{
		EventManager.Instance.RegisterEvent<EventPlayerFinished>(PlayerFinishedGame);
		EventManager.Instance.RegisterEvent<EventDevicePlayerTurn>(SetDevicePlayerTurn);
	}

	void DeregisterToEvents()
	{
		if (EventManager.Instance == null) return;

		EventManager.Instance.DeRegisterEvent<EventPlayerFinished>(PlayerFinishedGame);
		EventManager.Instance.DeRegisterEvent<EventDevicePlayerTurn>(SetDevicePlayerTurn);
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
					EventManager.Instance.TriggerEvent<EventDisonnectFromServer>(new EventDisonnectFromServer());
					m_bOnlineMultiplayer = false;
				}
				EventManager.Instance.TriggerEvent<EventShowMenuUI>(new EventShowMenuUI(true, eGameState.Menu));
				break;
			case 1:
				Debug.Log("[GameManager] Game Scene Loaded");
				EventManager.Instance.TriggerEvent<EventShowInGameUI>(new EventShowInGameUI(true,eGameState.InGame));

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

					StartCoroutine(InitializeGame(1f));
				}
				else
				{
					EventManager.Instance.TriggerEvent<EventInitializeNetworkApi>(new EventInitializeNetworkApi());
				}
				
				break;
		}
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
	public void SetPlayerData(PlayerData a_PlayerData)
	{
		//Setting the value of how many turns until the player gets a forced six if not got until then
		//a_PlayerData.m_iRollSixIn = Random.Range(5, 10);
		a_PlayerData.m_iRollSixIn = Random.Range(3, 10); ;

		Debug.Log("[GameManager] PlayerData PlayerTurn: "+a_PlayerData.m_enumPlayerTurn);
		Debug.Log("[GameManager] PlayerData PlayerToken: "+a_PlayerData.m_enumPlayerToken);
		Debug.Log("[GameManager] PlayerData PlayerToken: "+a_PlayerData.m_iRollSixIn);
		m_lstPlayerData.Add(a_PlayerData);
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

	public void RollTheDice()
	{
		//Let's Roll a D20, ;-P
		m_iCurrentDiceValue = UnityEngine.Random.Range(1, 7);
		m_iCurrentDiceValue = m_iCurrentDiceValue > 6?6:m_iCurrentDiceValue;
		CurrentPlayer.m_ePlayerState = ePlayerState.PlayerMoveToken;

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
				ChangePlayerTurn();
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
				ChangePlayerTurn();
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
		CurrentPlayer.m_iRollSixIn = Random.Range(3, 10);
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

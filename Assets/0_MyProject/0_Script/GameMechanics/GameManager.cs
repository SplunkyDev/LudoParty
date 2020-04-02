using System.Collections;
using System.Collections.Generic;
using GameUtility.Base;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MBSingleton<GameManager>
{

	private const int MAXTRY = 3;
	private int m_iNumberOfPlayers = 1;

	private PlayerData m_RefCurrentPlayer;
	private int m_iCurrentDiceValue = 0, m_iTotalRolls = MAXTRY;
	public int ICurrentDiceValue { get => m_iCurrentDiceValue;}

	private List<PlayerData> m_lstPlayerData = new List<PlayerData>();

	private eGameState m_enumGameState;
	public eGameState EnumGameState { get => m_enumGameState; set => m_enumGameState = value; }

	private ePlayerTurn m_enumCurrentPlayerTurn = ePlayerTurn.PlayerOne;
	private ePlayerTurn m_enumPlayerTurn = ePlayerTurn.PlayerOne;
	public ePlayerTurn EnumPlayerTurn { get => m_enumPlayerTurn; }

	public ePlayerToken EnumPlayerToken
	{
		get
		{
			for (int i = 0; i < m_lstPlayerData.Count; i++)
			{
				if (m_lstPlayerData[i].m_enumPlayerTurn == EnumPlayerTurn)
				{
					return m_lstPlayerData[i].m_enumPlayerToken;
				}				
			}
			return ePlayerToken.None;
		}
	}


	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;	
	}


	private void OnSceneLoaded(Scene a_scene, LoadSceneMode a_loadMode)
	{
		switch(a_scene.buildIndex)
		{
			case 0:
				Debug.Log("[GameManager] Menu Scene Loaded");
				break;
			case 1:
				Debug.Log("[GameManager] Game Scene Loaded");
				EventManager.Instance.TriggerEvent<EventShowInGameUI>(new EventShowInGameUI(true,eGameState.InGame));
				StartCoroutine(InitializeGame(1f));
				break;
		}
	}
	public void SetPlayerData(PlayerData a_PlayerData)
	{
		//Setting the value of how many turns until the player gets a forced six if not got until then
		a_PlayerData.m_iRollSixIn = Random.Range(5, 10);

		Debug.Log("[GameManager] PlayerData PlayerTurn: "+a_PlayerData.m_enumPlayerTurn);
		Debug.Log("[GameManager] PlayerData PlayerToken: "+a_PlayerData.m_enumPlayerToken);
		m_lstPlayerData.Add(a_PlayerData);

		m_iNumberOfPlayers = m_lstPlayerData.Count;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private IEnumerator InitializeGame(float a_fDelay)
	{
		yield return new WaitForSeconds(a_fDelay);
		EventManager.Instance.TriggerEvent<EventHighlightCurrentPlayer>(new EventHighlightCurrentPlayer(EnumPlayerToken));

		//Get Reference to PlayerData, dont need to loop through players when ever update in data is required
		CurrentPlayerData();
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
		m_iCurrentDiceValue = UnityEngine.Random.Range(1, 6);
	}

	public void CheckResult()
	{
		CheckPlayerPlayCondition();
	}

	private void CheckPlayerPlayCondition()
	{
		Debug.Log("[GameManager][CheckPlayerPlayCondition]");
		//Checking how many 6 dice value has been got by player
		if (m_iCurrentDiceValue == 6)
		{
			UpdatePlayerSixPossiblity();
			if (m_iTotalRolls <= 0)
			{
				ChangePlayerTurn();
			}
			Debug.Log("[GameManager][CheckPlayerPlayCondition] 6 got now");
		}
		else 
		{
			if (m_RefCurrentPlayer.m_iRollSixIn <= 0)
			{
				m_iCurrentDiceValue = 6;
			}

			if (!TokenManager.Instance.CheckValidTokenMovement(m_iCurrentDiceValue))
			{
				ChangePlayerTurn();
			}
			//If SIX has not been got by the player 
			IncreasePossiblityofGettingSix();
		}
	}

	private void ChangePlayerTurn()
	{
		Debug.Log("[GameManager][ChangePlayerTurn]");
		if ((int)m_enumCurrentPlayerTurn >= (m_iNumberOfPlayers-1))
		{
			m_enumPlayerTurn = 0;
		}
		else
		{
			m_enumPlayerTurn++;
		}


		if (m_enumCurrentPlayerTurn != m_enumPlayerTurn)
		{
			m_enumCurrentPlayerTurn = m_enumPlayerTurn;
			m_iTotalRolls = MAXTRY;
		}

		//Get Reference to PlayerData, dont need to loop through players when ever update in data is required
		CurrentPlayerData();
		EventManager.Instance.TriggerEvent<EventHighlightCurrentPlayer>(new EventHighlightCurrentPlayer(EnumPlayerToken));
	}



	#region Modify_PlayerData
	private void UpdatePlayerSixPossiblity()
	{
		m_RefCurrentPlayer.m_iRollSixIn = Random.Range(5, 10);
	}

	private void IncreasePossiblityofGettingSix()
	{
		m_RefCurrentPlayer.m_iRollSixIn--;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
	private static PathManager m_instance;
	public static PathManager Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = FindObjectOfType<PathManager>();
			}
			return m_instance;
		}
	}
	//This dictionary contains all the tile data
	private Dictionary<int,PathTileData> m_dicPathTileTypes = new Dictionary<int, PathTileData>();
	//This list contains the reference to the script that holds all the data of the safe bath to finish line
	[SerializeField] private List<StairwayToHeavenManager> m_lstStairwayToHeaven = new List<StairwayToHeavenManager>();
	//reference to the start tile for each of the token types
	private PathTileData m_refBlueStart, m_refYellowStart,m_refRedStart, m_refGreenStart;
	//This list is used to send it to token manager for the tween effect
	private List<Transform> m_lstTilePosition = new List<Transform>();
	private const int STEPCOUNT = 6;

	private void Awake()
	{
		if (m_instance == null)
			m_instance = this;
	}

	void Start()
    {
		int index = 0;
        foreach(Transform child in transform)
		{
			PathTileData refPathTileData = child.gameObject.GetComponent<PathTileData>();
			if(refPathTileData == null)
			{
				Debug.LogError("[PathManager] PathTileData not FOUND");
				return;
			}
			//Set the index to the tile so as to get the position from PathTileData
			refPathTileData.ITileIndex = index;
			m_dicPathTileTypes.Add(index, refPathTileData);
			switch (refPathTileData.EnumPathTileType)
			{
				case GameUtility.Base.ePathTileType.BlueStart:
					m_refBlueStart = refPathTileData;
					break;
				case GameUtility.Base.ePathTileType.YellowStart:
					m_refYellowStart = refPathTileData;
					break;
				case GameUtility.Base.ePathTileType.RedStart:
					m_refRedStart = refPathTileData;
					break;
				case GameUtility.Base.ePathTileType.GreenStart:
					m_refGreenStart = refPathTileData;
					break;
			}

			index++;
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public List<Transform> TokenStateUpdate(TokenData a_refTokenData, int a_iDiceValue)
	{
		m_lstTilePosition.Clear();

		if (a_refTokenData.ICurrentPathIndex == -1)
		{
			Debug.Log("[PathManager] Move to start Point: "+a_refTokenData.EnumTokenType.ToString());
			switch (a_refTokenData.EnumTokenType)
			{
				case GameUtility.Base.eTokenType.None:
					break;
				case GameUtility.Base.eTokenType.Blue:
					a_refTokenData.ICurrentPathIndex = m_refBlueStart.ITileIndex;
					m_lstTilePosition.Add(m_refBlueStart.gameObject.transform);
					break;
				case GameUtility.Base.eTokenType.Yellow:
					a_refTokenData.ICurrentPathIndex = m_refYellowStart.ITileIndex;
					m_lstTilePosition.Add(m_refYellowStart.gameObject.transform);
					break;
				case GameUtility.Base.eTokenType.Red:
					a_refTokenData.ICurrentPathIndex = m_refRedStart.ITileIndex;
					m_lstTilePosition.Add(m_refRedStart.gameObject.transform);
					break;
				case GameUtility.Base.eTokenType.Green:
					a_refTokenData.ICurrentPathIndex = m_refGreenStart.ITileIndex;
					m_lstTilePosition.Add(m_refGreenStart.gameObject.transform);
					break;
				default:
					break;
			}

		}
		else
		{
			if (a_refTokenData.EnumTokenState == GameUtility.Base.eTokenState.InStairwayToHeaven)
			{
				//TODO: get path to heaven
			}
			else
			{
				if (a_refTokenData.EnumTokenState == GameUtility.Base.eTokenState.InRoute || a_refTokenData.EnumTokenState == GameUtility.Base.eTokenState.InHideOut)
				{
					int iCurrentTile = (a_refTokenData.ICurrentPathIndex + 1);
					for (int i = iCurrentTile; i <= (iCurrentTile + a_iDiceValue); i++)
					{
						int pathIndex = iCurrentTile + a_iDiceValue;
						if (i > pathIndex)
						{
							//Path index ends at 51
							i -= 51;
						}
						PathTileData refPathTileData;
						m_dicPathTileTypes.TryGetValue(i, out refPathTileData);
						if (refPathTileData == null)
						{
							Debug.LogError("[PathManager] PathTileData is null: index: " + i);
						}
						a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;
						a_refTokenData.EnumTokenState = refPathTileData.EnumTokenState;
						m_lstTilePosition.Add(refPathTileData.gameObject.transform);
					}
				}
			}

			

		}

		return m_lstTilePosition;
	}


	//Checking while in stairway the heaven the dice value is valid to move to heaven
	public bool ValidateMovement(TokenData a_refTokenData, int a_iDiceValue)
	{
		bool bValid = false;

		if ((STEPCOUNT - a_refTokenData.ICurrentPathIndex) <= a_iDiceValue)
		{
			bValid = true;
		}
		return bValid;
	}
		
}

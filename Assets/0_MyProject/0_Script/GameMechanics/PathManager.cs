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

	[Header("WayPoints To Heaven (Blue,Yellow,Red,Green)")]
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

		if (a_refTokenData.EnumTokenState == GameUtility.Base.eTokenState.House)
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

			a_refTokenData.EnumTokenState = GameUtility.Base.eTokenState.InRoute;

		}
		else
		{
			//NOTE: USE DICE VALUE NOT TILE ID FOR GETTING TILE 
			if (a_refTokenData.EnumTokenState == GameUtility.Base.eTokenState.InStairwayToHeaven)
			{
				int iCurrentTile = a_refTokenData.ICurrentPathIndex;
				//Checking while in stairway the heaven the dice value is valid to move to heaven
				if ((iCurrentTile + a_iDiceValue) >= STEPCOUNT)
				{
					Debug.LogError("[PathManager] Cannot enter heaven to wealthy not humble");
					return null;
				}
				#region RetreiveWaypoint
				PathTileData refPathTileData;
				for (int i = 1; i < a_iDiceValue; i++)
				{
					iCurrentTile++;

					switch (a_refTokenData.EnumTokenType)
					{
						case GameUtility.Base.eTokenType.Blue:
							m_lstStairwayToHeaven[0].m_dicStairwayToHeaven.TryGetValue(iCurrentTile, out refPathTileData);
							if (refPathTileData == null)
							{
								Debug.LogError("[PathManager] PathTileData is null: index: " + iCurrentTile);
							}
							a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;
							m_lstTilePosition.Add(refPathTileData.gameObject.transform);
							break;
						case GameUtility.Base.eTokenType.Yellow:
							m_lstStairwayToHeaven[1].m_dicStairwayToHeaven.TryGetValue(iCurrentTile, out refPathTileData);
							if (refPathTileData == null)
							{
								Debug.LogError("[PathManager] PathTileData is null: index: " + iCurrentTile);
							}
							a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;
							m_lstTilePosition.Add(refPathTileData.gameObject.transform);
							break;
						case GameUtility.Base.eTokenType.Red:
							m_lstStairwayToHeaven[2].m_dicStairwayToHeaven.TryGetValue(iCurrentTile, out refPathTileData);
							if (refPathTileData == null)
							{
								Debug.LogError("[PathManager] PathTileData is null: index: " + iCurrentTile);
							}
							a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;
							m_lstTilePosition.Add(refPathTileData.gameObject.transform);
							break;
						case GameUtility.Base.eTokenType.Green:
							m_lstStairwayToHeaven[3].m_dicStairwayToHeaven.TryGetValue(iCurrentTile, out refPathTileData);
							if (refPathTileData == null)
							{
								Debug.LogError("[PathManager] PathTileData is null: index: " + iCurrentTile);
							}
							a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;
							m_lstTilePosition.Add(refPathTileData.gameObject.transform);
							break;
					}

				}
				#endregion
			}
			else if(a_refTokenData.EnumTokenState == GameUtility.Base.eTokenState.EntryToStairway)
			{
				int iCurrentTile = -1;
				#region RetreiveWaypoints
				PathTileData refPathTileData;			
				for (int i =1;i<a_iDiceValue;i++)
				{
					iCurrentTile++;

					switch (a_refTokenData.EnumTokenType)
					{
						case GameUtility.Base.eTokenType.Blue:
							m_lstStairwayToHeaven[0].m_dicStairwayToHeaven.TryGetValue(iCurrentTile, out refPathTileData);
							if (refPathTileData == null)
							{
								Debug.LogError("[PathManager] PathTileData is null: index: " + iCurrentTile);
							}
							a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;
							m_lstTilePosition.Add(refPathTileData.gameObject.transform);
							break;
						case GameUtility.Base.eTokenType.Yellow:
							m_lstStairwayToHeaven[1].m_dicStairwayToHeaven.TryGetValue(iCurrentTile, out refPathTileData);
							if (refPathTileData == null)
							{
								Debug.LogError("[PathManager] PathTileData is null: index: " + iCurrentTile);
							}
							a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;
							m_lstTilePosition.Add(refPathTileData.gameObject.transform);
							break;
						case GameUtility.Base.eTokenType.Red:
							m_lstStairwayToHeaven[2].m_dicStairwayToHeaven.TryGetValue(iCurrentTile, out refPathTileData);
							if (refPathTileData == null)
							{
								Debug.LogError("[PathManager] PathTileData is null: index: " + iCurrentTile);
							}
							a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;
							m_lstTilePosition.Add(refPathTileData.gameObject.transform);
							break;
						case GameUtility.Base.eTokenType.Green:
							m_lstStairwayToHeaven[3].m_dicStairwayToHeaven.TryGetValue(iCurrentTile, out refPathTileData);
							if (refPathTileData == null)
							{
								Debug.LogError("[PathManager] PathTileData is null: index: " + iCurrentTile);
							}
							a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;
							m_lstTilePosition.Add(refPathTileData.gameObject.transform);
							break;
					}
					
				}
				#endregion
			}
			else
			{
				if (a_refTokenData.EnumTokenState == GameUtility.Base.eTokenState.InRoute || a_refTokenData.EnumTokenState == GameUtility.Base.eTokenState.InHideOut)
				{

					for (int i = 1; i <= a_iDiceValue; i++)
					{
						int iCurrentTile = a_refTokenData.ICurrentPathIndex;
						iCurrentTile++;
						//TODO: If the token enters StairwayToHeavenState here
						if (iCurrentTile > 51)
						{
							iCurrentTile = 0; 
						}

						PathTileData refPathTileData;
						m_dicPathTileTypes.TryGetValue(iCurrentTile, out refPathTileData);
						if (refPathTileData == null)
						{
							Debug.LogError("[PathManager] PathTileData is null: index: " + iCurrentTile);
						}
						a_refTokenData.ICurrentPathIndex = refPathTileData.ITileIndex;

						//Updating token state based on which token reaches its entry tile
						#region UpdateTokenStateWhenReachingEntry
						if (refPathTileData.EnumTokenState == GameUtility.Base.eTokenState.EntryToStairway)
						{
							switch (refPathTileData.EnumPathTileType)
							{
								case GameUtility.Base.ePathTileType.BlueEnd:
									if (a_refTokenData.EnumTokenType == GameUtility.Base.eTokenType.Blue)
									{
										a_refTokenData.EnumTokenState = refPathTileData.EnumTokenState;
										//TODO: should enter the stairway to heaven
									}
									else
									{
										a_refTokenData.EnumTokenState = GameUtility.Base.eTokenState.InRoute;
									}
									break;
								case GameUtility.Base.ePathTileType.YellowEnd:
									if (a_refTokenData.EnumTokenType == GameUtility.Base.eTokenType.Yellow)
									{
										a_refTokenData.EnumTokenState = refPathTileData.EnumTokenState;
										//TODO: should enter the stairway to heaven
									}
									else
									{
										a_refTokenData.EnumTokenState = GameUtility.Base.eTokenState.InRoute;
									}
									break;
								case GameUtility.Base.ePathTileType.RedEnd:
									if (a_refTokenData.EnumTokenType == GameUtility.Base.eTokenType.Red)
									{
										a_refTokenData.EnumTokenState = refPathTileData.EnumTokenState;
										//TODO: should enter the stairway to heaven
									}
									else
									{
										a_refTokenData.EnumTokenState = GameUtility.Base.eTokenState.InRoute;
									}
									break;
								case GameUtility.Base.ePathTileType.GreenEnd:
									if (a_refTokenData.EnumTokenType == GameUtility.Base.eTokenType.Green)
									{
										a_refTokenData.EnumTokenState = refPathTileData.EnumTokenState;
										//TODO: should enter the stairway to heaven
									}
									else
									{
										a_refTokenData.EnumTokenState = GameUtility.Base.eTokenState.InRoute;
									}
									break;
							}

						}
						else
						{
							a_refTokenData.EnumTokenState = refPathTileData.EnumTokenState;
						}
						#endregion
						m_lstTilePosition.Add(refPathTileData.gameObject.transform);
					}
				}
			}



		}

		return m_lstTilePosition;
	}

	public bool ValidateMovement(TokenData a_refTokenData, int a_iDiceValue)
	{
		bool bvalid = false;
		if (a_refTokenData.EnumTokenState == GameUtility.Base.eTokenState.InStairwayToHeaven)
		{
			if ((a_refTokenData.ICurrentPathIndex + a_iDiceValue) < STEPCOUNT)
			{
				Debug.Log("[PathManager] Getting closer to heaven");
				bvalid = true;
			}
		}

		return bvalid;
	}





}

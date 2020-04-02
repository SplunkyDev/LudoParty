using System.Collections;
using System.Collections.Generic;
using GameUtility.Base;
using UnityEngine;

public class PathTileData : MonoBehaviour
{
	[SerializeField]private GameUtility.Base.ePathTileType m_enumPathTileType = GameUtility.Base.ePathTileType.None;
	public ePathTileType EnumPathTileType { get => m_enumPathTileType;}

	[SerializeField] private GameUtility.Base.eTokenState m_enumTokenState = GameUtility.Base.eTokenState.InRoute;
	public eTokenState EnumTokenState { get => m_enumTokenState; }

	private bool m_bTokenPresent = false;
	public bool BTokenPresent { get => m_bTokenPresent; set => m_bTokenPresent = value; }
	[SerializeField]private int m_iTileIndex = 0;
	public int ITileIndex { get => m_iTileIndex; set => m_iTileIndex = value; }

	void Start()
    {
        
    }

}

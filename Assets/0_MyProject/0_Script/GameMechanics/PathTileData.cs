using System.Collections;
using System.Collections.Generic;
using GameUtility.Base;
using UnityEngine;

public class PathTileData : MonoBehaviour
{
	[SerializeField]private GameUtility.Base.ePathTileType m_enumPathTileType = GameUtility.Base.ePathTileType.None;
	public ePathTileType EnumPathTileType { get => m_enumPathTileType;}
	private bool m_bTokenPresent = false;
	public bool BTokenPresent { get => m_bTokenPresent; set => m_bTokenPresent = value; }
	private int m_iTileIndex = 0;
	public int ITileIndex { get => m_iTileIndex; set => m_iTileIndex = value; }

	void Start()
    {
        
    }

}

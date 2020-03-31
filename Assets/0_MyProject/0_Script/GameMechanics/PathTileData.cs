using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTileData : MonoBehaviour
{
	[SerializeField]private GameUtility.Base.ePathTileType m_enumPathTileType = GameUtility.Base.ePathTileType.None;
	private bool m_bTokenPresent = false;
	public bool BTokenPresent { get => m_bTokenPresent; set => m_bTokenPresent = value; }

	void Start()
    {
        
    }

}

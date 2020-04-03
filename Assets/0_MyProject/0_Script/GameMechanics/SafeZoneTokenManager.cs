using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneTokenManager : MonoBehaviour
{
	[SerializeField] private List<PathTileData> m_lstSafeTile = new List<PathTileData>();
	private List<TokenData> m_lstTokenData = new List<TokenData>();
	private List<TokenData> m_lstTokenShare = new List<TokenData>();
	private int m_iCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void TokenEnterSafeZone(TokenData a_refTokenData)
	{
		m_lstTokenData.Clear();
		m_lstTokenData.Add(a_refTokenData);
	}


	private void CheckForTileSharingToken()
	{
		for (int i = 0; i < m_lstSafeTile.Count; i++)
		{
			for (int j = 0; j < m_lstTokenData.Count; j++)
			{
				if (m_lstSafeTile[i].ITileIndex == m_lstTokenData[i].ICurrentPathIndex)
				{
					m_iCount++;
					m_lstTokenShare.Add(m_lstTokenData[j]);
					
				}
			}

		}
	}
}

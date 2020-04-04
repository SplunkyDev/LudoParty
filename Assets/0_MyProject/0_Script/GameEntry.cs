using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

public class GameEntry : MonoBehaviour
{
    private void Awake()
    {
		EventManager.Instance.Initialize();
		GameManager.Instance.Initialize();
		TouchInputManager.Instance.Initialize();


#region Networking
		EssentialDataManager.Instance.Initialize();
		WarpNetworkManager.Instance.Initialize();
		GameNetworkManager.Instance.Initialize();
		MessageManager.Instance.Initialize();
#endregion
	}

}

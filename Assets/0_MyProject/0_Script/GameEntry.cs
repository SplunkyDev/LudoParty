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
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

public class DiceRollComplete : MonoBehaviour
{
	public void DiceAnimationComplete()
	{
		Debug.Log("[DiceAnimationComplete] Roll Dice Anim Complete: "+gameObject.name);
		EventManager.Instance.TriggerEvent<EventDiceRollAnimationComplete>(new EventDiceRollAnimationComplete());
	}
}

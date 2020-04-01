using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeSprite : MonoBehaviour
{
	private void Awake()
	{
		float width = ResizeSpriteManager.GetScreenToWorldWidth;
		transform.localScale = Vector3.one * width;
	}
}

using System.Collections;
using System.Collections.Generic;
using BKRacing.Items;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace BKRacing.GUI
{
	public class CollectibleDisplay : MonoBehaviour
	{
		private SVGImage[] _collectable;

		private void Start()
		{
			var items = Game.Instance.GetUiSprites();
			var color = Game.Instance.UncollectedColor;
			_collectable = new SVGImage[items.Length];
			var source = transform.GetChild(0).GetComponent<SVGImage>();
			source.sprite = items[0];
			source.color = color;

			for (int i = 1; i < items.Length; i++)
			{
				var current = Instantiate(source, transform);
				current.sprite = items[i];
				current.color = color;
				_collectable[i] = current;
			}
		}
	}
}

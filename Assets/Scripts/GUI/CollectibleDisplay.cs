using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace BKRacing.GUI
{
	public class CollectibleDisplay : MonoBehaviour
	{
		[SerializeField, Range(0.05f, 0.25f)]
		private float itemSize = 0.1f;

		private SVGImage[] _allItems;
		private List<SVGImage> _collected;
		private List<SVGImage> _notCollected;

		private void Start()
		{
			var items = Game.Instance.GetUiSprites();
			InitLists(items);
			var source = transform.GetChild(0).GetComponent<SVGImage>();
			
			for (int i = 0; i < items.Length; i++)
			{
				var current = i == 0 ? source : Instantiate(source, transform);
				current.sprite = items[i];
				current.color = Game.Instance.UncollectedColor;
				_allItems[i] = current;
				_notCollected.Add(current);
			}
		}

		private void InitLists(Sprite[] items)
		{
			_allItems = new SVGImage[items.Length];
			_collected = new List<SVGImage>();
			_notCollected = new List<SVGImage>();
		}

		public void CollectNew(Vector2 screenPosition)
		{
			var item = _notCollected[Random.Range(0, _notCollected.Count)];
			var collected = Instantiate(item, transform.parent);
			collected.sprite = item.sprite;
			collected.color = Color.white;
			var size = Screen.width * itemSize;
			collected.rectTransform.sizeDelta = new Vector2(size, size);
			collected.rectTransform.position = screenPosition;
		}
	}
}

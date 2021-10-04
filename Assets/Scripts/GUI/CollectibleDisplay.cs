using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace BKRacing.GUI
{
	public class CollectibleDisplay : MonoBehaviour
	{
		[SerializeField]
		private AnimationCurve toCenterCurve, toInventoryCurve;

		[SerializeField, Range(0, 1f)]
		private float itemYPosition = 0.75f, itemXPosition = 0.65f;

		[SerializeField]
		private float moveTime = 0.5f, waitTime = 1f;

		private SVGImage[] _allItems;
		private List<SVGImage> _notCollected;

		private void Start()
		{
			var items = Game.Instance.GetUiSprites();
			_allItems = new SVGImage[items.Length];
			_notCollected = new List<SVGImage>();
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

		public void CollectNew(Vector2 screenPosition)
		{
			if (_notCollected.Count == 0) { return; }

			var item = _notCollected[Random.Range(0, _notCollected.Count)];
			_notCollected.Remove(item);
			var collected = Instantiate(item, transform.parent);
			collected.sprite = item.sprite;
			collected.color = Color.white;
			collected.rectTransform.position = screenPosition;
			var size = Screen.width * Game.Instance.CollectedSize;
			var centerTarget = new Vector3(Screen.width * itemXPosition, Screen.height * itemYPosition, 0);
			
			// Immediately move the item to a "display position" on screen.
			StartCoroutine(MoveTo(collected.rectTransform, 0, screenPosition, 
				centerTarget, 0, size, toCenterCurve));

			// Wait and move the item to the inventory.
			StartCoroutine(MoveTo(collected.rectTransform, moveTime + waitTime, centerTarget,
				item.rectTransform.position, size, 1, toInventoryCurve));

			// Wait, finalize and destroy the collected item.
			StartCoroutine(FinalizeCollected(item, collected.gameObject));
		}

		private IEnumerator MoveTo(RectTransform rt, float preWait, Vector3 source, Vector3 target, 
			float startSize, float targetSize, AnimationCurve curve)
		{
			yield return new WaitForSeconds(preWait);
			float time = 0;

			while (time < moveTime)
			{
				var t = time / moveTime;
				rt.position = Vector3.LerpUnclamped(source, target, curve.Evaluate(t));
				rt.sizeDelta = Vector2.Lerp(Vector2.one * startSize, Vector2.one * targetSize, t);
				time += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator FinalizeCollected(SVGImage item, GameObject itemToDestroy)
		{
			yield return new WaitForSeconds(waitTime * 2);
			item.color = Color.white;
			Destroy(itemToDestroy, waitTime * 0.5f);
		}
	}
}

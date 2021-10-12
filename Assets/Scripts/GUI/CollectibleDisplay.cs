﻿using System.Collections;
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
		private GameObject _healthItem;
		private HealthBar _healthBar;

		private void Start()
		{
			_healthItem = CreateHealthItem();
			_healthBar = Game.Instance.HealthBar;
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

		private GameObject CreateHealthItem()
		{
			var go = new GameObject(Game.Instance.HealthSprite.name, typeof(Image));
			go.transform.SetParent(transform.parent, false);
			go.GetComponent<Image>().sprite = Game.Instance.HealthSprite;
			go.SetActive(false);
			return go;
		}

		public void CollectHealth(Vector2 screenPosition)
		{
			var health = Instantiate(_healthItem, transform.parent);
			health.SetActive(true);
			LaunchItemMovement(health.GetComponent<RectTransform>(), screenPosition, _healthBar.iconTransform);
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
			LaunchItemMovement(collected.rectTransform,screenPosition, item.rectTransform);
		}

		private void LaunchItemMovement(RectTransform rt, Vector3 source, RectTransform target)
		{
			var size = Screen.width * Game.Instance.CollectedSize;
			var centerTarget = new Vector3(Screen.width * itemXPosition, Screen.height * itemYPosition, 0);

			// Immediately move the item to a "display position" on screen.
			StartCoroutine(MoveTo(rt, 0, source, centerTarget, 0, size, 
				toCenterCurve));

			// Wait and move the item to the inventory.
			StartCoroutine(MoveTo(rt, moveTime + waitTime, centerTarget, target.position, size, 1, toInventoryCurve));

			// Wait, finalize and destroy the collected item.
			StartCoroutine(FinalizeCollected(target.gameObject.GetComponent<SVGImage>(), rt.gameObject));
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

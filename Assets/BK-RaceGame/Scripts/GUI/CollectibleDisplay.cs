using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BKRacing.GUI
{
	public class CollectibleDisplay : MonoBehaviour
	{
		// private Vector3 _centerTarget;
		private float _size;
		private float _centerX;
		private Image[] _allItems;
		private List<Image> _notCollected;
		private List<Image> _collected;
		private float _itemYPosition;
		private float _itemXPosition;
		private float _collectedSize;
		private float _parentScale;
		private AnimationCurve _loseItemCurve;
		private AnimationCurve _toCenterCurve;
		private AnimationCurve _toInventoryCurve;

		public Action allCollected;
		
		public bool AllCollected => _notCollected.Count == 0;

		private void Start()
		{
			_itemYPosition = Game.Instance.itemYPosition;
			_itemXPosition = Game.Instance.itemXPosition;
			_collectedSize = Game.Instance.itemSize;
			_size = Screen.width * _collectedSize;
			_centerX = Screen.width * _itemXPosition;
			var items = Game.Instance.GetUiSprites();
			_allItems = new Image[items.Length];
			_notCollected = new List<Image>();
			_collected = new List<Image>();
			_parentScale = transform.parent.parent.localScale.x;
			var source = transform.GetChild(0).GetComponent<Image>();

			for (int i = 0; i < items.Length; i++)
			{
				var current = i == 0 ? source : Instantiate(source, transform);
				current.sprite = items[i];
				current.color = Game.Instance.uncollectedColor;
				_allItems[i] = current;
				_notCollected.Add(current);
			}

			_loseItemCurve = Game.Instance.loseItemCurve;
			_toCenterCurve = Game.Instance.toCenterCurve;
			_toInventoryCurve = Game.Instance.toInventoryCurve;
		}

#if UNITY_EDITOR
		private void Update()
		{
			_itemYPosition = Game.Instance.itemYPosition;
			_itemXPosition = Game.Instance.itemXPosition;
			_collectedSize = Game.Instance.itemSize;
		}
#endif

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

			if (_notCollected.Count == 0)
			{
				allCollected?.Invoke();
			}
		}

		public void LoseCollected(Vector3 screenPosition)
		{
			if (_collected.Count == 0) { return; }

			var item = _collected[Random.Range(0, _collected.Count)];
			_collected.Remove(item);
			_notCollected.Add(item);
			var lost = Instantiate(item, transform.parent);
			lost.sprite = item.sprite;
			lost.rectTransform.position = screenPosition;
			item.color = Game.Instance.uncollectedColor;
			var target = new Vector3(Random.Range(0, Screen.width), 0, 0);
			StartCoroutine(LaunchLostItem(lost.rectTransform, target));
		}

		private IEnumerator LaunchLostItem(RectTransform rt, Vector3 target)
		{
			var source = rt.position;
			var size = Screen.width * _collectedSize;
			var height = Screen.height;
			float time = 0;
			rt.sizeDelta = new Vector3(size, size);
			var flyTime = Game.Instance.flyTime;

			while (time < flyTime)
			{
				var t = time / flyTime;
				Vector3 pos = new Vector3(
					Mathf.Lerp(source.x, target.x, t), 
					source.y + _loseItemCurve.Evaluate(t) * height, 
					0);
				rt.position = pos;
				time += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			Destroy(rt.gameObject);
		}

		private void LaunchItemMovement(RectTransform rt, Vector3 source, RectTransform target)
		{
			var x = Game.Instance.Player.transform.position.x < 0 ? _centerX : Screen.width - _centerX;
			var centerTarget = new Vector3(x, Screen.height * _itemYPosition, 0);
			centerTarget.x /= _parentScale;	// Scale the vector according to Canvas Scaler
			centerTarget.y /= _parentScale;
			var item = target.gameObject.GetComponent<Image>();
			var moveTime = Game.Instance.moveTime;
			var waitTime = Game.Instance.waitTime;

			// Immediately move the item to a "display position" on screen.
			StartCoroutine(MoveTo(rt, 0, source, centerTarget, 0, _size, 
				_toCenterCurve));

			// Wait and move the item to the inventory.
			StartCoroutine(MoveTo(rt, moveTime + waitTime, centerTarget, target.position, _size, 1, 
				_toInventoryCurve));

			// Wait, finalize and destroy the collected item.
			StartCoroutine(FinalizeCollected(item, rt.gameObject));
		}

		private IEnumerator MoveTo(RectTransform rt, float preWait, Vector3 source, Vector3 target, 
			float startSize, float targetSize, AnimationCurve curve)
		{
			var moveTime = Game.Instance.moveTime;
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

		private IEnumerator FinalizeCollected(Image item, GameObject itemToDestroy)
		{
			var waitTime = Game.Instance.waitTime;
			yield return new WaitForSeconds(waitTime * 2);
			item.color = Color.white;
			_collected.Add(item);
			Destroy(itemToDestroy, waitTime * 0.5f);
		}
	}
}

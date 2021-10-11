using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing.GUI
{
	public class HealthBar : MonoBehaviour
	{
		[SerializeField]
		private RectTransform healthTransform;

		private void Awake()
		{
			healthTransform.localScale = Vector3.one;
		}

		public void SetHealth(int current, int max)
		{
			var newScale = new Vector3((float) current / (float) max, 1, 1);
			healthTransform.localScale = newScale;
		}
	}
}

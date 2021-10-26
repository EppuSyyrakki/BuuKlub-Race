using System.Collections;
using UnityEngine;

namespace BKRacing.Environment
{
	public class Background : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<SpriteRenderer>().sprite = Game.Instance.BackgroundCard;
			SetTransform();
		}

		private void Update()
		{
			SetTransform();
		}

		private void SetTransform()
		{
			var y = Game.Instance.backgroundY;
			var z = Game.Instance.backgroundZ;
			var s = Game.Instance.backgroundScale;
			var position = new Vector3(0, y, z);
			var scale = new Vector3(s, s, 1);
			transform.localPosition = position;
			transform.localScale = scale;
		}
	}
}
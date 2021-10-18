using System.Collections;
using UnityEngine;

namespace BKRacing.Environment
{
	public class Background : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<SpriteRenderer>().sprite = Game.Instance.BackgroundCard;
		}
	}
}
using UnityEngine;
using UnityEngine.UI;

namespace BKRacing.Items
{
	[RequireComponent(typeof(Image), typeof(LayoutElement))]
	public class UiItem : Item
	{
		private Image _image;

		public override Sprite Sprite => _image.sprite;

		private void Awake()
		{
			_image = GetComponent<Image>();
		}

		public override void Update()
		{
			// ?
		}

		public override void Init(Sprite sprite)
		{
			GetComponent<Image>().sprite = sprite;
		}
	}
}
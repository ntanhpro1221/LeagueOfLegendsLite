using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class DisablableUI_Image<TProp> : DisablableUI<Image, TProp> { }
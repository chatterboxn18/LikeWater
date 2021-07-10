using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WebcamController : MonoBehaviour
{
	[SerializeField] private RawImage _texture;
	private WebCamTexture _webcam;
	[SerializeField] private AspectRatioFitter _aspectRatio;
	private void Start()
	{
		var devices = WebCamTexture.devices;
		_webcam = new WebCamTexture(devices[0].name, Screen.width,Screen.height,30);
		Debug.Log(_webcam.requestedWidth + " and height: " + _webcam.requestedHeight);
		_texture.texture = _webcam;
	}


	private void Update()
	{
		_orient();
	}
	
	private void _orient()
	{
		float physical = (float)_webcam.width/(float)_webcam.height;
		_aspectRatio.aspectRatio = physical;

		float scaleY = _webcam.videoVerticallyMirrored ? -1f : 1f;
		_texture.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

		int orient = -_webcam.videoRotationAngle;
		_texture.rectTransform.localEulerAngles = new Vector3(0f,0f,orient);
	}
	public void StartCamera()
	{
		if (_webcam!= null)
			_webcam.Play();
	}
	
	IEnumerator TakePhoto()  // Start this Coroutine on some button click
	{

		// NOTE - you almost certainly have to do this here:

		yield return new WaitForEndOfFrame(); 

		// it's a rare case where the Unity doco is pretty clear,
		// http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
		// be sure to scroll down to the SECOND long example on that doco page 

		Texture2D photo = new Texture2D(_webcam.width, _webcam.height);
		photo.SetPixels(_webcam.GetPixels());
		photo.Apply();

		//Encode to a PNG
		byte[] bytes = photo.EncodeToPNG();
		//Write out the PNG. Of course you have to substitute your_path for something sensible
		File.WriteAllBytes(Application.persistentDataPath + Path.DirectorySeparatorChar + "photo.png", bytes);
	}

	public void StopCamera()
	{
		if (_webcam != null)
			_webcam.Stop();
	}
}
	
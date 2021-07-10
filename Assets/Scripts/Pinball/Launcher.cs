/*
 * Copyright (c) 2020 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;

public class Launcher : MonoBehaviour
{
	// EfxZoom
	public GameObject efxZoomObj;
	public GameObject efxLightObj;

	// Touch Listener
	public bool isTouched = false;
	public bool isKeyPress = false;
	// Ready for Launch
	public bool isActive = true;
	// Timers
	private float pressTime = 0f;
	private float startTime = 0f;
	private int powerIndex;

	[SerializeField] private SpringJoint2D _springJoint;
	[SerializeField] private Rigidbody2D _rigidbody2D;
	[SerializeField] private float _force = 0f;
	[SerializeField] private float maxForce = 90f;
	
	void Start()
	{
		_springJoint.distance = 1f;
		// zoom animation object
		//efxZoomAniController = efxZoomObj.GetComponent<AnimateController>();
		//efxLightAniController = efxLightObj.GetComponent<AnimateController>();
		// zoom light object
		//efxZoomRenderer = efxZoomObj.GetComponent<Renderer>() as SpriteRenderer;
		//efxLightRenderer = efxLightObj.GetComponent<Renderer>() as SpriteRenderer;
		// sounds
	}

	void Update()
	{
		if (isActive)
		{
			if (Input.GetKeyDown("space"))
			{
				isKeyPress = true;
			}

			if (Input.GetKeyUp("space"))
			{
				isKeyPress = false;
			}

			// on keyboard press or touch hotspot
			if (isKeyPress == true && isTouched == false || isKeyPress == false && isTouched == true)
			{
				if (startTime == 0f)
				{
					startTime = Time.time;
				}
			}

			// on keyboard release 
			if (isKeyPress == false && isTouched == false && startTime != 0f)
			{
				// #1
				_force = powerIndex * maxForce;
				// reset values & animation
				pressTime = 0f;
				startTime = 0f;
				//efxLightRenderer.sprite = efxLightAniController.spriteSet[1];
				while (powerIndex >= 0)
				{
					//efxZoomRenderer.sprite = efxZoomAniController.spriteSet[powerIndex];
					powerIndex--;
				}
			}
		}

		// Start Press
		if (startTime != 0f)
		{
			pressTime = Time.time - startTime;
			// plays zoom animation on loop
			powerIndex = (int)Mathf.PingPong(pressTime * 60, 50);
			//efxZoomRenderer.sprite = efxZoomAniController.spriteSet[powerIndex];
			// turns on/ off zoom light based on powerIndex
		   /* if (powerIndex == efxZoomAniController.spriteSet.Length - 1)
			{
				//efxLightRenderer.sprite = efxLightAniController.spriteSet[0];
			}
			else
			{
				//efxLightRenderer.sprite = efxLightAniController.spriteSet[1];
			}*/
		}
	}

	private void FixedUpdate()
	{
		if (_force != 0)
		{
			_springJoint.distance = 1f;
			_rigidbody2D.AddForce(Vector3.up * _force);
			_force = 0;
		}

		if (pressTime != 0)
		{
			_springJoint.distance = 0.8f;
			_rigidbody2D.AddForce(Vector3.down * 400);
		}
	}
}

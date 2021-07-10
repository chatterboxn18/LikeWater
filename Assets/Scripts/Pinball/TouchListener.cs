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

public class TouchListener : MonoBehaviour
{
    public LayerMask hotspot;
    // put hotspots you want to detect into this layer
    private GameObject flipRightObj;
    private GameObject flipLeftObj;
    private GameObject launchObj;
    //
    private FlipControlLeft flipLeftScript;
    private FlipControlRight flipRightScript;
    private Launcher launchScript;
    private Vector2 touchPoint;
    private Collider2D hit;
    //
    public bool isTouched = false;

    void Start()
    {
        flipRightObj = GameObject.Find("FlipRgt-hingejoint");
        flipLeftObj = GameObject.Find("FlipLeft-hingejoint");
        launchObj = GameObject.Find("Plunger-springjoint");
        // check objects exists
        if (flipRightObj != null && flipLeftObj != null && launchObj != null)
        {
            flipRightScript = flipRightObj.GetComponent<FlipControlRight>();
            flipLeftScript = flipLeftObj.GetComponent<FlipControlLeft>();
            launchScript = launchObj.GetComponent<Launcher>();
        }
    }

    void Update()
    {
        // check object exists
        if (flipLeftScript == null || flipRightScript == null || launchScript == null)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            int tapCount = Input.touchCount;

            for (var i = 0; i < tapCount; i++)
            {
                touchPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                hit = Physics2D.OverlapPoint(touchPoint, hotspot);

                if (hit != null)
                {
                    Debug.Log(Input.GetTouch(i).phase);
                    if (Input.GetTouch(i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Stationary)
                    {
                        switch (hit.name)
                        {
                            case "touchRgt":
                                flipRightScript.isTouched = true;
                                break;
                            case "touchLeft":
                                flipLeftScript.isTouched = true;
                                break;
                            case "touchLaunch":
                                launchScript.isTouched = true;
                                break;
                        }
                    }
                }
            }
        }
        else
        {
            flipRightScript.isTouched = false;
            flipLeftScript.isTouched = false;
            launchScript.isTouched = false;
        }
    }
}

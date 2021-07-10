﻿/*
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

public class DestroyBall : MonoBehaviour
{
    public GameObject newBall;
    public GameObject golight;
    //
    private Launcher launcherScript;
    private SpriteRenderer golightRenderer;
    private AnimateController golightAniController;
    private SoundController sound;

    void Start()
    {
        sound = GameObject.Find("SoundObjects").GetComponent<SoundController>();
        golightRenderer = golight.GetComponent<Renderer>() as SpriteRenderer;
        golightAniController = golight.GetComponent<AnimateController>();
        // check launcher object exists
        GameObject launcherObj = GameObject.Find("Plunger-springjoint");
        if (launcherObj != null)
        {
            launcherScript = launcherObj.GetComponent<Launcher>();
        }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.name == "ball")
        {
            // on light
            golightRenderer.sprite = golightAniController.spriteSet[0];
            sound.die.Play();
        }
    }

    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.name == "ball" && launcherScript != null)
        {
            // off light & Destroy ball
            golightRenderer.sprite = golightAniController.spriteSet[1];
            Destroy(obj.gameObject);
            // new 
            GameObject newObj = Instantiate(newBall) as GameObject;
            newObj.name = "ball";
            newObj.transform.position = new Vector3(2.85f, -1f, 0f);
            // reset launcher
            launcherScript.isActive = true;
        }
    }
}

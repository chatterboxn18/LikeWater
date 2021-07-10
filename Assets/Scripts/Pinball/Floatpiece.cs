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
using System.Collections;

public class Floatpiece : MonoBehaviour
{
    // Sound & Animation
    public GameObject handcamObj;
    public GameObject golightObj;
    private SpriteRenderer handcamRenderer;
    private SpriteRenderer golightRenderer;
    private AnimateController handcamAniController;
    private AnimateController golightAniController;
    private SoundController sound;

    // Score
    private ScoreBoard scoreBoard;

    void Start()
    {
        // Get scoreboard and sound object
        GameObject obj = GameObject.Find("scoreText");
        scoreBoard = obj.GetComponent<ScoreBoard>();
        sound = GameObject.Find("SoundObjects").GetComponent<SoundController>();
        // Animation objects
        handcamRenderer = handcamObj.GetComponent<Renderer>() as SpriteRenderer;
        golightRenderer = golightObj.GetComponent<Renderer>() as SpriteRenderer;
        handcamAniController = handcamObj.GetComponent<AnimateController>();
        golightAniController = golightObj.GetComponent<AnimateController>();
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.name == "ball")
        {
            sound.bonus.Play();
            scoreBoard.gamescore = scoreBoard.gamescore + 10;
            golightRenderer.sprite = golightAniController.spriteSet[0];
            StartCoroutine(BeginFloat());
        }
    }

    IEnumerator BeginFloat()
    {
        while (true)
        {
            // Play animation loop
            int index = (int)Mathf.PingPong(handcamAniController.fps * Time.time, handcamAniController.spriteSet.Length);
            handcamRenderer.sprite = handcamAniController.spriteSet[index];
            yield return new WaitForSeconds(0.1f);

            // Stop & Reset Timer 
            sound.bonus.Stop();
            golightRenderer.sprite = golightAniController.spriteSet[1];
            handcamRenderer.sprite = handcamAniController.spriteSet[handcamAniController.spriteSet.Length - 1];
            break;
        }
    }
}

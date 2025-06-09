using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerV2 : MonoBehaviour
{
    [System.Serializable]
    public class AudioPoint{ //audiopoint objects are points on levels that when approached by the player, the sound will gradually change to a different sound. both mono and stereo sounds are supported.
        //xPoint is the x position of the audiopoint, leftClip is the sound that will be played when the player is approaching the point from the right, rightClip is the sound that will be played when the player approaches the point from the left, and detectionWidth is how wide the detection area is for the audiopoint
        public AudioClip leftClip;
        public AudioClip rightClip;
        public Vector2 centerPoint;
        public Vector2 detectionSize;
        public Collider2D playerCollider;
        public AudioPoint(Vector2 centerPoint, AudioClip leftClip, AudioClip rightClip, Vector2 detectionSize){
            this.centerPoint = centerPoint;
            this.leftClip = leftClip;
            this.rightClip = rightClip;
            this.detectionSize = detectionSize;
        }
    }
    public List<AudioPoint> audioPoints;
    public AudioSource currentSource;
    public AudioSource incomingSource;
    public bool fading = false;
    void Start()
    {
        List<AudioSource> sources = new List<AudioSource>(GetComponents<AudioSource>());
        currentSource = sources[0];
        incomingSource = sources[1];
        incomingSource.volume = 0;
        currentSource.volume = 1;
        currentSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioPoints.Count != 0)
        {
            for (int i = 0; i < audioPoints.Count; i++)
            {
                AudioPoint ap = audioPoints[i];
                ap.playerCollider = Physics2D.OverlapBox(ap.centerPoint, ap.detectionSize, 0, LayerMask.GetMask("Player"));
                if (ap.playerCollider != null && !fading)
                {
                    StartCoroutine(fadeVolume(ap));
                }
            }
        }
    }
    private void OnDrawGizmosSelected() {
        if (audioPoints.Count != 0)
        {
            foreach (AudioPoint ap in audioPoints){
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(ap.centerPoint, ap.detectionSize);
            Gizmos.DrawLine(ap.centerPoint - new Vector2(0, ap.detectionSize.y/2), ap.centerPoint + new Vector2(0, ap.detectionSize.y/2));
        }
        }
    }
    //for future reference, look into spatial blend and stereo pan
    private IEnumerator fadeVolume(AudioPoint ap){
        fading = true;
        float maxDistance = ap.detectionSize.x;
        if(ap.playerCollider.transform.position.x < ap.centerPoint.x){
            if(!currentSource.clip.Equals(ap.leftClip)){ //basically if its not already on the clip it needs to be and playing it, do that
                currentSource.clip = ap.leftClip;
            }
            if (!currentSource.isPlaying) {currentSource.Play();}
            if (!incomingSource.clip.Equals(ap.rightClip)) {
                incomingSource.clip = ap.rightClip;
            }
            if (!incomingSource.isPlaying) {incomingSource.Play();}
        }else{
            if(!currentSource.clip.Equals(ap.rightClip)){ //this code is genuinely so shit oh my god im sorry il lfix it later
                currentSource.clip = ap.rightClip;
            }
            if (!currentSource.isPlaying) {currentSource.Play();}
            if(!incomingSource.clip.Equals(ap.leftClip)){
                incomingSource.clip = ap.leftClip;
            }
            if (!incomingSource.isPlaying) {incomingSource.Play();}
        } //heeeheheheheheheh i used a ternary operator im so cool
        float xEndPoint = (ap.playerCollider.transform.position.x < ap.centerPoint.x ? 1: -1)*ap.detectionSize.x/2 + ap.centerPoint.x; //so this should represen the opposite end of the overlap box from where the player entered
        float currentEndPan = ap.playerCollider.transform.position.x < ap.centerPoint.x ? -1: 1; //which direction audio should pan, -1 : left and 1 : right
        while (ap.playerCollider != null){
            float distance = Math.Abs(ap.playerCollider.transform.position.x - xEndPoint);
            currentSource.volume = distance/maxDistance;
            currentSource.panStereo = Mathf.Lerp(0, currentEndPan, 1 - distance/maxDistance);
            incomingSource.volume = 1 - distance/maxDistance;
            incomingSource.panStereo = Mathf.Lerp(-currentEndPan, 0, 1 - distance/maxDistance);

            yield return null;
        }
        if(incomingSource.volume > currentSource.volume){
            currentSource.volume = 0;
            currentSource.Stop();
            (currentSource, incomingSource) = (incomingSource, currentSource); //if the player fully crosses over the audiopoint, move forward with the incoming audio source as the current source
        }else{
            incomingSource.Stop();
        }
        fading = false;
    }
}

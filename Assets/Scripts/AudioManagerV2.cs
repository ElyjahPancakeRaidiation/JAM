using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class AudioManagerV2 : MonoBehaviour
{
    public enum AudioState
    {
        FADE_IN,
        FADE_OUT,
    }
    [System.Serializable]
    public class AudioPoint
    { //audiopoint objects are points on levels that when approached by the player, the sound will gradually change to a different sound. both mono and stereo sounds are supported.
        //leftClip is the sound that will be played when the player is approaching the point from the right, rightClip is the sound that will be played when the player approaches the point from the left, and detectionWidth is how wide the detection area is for the audiopoint
        public AudioClip leftClip;
        public AudioClip rightClip;
        [SerializeField] public bool soloFadeIn;
        [SerializeField] public bool soloFadeOut;
        [SerializeField] public float fadeLength;
        public Vector2 centerPoint;
        public Vector2 detectionSize;
        public Collider2D playerCollider;
        public AudioPoint(Vector2 centerPoint, AudioClip leftClip, AudioClip rightClip, Vector2 detectionSize)
        {
            this.centerPoint = centerPoint;
            this.leftClip = leftClip;
            this.rightClip = rightClip;
            this.detectionSize = detectionSize;
        }
    }
    [System.Serializable]
    public class PlayerSFX
    {
        [Range(-70, 0)]
        public float maxVolume; //yo fyi when using this "max volume" variable in the inspector, its to be converted to the linear system unity uses with the method "decibelToLinear"
        public AudioClip sfxClip;
        public AnimationCurve fadeVolumeCurve;
        public String tag;
        public float fadeLength;

        public PlayerSFX(AudioClip sfxClip, AnimationCurve fadeVolumeCurve, String tag, float fadeLength)
        {
            this.sfxClip = sfxClip;
            this.fadeVolumeCurve = fadeVolumeCurve;
            this.tag = tag;
            this.fadeLength = fadeLength;
        }

    }
    public List<AudioPoint> audioPoints;
    public List<PlayerSFX> playerSFXs;
    public Dictionary<String, AudioSource> SFXsources;

    public AudioSource currentSource;
    public AudioSource incomingSource;
    [Range(-70, 0)]
    public float mainVolume;
    public AnimationCurve volumeCurve;
    public bool fading = false;
    void Start()
    {
        List<AudioSource> sources = new List<AudioSource>(GetComponents<AudioSource>());
        currentSource = sources[0];
        incomingSource = sources[1];
        GameManager.current.pauseEvent += currentSource.Pause;
        GameManager.current.unPauseEvent += currentSource.UnPause;
        SFXsources = new Dictionary<string, AudioSource>();
        foreach (PlayerSFX p in playerSFXs)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = p.sfxClip;
            SFXsources.Add(p.tag, audioSource);
        }
        incomingSource.volume = 0;
        //currentSource.volume = 1;
        //currentSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < audioPoints.Count; i++)
        {
            AudioPoint ap = audioPoints[i];
            ap.playerCollider = Physics2D.OverlapBox(ap.centerPoint, ap.detectionSize, 0, LayerMask.GetMask("Player")); //this might be hindering performance, perhaps move this to oncollisionenter method or wtv
            //************************************************************************************to felix in the future or whoever else is editing this code
            if (ap.playerCollider != null && !fading)
            {
                if (ap.soloFadeIn) //if the boolean soloFade is checked, only one of the clips should be loaded
                {
                    StartCoroutine(FadeTrack(AudioState.FADE_IN, ap)); //these will be triggers, fade is not determined by player location
                }
                else if (ap.soloFadeOut)
                {
                    StartCoroutine(FadeTrack(AudioState.FADE_OUT, ap));
                }
                else
                {
                    StartCoroutine(TransitionFade(ap)); // these will be fades based on player location
                    //can fade out to silence and from silence to track
                }
            }
        }
        if (!fading)
        {
            currentSource.volume = decibelToLinear(mainVolume);
        }
    }
    private void OnDrawGizmosSelected()
    {
        foreach (AudioPoint ap in audioPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(ap.centerPoint, ap.detectionSize);
            Gizmos.DrawLine(ap.centerPoint - new Vector2(0, ap.detectionSize.y / 2), ap.centerPoint + new Vector2(0, ap.detectionSize.y / 2));
        }
    }
    //for future reference, look into spatial blend and stereo pan
    private IEnumerator TransitionFade(AudioPoint ap)
    {
        fading = true;
        float maxDistance = ap.detectionSize.x;
        float direction = ap.playerCollider.transform.position.x < ap.centerPoint.x ? 1 : -1;
        float xEndPoint = direction * ap.detectionSize.x / 2 + ap.centerPoint.x; //so this should represen the opposite end of the overlap box from where the player entered

        if (direction == 1)
        {
            currentSource.clip = ap.leftClip;
            incomingSource.clip = ap.rightClip;
        }
        else
        {
            currentSource.clip = ap.rightClip;
            incomingSource.clip = ap.leftClip;
        }
        if (!currentSource.isPlaying) { currentSource.Play(); }
        if (!incomingSource.isPlaying) { incomingSource.Play(); }
        while (ap.playerCollider != null)
        {
            float distance = Math.Abs(ap.playerCollider.transform.position.x - xEndPoint);
            currentSource.volume = decibelToLinear(Mathf.Lerp(-70, mainVolume, distance / maxDistance));
            currentSource.panStereo = Mathf.Lerp(0, -direction, 1 - distance / maxDistance);
            incomingSource.volume = decibelToLinear(Mathf.Lerp(mainVolume, -70, distance / maxDistance)); ;
            incomingSource.panStereo = Mathf.Lerp(direction, 0, 1 - distance / maxDistance);

            yield return null;
        }
        if (incomingSource.volume > currentSource.volume)
        {
            currentSource.volume = 0;
            currentSource.Stop();
            (currentSource, incomingSource) = (incomingSource, currentSource); //if the player fully crosses over the audiopoint, move forward with the incoming audio source as the current source
        }
        else
        {
            incomingSource.Stop();
        }
        fading = false;
    }
    private IEnumerator FadeTrack(AudioState state, AudioPoint ap)
    {
        fading = true;
        AudioClip clip = ap.leftClip != null ? ap.leftClip : ap.rightClip; //this dont really matter if the audio state is fade out

        if ((state == AudioState.FADE_IN && currentSource.clip == clip && currentSource.isPlaying) || (state == AudioState.FADE_OUT && currentSource.clip == null)) //precautions
        {
            fading = false;
            yield break;
        }

        float elapsedTime = 0;
        if (state == AudioState.FADE_IN)
        {
            currentSource.clip = clip; //load the clip, play and raise volume until reach the mainVolume
            currentSource.Play();
            while (elapsedTime < ap.fadeLength)
            {
                currentSource.volume = decibelToLinear(Mathf.Lerp(-70, mainVolume, elapsedTime / ap.fadeLength));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            fading = false;
            yield break;
        }
        else if (state == AudioState.FADE_OUT) //decrease volume until effectively 0, then stop the source.
        {
            while (elapsedTime < ap.fadeLength)
            {
                currentSource.volume = decibelToLinear(Mathf.Lerp(mainVolume, -70f, elapsedTime / ap.fadeLength));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            currentSource.volume = 0;
            currentSource.Stop();
            fading = false;
            yield break;
        }
        fading = false;
    }

    public IEnumerator playPlayerSFX(String tag)
    {
        PlayerSFX sfx = playerSFXs.Find(x => x.tag == tag);
        if (sfx != null)
        {
            AudioSource audioSource = SFXsources[tag];
            audioSource.volume = decibelToLinear(sfx.maxVolume);
            audioSource.Play();

            float trackLength = sfx.sfxClip.length;
            //fade in

            //i didnt do this yet but idk if we're committing to this

            //fade out
            yield return new WaitForSeconds(trackLength - sfx.fadeLength); //wait for track to get to point where fade starts
            float time = 0;
            while (time < sfx.fadeLength)
            {
                time += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0, decibelToLinear(sfx.maxVolume), sfx.fadeVolumeCurve.Evaluate(time / sfx.fadeLength));
                yield return null;
            }
        }
        else
        {
            Debug.Log("No SFX found with tag: " + tag);
        }
    }
    public float decibelToLinear(float db)
    {
        return Mathf.Pow(10f, db / 20f);
    }
    void OnDestroy()
    {
        GameManager.current.pauseEvent -= currentSource.Pause;
        GameManager.current.unPauseEvent -= currentSource.UnPause;
        
    }
}


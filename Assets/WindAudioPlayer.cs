using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class WindAudioPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] AudioSource source;

    [SerializeField] bool isPlayerWithinZone;

    ManageWind manageWind;
    [Header("Addition, so 0 means no extra volume is added")]
    [SerializeField] private float addedVolume;
    public float pitchUpper, pitchLower;
    private float volumeLower, volumeUpper, volumeInterpolatorValue, pitchInterpolatorValue;
    public bool isAudioPlaying;

    public AnimationCurve volumeAdjustment;
    public float multiplierZoneExtraVolume;
    public AnimationCurve pitchAdjustment;
 
    [SerializeField] Vector2[] volumeFrames;
    [SerializeField] Vector2[] pitchFrames;
    private float actualVolume;
    void Awake()
    {
        //manageWind = GameObject.FindGameObjectWithTag("Wind").GetComponent<ManageWind>();

    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("In Wind: " + ManageWind.IsPlayerInAnyZone);
        //Debug.Log("On wind: " + IsPlayerWithinZone());
        // PlayAudio();
        // Debug.Log("Wind counter: " + ManageWind.windCounter);
        if (runNow)
        {
            currentTimer += Time.deltaTime;
        }
        else currentTimer = 0;
        Debug.Log("currenTImer: " + currentTimer);
    }
    float currentPitch;
    float currentVolume;
    void Start()
    {
        currentVolume = source.volume;
        currentPitch = source.pitch;
        isAudioPlaying = false;
    }
    //control pitch/ volume
    float currentTimer = 0;
    // public IEnumerator EditAudio()
    // {
    //     // timer += Time.deltaTime;
    //     source.volume = Mathf.Lerp(volumeLower, volumeUpper, volumeInterpolatorValue * timer / 1.3f);
    //     yield return new WaitForSeconds(.5f);
    //     source.pitch = Mathf.Lerp(pitchLower, pitchUpper, pitchInterpolatorValue * timer / 1.3f);
    //     yield return new WaitForSeconds(2f);
    //     source.pitch = Mathf.Lerp(currentPitch, pitchUpper, pitchInterpolatorValue * timer / 1.3f);
    //     source.volume = Mathf.Lerp(currentVolume, volumeUpper, volumeInterpolatorValue * timer / 1.3f);

    //     // if (timer > 4f)
    //     // {
    //     //     timer = 1;
    //     // }
    // }
    bool runNow = false;


    public void AdjustVolume(float timer, bool audioPlayed, bool multiplierOn)
    {
        float extraVolume = multiplierZoneExtraVolume;
        if (audioPlayed)
        {
            if (!multiplierOn)
            {
                volumeAdjustment = new AnimationCurve(new Keyframe(volumeFrames[0].x, volumeFrames[0].y + addedVolume), new Keyframe
             (volumeFrames[1].x, volumeFrames[1].y + addedVolume), new Keyframe(volumeFrames[2].x, volumeFrames[2].y + addedVolume));

                source.volume = volumeAdjustment.Evaluate(timer);
                volumeAdjustment.preWrapMode = WrapMode.PingPong;
                volumeAdjustment.postWrapMode = WrapMode.PingPong;
                runNow = false;
            }
            else volumeAdjustment = new AnimationCurve(new Keyframe(volumeFrames[0].x, volumeFrames[0].y + extraVolume + addedVolume), new Keyframe
             (volumeFrames[1].x, volumeFrames[1].y + extraVolume + addedVolume), new Keyframe(volumeFrames[2].x, volumeFrames[2].y + extraVolume + addedVolume));

            source.volume = volumeAdjustment.Evaluate(timer);
            volumeAdjustment.preWrapMode = WrapMode.PingPong;
            volumeAdjustment.postWrapMode = WrapMode.PingPong;
            runNow = false;
            

        }
        else
        {
            if (timer == 0)
            {
                runNow = true;
                //Debug.Log("ami runign");    

            }
            if (runNow)
            {
                if (!multiplierOn)
                {

                    volumeAdjustment = new AnimationCurve(new Keyframe(volumeFrames[3].x, volumeFrames[3].y + addedVolume), new Keyframe
           (volumeFrames[4].x, volumeFrames[4].y + addedVolume), new Keyframe(volumeFrames[5].x, volumeFrames[5].y + addedVolume));

                    source.volume = volumeAdjustment.Evaluate(currentTimer);
                    volumeAdjustment.preWrapMode = WrapMode.PingPong;
                    volumeAdjustment.postWrapMode = WrapMode.PingPong;
                    currentVolume = source.volume;

                }
                //         else
                //         {

                //             volumeAdjustment = new AnimationCurve(new Keyframe(volumeFrames[3].x, volumeFrames[3].y + extraVolume + addedVolume ), new Keyframe
                //    (volumeFrames[4].x, volumeFrames[4].y + extraVolume + addedVolume), new Keyframe(volumeFrames[5].x, volumeFrames[5].y + extraVolume + addedVolume));

                //             source.volume = volumeAdjustment.Evaluate(timer);
                //               volumeAdjustment.preWrapMode = WrapMode.PingPong;
                //             volumeAdjustment.postWrapMode = WrapMode.PingPong;
                //             currentVolume = source.volume;

                //         }

            }

        }

    }
    float pitchTimer = 0;
    public void AdjustPitch(float timer)
    {
        pitchTimer += Time.deltaTime;
        pitchAdjustment = new AnimationCurve(
        new Keyframe(pitchFrames[0].x, pitchFrames[0].y),
        new Keyframe(pitchFrames[1].x, pitchFrames[1].y),
        new Keyframe(pitchFrames[2].x, pitchFrames[2].y),
        new Keyframe(pitchFrames[3].x, pitchFrames[3].y));
        pitchAdjustment.preWrapMode = WrapMode.PingPong;
        pitchAdjustment.postWrapMode = WrapMode.PingPong;
        source.pitch = pitchAdjustment.Evaluate(timer);

    }

    public void AdjustPitchV2()
    {
        source.pitch = Random.Range(pitchLower, pitchUpper);
    }
    public void PlayAudio()
    {
        source.Play();
    }

    public void StopAudio()
    {
        source.Stop();
    }

    public bool GetAudioPlaying()
    {
        return isAudioPlaying;
    }

    public bool GetAudioPlaying(bool a)
    {
        return isAudioPlaying = a;
    }


}

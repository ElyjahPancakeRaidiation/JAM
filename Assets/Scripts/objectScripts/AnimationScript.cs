using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AnimationScript : MonoBehaviour
{

    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    private SpriteRenderer spriteRend;
    [SerializeField] private CutSceneManager cutsceneManager;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();
        spriteRend = GetComponent<SpriteRenderer>();
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        spriteRend.color = Color.black;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        spriteRend.color = Color.white;
        videoPlayer.Play();
        audioSource.Play();
        cutsceneManager.playCutScene();
    }
}

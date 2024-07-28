using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioEnvelopeSpeaking : MonoBehaviour
{
    public Transform targetTransform;
    public float maxHeightOffset = 15f; // Offset from the initial Y position
    private AudioSource audioSource;
    private float initialYPosition;

    [Header("Testing")]
    public AudioClip testAudio;
    public bool testing = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if(testing)
        {
            AnalyzeAudioClip(testAudio);
        }
    }

    public void AnalyzeAudioClip(AudioClip clip)
    {
        if (targetTransform == null)
        {
            Debug.LogError("Target Transform is not assigned.");
            return;
        }

        if (clip == null)
        {
            Debug.LogError("AudioClip is null.");
            return;
        }

        initialYPosition = targetTransform.position.y; // Set initial Y position at the start of analysis
        audioSource.clip = clip;
        audioSource.Play();
        StartCoroutine(AnalyzeAudioCoroutine(clip.length, clip));
    }

    private IEnumerator AnalyzeAudioCoroutine(float duration, AudioClip clip)
    {
        float[] clipSampleData = new float[1024];
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            audioSource.GetOutputData(clipSampleData, 0);
            float currentAverageVolume = GetCurrentAverageVolume(clipSampleData);

            // Map the volume to the transform's position, starting from initial Y position
            float mappedHeight = Mathf.Lerp(initialYPosition, initialYPosition + maxHeightOffset, currentAverageVolume);
            Vector3 currentPosition = targetTransform.position;
            currentPosition.y = mappedHeight;
            targetTransform.position = currentPosition;

            yield return null;
        }

        audioSource.Stop();
        AnalyzeAudioClip(clip);
    }

    float GetCurrentAverageVolume(float[] data)
    {
        float total = 0;
        foreach (float datum in data)
        {
            total += Mathf.Abs(datum);
        }
        return total / 1024; // Average
    }
}

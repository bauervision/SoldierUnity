using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;
    public AudioClip removeAttachment;
    public AudioClip[] weaponAudioClips; // Array to hold audio clips
    public AudioClip[] equipmentAudioClips; // Array to hold audio clips

    void Awake() { Instance = this; }

    public void PlayRemove() { audioSource.PlayOneShot(removeAttachment); }

    public void PlaySound(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.WEAPON:
                {
                    if (weaponAudioClips.Length > 0)
                    {
                        // Select a random clip from the array
                        int randomIndex = Random.Range(0, weaponAudioClips.Length);
                        AudioClip clipToPlay = weaponAudioClips[randomIndex];

                        // Play the selected clip
                        audioSource.PlayOneShot(clipToPlay);
                    }
                    else
                    {
                        Debug.LogWarning("No weapon audio clips available in the AudioManager.");
                    }
                    break;
                }

            default:
                {
                    if (equipmentAudioClips.Length > 0)
                    {
                        // Select a random clip from the array
                        int randomIndex = Random.Range(0, equipmentAudioClips.Length);
                        AudioClip clipToPlay = equipmentAudioClips[randomIndex];

                        // Play the selected clip
                        audioSource.PlayOneShot(clipToPlay);
                    }
                    else
                    {
                        Debug.LogWarning("No Equipment audio clips available in the AudioManager.");
                    }
                    break;
                }
        }

    }
}

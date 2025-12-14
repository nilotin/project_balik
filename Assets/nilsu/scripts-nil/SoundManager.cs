using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource uiSource;
    public AudioSource sfxSource;
    public AudioSource ambienceSource;

    [Header("UI Clips")]
    public AudioClip uiOpen;
    public AudioClip uiClose;
    public AudioClip uiHover;
    public AudioClip uiError;

    [Header("Gameplay Clips")]
    public AudioClip powerUp;
    public AudioClip hit;
    public AudioClip enemyAttack;
    public AudioClip death;
    
    [Header("Fish Pickup")]
    public AudioClip fishPickup;
    [Range(0.8f, 1.2f)] public float fishPitchMin = 0.95f;
    [Range(0.8f, 1.2f)] public float fishPitchMax = 1.1f;
    [Header("Power Up Pickup")]
    public AudioClip powerUpPickup;
    [Range(0.9f, 1.3f)] public float powerUpPitchMin = 1.0f;
    [Range(0.9f, 1.3f)] public float powerUpPitchMax = 1.2f;
    [Header("Damage / Hit")]
    public AudioClip damageHit;
    [Range(0.8f, 1.2f)] public float damagePitchMin = 0.9f;
    [Range(0.8f, 1.2f)] public float damagePitchMax = 1.1f;
    [Header("Ambience")]
    public AudioClip oceanAmbience;
    [Range(0f, 1f)] public float ambienceVolume = 0.3f;





    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- UI ----------
    public void PlayUI(AudioClip clip)
    {
        if (clip) uiSource.PlayOneShot(clip);
    }

    // ---------- GAMEPLAY ----------
    public void PlaySFX(AudioClip clip)
    {
        if (clip) sfxSource.PlayOneShot(clip);
    }

    // ---------- AMBIENCE ----------
    public void PlayAmbience(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        ambienceSource.clip = clip;
        ambienceSource.loop = loop;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }
    
    public void PlayFishPickup()
    {
        if (fishPickup == null || sfxSource == null) return;

        float originalPitch = sfxSource.pitch;
        sfxSource.pitch = Random.Range(fishPitchMin, fishPitchMax);
        sfxSource.PlayOneShot(fishPickup);
        sfxSource.pitch = originalPitch;
    }
    public void PlayPowerUpPickup()
    {
        if (powerUpPickup == null || sfxSource == null) return;

        float originalPitch = sfxSource.pitch;
        sfxSource.pitch = Random.Range(powerUpPitchMin, powerUpPitchMax);
        sfxSource.PlayOneShot(powerUpPickup);
        sfxSource.pitch = originalPitch;
    }
    public void PlayDamage()
    {
        if (damageHit == null || sfxSource == null) return;

        float originalPitch = sfxSource.pitch;
        sfxSource.pitch = Random.Range(damagePitchMin, damagePitchMax);
        sfxSource.PlayOneShot(damageHit);
        sfxSource.pitch = originalPitch;
    }
    
    public void PlayAmbience()
    {
        if (ambienceSource == null || oceanAmbience == null) return;

        ambienceSource.clip = oceanAmbience;
        ambienceSource.loop = true;
        ambienceSource.volume = ambienceVolume;
        ambienceSource.Play();
    }


}
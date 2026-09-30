using UnityEngine;

namespace CubeTMG.Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private AudioSource audioSource;

        private AudioClip shootClip;
        private AudioClip spellbombClip;
        private AudioClip hitEnemyClip;
        private AudioClip playerHurtClip;
        private AudioClip levelUpClip;
        private AudioClip potionDrinkClip;
        private AudioClip whiteBagClip;
        private AudioClip nexusClip;
        private AudioClip enemyShootClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D clean sound

            GenerateAllAudioClips();
        }

        private void GenerateAllAudioClips()
        {
            shootClip = CreateTone(44100, 0.09f, 600f, 250f, 0.25f, WaveType.Square);
            enemyShootClip = CreateTone(44100, 0.12f, 320f, 120f, 0.2f, WaveType.Sawtooth);
            spellbombClip = CreateNoiseBurst(44100, 0.35f, 0.5f);
            hitEnemyClip = CreateTone(44100, 0.06f, 350f, 180f, 0.18f, WaveType.Triangle);
            playerHurtClip = CreateTone(44100, 0.14f, 180f, 60f, 0.35f, WaveType.NoiseBlend);
            levelUpClip = CreateArpeggio(44100, new float[] { 261.63f, 329.63f, 392.00f, 523.25f, 659.25f }, 0.08f, 0.35f);
            potionDrinkClip = CreateTone(44100, 0.18f, 300f, 750f, 0.25f, WaveType.Sine);
            whiteBagClip = CreateArpeggio(44100, new float[] { 523.25f, 659.25f, 783.99f, 1046.50f, 1318.51f }, 0.12f, 0.45f);
            nexusClip = CreateTone(44100, 0.4f, 200f, 880f, 0.3f, WaveType.Sine);
        }

        private enum WaveType { Sine, Square, Sawtooth, Triangle, NoiseBlend }

        private AudioClip CreateTone(int sampleRate, float duration, float startFreq, float endFreq, float volume, WaveType type)
        {
            int numSamples = Mathf.RoundToInt(sampleRate * duration);
            float[] samples = new float[numSamples];

            float phase = 0f;
            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, t);
                phase += 2f * Mathf.PI * currentFreq / sampleRate;

                float sample = 0f;
                switch (type)
                {
                    case WaveType.Sine:
                        sample = Mathf.Sin(phase);
                        break;
                    case WaveType.Square:
                        sample = Mathf.Sin(phase) >= 0 ? 0.8f : -0.8f;
                        break;
                    case WaveType.Sawtooth:
                        sample = (2f * (phase / (2f * Mathf.PI) - Mathf.Floor(phase / (2f * Mathf.PI) + 0.5f)));
                        break;
                    case WaveType.Triangle:
                        sample = Mathf.PingPong(phase / Mathf.PI, 1f) * 2f - 1f;
                        break;
                    case WaveType.NoiseBlend:
                        sample = Mathf.Lerp(Mathf.Sin(phase), (Random.value * 2f - 1f), 0.5f);
                        break;
                }

                // Envelope: quick attack, smooth linear decay
                float envelope = (1f - t);
                samples[i] = sample * envelope * volume;
            }

            AudioClip clip = AudioClip.Create("GeneratedTone", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateNoiseBurst(int sampleRate, float duration, float volume)
        {
            int numSamples = Mathf.RoundToInt(sampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float noise = Random.value * 2f - 1f;
                float envelope = Mathf.Pow(1f - t, 2.5f);
                samples[i] = noise * envelope * volume;
            }

            AudioClip clip = AudioClip.Create("GeneratedNoise", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateArpeggio(int sampleRate, float[] freqs, float noteDuration, float volume)
        {
            int noteSamples = Mathf.RoundToInt(sampleRate * noteDuration);
            int totalSamples = noteSamples * freqs.Length;
            float[] samples = new float[totalSamples];

            for (int noteIdx = 0; noteIdx < freqs.Length; noteIdx++)
            {
                float freq = freqs[noteIdx];
                float phase = 0f;
                for (int i = 0; i < noteSamples; i++)
                {
                    int overallIdx = noteIdx * noteSamples + i;
                    phase += 2f * Mathf.PI * freq / sampleRate;
                    float t = (float)i / noteSamples;
                    float envelope = 1f - t * 0.7f;
                    samples[overallIdx] = Mathf.Sin(phase) * envelope * volume;
                }
            }

            AudioClip clip = AudioClip.Create("GeneratedArp", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        public void PlayShoot(Vector3 pos) => PlayOneShot(shootClip, pos, 0.4f);
        public void PlayEnemyShoot(Vector3 pos) => PlayOneShot(enemyShootClip, pos, 0.35f);
        public void PlaySpellbomb(Vector3 pos) => PlayOneShot(spellbombClip, pos, 0.7f);
        public void PlayHitEnemy(Vector3 pos) => PlayOneShot(hitEnemyClip, pos, 0.35f);
        public void PlayPlayerHurt() => PlayOneShot(playerHurtClip, Camera.main ? Camera.main.transform.position : Vector3.zero, 0.6f);
        public void PlayLevelUp() => PlayOneShot(levelUpClip, Camera.main ? Camera.main.transform.position : Vector3.zero, 0.7f);
        public void PlayPotionDrink() => PlayOneShot(potionDrinkClip, Camera.main ? Camera.main.transform.position : Vector3.zero, 0.5f);
        public void PlayWhiteBagDrop(Vector3 pos) => PlayOneShot(whiteBagClip, pos, 0.8f);
        public void PlayNexusRecall() => PlayOneShot(nexusClip, Camera.main ? Camera.main.transform.position : Vector3.zero, 0.6f);

        private void PlayOneShot(AudioClip clip, Vector3 pos, float volume)
        {
            if (clip == null) return;
            audioSource.PlayOneShot(clip, volume);
        }
    }
}

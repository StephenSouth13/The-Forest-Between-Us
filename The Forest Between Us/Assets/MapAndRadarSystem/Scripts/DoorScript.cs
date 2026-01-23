using UnityEngine;

namespace MapAndRadarSystem
{
    public class DoorScript : MonoBehaviour
    {
        private Animation animation;
        public AudioClip audioOpen;
        public AudioClip audioClose;
        private AudioSource audioSource;
        // Start is called before the first frame update
        void Start()
        {
            animation = GetComponent<Animation>();
            audioSource = GetComponent<AudioSource>();
        }

        float lastTimeInteracted = 0;
        public void DoorInteraction(bool open)
        {
            if (Time.time > lastTimeInteracted + 0.5f)
            {
                lastTimeInteracted = Time.time;
                if (open)
                {
                    animation.Play("Open");
                    audioSource.PlayOneShot(audioOpen);
                }
                else
                {
                    animation.Play("Close");
                    audioSource.PlayOneShot(audioClose);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("NPC"))
            {
                DoorInteraction(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("NPC"))
            {
                DoorInteraction(false);
            }
        }
    }
}

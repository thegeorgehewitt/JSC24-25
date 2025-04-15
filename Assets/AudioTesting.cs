using UnityEngine;

using Custom.Manager.Audio;

public class AudioTesting : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AudioManager.PlayMusic(MusicGroup.Background, MusicPlayingFlags.Unique | MusicPlayingFlags.Override);
        }
    }
}

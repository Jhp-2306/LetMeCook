using UnityEngine;

//[RequireComponent (typeof(AudioSource))]
public class ButtonPress : MonoBehaviour
{
    public Vector2 PitchRange = new Vector2(0.8f,1.2f);
    //AudioSource audioSource;
    public AudioClip clip;
    public void onClick()
    {
        //audioSource = GetComponent<AudioSource> ();
        //audioSource.pitch=Random.Range (PitchRange.x, PitchRange.y);
        //audioSource.Play ();
        GameMusicNSoundsDNDL.Instance.PlaySfx(clip, PitchRange.x, PitchRange.y);

    }
}

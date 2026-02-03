using UnityEngine;

public class AnimControll : MonoBehaviour
{
    public Animator anim;

    void Start()
    {
        if(anim == null) anim = GetComponent<Animator>();
    }

    public void PlayBoolAnimation(string name, bool state)
    {
        anim.SetBool(name, state);
    }
    
    public void PlayTriggerAnimation(string name)
    {
        anim.SetTrigger(name);
    }
    public void SetIdle()
    {
        anim.SetBool("IsIdle_Combat", false);
        anim.SetBool("IsWalking", false);
    }
    public void SetUpRotation(float rotationY)
    {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(0, rotationY, 0);
    }
    public void EnableRootMotion(int state)
    {
        anim.applyRootMotion = (state != 0);

    }


}

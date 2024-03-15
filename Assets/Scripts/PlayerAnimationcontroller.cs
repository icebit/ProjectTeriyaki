using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationcontroller : MonoBehaviour
{
    // source https://www.youtube.com/watch?v=Vsj_UpnLFF8

    Animator playerAnim;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        // TODO: use player speed components to change the animation states of the model
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

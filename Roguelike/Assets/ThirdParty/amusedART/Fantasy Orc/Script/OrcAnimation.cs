using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcAnimation : MonoBehaviour {

    Animator anim;


	void Start () {

        anim = GetComponent<Animator>();
		
	}

    public void Idle_Ani() {
        anim.SetTrigger("IDLE");
    }

    public void Run_Ani()
    {
        anim.SetTrigger("RUN");
    }

    public void Attack_Ani()
    {
        anim.SetTrigger("ATTACK"); ;
    }

    public void Skill_Ani()
    {
        anim.SetTrigger("SKILL");
    }

    public void Damage_Ani()
    {
        anim.SetTrigger("DAMAGE");
    }

    public void Stun_Ani()
    {
        anim.SetTrigger("STUN");
    }

    public void Death_Ani()
    {
        anim.SetTrigger("DEATH");
    }


}

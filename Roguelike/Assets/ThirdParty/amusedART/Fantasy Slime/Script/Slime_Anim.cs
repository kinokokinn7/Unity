using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_Anim : MonoBehaviour {

	Animator anim;

	// Use this for initialization
	void Start () {

		anim = GetComponent <Animator> ();
		
	}
	
	public void Idle_Ani ()
	{
		anim.SetTrigger ("IDLE");
	}

	public void Run_Ani ()
	{
		anim.SetTrigger ("RUN");		
	}

	public void Attack_Ani ()
	{
		anim.SetTrigger ("ATTACK");		
	}

	public void Damage_Ani ()
	{
		anim.SetTrigger ("DAMAGE");	
	}

	public void Death_Ani ()
	{
		anim.SetTrigger ("DEATH");		
	}


}

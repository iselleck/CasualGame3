﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

	public float p1Speed = 3; 
	public float p2Speed = 3; 
	public float jumpHeight = 5;
	public float verticalSlow = 2;
	public float startingHealth = 1000;
	public float healthChng = 2.5f;
	public float healthDegn = 0.33f;
	public Slider p1Health, p2Health;

	public static int p1Kills, p2Kills;
	private GameObject p1HitBx, p2HitBx;
	private BoxCollider p1Hit, p2Hit;
	public Text p1Killnum, p2Killnum;
	public float startTimer = 1.5f;

	private GameObject p1, p2;
	public Rigidbody p1Rigidbody, p2Rigidbody;

	private bool jump = true;
	private bool playable = true;
	private float p1RayToGround;
	private float p2RayToGround;

	//BLEED OUT
	public float decomposeRate = 0;
	private float decomposeValue;

	// Use this for initialization
	void Start () {
		p1 = GameObject.FindGameObjectWithTag ("Player1");
		p2 = GameObject.FindGameObjectWithTag ("Player2");
		p1Rigidbody = p1.GetComponent<Rigidbody>();
		p2Rigidbody = p2.GetComponent<Rigidbody>();
		p1RayToGround = p1.GetComponent<BoxCollider>().bounds.extents.y;
		p2RayToGround = p2.GetComponent<BoxCollider>().bounds.extents.y;

		p1HitBx = GameObject.FindGameObjectWithTag ("P1HitBx");
		p2HitBx = GameObject.FindGameObjectWithTag ("P2HitBx");

		p1Health.value = startingHealth;
		p2Health.value = startingHealth;

		p1Hit = p1HitBx.GetComponent<BoxCollider> ();
		p2Hit = p2HitBx.GetComponent<BoxCollider> (); 

		p1Hit.enabled = false;
		p2Hit.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		p1Killnum.text = p1Kills.ToString ();
		p2Killnum.text = p2Kills.ToString ();

		if (playable) {

			 p1 = GameObject.FindGameObjectWithTag ("Player1");
			 p2 = GameObject.FindGameObjectWithTag ("Player2");

			if(p1 != null)
			{
				p1HitBx = GameObject.FindGameObjectWithTag ("P1HitBx");
				p1Hit = p1HitBx.GetComponent<BoxCollider> ();
				p1Rigidbody = p1.GetComponent<Rigidbody>();
			}



			if(p2 != null)
			{
				p2HitBx = GameObject.FindGameObjectWithTag ("P2HitBx");
				p2Hit = p2HitBx.GetComponent<BoxCollider> ();
				p2Rigidbody = p2.GetComponent<Rigidbody>();
			}
		

			if(p1Health.value <= 0 || p2Health.value <= 0){
				roundOver ();
			}

			p1Movement ();
			p2Movement ();
			decomposition();

		}

		if (!playable) {

			SceneManager.LoadScene ("scene1");
		}
	}

	void p1Movement(){
		// player 1 input
		if (p1 != null) {
			if (Input.GetAxis ("Horizontal") < 0) {
				if (p1Grounded ()) {
					p1Health.value += healthChng;
					if (p2Health.value <= startingHealth) {
						p2Health.value -= healthChng;//*healthDegn; 
					}
					p1.transform.position -= new Vector3 (p1Speed * Time.deltaTime, 0f, 0f);
				} else {
					p1.transform.position -= new Vector3 (p1Speed / verticalSlow * Time.deltaTime, 0f, 0f);
				}
			}

			if (Input.GetAxis ("Horizontal") > 0) {
				if (p1Grounded ()) {
					p1Health.value += healthChng;
					if (p2Health.value <= startingHealth) {
						p2Health.value -= healthChng;//*healthDegn; 
					}
					p1.transform.position += new Vector3 (p1Speed * Time.deltaTime, 0f, 0f);
				} else {
					p1.transform.position += new Vector3 (p1Speed / verticalSlow * Time.deltaTime, 0f, 0f);
				}
			}

			if (Input.GetAxis ("Jump") > 0 && p1Grounded ()) {
				p1Rigidbody.AddForce (transform.up * jumpHeight);
			}

			if (Input.GetAxis ("Attack") > 0) {
				attackC (p1Hit);
			} else {
				p1Hit.enabled = false;
			}
		}
			
	
	}
	void p2Movement(){
		if (p2 != null) {
			if (Input.GetAxis ("P2Horizontal") < 0) {
				if (p2Grounded ()) {
					p2Health.value += healthChng;
					if (p1Health.value <= startingHealth) {
						p1Health.value -= healthChng;//*healthDegn; 
					}
					p2.transform.position -= new Vector3 (p2Speed * Time.deltaTime, 0f, 0f);
				} else {
					p2.transform.position -= new Vector3 (p2Speed / verticalSlow * Time.deltaTime, 0f, 0f);
				}
			}
		
			if (Input.GetAxis ("P2Horizontal") > 0) {
				if (p2Grounded ()) {
					p2Health.value += healthChng;
					if (p1Health.value <= startingHealth) {
						p1Health.value -= healthChng;//*healthDegn; 
					}
					p2.transform.position += new Vector3 (p2Speed * Time.deltaTime, 0f, 0f);
				} else {
					p2.transform.position += new Vector3 (p2Speed / verticalSlow * Time.deltaTime, 0f, 0f);
				}
			}

			if (Input.GetAxis ("P2Jump") > 0 && p2Grounded ()) {
				p2Rigidbody.AddForce (transform.up * jumpHeight);
			}

			if (Input.GetAxis ("P2Attack") > 0) {
				attackC (p2Hit);
			} else {
				p2Hit.enabled = false;
			}
		}
	}

	void roundOver(){
		playable = false;
	}
	bool p1Grounded(){
		if (p1 != null) {
			return Physics.Raycast (p1.transform.position, -Vector3.up, p1RayToGround + 1.0f);
		} else {
			return false; 
		}
	}
	bool p2Grounded(){
		if (p2 != null) {

			return Physics.Raycast (p2.transform.position, -Vector3.up, p2RayToGround + 1.0f);
		} else {
			return false;
		}
			//p2Speed = p2Speed / 2;
	}

	void attackC(BoxCollider hitBx){
		hitBx.enabled = true;
	}

	void decomposition(){
		decomposeValue = decomposeRate*Time.deltaTime;
		
		p1Health.value -= decomposeValue;
		p2Health.value -= decomposeValue;

	}

}

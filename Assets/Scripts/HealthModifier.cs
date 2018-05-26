using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModifier : MonoBehaviour {

    public float healthAmount;

    [HideInInspector]
    public bool isHit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Hit() {
        isHit = true;
    }
}

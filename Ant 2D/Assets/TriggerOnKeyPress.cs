using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnKeyPress : MonoBehaviour {

    public string trigger_name;
    public KeyCode[] keys;
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < keys.Length; i++)
        {
            if(Input.GetKeyDown(keys[i]))
            {
                GetComponent<Animator>().SetTrigger(trigger_name);
                enabled = false;
                i = keys.Length;
            }
        }
	}
}

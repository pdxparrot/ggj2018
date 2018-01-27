using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public List<UnityEngine.Camera> cameras;

	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKey("2")) {
            for(int i = 0; i < 4; i++) {
                cameras[i].enabled = i < 2;
            }
            cameras[0].rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
            cameras[1].rect = new Rect(0.5f, 0.0f, 1.0f, 1.0f);
        } else if(Input.GetKey("3")) {
            for(int i = 0; i < 4; i++) {
                cameras[i].enabled = i < 3;
            }
            cameras[0].rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
            cameras[1].rect = new Rect(0.0f, 0.5f, 0.5f, 1.0f);
            cameras[2].rect = new Rect(0.5f, 0.5f, 1.0f, 1.0f);
        } else if(Input.GetKey("4")) {
            for(int i = 0; i < 4; i++) {
                cameras[i].enabled = i < 4;
            }
            cameras[0].rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
            cameras[1].rect = new Rect(0.5f, 0.0f, 1.0f, 0.5f);
            cameras[2].rect = new Rect(0.0f, 0.5f, 0.5f, 1.0f);
            cameras[3].rect = new Rect(0.5f, 0.5f, 1.0f, 1.0f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Bg : MonoBehaviour {

	// Use this for initialization
	void Start () {
        BackgroundManager.GetInstance().OnLoadBackgroundSceneComplete(transform);
	}
}

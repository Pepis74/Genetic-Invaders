using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBack : MonoBehaviour {

	void Start ()
    {
        GetComponent<RectTransform>().SetAsFirstSibling();
	}
}

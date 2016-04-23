using UnityEngine;
using System.Collections;

public class chapterButton : MonoBehaviour {

	public commandButton m_button;
	public simpleLabel m_label;

	public void changeParameter(string para){

		m_button.parameter = para;

	}

	public void changeLabel(string label){

		m_label.label = label;
	}

	public void moveButtonX(float location){
		m_button.targetX = location;
	}

	public void removeButton(){
		Destroy (this.gameObject);
	}

}

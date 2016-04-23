using UnityEngine;
using System.Collections;

public class skillButton : MonoBehaviour {

	public Move move;
	public commandButton m_button;
	public simpleLabel m_namelabel;
	public simpleLabel m_costlabel;
	public battleMain m_battle;

	
	void Start(){
		//These 3 are for testing
//		Move testmove = new Move ();
//		testmove.createMove ("30001001");
//		changeMove (testmove);

		m_namelabel.m_parent = m_button;
		m_namelabel.m_DrawArea.x += m_button.m_DrawArea.width / 20.0f;
		m_namelabel.m_DrawArea.width = m_button.m_DrawArea.width;
		m_namelabel.m_DrawArea.height = m_button.m_DrawArea.height;

		m_costlabel.m_parent = m_button;
		m_costlabel.m_DrawArea.x -= m_button.m_DrawArea.width / 20.0f;
		m_costlabel.m_DrawArea.width = m_button.m_DrawArea.width;
		m_costlabel.m_DrawArea.height = m_button.m_DrawArea.height;

		m_button.speedX = 40.0f;
		m_button.setTargetX(100.0f);
	
	}

	public void changeMove(Move newMove){
		move = newMove;
		m_namelabel.label = "<color=white>" + move.moveName+"</color>";
		m_costlabel.label = "<color=white>"+ move.moveCost +"ap </color>";
	}

	public void sendMove(){
		m_battle.prepareMove (move);
	}

	public void destroyButton(){
		Destroy (this.gameObject);
	}
}

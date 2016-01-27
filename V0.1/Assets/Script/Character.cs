using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	//All the status
	private long uID { get; set; }
	public string name { get; set; }
	public int level{get; set;}
	public int attackPower { get; set; }
	public int magicAttackPower { get; set; }
	public int defense { get; set; }
	public int maxHP { get; set; }
	public int currentHP { get; set; }
	public int maxMP { get; set; }
	public int currentMP { get; set; }
	public int maxAP {get; set;}
	public int currentAP {get; set;}
	public string job { get; set; }
	public string side { get; set; }
	public bool isAlive { get; set; }
	public bool isDisabled { get; set; }
	public string disabilityName { get; set; }
	public int disabilityDuration { get; set; }

	//for GUI purpose
	public int depth;
	public simpleWindow m_parent = null;
	public string m_texturePath;
	public Texture m_texture;
	//special x case required for moving number label damage.
	public float xBeforeResize;
	public Rect m_DrawArea;
	public float alpha;
	public float targetAlpha{get; set;}
	public float alphaspeed {get; set;}
	// 1.0 = alone, 1.5 = duo, 2.0 = trio
	public float partySize = 1.00f;
	//This part for shaking on death
	public float targetX {get; set;}
	public float targetY {get; set;}
	public float defaultX {get; set;}
	public float defaultY {get; set;}
	public float speedX;
	public float speedY;
	private bool shake = false;
	//put whether it's an ENEMY or PLAYER to make it seasier to decide.
	public string characterType = "";
	//For sound
	private AudioSource source;
	public string audioPath;
	public AudioClip audioSE;
	
	void Start()	{
		xBeforeResize = m_DrawArea.x;
		alive ();
		if (m_texturePath != "")
			m_texture = Resources.Load (m_texturePath) as Texture;
		m_DrawArea = resizeGUI (m_DrawArea);
		//Size of the picture is reduced based on size of party (usually for displaying enemies)
		m_DrawArea = resizeParty ();
		defaultX = m_DrawArea.x;
		defaultY = m_DrawArea.y;
	}

	public void drawCharacter(){
		xBeforeResize = m_DrawArea.x;
		alive ();
		if (m_texturePath != "")
			m_texture = Resources.Load (m_texturePath) as Texture;
		m_DrawArea = resizeGUI (m_DrawArea);
		//Size of the picture is reduced based on size of party (usually for displaying enemies)
		m_DrawArea = resizeParty ();
		defaultX = m_DrawArea.x;
		defaultY = m_DrawArea.y;
	}

	void OnGUI(){
		
		if(m_parent != null) GUILayout.BeginArea(m_parent.getContentRect());
		GUI.depth = depth;
		GUI.color = new Color(1,1,1,alpha);
		GUI.DrawTexture (resizeParty (), m_texture);
		GUI.color = Color.white;
		if(m_parent != null) GUILayout.EndArea ();
	}

	void Update(){
		setOpacity ();
		setLocation ();
		if (currentHP <= 0 && isAlive == true) {
			die ();
		}
		if (isAlive == false && shake == false) {
//			StartCoroutine(shaking());
		}
	}

	public void setOpacity(){
		if (alpha < targetAlpha) {
			alpha += alphaspeed;
			if (alpha > targetAlpha)
				alpha = targetAlpha;
		}
		
		if (alpha > targetAlpha) {
			alpha -= alphaspeed;
			if (alpha < targetAlpha)
				alpha = targetAlpha;
		}
		
	}

	//Move the texture around on death or on attack.
	public void setLocation(){
		if (m_DrawArea.x <= targetX) {
			m_DrawArea.x += speedX;
		}

		if (m_DrawArea.x > targetX) {
			m_DrawArea.x -= speedX;
		}

		if (m_DrawArea.y <= targetY) {
			m_DrawArea.y += speedY;
		}

		if (m_DrawArea.y > targetY) {
			m_DrawArea.y -= speedY;
		}
	}

	public IEnumerator shaking(){
		targetX = defaultX + recalculateX(30.0f);
		yield return new WaitForSeconds (0.05f);
		targetX = defaultX - recalculateX (30.0f);
		yield return new WaitForSeconds (0.05f);
		if (alpha == 0) {
			targetX = defaultX;
			shake = true;
		}
	}

	public IEnumerator getHit(){
		speedX = recalculateX (10.0f);
		targetX = defaultX + recalculateX(30.0f);
		yield return new WaitForSeconds (0.1f);
		targetX = defaultX - recalculateX (30.0f);
		yield return new WaitForSeconds (0.1f);
		targetX = defaultX;
	}

	public void fadeAway (){
		alphaspeed = 0.015f;
		targetAlpha = 0.00f;
		speedX = recalculateX (20.0f);
	}

	public void fadeIn (){
		alphaspeed = 0.1f;
		targetAlpha = 1.00f;
	}

	public Rect resizeParty(){

		float newRectHeight = m_DrawArea.width / partySize;
		float newRectWidth = m_DrawArea.height / partySize;
		return new Rect (m_DrawArea.x , m_DrawArea.y, newRectHeight, newRectWidth);
	}
	public void die()	{
		isAlive = false;
		speedY = recalculateY(2.0f);
		targetY = defaultY + recalculateY (70.0f);
		if (side == "Enemy" && job == "Monster") {
			playSound ("Audio/se/monDie");
			fadeAway ();
		}
		Destroy(this.gameObject,1.5f);
	}
	
	public void alive()	{
		isAlive = true;
		if (side == "Enemy" && job == "Monster") 
			fadeIn ();
		
	}
	
	public void isHurt(int hurt){
		currentHP -= hurt;
		if (side == "Enemy")
			StartCoroutine (getHit ());
	}
	
	public void isHeal(int heal)	{
		//Prevent overhealing
		if (( currentHP + heal) > maxHP)
			heal = maxHP - currentHP;

		currentHP += heal;

	}
	
	public void isCure()	{

		isDisabled = false;
	}

	public Rect resizeGUI(Rect Drect){
		
		float widthScale = Screen.width / 800.00f;
		float heightScale = Screen.height / 1280.000f;
		
		float rectWidth = widthScale * Drect.width;
		float rectHeight = heightScale * Drect.height;
		
		float rectX = Drect.x * widthScale;
		float rectY = Drect.y * heightScale;
		
		return new Rect (rectX, rectY, rectWidth, rectHeight);
	}

	public float recalculateX( float x){

		return x*Screen.width / 1600.00f;
	}

	public float recalculateY( float y){
		
		return y*Screen.height / 2560.000f;
	}

	public Rect getContentRect(){

		return resizeParty ();
	}

	public float getRectX(){

		return m_DrawArea.x;
	}

	public float getOpacity(){
		return alpha;
	}

	public void playSound(string path){
		audioPath = path;
		if (audioPath != "") {
			source = GetComponent<AudioSource>();
			audioSE = Resources.Load (audioPath) as AudioClip;
		}
		source.PlayOneShot (audioSE);
	}	

}

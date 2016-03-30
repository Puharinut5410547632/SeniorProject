using UnityEngine;
using System.Collections;
using System.IO;

public class CStringList : MonoBehaviour {
	/// <summary>
	/// テキストファイルのパス(Inspectorで指定しておくとAwakeでLoadしにいきます)
	/// </summary>
	public string filePath = null;
	public string[] filePathList = new string[0];
	
	/// <summary>
	/// Loadが実行されているかどうか
	/// </summary>
	/// <value>
	/// Loadが実行され成功していればtrue
	/// </value>
	public bool isLoad { get { return isLoad_; } }
	
	/// <summary>
	/// キーと値のセットの文字列
	/// </summary>
	private Hashtable stringList = new Hashtable();
	
	/// <summary>
	/// Loadが実行されているかどうか
	/// </summary>
	private bool isLoad_ = false;

	private string lastLoadPath = "";
	
	protected bool IsComment( string line ) {
		string[] headCommentIdentify = {
			"#",
			"//"
		};
		foreach ( string commentMark in headCommentIdentify ) {
			int commentIndex = line.IndexOf( commentMark );
			if ( commentIndex >= 0 ) {
				string checkString = line.Substring( 0, commentIndex + commentMark.Length );
				checkString = checkString.Trim();
				if ( checkString.Equals( commentMark ) ) {
					return true;
				}
			}
		}
		return false;
	}
	
	public bool Load( string path ) {
		return Load( path, true );
	}
	
	public bool LoadAdditive( string path ) {
		return Load( path, false );
	}
	
	/// <summary>
	/// pathで指定されたファイルからキーと値のセットになっている文字列を読み込む
	/// </summary>
	/// <param name='path'>
	/// キーと値がセットになっているテキストファイルのパス
	/// </param>
	protected bool Load( string path, bool isClear ) {
		isLoad_ = false;
		lastLoadPath = path;

		TextAsset original = Resources.Load(path) as TextAsset;
		if ( original == null ) {
			Debug.Log( "[CStringList] resource not found (" + path + ")" );
			return false;
		}
		
		TextAsset textAsset = (TextAsset)Instantiate( original );
		if ( textAsset == null ) {
			Debug.Log( "[CStringList] resource cant instantiate (" + path + ")" );
			Resources.UnloadAsset( original );
			return false;
		}

		isLoad_ = init(textAsset.text, isClear);
		Resources.UnloadAsset(original);

		return isLoad_;
	}
		
	public bool init(string text, bool isClear)
	{
		if (isClear)
		{
			stringList.Clear();
		}

		StringReader reader = new StringReader( text );
		string key = null;
		string value = null;
		string line = null;
		bool isSearchValue = false;
		string firstQuatation = "\"";
		while ( ( line = reader.ReadLine() ) != null ) {
			if ( isSearchValue ) {
				int foundLastQuateIndex = line.LastIndexOf( firstQuatation );
				if ( foundLastQuateIndex != -1 ) {
					value += "\n";
					value += line.Substring(0, foundLastQuateIndex);
					if ( stringList.ContainsKey( key ) )
					{
						Debug.Log( "key[" + key + "] already add key [value:" + value + "] [already value:" + stringList[key] + "]: " + gameObject.name );
					}
					else
					{
						stringList.Add( key, value );
					}
					key = null;
					value = null;
					isSearchValue = false;
				}
				else {
					value += "\n";
					value += line;
				}
			}
			else {
				// コメント対応
				if ( IsComment( line ) ) {
					continue;
				}
				int foundEqualIndex = line.IndexOf("=");
				if ( foundEqualIndex != -1 ) {
					key = line.Substring( 0, foundEqualIndex );
					key = key.Trim();
					
					isSearchValue = true;
					int foundDoubleQuateIndex = line.IndexOf( "\"", foundEqualIndex );
					int foundSingleQuateIndex = line.IndexOf( "'", foundEqualIndex );
					int foundQuateIndex = -1;
					if ( foundSingleQuateIndex != -1 && ( foundDoubleQuateIndex == -1 ? true : foundDoubleQuateIndex > foundSingleQuateIndex ) ) {
						firstQuatation = "'";
						foundQuateIndex = foundSingleQuateIndex;
					}
					else {
						firstQuatation = "\"";
						foundQuateIndex = foundDoubleQuateIndex;
					}
					if ( foundQuateIndex != -1 ) {
						int foundLastQuateIndex = line.LastIndexOf( firstQuatation );
						if ( foundLastQuateIndex != 1 && foundLastQuateIndex != foundQuateIndex) {
							value = line.Substring( foundQuateIndex + 1, foundLastQuateIndex - foundQuateIndex - 1 );
							isSearchValue = false;
							if ( stringList.ContainsKey( key ) )
							{
								Debug.Log( "key[" + key + "] already add key: " + gameObject.name);
							}
							else
							{
								stringList.Add( key, value );
							}
							key = null;
							value = null;
						}
						else {
							value = line.Substring( foundQuateIndex + 1 );
						}
					}
					else {
						value = "";
					}
				}
			}
		}
		reader.Close();
		if ( key != null || value != null ) {
			Debug.Log("[CStringList] error occur something(in " + lastLoadPath + ")\n key is " + key + "\n value is " + value);
			return false;
		}
		return true;
	}
	

	public string GetText( string key) {
/*		CustomDebug.Log("key: " + key);
		foreach (string str in stringList.Keys)
		{
			CustomDebug.Log(str);
		}*/
		if (stringList.ContainsKey(key))
		{
			return stringList[key] as string;
		}
//		Debug.Log("[CStringList] text not found (" + key + ")");
		return "0";
	}

	// 2012/10/04	五十島	キーの一覧を取得	.
	public ICollection getKeys()
	{
		return stringList.Keys;
	}

	/// <summary>
	/// キーが存在しているか調べる
	/// (中身の文字列がちゃんとあるかどうかは別)
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public bool isEnableKey(string checkKey)
	{
		ICollection keys = getKeys();
		if (keys == null) return false;
		foreach (string key in keys)
		{
			if (key.Equals(checkKey))			// 同じものがあったら有効.
			{
				return true;
			}
		}
		return false;
	}

	public void reload()
	{
		bool isLoadLocal = isLoad;
		// ↓filePathがnullの時には読み込まない 2012/11/08 kikuchi 
		if (string.IsNullOrEmpty( filePath ) == false && !isLoad)	// 2012/10/03	五十島	Awake時は読み込まれなかったときのみ読み込む.
		{
			if (Load(filePath) == false)
			{
				Debug.Log("[CStringList] failed load");
			}
		}
		if (filePathList != null && !isLoadLocal)
		{
			foreach (string path in filePathList)
			{
				if (string.IsNullOrEmpty(path) == false)
				{
					if (LoadAdditive(path) == false)
					{
						Debug.Log("[CStringList] failed load(" + path + ")");
					}
				}
			}
		}

	}

	void Awake() {
		reload();
	}
	
	void Start() {
		// GetTextにアクセスできさえすればよく、UpdateやOnGUIが呼ばれる必要がないため.
		enabled = false;
	}
}

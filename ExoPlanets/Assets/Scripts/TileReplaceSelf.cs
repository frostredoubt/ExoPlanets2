using UnityEngine;

/// <summary>
/// Represents that tiles are initially empty prefabs which destroy themselves and replace themsleves with a true prefab.
/// This allows dynamism and nested prefabs to continue updating whereas they normally would not be able to because Unity
/// makes some weird decisions about object hierarchy splitting.
/// </summary>
public class TileReplaceSelf : MonoBehaviour
{

    /// <summary>
    /// The actual prefab to associate with a tile.
    /// </summary>
	public GameObject prefab;

	/// <summary>
	/// During awake, destroy this tile and instead instantiate another tile underneath its parent.
	/// </summary>
	void Awake()
    {
		Transform parentTransform = this.transform.parent;
		GameObject replacement = Instantiate(prefab, this.transform.position, this.transform.rotation) as GameObject;        
		replacement.transform.parent = parentTransform;
		Destroy(this.gameObject);
        return;
	}
	
}

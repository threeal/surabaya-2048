using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour
{
	public int level_ = 1;
	public GameObject model_ = null;
	public bool upgrade_ = false;
	public bool disable_ = false;
	public bool busy_ = false;	
	public float move_speed_ = 5.0f;
	public Vector2Int tile_pos_ = new Vector2Int(0, 0);

	public Landmark target_merge_ = null;

	public void Move ()
	{
		Vector3 target = PosToWorld(tile_pos_);
		float distance = Vector3.Distance(target, transform.localPosition);
		if (distance <= move_speed_ * Time.deltaTime) {
			busy_ = false;
			transform.localPosition = target;
			if (target_merge_) {
				target_merge_.disable_ = true;
				upgrade_ = true;
			}
		}
		else {
			busy_ = true;
			transform.localPosition += (target - transform.localPosition).normalized * move_speed_ * Time.deltaTime;
		}
	}
	
	public Vector3 PosToWorld(Vector2Int pos)
	{
		Vector3 world;
		world.x = ((float)pos.x - (1.5f)) * 1.0f;
		world.y = 0;
		world.z = ((float)pos.y - (1.5f)) * 1.0f;
		return world;
	}
}

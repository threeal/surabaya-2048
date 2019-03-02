using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour
{
	public int level_ = 1;
	public GameObject model_ = null;
	public bool upgrade_ = false;
	public bool busy_ = true;
	public float move_speed_ = 5.0f;
    Vector3 acceleration_ = new Vector3(0, 0, 0);
	public Vector2Int tile_pos_ = new Vector2Int(0, 0);

	public Landmark target_merge_ = null;
    
    public void Move ()
	{
        if (transform.localPosition.y > 0) {
            acceleration_.y -= 2 * Time.deltaTime;
            Vector3 local_position = transform.localPosition;
            local_position.y += acceleration_.y;

            if (local_position.y < 0.5) {
                if (target_merge_)
                {
                    Vector3 target_position = target_merge_.transform.localPosition;
                    target_position.y = local_position.y - 0.5f;

                    target_merge_.transform.localPosition = target_position;
                }
            }

            if (local_position.y < 0) {
                local_position.y = 0;
                acceleration_.y = 0;

                if (target_merge_)
                    Destroy(target_merge_.gameObject);
            }
        
            transform.localPosition = local_position;
        }
        else {
            Vector3 target = PosToWorld(tile_pos_);
            float distance = Vector3.Distance(target, transform.localPosition);
            if (distance <= move_speed_ * Time.deltaTime)
            {
                busy_ = false;
                transform.localPosition = target;
                if (target_merge_)
                {
                    Destroy(target_merge_.gameObject);
                    upgrade_ = true;
                }
            }
            else
            {
                busy_ = true;
                transform.localPosition += (target - transform.localPosition).normalized * move_speed_ * Time.deltaTime;
            }
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

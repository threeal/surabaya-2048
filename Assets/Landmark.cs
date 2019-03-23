using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour
{
	public int level_ = 1;
	public GameObject model_ = null;
	public bool busy_ = true;
	public float move_speed_ = 5.0f;
    Vector3 acceleration_ = new Vector3(0, 0, 0);

    public Landmarks landmarks_ = null;
    public List<Landmark> target_ = new List<Landmark>();

    public void Move (float mul)
	{
        if (transform.localPosition.y > 0)
        {
            busy_ = true;
            acceleration_.y -= 2 * Time.deltaTime * mul;
            Vector3 local_position = transform.localPosition;
            local_position.y += acceleration_.y;

            if (local_position.y < 0.5)
            {
                foreach (Landmark landmark in target_)
                {
                    Destroy(landmark.gameObject);
                }
                target_.Clear();
            }

            if (local_position.y < 0)
            {
                local_position.y = 0;
                acceleration_.y = 0;
            }
        
            transform.localPosition = local_position;
        }
        else
        {
            Vector3 target = landmarks_.transform.position;
            float distance = Vector3.Distance(target, transform.position);
            if (distance <= move_speed_ * Time.deltaTime * mul)
            {
                busy_ = false;
                transform.position = target;
            }
            else
            {
                busy_ = true;
                transform.position += (target - transform.position).normalized * move_speed_ * Time.deltaTime * mul;
            }
        }
	}
}

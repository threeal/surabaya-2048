using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour
{
	public int level_ = 1;
	public GameObject model_ = null;
    MeshRenderer model_renderer_ = null;
    public GameObject number_ = null;
    MeshRenderer number_renderer_ = null;
	public bool busy_ = true;
	public float move_speed_ = 5.0f;
    Vector3 acceleration_ = new Vector3(0, 0, 0);

    public Landmarks landmarks_ = null;
    public List<Landmark> target_ = new List<Landmark>();

    public void SetModel(GameObject model)
    {
        model_ = model;
        model_renderer_ = model_.GetComponentInChildren<MeshRenderer>();
    }

    public void SetNumber(GameObject number)
    {
        number_ = number;
        number_renderer_ = number_.GetComponentInChildren<MeshRenderer>();
    }

    public void SetAlpha(float alpha)
    {
        if (!model_renderer_ || !number_renderer_)
            return;

        // Color model_color = model_renderer_.material.color;
        // Color number_color = number_renderer_.material.color;

        // model_color.a = alpha;
        // number_color.a = 1 - alpha;

        // model_renderer_.material.color = model_color;
        // number_renderer_.material.color = number_color;

        if (alpha > 0.5)
        {
            model_.SetActive(true);
            number_.SetActive(false);
        }
        else
        {
            model_.SetActive(false);
            number_.SetActive(true);
        }
    }

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

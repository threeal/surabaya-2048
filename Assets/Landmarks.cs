using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmarks : MonoBehaviour
{
    public List<Landmark> landmark_list_ = new List<Landmark>();
    public Vector2Int tile_pos_ = new Vector2Int(0, 0);

    public bool merged_ = false;

    public void Move(float mul)
    {
        foreach (Landmark landmark in landmark_list_)
        {
            landmark.Move(mul);
        }
    }

    public int Level()
    {
        int sum = 0;
        foreach (Landmark landmark in landmark_list_)
        {
            sum += (int)Mathf.Pow(2, landmark.level_);
        }

        return sum;
    }

    public bool Empty()
    {
        return (landmark_list_.Count == 0);
    }

    public bool Busy()
    {
        foreach (Landmark landmark in landmark_list_)
        {
            if (landmark.busy_)
                return true;
        }

        return false;
    }

    public void MoveLandmarks(Landmarks landmarks)
    {
        foreach (Landmark landmark in landmark_list_)
        {
            landmark.landmarks_ = landmarks;
            landmark.transform.SetParent(landmarks.transform);
            landmarks.landmark_list_.Add(landmark);
        }

        landmarks.merged_ = merged_;
        merged_ = false;

        landmark_list_.Clear();
    }

    public void AddLandmark(Landmark landmark)
    {
        landmark.landmarks_ = this;
        landmark_list_.Add(landmark);

        landmark.transform.SetParent(transform);
        landmark.transform.localPosition = new Vector3(0f, 5f, 0f);
    }

    public void ClearLandmark()
    {
        landmark_list_.Clear();
    }

    public void UpgradeLandmark(Landmark upgraded)
    {
        foreach (Landmark landmark in landmark_list_)
        {
            upgraded.target_.Add(landmark);
        }

        ClearLandmark();
        AddLandmark(upgraded);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public GameObject landmarks_prefab_;
	public GameObject landmark_prefab_;
    public GameObject[] model_prefab_;
	Landmarks[,] landmarks_map_ = new Landmarks[4,4];
	bool new_turn_ = false;
    int highest_level_ = 1;
    bool is_game_over_ = false;
    int score_ = 0;
    float mul = 1;

    public GameObject ui_game_over_;
    public Text ui_score_;

	void Start ()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Landmarks landmarks = landmarks_map_[i , j] = Instantiate(landmarks_prefab_).GetComponent<Landmarks>();
                landmarks.transform.SetParent(transform);
                landmarks.tile_pos_ = new Vector2Int(i, j);

                Vector3 local_position = PosToWorld(landmarks.tile_pos_);
                landmarks.transform.localPosition = local_position;
            }
        }


        int count = Random.Range(3, 6);
        for (int i = 0; i < count; i++)
        {
            Landmarks landmarks;
            do
            {
                landmarks = landmarks_map_[Random.Range(0, 4), Random.Range(0, 4)];
            }
            while (!landmarks.Empty());

            landmarks.AddLandmark(CreateLandmark(Random.Range(1, 3)));
        }
	}

    void Update()
    {
        GameUpdate();
	}

    void GameUpdate()
    {
        bool busy = false;
        int score = 0;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (mul < 3f)
                mul += 0.02f;
        }
        else
        {
            if (mul > 1f)
                mul -= 0.1f;
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Landmarks landmarks = landmarks_map_[i, j];
                
                landmarks.merged_ = false;

                foreach (Landmark landmark in landmarks.landmark_list_)
                {
                    landmark.Move(mul);
                }

                if (!landmarks.Busy() && landmarks.landmark_list_.Count > 1)
                {
                    int level = (int)Mathf.Log(landmarks.Level(), 2);
                    landmarks.UpgradeLandmark(CreateLandmark(level));
                }

                score += landmarks.Level() * 5;
                
                busy = busy || landmarks.Busy();
            }
        }

        if (score > score_)
        {
            score_ = score;
            ui_score_.text = score_.ToString();
        }

        if (busy)
        {
            return;
        }

        if (is_game_over_)
        {
            if (!ui_game_over_.activeInHierarchy)
            {
                ui_game_over_.SetActive(true);
                ui_score_.gameObject.SetActive(false);
                return;
            }

            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(0);
            }
            return;
        }

        if (new_turn_)
        {
            new_turn_ = false;
            
            List<Landmarks> empty_landmarks = new List<Landmarks>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Landmarks landmarks = landmarks_map_[i, j];

                    if (landmarks.Empty())
                    {
                        empty_landmarks.Add(landmarks);
                    }
                }
            }

            float max_step = 0;
            for (int i = 0; i < highest_level_; i++)
            {
                max_step += i * i * i;
            }

            float value = Random.value;
            float step = 0;
            int level = 1;
            for (int i = 0; i < highest_level_; i++)
            {
                step += i * i * i;
                if (value <= step / max_step)
                {
                    level = highest_level_ - i;
                    break;
                }
            }

            empty_landmarks[Random.Range(0, empty_landmarks.Count)].AddLandmark(CreateLandmark(level));

            if (empty_landmarks.Count <= 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Landmarks landmark = landmarks_map_[i, j];
                        Landmarks next = landmarks_map_[i, j + 1];
                        if (landmark.Level() == next.Level())
                            return;
                    }

                    for (int j = 3; j > 0; j--)
                    {
                        Landmarks landmark = landmarks_map_[i, j];
                        Landmarks next = landmarks_map_[i, j - 1];
                        if (landmark.Level() == next.Level())
                            return;
                    }
                }

                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Landmarks landmark = landmarks_map_[i, j];
                        Landmarks next = landmarks_map_[i + 1, j];
                        if (landmark.Level() == next.Level())
                            return;
                    }

                    for (int i = 3; i > 0; i--)
                    {
                        Landmarks landmark = landmarks_map_[i, j];
                        Landmarks next = landmarks_map_[i - 1, j];
                        if (landmark.Level() == next.Level())
                            return;
                    }
                }

                is_game_over_ = true;
                return;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 2; j >= 0; j--)
                {
                    Landmarks landmarks = landmarks_map_[i, j];
                    if (landmarks.Empty())
                        continue;

                    Landmarks next = landmarks_map_[i, j + 1];
                    if (next.Empty() || next.Level() == landmarks.Level())
                    {
                        if (next.Level() == landmarks.Level())
                        {
                            if (landmarks.merged_ || next.merged_)
                                continue;
                            else
                                landmarks.merged_ = true;
                        }
                    
                        landmarks.MoveLandmarks(next);
                        new_turn_ = true;
                        j = 3;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    Landmarks landmarks = landmarks_map_[i, j];
                    if (landmarks.Empty())
                        continue;

                    Landmarks next = landmarks_map_[i, j - 1];
                    if (next.Empty() || next.Level() == landmarks.Level())
                    {
                        if (next.Level() == landmarks.Level())
                        {
                            if (landmarks.merged_ || next.merged_)
                                continue;
                            else
                                landmarks.merged_ = true;
                        }
                    
                        landmarks.MoveLandmarks(next);
                        new_turn_ = true;
                        j = 0;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            for (int j = 0; j < 4; j++)
            {
                for (int i = 1; i < 4; i++)
                {
                    Landmarks landmarks = landmarks_map_[i, j];
                    if (landmarks.Empty())
                        continue;

                    Landmarks next = landmarks_map_[i - 1, j];
                    if (next.Empty() || next.Level() == landmarks.Level())
                    {
                        if (next.Level() == landmarks.Level())
                        {
                            if (landmarks.merged_ || next.merged_)
                                continue;
                            else
                                landmarks.merged_ = true;
                        }

                        landmarks.MoveLandmarks(next);
                        new_turn_ = true;
                        i = 0;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int j = 0; j < 4; j++)
            {
                for (int i = 2; i >= 0; i--)
                {
                    Landmarks landmarks = landmarks_map_[i, j];
                    if (landmarks.Empty())
                        continue;

                    Landmarks next = landmarks_map_[i + 1, j];
                    if (next.Empty() || next.Level() == landmarks.Level())
                    {
                        if (next.Level() == landmarks.Level())
                        {
                            if (landmarks.merged_ || next.merged_)
                                continue;
                            else
                                landmarks.merged_ = true;
                        }

                        landmarks.MoveLandmarks(next);
                        new_turn_ = true;
                        i = 3;
                    }
                }
            }
        }
    }

    Landmark CreateLandmark(int level)
    {
        if (level > highest_level_)
            highest_level_ = level;

        Landmark landmark = Instantiate(landmark_prefab_).GetComponent<Landmark>();
        landmark.level_ = level;
        landmark.model_ = Instantiate(model_prefab_[landmark.level_ - 1]);
        landmark.model_.transform.SetParent(landmark.transform);
        landmark.model_.transform.localPosition = Vector3.zero;

        return landmark;
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

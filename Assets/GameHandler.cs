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
    public GameObject[] number_prefab_;
	Landmarks[,] landmarks_map_ = new Landmarks[4,4];
	bool new_turn_ = false;
    int highest_level_ = 1;
    bool is_game_over_ = false;
    bool is_paused_ = false;
    bool is_game_ = false;
    int score_ = 0;
    float mul = 1;

    float alpha_timer_ = 0;
    
    float view_y_ = 30;

    public GameObject camera_pan_;
    public GameObject ui_pause_;
    public GameObject ui_game_over_;
    public GameObject ui_game_;
    public GameObject ui_menu_;

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

        MenuStart();
    }

    void Update()
    {
        if (is_game_)
        {
            GameUpdate();
        }
        else
        {
            MenuUpdate();
        }
	}

    public void GameStart()
    {
        is_paused_ = false;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                landmarks_map_[i, j].ResetLandmark();
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
        
        view_y_ = 30;

        UIOffAll();
        ui_game_.SetActive(true);
        is_game_ = true;
    }

    void GameUpdate()
    {
        if (is_paused_)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !is_game_over_)
        {
            Pause();
        }

        Vector3 euler_angle = camera_pan_.transform.localEulerAngles;
        if (euler_angle.y < view_y_)
        {
            euler_angle.y += 100 * Time.deltaTime;
            if (euler_angle.y > view_y_)
                euler_angle.y = view_y_;

            camera_pan_.transform.localEulerAngles = euler_angle;
        }
        if (euler_angle.y > view_y_)
        {
            euler_angle.y -= 100 * Time.deltaTime;
            if (euler_angle.y < view_y_)
                euler_angle.y = view_y_;

            camera_pan_.transform.localEulerAngles = euler_angle;
        }

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

                if (alpha_timer_ > 1.5)
                {
                    landmarks.SetAlpha(0f);
                }
                else
                {
                    landmarks.SetAlpha(1f);
                }

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
            Score.score_ = score_;
        }

        if (busy)
        {
            return;
        }

        alpha_timer_ += Time.deltaTime;

        if (is_game_over_)
        {
            if (!ui_game_over_.activeInHierarchy)
            {
                UIOffAll();
                ui_game_over_.SetActive(true);
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

        int dir = -1;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            dir = 0;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            dir = 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir = 2;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir = 3;
        }


        if (dir == 0)
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
                        alpha_timer_ = 0;
                        j = 3;
                    }
                }
            }
        }
        else if (dir == 1)
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
                        alpha_timer_ = 0;
                        j = 0;
                    }
                }
            }
        }
        else if (dir == 2)
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
                        alpha_timer_ = 0;
                        i = 0;
                    }
                }
            }
        }
        else if (dir == 3)
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
                        alpha_timer_ = 0;
                        i = 3;
                    }
                }
            }
        }
    }

    public void MenuStart()
    {
        is_game_ = false;
        UIOffAll();
        ui_menu_.SetActive(true);
    }

    void MenuUpdate()
    {
        Vector3 euler = camera_pan_.transform.localEulerAngles;
        euler.y += 5f * Time.deltaTime;
        camera_pan_.transform.localEulerAngles = euler;
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void GameHelp()
    {
    }

    Landmark CreateLandmark(int level)
    {
        if (level > highest_level_)
            highest_level_ = level;

        Landmark landmark = Instantiate(landmark_prefab_).GetComponent<Landmark>();
        landmark.level_ = level;
        landmark.SetModel(Instantiate(model_prefab_[landmark.level_ - 1]));
        landmark.model_.transform.SetParent(landmark.transform);
        landmark.model_.transform.localPosition = Vector3.zero;

        landmark.SetNumber(Instantiate(number_prefab_[landmark.level_ - 1]));
        landmark.number_.transform.SetParent(landmark.transform);
        landmark.number_.transform.localPosition = Vector3.zero;

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
    
    public void UIOffAll()
    {
        ui_game_.SetActive(false);
        ui_pause_.SetActive(false);
        ui_game_over_.SetActive(false);
        ui_menu_.SetActive(false);
    }

    public void Pause()
    {
        is_paused_ = true;
        UIOffAll();
        ui_pause_.SetActive(true);
    }

    public void Unpause()
    {
        is_paused_ = false;
        UIOffAll();
        ui_game_.SetActive(true);
    }
}

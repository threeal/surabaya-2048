using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
	public GameObject landmark_prefab_;
    public GameObject[] model_prefab_;
	Landmark[,] landmark_map_ = new Landmark[4,4];
	bool all_busy_ = false;
    int highest_level_ = 1;
    bool is_game_over_ = false;
    int score_ = 0;

    public GameObject ui_game_over_;
    public Text ui_score_;

	void Start () {
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				if (Random.value > 0.15)
					continue;

                AddLandmark(new Vector2Int(i, j), 1);
            }
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
        bool empty = false;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Landmark landmark = landmark_map_[i, j];
                if (!landmark) {
                    empty = true;
                    continue;
                }

                for (int k = 1; k <= landmark.level_; k++) {
                    score += 5 * (int)Mathf.Pow(2, k);
                }

                if (landmark.upgrade_)
                {
                    empty = true;
                    AddLandmark(landmark.tile_pos_, landmark.level_ + 1);
                    continue;
                }

                landmark.Move();
                busy = busy || landmark.busy_;
            }
        }


        if (is_game_over_)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(0);
            }
            return;
        }

        if (score > score_) {
            score_ = score;
            ui_score_.text = score_.ToString();
        }

        if (!empty) {
            is_game_over_ = true;
            ui_game_over_.SetActive(true);
            ui_score_.gameObject.SetActive(false);
            return;
        }

        if (busy)
        {
            all_busy_ = true;
            return;
        }

        if (all_busy_)
        {
            all_busy_ = false;

            List<Vector2Int> empty_list = new List<Vector2Int>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (!landmark_map_[i, j])
                    {
                        empty_list.Add(new Vector2Int(i, j));
                    }
                }
            }

            if (empty_list.Count <= 0)
                return;

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

            AddLandmark(empty_list[Random.Range(0, empty_list.Count)], level);
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            for (int i = 0; i < 4; i++)
            {
                int pos = 3;
                bool merged = false;
                for (int j = 3; j >= 0; j--)
                {
                    Landmark landmark = landmark_map_[i, j];
                    if (!landmark)
                        continue;

                    if (!merged && j == pos && j < 3)
                    {
                        Landmark prev = landmark_map_[i, j + 1];
                        if (prev)
                        {
                            if (prev.level_ == landmark.level_)
                            {
                                landmark.target_merge_ = prev;
                                MoveLandmark(landmark, new Vector2Int(i, j + 1));
                                merged = true;
                                continue;
                            }
                        }
                    }

                    if (pos != j)
                        MoveLandmark(landmark, new Vector2Int(i, pos));
                    pos--;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            for (int i = 0; i < 4; i++)
            {
                int pos = 0;
                bool merged = false;
                for (int j = 0; j < 4; j++)
                {
                    Landmark landmark = landmark_map_[i, j];
                    if (!landmark)
                        continue;

                    if (!merged && j == pos & j > 0)
                    {
                        Landmark prev = landmark_map_[i, j - 1];
                        if (prev)
                        {
                            if (prev.level_ == landmark.level_)
                            {
                                landmark.target_merge_ = prev;
                                MoveLandmark(landmark, new Vector2Int(i, j - 1));
                                merged = true;
                                continue;
                            }
                        }
                    }

                    if (pos != j)
                        MoveLandmark(landmark, new Vector2Int(i, pos));
                    pos++;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            for (int j = 0; j < 4; j++)
            {
                int pos = 0;
                bool merged = false;
                for (int i = 0; i < 4; i++)
                {
                    Landmark landmark = landmark_map_[i, j];
                    if (!landmark)
                        continue;

                    if (!merged && i == pos & i > 0)
                    {
                        Landmark prev = landmark_map_[i - 1, j];
                        if (prev)
                        {
                            if (prev.level_ == landmark.level_)
                            {
                                landmark.target_merge_ = prev;
                                MoveLandmark(landmark, new Vector2Int(i - 1, j));
                                merged = true;
                                continue;
                            }
                        }
                    }

                    if (pos != i)
                        MoveLandmark(landmark, new Vector2Int(pos, j));
                    pos++;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int j = 0; j < 4; j++)
            {
                int pos = 3;
                bool merged = false;
                for (int i = 3; i >= 0; i--)
                {
                    Landmark landmark = landmark_map_[i, j];
                    if (!landmark)
                        continue;

                    if (!merged && i == pos & i < 3)
                    {
                        Landmark prev = landmark_map_[i + 1, j];
                        if (prev)
                        {
                            if (prev.level_ == landmark.level_)
                            {
                                landmark.target_merge_ = prev;
                                MoveLandmark(landmark, new Vector2Int(i + 1, j));
                                merged = true;
                                continue;
                            }
                        }
                    }

                    if (pos != i)
                        MoveLandmark(landmark, new Vector2Int(pos, j));
                    pos--;
                }
            }
        }
    }

    void MoveLandmark(Landmark landmark, Vector2Int pos)
    {
        landmark_map_[landmark.tile_pos_.x, landmark.tile_pos_.y] = null;
        landmark_map_[pos.x, pos.y] = landmark;
        landmark.tile_pos_ = pos;
    }

    void DeleteLandmark(Vector2Int pos)
    {
        landmark_map_[pos.x, pos.y] = null;
        Destroy(landmark_map_[pos.x, pos.y].gameObject);
    }

    void DeleteLandmark(Landmark landmark)
    {
        landmark_map_[landmark.tile_pos_.x, landmark.tile_pos_.y] = null;
        Destroy(landmark.gameObject);
    }

    Landmark AddLandmark(Vector2Int pos, int level)
    {
        Landmark prev = landmark_map_[pos.x, pos.y];

        if (level > highest_level_)
            highest_level_ = level;

        Landmark landmark = landmark_map_[pos.x, pos.y] = Instantiate(landmark_prefab_).GetComponent<Landmark>();
        landmark.level_ = level;
        landmark.model_ = Instantiate(model_prefab_[landmark.level_ - 1]);
        landmark.model_.transform.SetParent(landmark.transform);
        landmark.model_.transform.localPosition = Vector3.zero;
        landmark.tile_pos_ = pos;
        landmark.transform.SetParent(transform);

        Vector3 local_position = landmark.PosToWorld(landmark.tile_pos_);
        local_position.y = 5;
        landmark.transform.localPosition = local_position;

        if (prev) {
            landmark.target_merge_ = prev;
        }

        return landmark;
    }
}

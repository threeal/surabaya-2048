using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
	public GameObject landmark_prefab_;
    public GameObject[] model_prefab_;
	Landmark[,] landmark_map_ = new Landmark[4,4];

	void Start () {
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				if (Random.value > 0.15)
					continue;

                AddLandmark(new Vector2Int(i, j), 1);
            }
		}
	}
	
	void Update ()
	{
        bool moved = false;
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			for (int i = 0; i < 4; i++)
            {
                bool merged = false;
                int pos = (landmark_map_[i, 3]) ? 2 : 3;
				for (int j = 2; j >= 0; j--) {
					Landmark landmark = landmark_map_[i, j];
					if (landmark) {
                        Landmark prev = landmark_map_[i, j + 1];
                        if (prev)
                        {
                            pos = j;
                            if (prev.level_ == landmark.level_ && prev.level_ <= 7 && !merged)
                            {
                                merged = true;
                                moved = true;
                                Vector2Int tile_pos = prev.tile_pos_;
                                int level = prev.level_ + 1;
                                DeleteLandmark(landmark);
                                DeleteLandmark(prev);
                                prev = AddLandmark(tile_pos, level);
                                continue;
                            }
                            pos = j - 1;
                        }
                        else
                        {
                            prev = landmark;
                            if (j != pos)
                            {
                                merged = true;
                                moved = true;
                                MoveLandmark(landmark, new Vector2Int(i, pos));
                                pos--;
                            }
                        }
                    }
                }
			}
		} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			for (int i = 0; i < 4; i++)
            {
                bool merged = false;
                int pos = (landmark_map_[i, 0]) ? 1 : 0;
                for (int j = 1; j < 4; j++) {
					Landmark landmark = landmark_map_[i, j];
					if (landmark)
                    {
                        Landmark prev = landmark_map_[i, j - 1];
                        if (prev)
                        {
                            pos = j;
                            if (prev.level_ == landmark.level_ && prev.level_ <= 7 && !merged)
                            {
                                merged = true;
                                moved = true;
                                Vector2Int tile_pos = prev.tile_pos_;
                                int level = prev.level_ + 1;
                                DeleteLandmark(landmark);
                                DeleteLandmark(prev);
                                prev = AddLandmark(tile_pos, level);
                                continue;
                            }
                            pos = j + 1;
                        }
                        else
                        {
                            prev = landmark;
                            if (j != pos)
                            {
                                merged = true;
                                moved = true;
                                MoveLandmark(landmark, new Vector2Int(i, pos));
                                pos++;
                            }
                        }
                    }
				}
			}
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			for (int j = 0; j < 4; j++)
            {
                bool merged = false;
                int pos = (landmark_map_[0, j]) ? 1 : 0;
                for (int i = 1; i < 4; i++) {
					Landmark landmark = landmark_map_[i, j];
					if (landmark)
                    {
                        Landmark prev = landmark_map_[i - 1, j];
                        if (prev)
                        {
                            pos = i;
                            if (prev.level_ == landmark.level_ && prev.level_ <= 7 && !merged)
                            {
                                merged = true;
                                moved = true;
                                Vector2Int tile_pos = prev.tile_pos_;
                                int level = prev.level_ + 1;
                                DeleteLandmark(landmark);
                                DeleteLandmark(prev);
                                prev = AddLandmark(tile_pos, level);
                                continue;
                            }
                            pos = i + 1;
                        }
                        else
                        {
                            prev = landmark;
                            if (i != pos)
                            {
                                merged = true;
                                moved = true;
                                MoveLandmark(landmark, new Vector2Int(pos, j));
                                pos++;
                            }
                        }
                    }
				}
			}
		} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			for (int j = 0; j < 4; j++)
            {
                bool merged = false;
                int pos = (landmark_map_[3, j]) ? 2 : 3;
				for (int i = 2; i >= 0; i--) {
					Landmark landmark = landmark_map_[i, j];
					if (landmark)
                    {
                        Landmark prev = landmark_map_[i + 1, j];
                        if (prev)
                        {
                            pos = i;
                            if (prev.level_ == landmark.level_ && prev.level_ <= 7 && !merged)
                            {
                                merged = true;
                                moved = true;
                                Vector2Int tile_pos = prev.tile_pos_;
                                int level = prev.level_ + 1;
                                DeleteLandmark(landmark);
                                DeleteLandmark(prev);
                                prev = AddLandmark(tile_pos, level);
                                continue;
                            }
                            pos = i - 1;
                        }
                        else
                        {
                            prev = landmark;
                            if (i != pos)
                            {
                                merged = true;
                                moved = true;
                                MoveLandmark(landmark, new Vector2Int(pos, j));
                                pos--;
                            }
                        }
                    }
				}
			}
		}

		if (moved) {
			List<Vector2Int> empty_list = new List<Vector2Int>();
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					if (!landmark_map_[i, j]) {
						empty_list.Add(new Vector2Int(i, j));
					}
				}
			}

			if (empty_list.Count > 0) {
                AddLandmark(empty_list[Random.Range(0, empty_list.Count)], 1);
			}

            moved = false;
		}
	}

    void MoveLandmark(Landmark landmark, Vector2Int pos)
    {
        landmark_map_[landmark.tile_pos_.x, landmark.tile_pos_.y] = null;
        landmark_map_[pos.x, pos.y] = landmark;
        landmark.tile_pos_ = pos;
        landmark.transform.localPosition = PosToWorld(landmark.tile_pos_);
    }

    void DeleteLandmark(Vector2Int pos)
    {
        Destroy(landmark_map_[pos.x, pos.y].gameObject);
        landmark_map_[pos.x, pos.y] = null;
    }

    void DeleteLandmark(Landmark landmark)
    {
        landmark_map_[landmark.tile_pos_.x, landmark.tile_pos_.y] = null;
        Destroy(landmark.gameObject);
    }

    Landmark AddLandmark(Vector2Int pos, int level)
    {
        if (!landmark_map_[pos.x, pos.y]) {
            Landmark landmark = landmark_map_[pos.x, pos.y] = Instantiate(landmark_prefab_).GetComponent<Landmark>();
            landmark.level_ = level;
            landmark.model_ = Instantiate(model_prefab_[landmark.level_ - 1]);
            landmark.model_.transform.SetParent(landmark.transform);
            landmark.model_.transform.localPosition = Vector3.zero;
            landmark.tile_pos_ = pos;
            landmark.transform.SetParent(transform);
            landmark.transform.localPosition = PosToWorld(landmark.tile_pos_);

            return landmark;
        }

        return null;
    }

	Vector3 PosToWorld(Vector2Int pos)
	{
		Vector3 world;
		world.x = ((float)pos.x - (1.5f)) * 1.0f;
		world.y = 0;
		world.z = ((float)pos.y - (1.5f)) * 1.0f;
		return world;
	}
}

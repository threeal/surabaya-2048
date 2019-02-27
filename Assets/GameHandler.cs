using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
	public GameObject landmark_prefab_;
    public GameObject[] model_prefab_;
	Landmark[,] landmark_map_ = new Landmark[4,4];
    List<Landmark> landmark_destroy_ = new List<Landmark>();
	bool all_busy_ = false;

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
        bool busy = false;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                Landmark landmark = landmark_map_[i, j];
                if (!landmark)
                    continue;

                if (landmark.disable_) {
                    DeleteLandmark(landmark);
                    continue;
                }

                if (landmark.upgrade_) {
                    AddLandmark(landmark.tile_pos_, landmark.level_ + 1);
                    continue;
                }

                landmark.Move();
                busy = busy || landmark.busy_;
            }
        }

        foreach (Landmark landmark in landmark_destroy_) {
            Destroy(landmark.gameObject);
        }
        landmark_destroy_.Clear();

        if (busy) {
            all_busy_ = true;
            return;
        }

        if (all_busy_) {
            all_busy_ = false;

            List<Vector2Int> empty_list = new List<Vector2Int>();
            for (int i = 0; i < 4; i++) {
                if (i > 0 && i < 3)
                    continue;

                for (int j = 0; j < 4; j++) {
                    if (j > 0 && j < 3)
                        continue;

                    if (!landmark_map_[i, j]) {
                        empty_list.Add(new Vector2Int(i, j));
                    }
                }
            }
            if (empty_list.Count <= 0)
                return;

            AddLandmark(empty_list[Random.Range(0, empty_list.Count)], 1);
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            for (int i = 0; i < 4; i++) {
                int pos = 3;
                bool merged = false;
                for (int j = 3; j >= 0; j--) {
                    Landmark landmark = landmark_map_[i, j];
                    if (!landmark)
                        continue;
                    
                    if (!merged && j == pos && j < 3) {
                        Landmark prev = landmark_map_[i, j + 1];
                        if (prev) {
                            if (prev.level_ == landmark.level_) {
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
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            for (int i = 0; i < 4; i++) {
                int pos = 0;
                bool merged = false;
                for (int j = 0; j < 4; j++) {
                    Landmark landmark = landmark_map_[i, j];
                    if (!landmark)
                        continue;
                    
                    if (!merged && j == pos & j > 0) {
                        Landmark prev = landmark_map_[i, j - 1];
                        if (prev) {
                            if (prev.level_ == landmark.level_) {
                                landmark.target_merge_ = prev;
                                MoveLandmark(landmark, new Vector2Int(i, j - i));
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
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            for (int j = 0; j < 4; j++) {
                int pos = 0;
                bool merged = false;
                for (int i = 0; i < 4; i++) {
                    Landmark landmark = landmark_map_[i, j];
                    if (!landmark)
                        continue;

                    if (!merged && i == pos & i > 0) {
                        Landmark prev = landmark_map_[i - 1, j];
                        if (prev) {
                            if (prev.level_ == landmark.level_) {
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
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            for (int j = 0; j < 4; j++) {
                int pos = 3;
                bool merged = false;
                for (int i = 3; i >= 0; i--) {
                    Landmark landmark = landmark_map_[i, j];
                    if (!landmark)
                        continue;

                    if (!merged && i == pos & i < 3) {
                        Landmark prev = landmark_map_[i + 1, j];
                        if (prev) {
                            if (prev.level_ == landmark.level_) {
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
        Landmark prev = landmark_map_[pos.x, pos.y];
        if (prev)
            DeleteLandmark(prev);

        landmark_map_[landmark.tile_pos_.x, landmark.tile_pos_.y] = null;
        landmark_map_[pos.x, pos.y] = landmark;
        landmark.tile_pos_ = pos;
    }

    void DeleteLandmark(Vector2Int pos)
    {
        landmark_map_[pos.x, pos.y] = null;
        landmark_destroy_.Add(landmark_map_[pos.x, pos.y]);
    }

    void DeleteLandmark(Landmark landmark)
    {
        landmark_map_[landmark.tile_pos_.x, landmark.tile_pos_.y] = null;
        landmark_destroy_.Add(landmark);
    }

    Landmark AddLandmark(Vector2Int pos, int level)
    {
        Landmark landmark = landmark_map_[pos.x, pos.y];
        if (landmark)
            DeleteLandmark(landmark);

        landmark = landmark_map_[pos.x, pos.y] = Instantiate(landmark_prefab_).GetComponent<Landmark>();
        landmark.level_ = level;
        landmark.model_ = Instantiate(model_prefab_[landmark.level_ - 1]);
        landmark.model_.transform.SetParent(landmark.transform);
        landmark.model_.transform.localPosition = Vector3.zero;
        landmark.tile_pos_ = pos;
        landmark.transform.SetParent(transform);
        landmark.transform.localPosition = landmark.PosToWorld(landmark.tile_pos_);

        return landmark;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
	public GameObject landmark_prefab_;
	Landmark[,] landmark_map_ = new Landmark[4,4];

	bool is_busy_ = false;

	void Start () {
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				if (Random.value > 0.15)
					continue;

				Landmark landmark = landmark_map_[i, j] = Instantiate(landmark_prefab_).GetComponent<Landmark>();
				landmark.tile_pos_ = new Vector2Int(i, j);
				landmark.transform.SetParent(transform);
				landmark.transform.localPosition = PosToWorld(landmark.tile_pos_);
			}
		}
	}
	
	void Update ()
	{
		bool moved = false;
		if (Input.GetKey(KeyCode.UpArrow)) {
			for (int i = 0; i < 4; i++) {
				int pos = 3;
				for (int j = 3; j >= 0; j--) {
					Landmark landmark = landmark_map_[i, j];
					if (landmark) {
						if (j != pos) {
							landmark_map_[i, j] = null;
							landmark_map_[i, pos] = landmark;
							landmark.tile_pos_ = new Vector2Int(i, pos);
							landmark.transform.localPosition = PosToWorld(landmark.tile_pos_);
							moved = true;
						}
						pos--;
					}
				}
			}
		} else if (Input.GetKey(KeyCode.DownArrow)) {
			for (int i = 0; i < 4; i++) {
				int pos = 0;
				for (int j = 0; j < 4; j++) {
					Landmark landmark = landmark_map_[i, j];
					if (landmark) {
						if (j != pos) {
							landmark_map_[i, j] = null;
							landmark_map_[i, pos] = landmark;
							landmark.tile_pos_ = new Vector2Int(i, pos);
							landmark.transform.localPosition = PosToWorld(landmark.tile_pos_);
							moved = true;
						}
						pos++;
					}
				}
			}
		} else if (Input.GetKey(KeyCode.LeftArrow)) {
			for (int j = 0; j < 4; j++) {
				int pos = 0;
				for (int i = 0; i < 4; i++) {
					Landmark landmark = landmark_map_[i, j];
					if (landmark) {
						if (i != pos) {
							landmark_map_[i, j] = null;
							landmark_map_[pos, j] = landmark;
							landmark.tile_pos_ = new Vector2Int(pos, j);
							landmark.transform.localPosition = PosToWorld(landmark.tile_pos_);
							moved = true;
						}
						pos++;
					}
				}
			}
		} else if (Input.GetKey(KeyCode.RightArrow)) {
			for (int j = 0; j < 4; j++) {
				int pos = 3;
				for (int i = 3; i >= 0; i--) {
					Landmark landmark = landmark_map_[i, j];
					if (landmark) {
						if (i != pos) {
							landmark_map_[i, j] = null;
							landmark_map_[pos, j] = landmark;
							landmark.tile_pos_ = new Vector2Int(pos, j);
							landmark.transform.localPosition = PosToWorld(landmark.tile_pos_);
							moved = true;
						}
						pos--;
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

			if (empty_list.Count > 0)
			{
				int pos = Random.Range(0, empty_list.Count);
				Landmark landmark = landmark_map_[i, j] = Instantiate(landmark_prefab_).GetComponent<Landmark>();
				landmark.tile_pos_ = new Vector2Int(i, j);
				landmark.transform.SetParent(transform);
				landmark.transform.localPosition = PosToWorld(landmark.tile_pos_);
			}
		}
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

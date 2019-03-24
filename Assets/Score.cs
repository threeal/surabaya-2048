using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    static public int score_ = 0;
    public int spacing_ = 8;
    int prev_score_ = 0;

    public GameObject number_prefab_;
    public Sprite[] number_sprites_;
    List<Image> numbers_ = new List<Image>();
    List<int> numbers_value_ = new List<int>();

    private void Start()
    {
        numbers_.Clear();
        numbers_value_.Clear();
    }

    private void LateUpdate()
    {
        if (score_ > prev_score_)
        {
            string str = score_.ToString();

            while (str.Length < numbers_.Count)
            {
                Image image = numbers_[numbers_.Count - 1];
                numbers_.RemoveAt(numbers_.Count - 1);
                numbers_value_.RemoveAt(numbers_.Count - 1);
                Destroy(image.gameObject);
            }

            while (str.Length > numbers_.Count)
            {
                Image image = Instantiate(number_prefab_).GetComponent<Image>();
                image.gameObject.transform.SetParent(transform);
                image.gameObject.transform.localPosition = Vector3.zero;

                numbers_.Add(image);
                numbers_value_.Add(0);
            }

            for (int i = str.Length - 1; i >= 0; i--)
            {
                int val = str[str.Length - 1 - i] - '0';

                if (val < 0 || val > 9)
                    continue;

                if (numbers_value_[i] != val)
                {
                    numbers_value_[i] = val;
                    numbers_[i].sprite = number_sprites_[val];
                }
            }

            Reposition();
        }

        prev_score_ = score_;
    }

    void Reposition()
    {
        float size = numbers_.Count;
        float center = (size - 1) / 2;
        for (int i = 0; i < numbers_.Count; i++)
        {
            Image image = numbers_[i];

            Vector3 pos = Vector3.zero;
            image.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((center - i) * spacing_, 0);
        }
    }
}

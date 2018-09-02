using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text_manipulator : MonoBehaviour {

    
    Text ui_text;

    public Player_input player;
    public Slider s;

    public bool absorbs, points, kyu_img, hp, combo;

    float limit_width;
	// Use this for initialization
	void Start ()
    {
        ui_text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (absorbs)
        {
            ui_text.text = player.absorbs + "/" + player.absorbs_to_super[player.kyu];
            s.value = (float)player.absorbs / (float)player.absorbs_to_super[player.kyu];
        }
        if (points)
        {
            ui_text.text = player.points + "/" + player.absorbs_to_next[player.kyu];
            s.value = (float)player.points / (float)player.absorbs_to_next[player.kyu];
        }

        if (hp)
        {
            ui_text.text = "hp: " + player.curr_hp;
            s.value = (float)player.curr_hp / (float)player.hp_s[player.kyu];
        }

        if(combo)
        {
            ui_text.text = player.combo + "";
            
        }
        if (kyu_img)
        {
            if(player.kyu == 11) ui_text.text = "o7 o7";
            else
            {
                if (player.kyu < 8) ui_text.text = Mathf.Abs(player.kyu - 8) + " kyu";

                else ui_text.text = Mathf.Abs(player.kyu - 8)+1  + " dan";
            }
        }
        //prog_bar.rectTransform.sizeDelta = new Vector2(player.absorbs * limit_width / player.absorbs_to_super[player.kyu], prog_bar.rectTransform.rect.height);
    }
}

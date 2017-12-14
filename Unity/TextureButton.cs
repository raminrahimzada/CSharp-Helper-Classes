using System.Collections;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(AudioSource))]
    public class TextureButton : MonoBehaviour {

        public Texture2D NormalTexture;
        public Texture2D RollOverTexture;
        public AudioClip ClickSound;

        public GameObject Key;
        public GameObject Player;

        public GUITexture LocalGuiTexture
        {
            get { return GetComponent<GUITexture>(); }
        }
        public void OnMouseEnter()
        {  //Mouse Roll over function
            LocalGuiTexture.texture = RollOverTexture;
        }

        public void OnMouseExit()
        { //Mouse Roll out function
            LocalGuiTexture.texture = NormalTexture;
        }

        public IEnumerator OnMouseUp()
        {
            var audionew = GetComponent<AudioSource>();
            // Mouse up function
            audionew.PlayOneShot(ClickSound);
            yield return new WaitForSeconds(1.0f); //Wait for 0.5 secs. until do the next function
            //Create a new Player at the start position by cloning from our prefab
            Instantiate(Player, new Vector3(Player.transform.position.x, Player.transform.position.y, 0.0f), Player.transform.rotation);
            //Create a new key at the start position by cloning from our prefab
            Instantiate(Key, new Vector3(Key.transform.position.x, Key.transform.position.y, 0.0f), Key.transform.rotation);
            //Hide restart button
            LocalGuiTexture.enabled = false;
        }
    }
}


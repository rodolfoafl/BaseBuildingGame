using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    float soundCooldown = 0;

	// Use this for initialization
	void Start () {
        WorldController.Instance.World.RegisterInstalledObjectCreated(OnInstalledObjectCreated);
        WorldController.Instance.World.RegisterTileChanged(OnTileTypeChanged);
    }

    void Update()
    {
        soundCooldown -= Time.deltaTime;
    }
	
    void OnTileTypeChanged(Tile tile_data)
    {
        if (soundCooldown > 0)
        {
            return;
        }

        AudioClip ac = Resources.Load<AudioClip>("_SFX/BuildFloor_SFX");
        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        soundCooldown = 0.1f;
    }

    public void OnInstalledObjectCreated(InstalledObject obj)
    {
        if (soundCooldown > 0)
        {
            return;
        }

        AudioClip ac = Resources.Load<AudioClip>("_SFX/BuildWall_SFX");
        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        soundCooldown = 0.1f;
    }
}

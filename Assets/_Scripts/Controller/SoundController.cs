using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    float _soundCooldown = 0f;

	// Use this for initialization
	void Start () {
        WorldController.Instance.World.RegisterInstalledObjectCreated(OnInstalledObjectCreated);
        WorldController.Instance.World.RegisterTileChanged(OnTileChanged);
    }

    void Update()
    {
        if (_soundCooldown > 0)
        {
            _soundCooldown -= Time.deltaTime;
        }
    }

    void OnTileChanged(Tile tile_data)
    {
        if(_soundCooldown > 0)
        {
            return;
        }

        AudioClip audioclip = Resources.Load<AudioClip>("_SFX/BuildFloor_SFX");
        AudioSource.PlayClipAtPoint(audioclip, Camera.main.transform.position);
        _soundCooldown = 0.1f;
    }

    public void OnInstalledObjectCreated(InstalledObject obj)
    {
        if (_soundCooldown > 0)
        {
            return;
        }

        AudioClip audioclip = Resources.Load<AudioClip>("_SFX/Build" + obj.ObjectType + "_SFX");
        if (audioclip == null)
        {
            return;
        }
        else
        {
            AudioSource.PlayClipAtPoint(audioclip, Camera.main.transform.position);
            _soundCooldown = 0.1f;
        }
    }
}

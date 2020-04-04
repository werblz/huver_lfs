using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxDestroy : MonoBehaviour {

    [Tooltip("If the effect is set to loop, check this")]
    [SerializeField]
    bool destroyByDuration = false;

    [SerializeField]
    private ParticleSystem ps = null;
    float duration = 0.0f;

	// Use this for initialization
	void Awake () {
        
        duration = ps.main.duration;
	}
	
	// Update is called once per frame
	void Update () {
		// Destroys based on header  duration
        if (destroyByDuration)
        {
            duration -= Time.deltaTime;
            if ( duration <= 0)
            {
                DestroyImmediate(gameObject);
            }
        }
        else
        {
            // Destroy sself if all particles are done
            if (ps.IsAlive(true) == false)
            {
                DestroyImmediate(gameObject);
            }
        }

	}
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}

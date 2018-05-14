using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField] private Transform target;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float damage;
    [SerializeField] private Unit sender;
    [SerializeField] private ParticleSystem myParticleSystem;
    public AudioClip audioclip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        myParticleSystem = GetComponent<ParticleSystem>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        try
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), projectileSpeed * Time.deltaTime);
            transform.position += transform.forward * projectileSpeed * Time.deltaTime;
        }
        catch
        {
            Destroy(gameObject);
        }
    }

    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }
    public float ProjectileSpeed
    {
        get { return projectileSpeed; }
        set { projectileSpeed = value; }
    }
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public Unit Sender
    {
        get
        {
            return sender;
        }

        set
        {
            sender = value;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        try
        {
            if (collider.gameObject.Equals(target.gameObject))
            {
                //print(collider.gameObject + " hitted " + target.gameObject);
                collider.gameObject.GetComponent<Unit>().GetDamage(sender, damage);
                if(audioSource.clip)
                    AudioSource.PlayClipAtPoint(audioSource.clip, transform.position, 10f);
                if (myParticleSystem)
                {
                    myParticleSystem.Stop();
                    Invoke("DestroyIt", myParticleSystem.main.startLifetimeMultiplier);
                }
                else
                    Destroy(this.gameObject);
            }
        }
        catch
        {
            print("Execption catch");
            if(myParticleSystem)
                myParticleSystem.Stop();
            Invoke("DestroyIt", myParticleSystem.main.startLifetimeMultiplier);
        }
    }

    void DestroyIt()
    {
        
        Destroy(this.gameObject);
    }
}

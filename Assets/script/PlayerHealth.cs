using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int health;
    public int HealthMax;
    public int Blinks;
    public int lives;
    public float time;
    public float dieTime;
    public float hitboxCDTime;
    public float respawnDelay;

    private Renderer myRender;
    private Animator anim;
    private ScreenRed sf;
    private Rigidbody2D rb2d;
    private PolygonCollider2D polygonCollider2D;
    
    // Start is called before the first frame update
    void Start()
    {
        
        myRender = GetComponent<Renderer>();
        anim = GetComponent<Animator>();
        sf = GetComponent<ScreenRed>();
        rb2d = GetComponent<Rigidbody2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamagePlayer(int damage)
	{
        sf.FlashScreen();
        health -= damage;
        if(health<0)
		{
            health = 0;
		}

        HealthBar.HealthCurrent = health;
        if(health<=0)
		{
            lives -= 1;
            rb2d.velocity = new Vector2(0, 0);
            rb2d.gravityScale = 0.0f;
            GameController.isGameAlive = false;
            anim.SetTrigger("Die");
            //Invoke("KillPlayer", dieTime); 
            //Invoke("RespawnPlayer", respawnDelay);
            RespawnPlayer();
        }
        BlinkPlayer(Blinks, time);
        polygonCollider2D.enabled = false;
        StartCoroutine(ShowPlayerHitBox());
	}

    IEnumerator ShowPlayerHitBox()
	{
        yield return new WaitForSeconds(hitboxCDTime);
        polygonCollider2D.enabled = true;
	}

    void KillPlayer()
	{
        Destroy(gameObject);
	}

    void BlinkPlayer(int numBlinks,float seconds)
	{
        StartCoroutine(DoBlinks(numBlinks, seconds));
	}
    
    IEnumerator DoBlinks(int numBlinks,float seconds)
	{
        for (int i=0;i<numBlinks*2;i++)
		{
            myRender.enabled = !myRender.enabled;
            yield return new WaitForSeconds(seconds);
		}
        myRender.enabled = true;
	}

    void RespawnPlayer()
    {
        Debug.Log("Respawn");
        //health = HealthMax;
        //HealthBar.HealthCurrent = health;
        GameController.isGameAlive = true;  // bring the game back to life
        gameObject.SetActive(true);  // re-enable player's GameObject
        rb2d.gravityScale = 1.0f;  // restore gravity
        // You might also want to reposition player's GameObject to its starting position at this point
        StartCoroutine(RefillHealthWithDelay());
    }
    IEnumerator RefillHealthWithDelay()
    {
        // wait for the delay
        //yield return new WaitForSeconds(delay);

        // refill health and update health bar
        while (health < HealthMax)
        {
            health++;
            HealthBar.HealthCurrent = health;
            yield return new WaitForSeconds(0.2f);
        }
        gameObject.SetActive(true);

    }

}


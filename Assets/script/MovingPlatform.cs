using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public float speed;
    public float waitTime;
    public Transform[] movePos; //move pos

    private int i;
    private Transform playerDefTransform;

    // Start is called before the first frame update
    void Start()
    {
        i = 1;
        playerDefTransform = GameObject.FindGameObjectWithTag("Player").transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePos[i].position, speed * Time.deltaTime);
        if(Vector2.Distance(transform.position,movePos[i].position)<0.1f)
		{
            if(waitTime<0.0f)
			{
                if(i==0)
				{
                    i = 1;
				}
                else
				{
                    i = 0;
				}
                waitTime = 0.5f;
			}
			else
			{
                waitTime -= Time.deltaTime;
			}
		}
    }


	void OnTriggerEnter2D(Collider2D other)
	{
        if(other.CompareTag("Player"))
		{
            other.gameObject.transform.parent = gameObject.transform;
		}
    }

	void OnTriggerExit2D(Collider2D other)
	{
        if (other.CompareTag("Player"))
        {
            other.gameObject.transform.parent = playerDefTransform;
        }
    }

    /*bool IsPlayerOnPlatform(GameObject player)
	{
        float distanceToPlatform = Vector2.Distance(player.transform.position, transform.position);
        float maxDistance = 1f;
        return distanceToPlatform <= maxDistance;
	}*/
}

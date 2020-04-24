using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public float speed = 50.0f;
    private Rigidbody2D rb;
    private bool libera;
    public GameObject player;
    public GameObject xplosion;
    public bool atingiu;
    public int sortingOrder = 11;
    private SpriteRenderer sprite;

    // Use this for initialization
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(speed, 0);
        libera = true;
        xplosion = GameObject.FindGameObjectWithTag("kkk");
        atingiu = false;
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        sprite.sortingOrder = sortingOrder;

        if (GameObject.Find("Player").GetComponent<PlayerController>().isShooting && libera)
        {
            if (GameObject.Find("Player").GetComponent<PlayerController>().isFacingRight) 
                rb.velocity = new Vector2(speed, 0);
            else
                rb.velocity = new Vector2(-speed, 0);
           libera = false;
            
        }

        if (transform.position.x > GameObject.Find("Player").transform.position.x + 1.1 || transform.position.x < GameObject.Find("Player").transform.position.x - 1.1)
        {
            Destroy(this.gameObject);
            GameObject.Find("Player").GetComponent<PlayerController>().isShooting = false;
            libera = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject.Find("Exploss").GetComponent<explosao>().Atingiu = true;
       //  if (other.tag == "blocodestruivel")
      //  {
        //GameObject g = Instantiate(xplosion) as GameObject;
            atingiu = true;
            xplosion.transform.position = this.transform.position;
         //  Destroy(other.gameObject);
            Destroy(this.gameObject);
            GameObject.Find("Player").GetComponent<PlayerController>().isShooting = false;
        if (other.tag == "blocodestruivel")
        {
            Destroy(other.gameObject);
        }
      // }

    }
}
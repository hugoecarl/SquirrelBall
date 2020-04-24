using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{

    public CharacterController2D.CharacterCollisionState2D flags;
    public float walkSpeed = 4.0f;     // Depois de incluido, alterar no Unity Editor
    public float jumpSpeed = 8.0f;     // Depois de incluido, alterar no Unity Editor
    public float doubleJumpSpeed = 6.0f; //Depois de incluido, alterar no Editor
    public float gravity = 9.8f;       // Depois de incluido, alterar no Unity Editor
    public GameObject Shootprefab;
    public Transform player;
    public AudioClip coin;
    public AudioClip Atirando;
    public AudioClip hurt;
    public int vidas = 3;
    private int esferas_count = 0;
    public GameObject E1;
    public GameObject E2;
    public GameObject E3;
    public GameObject E4;
    public GameObject E5;
    public GameObject E6;
    public GameObject E7;
    public GameObject SoundMain;
    public GameObject SoundCave;
    private List<GameObject> intList;

 
    private AudioSource audioSource;

    public bool doubleJumped; // informa se foi feito um pulo duplo
    public bool isGrounded;     // Se está no chão
    public bool isJumping;      // Se está pulando
    public bool isFalling;
    public bool isFlying;
    public bool isFacingRight;      // Se está olhando para a direita
    public bool isShooting;
    public bool isClimbing;
    private bool ir = true;


    private Vector3 moveDirection = Vector3.zero; // direção que o personagem se move
    private CharacterController2D characterController;  //Componente do Char. Controller
    private Animator animator;

    public LayerMask mask;  // para filtrar os layers a serem analisados


    public void shootBullet()
    {
        GameObject b = Instantiate(Shootprefab) as GameObject;
        b.transform.position = player.transform.position;
        

    }


    void Start()
    {
        characterController = GetComponent<CharacterController2D>(); //identif. o componente
        animator = GetComponent<Animator>();
        isFacingRight = true;
        isShooting = false;
        intList = new List<GameObject>() { E1, E2, E3, E4, E5, E6, E7 };
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {

        animator.SetFloat("movementX", Mathf.Abs(moveDirection.x / walkSpeed)); // +Normalizado
        animator.SetFloat("movementY", moveDirection.y);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isFlying", isFlying);
        animator.SetBool("isShooting", isShooting);
        animator.SetBool("isClimbing", isClimbing);

        if (GameObject.Find("Player").transform.position.x > 12.5 && GameObject.Find("Player").transform.position.y < 3.3)
        {
            SoundMain.SetActive(false);
            SoundCave.SetActive(true);
        } else
        {
            SoundMain.SetActive(true);
            SoundCave.SetActive(false);
        }

        moveDirection.x = Input.GetAxis("Horizontal"); // recupera valor dos controles
        moveDirection.x *= walkSpeed;


        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 4f, mask);
        if (hit.collider != null && isGrounded)
        {
            transform.SetParent(hit.transform);
            if (Input.GetAxis("Vertical") < 0 && Input.GetButtonDown("Jump"))
            {
                moveDirection.y = -jumpSpeed;
                StartCoroutine(PassPlatform(hit.transform.gameObject));
            }
        }
        else
        {
            transform.SetParent(null);
        }

        if (isGrounded == true)
            isFalling = false;
        else if (moveDirection.y < 1)
        {
            isFalling = true;
            isFlying = false;
        }
        else
            isFalling = false;

        // Conforme direção do personagem girar ele no eixo Y
        if (moveDirection.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            isFacingRight = false;
        }
        else if (moveDirection.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            isFacingRight = true;
        } // se direção em x == 0 mantenha como está a rotação

        if (isGrounded)
        {            // caso esteja no chão
            moveDirection.y = 0.0f;    // se no chão nem subir nem descer

            isJumping = false;
            doubleJumped = false; // se voltou ao chão pode faz pulo duplo
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
                isJumping = true;
            }
        }
        else
        {            // caso esteja pulando 
            if (Input.GetButtonUp("Jump") && moveDirection.y > 0) // Soltando botão diminui pulo
                moveDirection.y *= 0.5f;

            if (Input.GetButtonDown("Jump") && !doubleJumped) // Segundo clique faz pulo duplo
            {
                moveDirection.y = doubleJumpSpeed;
                doubleJumped = true;
                isFlying = true;
            }

        }


        moveDirection.y -= gravity * Time.deltaTime;    // aplica a gravidade

        animator.speed = 1.0f;
        if (isClimbing)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                moveDirection.y = walkSpeed;
                isJumping = true;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                moveDirection.y = -walkSpeed;
            }
            else
            {
                if (!isGrounded)
                {
                    moveDirection.y = 0.0f;
                    animator.speed = 0.0f;
                }
            }
        }



        characterController.move(moveDirection * Time.deltaTime);   // move personagem	

        flags = characterController.collisionState;     // recupera flags
        isGrounded = flags.below;               // define flag de chão

        if (Input.GetKeyDown("b") && isShooting == false)
        {
            audioSource.PlayOneShot(Atirando);
            isShooting = true;
            shootBullet();

        }

    }

    IEnumerator PassPlatform(GameObject platform)
    {
        platform.GetComponent<EdgeCollider2D>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        platform.GetComponent<EdgeCollider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Coins"))
        {
            AudioSource.PlayClipAtPoint(coin, this.gameObject.transform.position);
            Destroy(other.gameObject);
            walkSpeed += 0.22f;
            jumpSpeed += 0.11f;
            doubleJumpSpeed += 0.22f;
            for (int j = 1; j < 8; j++)
            {
                string buf = "ESFERA" + j;
                if (other.tag == buf)
                {
                    intList[j-1].SetActive(true);
                }
            }
            esferas_count++;
            if (esferas_count == 7)
            {
                SceneManager.LoadScene("YouWin");
            }

        }

        if (other.gameObject.layer == LayerMask.NameToLayer("LadderEffectors"))
        {
            isClimbing = true;
        }

        if (ir)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                ir = false;
                animator.SetBool("isFalling", false);
                animator.SetTrigger("sinking");
                StartCoroutine(Sink());
                enabled = false;
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                ir = false;
                animator.SetBool("isFalling", false);
                animator.SetTrigger("sinking");
                StartCoroutine(Die());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("LadderEffectors"))
        {
            isClimbing = false;
        }


    }

    IEnumerator Sink()
    {


        audioSource.PlayOneShot(hurt);
        for (int i = 0; i <= 20; i++)
        {
            characterController.move(new Vector3(0.0f, -1, 0.0f) * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
            

        }
        enabled = true;
        vidas--;
        if (vidas == 2)
            Destroy(GameObject.FindWithTag("vida3"));
        if (vidas == 1)
            Destroy(GameObject.FindWithTag("vida2"));
        if (vidas == 0)
        {
            Destroy(GameObject.FindWithTag("vida1"));
            SceneManager.LoadScene("GameOver");
        }

        player.transform.position = new Vector3(0, -0.10f, 0);
        ir = true;
    }
        
    IEnumerator Die()
    {
        audioSource.PlayOneShot(hurt);
        enabled = false;
        yield return new WaitForSeconds(0.4f);
        enabled = true;
        


        vidas--;
        if (vidas == 2)
            Destroy(GameObject.FindWithTag("vida3"));
        if (vidas == 1)
            Destroy(GameObject.FindWithTag("vida2"));
        if (vidas == 0)
        {
            Destroy(GameObject.FindWithTag("vida1"));
            SceneManager.LoadScene("GameOver");
        }


        player.transform.position = new Vector3(0, -0.10f, 0);
        ir = true;

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdScript : MonoBehaviour
{
    public static BirdScript instance;
    public bool isAlive;

    [SerializeField]
    private Rigidbody2D myRigidBody;


    [SerializeField]
    private Animator anim;


    private float forwardSpeed = 3f;
    private float bounceSpeed = 4f;
    private bool didFlap;
    private Button flapButton;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip coinClip, pointClip, diedClip;

    [SerializeField]
    private AudioClip[] flapClipsArray;

    public int score, coinScore;
    int countflaps;

    void Awake(){
        if (instance == null){
            instance = this;
        }
        isAlive = true;
        score = GameplayController.currentScoreSt + 0;
        coinScore = GameplayController.coinScoreGameSt + 0;
        flapButton = GameObject.FindGameObjectWithTag("FlapButton").GetComponent<Button>();
        flapButton.onClick.AddListener(() => FlapTheBird());
        SetCamerasX();
        countflaps = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAlive) {

            Vector3 temp = transform.position;
            temp.x += forwardSpeed * Time.deltaTime;
            transform.position = temp;

            if (didFlap) {
                didFlap = false;
                myRigidBody.velocity = new Vector2(0,bounceSpeed);
                anim.SetTrigger("Flap");
                countflaps++;
                Debug.Log("CountFlaps:" +countflaps);

                if (countflaps >= 5){
                    int pickrandomAudioClip = Random.Range(0, flapClipsArray.Length);
                    audioSource.PlayOneShot(flapClipsArray[pickrandomAudioClip]);
                    countflaps = 0;
                }
                
            }
            if(myRigidBody.velocity.y >= 0){
                transform.rotation = Quaternion.Euler(0, 0, 0);
            } else{
                float angle = 0;
                angle = Mathf.Lerp(0, -90, -myRigidBody.velocity.y / 7);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
       
    }

    public void FlapTheBird() {
        didFlap = true;
    }
    public float GetPositionX(){
        return transform.position.x;
    }

   
    void SetCamerasX() {
        CameraSript.offsetX = (Camera.main.transform.position.x - transform.position.x) -1f;
    }

    void OnCollisionEnter2D(Collision2D target){
        if(target.gameObject.tag == "Ground" || target.gameObject.tag == "Pipe") {
            if (isAlive) {
              isAlive = false;
              anim.SetTrigger("Bird Died");
              audioSource.PlayOneShot(diedClip);

                Debug.Log("adLifeChancesBird: " + GameplayController.adLifeChancesBird);

                // GameplayController.instance.PlayerDiedShowScore(score);
                if (GameplayController.adLifeChancesBird >= 3){
                    GameplayController.instance.PlayerDiedShowScore(score, coinScore);
                    GameplayController.adLifeChancesBird = 0;
                    Debug.Log("adLifeChancesBird If OnCollision BirdScript #114: " + GameplayController.adLifeChancesBird);
                }
                else {
                    GameplayController.instance.ShowHitWatchAdPanel(score, coinScore);
                    Debug.Log("adLifeChancesBird Else OnCollision BirdScript #118: " + GameplayController.adLifeChancesBird);
                }
                    
            }
        }
    }

    void OnTriggerEnter2D(Collider2D target){
        if(target.gameObject.tag == "PipeHolder") {
            score++;
            GameplayController.instance.SetScore(score);
            audioSource.PlayOneShot(pointClip);
        }
      
        if (target.gameObject.tag == "CoinScoreHit"){

            //CoinPrefManager.coins += 10;
            coinScore += 10;
            CoinPrefManager.instance.UpdateCoins(target.transform.position, 10,true);
            audioSource.PlayOneShot(coinClip);

        }
    }
}

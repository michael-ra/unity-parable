using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour {

    
	private Rigidbody2D _rb2d;
	public Tilemap tilemap; //dirt
    public Tilemap mauer;
	private Vector2 _speedX = new Vector2(1, 0);
	private Vector2 _speedY = new Vector2(0, 1);
    public Vector3 prePosition; //TODO-Further-Use: Keep for use in future
    public int collectables;
    private int _collectableDelay;
    private bool _waiting;
    public bool debugMove;
    private bool _nonMoveable;
    public Animator playerAnim;
    private int _lastDirection;

	// Use this for initialization
	void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        prePosition = _rb2d.gameObject.transform.position;
        collectables = 0;
        _nonMoveable = false;
    }
    
	// Update is called once per frame
	public void Update()
    {

        if (Input.GetKeyDown(KeyCode.X))
        {
            SceneManager.LoadScene("GameOver");
            return;
        }

        //nonMoveable set in Iterator down below, delay for movement
        if (_nonMoveable)
        {
            return;
            
        }
        //TODO-Done: Delay to avoid glitching in between grids
        // || mauer.GetTile(new Vector3Int((int)rb2d.position.x - 2, (int)rb2d.position.y, 0)) != null
        //useful https://answers.unity.com/questions/756380/raycast-ignore-itself.html
        if (Input.GetKeyDown("left") && mauer.GetTile(new Vector3Int((int)_rb2d.position.x-2, (int)_rb2d.position.y, 0)) == null) 
        {
            /*RaycastHit2D boulderCheckHit = Physics2D.Raycast(rb2d.transform.position + new Vector3(-1,0,0), new Vector3(0,1,0), 1);
             //Not working due to rockford moving too slow/collision happens while setCanMove = true
            if (boulderCheckHit.collider != null && boulderCheckHit.collider.CompareTag("rocks"))
            {
                CollectableDelay.setCanMove(false);
            }
            else
            {
                CollectableDelay.setCanMove(true);
            }*/
            RaycastHit2D hit = Physics2D.Raycast(_rb2d.transform.position, new Vector3(-1,0,0), 1);
            if (debugMove)
            {
                Debug.Log(hit.collider);
            }
            if (hit.collider != null && hit.collider.CompareTag("rocks"))
            {
                if (tilemap.GetTile(new Vector3Int((int) hit.collider.GetComponent<Rigidbody2D>().position.x,
                        (int) hit.collider.GetComponent<Rigidbody2D>().position.y, 0)) == null
                    && tilemap.GetTile(new Vector3Int((int) hit.collider.GetComponent<Rigidbody2D>().position.x - 2,
                        (int) hit.collider.GetComponent<Rigidbody2D>().position.y, 0)) == null
                    && mauer.GetTile(new Vector3Int((int) hit.collider.GetComponent<Rigidbody2D>().position.x,
                        (int) hit.collider.GetComponent<Rigidbody2D>().position.y, 0)) == null
                    && mauer.GetTile(new Vector3Int((int) hit.collider.GetComponent<Rigidbody2D>().position.x - 2,
                        (int) hit.collider.GetComponent<Rigidbody2D>().position.y, 0)) == null)
                {
                    RaycastHit2D rayBoulder = Physics2D.Raycast(hit.collider.GetComponent<Rigidbody2D>().transform.position, new Vector3(-1,0,0), 1);
                    if (rayBoulder.collider != null)
                    {
                        if (rayBoulder.collider.CompareTag("collectable"))
                        {
                            return;
                        }

                        if (rayBoulder.collider.CompareTag("rocks"))
                        {
                            return;
                        }
                    }
                    hit.collider.GetComponent<Rigidbody2D>().MovePosition(hit.collider.GetComponent<Rigidbody2D>().transform.position - new Vector3(1, 0, 0));
                    prePosition = _rb2d.gameObject.transform.position;
                    _rb2d.MovePosition(_rb2d.position - _speedX);
                    playerAnim.SetBool("runLeft", true);
                    StartCoroutine(DisableMovementFor());
                }
            }
            else
            {
                playerAnim.SetBool("runLeft", true);
                prePosition = _rb2d.gameObject.transform.position;
                _rb2d.MovePosition(_rb2d.position - _speedX);
                StartCoroutine(DisableMovementFor());
            }
        }
        if (Input.GetKeyDown("right") && mauer.GetTile(new Vector3Int((int)_rb2d.position.x, (int)_rb2d.position.y, 0)) == null)         
        {
            RaycastHit2D hit = Physics2D.Raycast(_rb2d.transform.position, new Vector3(1,0,0), 1);
            if (debugMove)
            {
                Debug.Log(hit.collider);
            }
            if (hit.collider != null && hit.collider.CompareTag("rocks"))
            {
                if (tilemap.GetTile(new Vector3Int((int) hit.collider.GetComponent<Rigidbody2D>().position.x,
                        (int) hit.collider.GetComponent<Rigidbody2D>().position.y, 0)) == null &&
                    tilemap.GetTile(new Vector3Int((int) hit.collider.GetComponent<Rigidbody2D>().position.x - 2,
                        (int) hit.collider.GetComponent<Rigidbody2D>().position.y, 0)) == null &&
                    mauer.GetTile(new Vector3Int((int) hit.collider.GetComponent<Rigidbody2D>().position.x,
                        (int) hit.collider.GetComponent<Rigidbody2D>().position.y, 0)) == null &&
                    mauer.GetTile(new Vector3Int((int) hit.collider.GetComponent<Rigidbody2D>().position.x - 2,
                        (int) hit.collider.GetComponent<Rigidbody2D>().position.y, 0)) == null) 
                {
                    RaycastHit2D rayBoulder = Physics2D.Raycast(hit.collider.GetComponent<Rigidbody2D>().transform.position, new Vector3(1,0,0), 1);
                    if (rayBoulder.collider != null)
                    {
                        if (rayBoulder.collider.CompareTag("collectable"))
                        {
                            return;
                        }

                        if (rayBoulder.collider.CompareTag("rocks"))
                        {
                            return;
                        }
                    }
                    hit.collider.GetComponent<Rigidbody2D>().MovePosition(hit.collider.GetComponent<Rigidbody2D>().transform.position + new Vector3(1, 0, 0));
                    playerAnim.SetBool("runRight", true);
                    prePosition = _rb2d.gameObject.transform.position;
                    _rb2d.MovePosition(_rb2d.position + _speedX);
                    StartCoroutine(DisableMovementFor());
                }
            }
            else
            {
                playerAnim.SetBool("runRight", true);
                prePosition = _rb2d.gameObject.transform.position;
                _rb2d.MovePosition(_rb2d.position + _speedX);
                StartCoroutine(DisableMovementFor());
            }
        }
        if (Input.GetKeyDown("up") && mauer.GetTile(new Vector3Int((int)_rb2d.position.x-1, (int)_rb2d.position.y+1, 0)) == null) {
            RaycastHit2D hit = Physics2D.Raycast(_rb2d.transform.position, new Vector3(0,1,0), 1);
            if (debugMove)
            {
                Debug.Log(hit.collider);
            }
            if (hit.collider != null && hit.collider.CompareTag("rocks"))
            {
                return;
            }
            playerAnim.SetBool("runRight", true);
            prePosition = _rb2d.gameObject.transform.position;
            _rb2d.MovePosition(_rb2d.position + _speedY);
            StartCoroutine(DisableMovementFor());
        }
        if (Input.GetKeyDown("down") && mauer.GetTile(new Vector3Int((int)_rb2d.position.x-1, (int)_rb2d.position.y-1, 0)) == null) {
            RaycastHit2D hit = Physics2D.Raycast(_rb2d.transform.position, new Vector3(0,-1,0), 1);
            if (debugMove)
            {
                Debug.Log(hit.collider);
            }
            if (hit.collider != null && hit.collider.CompareTag("rocks"))
            {
                return;
            }
            playerAnim.SetBool("runLeft", true);
            prePosition = _rb2d.gameObject.transform.position;
            _rb2d.MovePosition(_rb2d.position - _speedY);
            StartCoroutine(DisableMovementFor());
        }
	}
    
    
    //avoid bugs with movement between tiles
    IEnumerator DisableMovementFor()
    {
        if (debugMove) Debug.Log("non moveable");
        _nonMoveable = true;
        yield return new WaitForSeconds(0.1F);
        playerAnim.SetBool("runLeft", false);
        playerAnim.SetBool("runRight", false);
        _nonMoveable = false;
        
        if(debugMove) Debug.Log("now moveable");
    }
    
    
    void OnTriggerEnter2D(Collider2D col)
    {
        //Remove dirt
        if (col.gameObject.CompareTag("dirt"))
        {
            tilemap.SetTile(new Vector3Int((int)_rb2d.position.x-1, (int)_rb2d.position.y, 0), null);
        }
        
        //Gets collectable
        if (col.gameObject.CompareTag("collectable"))
        {
            collectables++;
            col.gameObject.SetActive(false);
        }

        //TODO: Remove outdates code if not needed in future & lookup how Angle works in Unity
        //Move rocks
        /*if (!col.gameObject.CompareTag("rocks")) return;
        Debug.Log(Vector3.Angle(_prePosition, col.gameObject.transform.position));
        if (Vector3.Angle(_prePosition, col.gameObject.transform.position) > 4)
        {
            rb2d.MovePosition(_prePosition);
            return;
        }

        if (col.gameObject.transform.position.x > _prePosition.x && tilemap.GetTile(new Vector3Int((int)col.GetComponent<Rigidbody2D>().position.x, (int)col.GetComponent<Rigidbody2D>().position.y, 0)) == null && tilemap.GetTile(new Vector3Int((int)col.GetComponent<Rigidbody2D>().position.x-2, (int)col.GetComponent<Rigidbody2D>().position.y, 0)) == null
            && mauer.GetTile(new Vector3Int((int)col.GetComponent<Rigidbody2D>().position.x, (int)col.GetComponent<Rigidbody2D>().position.y, 0)) == null && mauer.GetTile(new Vector3Int((int)col.GetComponent<Rigidbody2D>().position.x-2, (int)col.GetComponent<Rigidbody2D>().position.y, 0)) == null
        )
        {
            col.GetComponent<Rigidbody2D>().MovePosition(col.GetComponent<Rigidbody2D>().position + speedX);
        }tilemap.GetTile(new Vector3Int((int)col.GetComponent<Rigidbody2D>().position.x, (int)col.GetComponent<Rigidbody2D>().position.y, 0)) == null && tilemap.GetTile(new Vector3Int((int)col.GetComponent<Rigidbody2D>().position.x-2, (int)col.GetComponent<Rigidbody2D>().position.y, 0)) == null
         && mauer.GetTile(new Vector3Int((int)col.GetComponent<Rigidbody2D>().position.x, (int)col.GetComponent<Rigidbody2D>().position.y, 0)) == null && mauer.GetTile(new Vector3Int((int)col.GetComponent<Rigidbody2D>().position.x-2, (int)col.GetComponent<Rigidbody2D>().position.y, 0)) == null
        
        {
            col.GetComponent<Rigidbody2D>().MovePosition(col.GetComponent<Rigidbody2D>().position - speedX);
        }
        else
        {
            rb2d.MovePosition(_prePosition);
        }*/
    }

}

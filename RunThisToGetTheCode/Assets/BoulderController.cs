using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class BoulderController : MonoBehaviour {

    public Tilemap tilemap; //dirt
    public Tilemap mauer; //dirt
    private Rigidbody2D _boudlerRb2d;
    private Movement _player;
    private Vector2 _speedX = new Vector2(1, 0);
    private Vector2 _speedY = new Vector2(0, 1);
    private int _collectableDelay;
    private bool _waiting;

    // Use this for initialization
    void Start () {
        _boudlerRb2d = GetComponent<Rigidbody2D>();
        _collectableDelay = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        RaycastHit2D hitThis = Physics2D.Raycast(_boudlerRb2d.transform.position, new Vector3(0, -1, 0), 1);

        /*TODO-Done: fix tilemap + mauer bugs
         Debug.Log(tilemap.GetTile(new Vector3Int((int) boudler_rb2d.position.x-1, (int) boudler_rb2d.position.y - 1, 0)));
         Debug.Log(mauer.GetTile(new Vector3Int((int) boudler_rb2d.position.x -1, (int) boudler_rb2d.position.y - 1, 0)));
        tilemap.SetTile(new Vector3Int((int) boudler_rb2d.position.x-1, (int) boudler_rb2d.position.y - 1, 0), null);
        mauer.SetTile(new Vector3Int((int) boudler_rb2d.position.x -1, (int) boudler_rb2d.position.y - 1, 0), null);*/
        if (hitThis.collider != null && hitThis.collider.CompareTag("collectable") || tilemap.GetTile(new Vector3Int((int) _boudlerRb2d.position.x-1, (int) _boudlerRb2d.position.y - 1, 0)) !=
            null || mauer.GetTile(new Vector3Int((int) _boudlerRb2d.position.x-1, (int) _boudlerRb2d.position.y - 1, 0)) !=
            null)
        {
            
            _collectableDelay = 0;
            return;
        }
        if (hitThis.collider != null && hitThis.collider.CompareTag("Player") && _collectableDelay == 0)
        {
            _collectableDelay = 0;
            return;
        }
        if (hitThis.collider != null && hitThis.collider.CompareTag("Player") && _collectableDelay > 0)
        {
            SceneManager.LoadScene("GameOver");
        }
        
        if (_waiting == false && hitThis.collider == null)
        {
            
            StartCoroutine(WaitForNextMove());
            _boudlerRb2d.MovePosition(_boudlerRb2d.position - _speedY);
        }

        //TODO-Done: Check for Boulder falling
        if (_waiting == false && hitThis.collider != null && (hitThis.collider.CompareTag("rocks") || hitThis.collider.CompareTag("collectable")))
        {
            RaycastHit2D hitRight = Physics2D.Raycast(_boudlerRb2d.transform.position, new Vector3(1, 0, 0), 1);
            if (hitRight.collider == null)
            {
                RaycastHit2D hitRightDown = Physics2D.Raycast(_boudlerRb2d.transform.position + new Vector3(1, 0, 0), new Vector3(0, -1, 0), 1);
                if (hitRightDown.collider == null)
                {
                    StartCoroutine(WaitForNextMove());
                    _boudlerRb2d.MovePosition(_boudlerRb2d.position + _speedX);
                }
            }
            RaycastHit2D hitLeft = Physics2D.Raycast(_boudlerRb2d.transform.position, new Vector3(-1, 0, 0), 1);
            if (hitLeft.collider == null)
            {
                RaycastHit2D hitLeftDown = Physics2D.Raycast(_boudlerRb2d.transform.position + new Vector3(-1, 0, 0), new Vector3(0, -1, 0), 1);
                if (hitLeftDown.collider == null)
                {
                StartCoroutine(WaitForNextMove());
                _boudlerRb2d.MovePosition(_boudlerRb2d.position - _speedX);
                }
            }
        }

        // Ray from Ray: RaycastHit2D hit = Physics2D.Raycast(boudler_rb2d.transform.position + new Vector3(0, -1, 0), new Vector3(0,-1,0), 1);

        
    }
    
    IEnumerator WaitForNextMove()
    {
        _collectableDelay++;
        _waiting = true;
        yield return new WaitForSeconds(0.2F);
        _waiting = false;
    }
}

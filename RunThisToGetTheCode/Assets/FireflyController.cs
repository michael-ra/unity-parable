using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class FireflyController : MonoBehaviour
{
    public Tilemap tilemap; //dirt
    public Tilemap mauer; //dirt
    private Rigidbody2D _fireflyRb2d;
    private Vector2 _speedX = new Vector2(1, 0);
    private Vector2 _speedY = new Vector2(0, 1);
    private bool _waiting;
    public bool _isDefyingLogic;
    private bool _isRightStop;
    public int _countTimesDenyLeftMove;
    private bool _isDownStop;
    [FormerlySerializedAs("_debug")] public bool debug;
    
    void Start()
    {
        _isDefyingLogic = false;
        _fireflyRb2d = GetComponent<Rigidbody2D>();
        _isDownStop = true;
        _isRightStop = true;
    }

    void Update()
    {
        if (_countTimesDenyLeftMove < 1)
        {
            _isDefyingLogic = false;
        }

        if (_waiting) {return;}

        if (!_isRightStop)
        {
            RaycastHit2D hitRight = Physics2D.Raycast(_fireflyRb2d.transform.position, new Vector3(1, 0, 0), 1);

            if (hitRight.collider == null)
            {
                StartCoroutine(WaitForNextMove());
                if (_isDefyingLogic)
                {
                    return;
                }
                _fireflyRb2d.MovePosition(_fireflyRb2d.position + _speedX);
                return;
            }
            else
            {
                if (hitRight.collider.CompareTag("Player"))
                {
                    SceneManager.LoadScene("GameOver");
                    return;
                }
            }
            _isRightStop = true;
            _isDownStop = false;
            return;
        }
        
        if (!_isDownStop)
        {
            RaycastHit2D hitDown = Physics2D.Raycast(_fireflyRb2d.transform.position, new Vector3(0, -1, 0), 1);

            if (hitDown.collider == null)
            {
                StartCoroutine(WaitForNextMove());
                if (_isDefyingLogic)
                {
                    return;
                }
                _fireflyRb2d.MovePosition(_fireflyRb2d.position - _speedY);
                return;
            }
            else if (hitDown.collider.CompareTag("Player"))
            {
                SceneManager.LoadScene("GameOver");
                return;
            }
            _isDownStop = true;
            return;
        }
        
        RaycastHit2D hitLeft = Physics2D.Raycast(_fireflyRb2d.transform.position, new Vector3(-1, 0, 0), 1);
        if(debug) Debug.Log(hitLeft.collider + "left");
        if (hitLeft.collider != null && hitLeft.collider.CompareTag("collectable") || tilemap.GetTile(new Vector3Int((int) _fireflyRb2d.position.x-2, (int) _fireflyRb2d.position.y, 0)) !=
            null || mauer.GetTile(new Vector3Int((int) _fireflyRb2d.position.x-2, (int) _fireflyRb2d.position.y, 0)) != null)
        {
            
            RaycastHit2D hitTop = Physics2D.Raycast(_fireflyRb2d.transform.position, new Vector3(0, 1, 0), 1);
            if(debug) Debug.Log(hitTop.collider + "top");
            if (hitTop.collider != null && hitTop.collider.CompareTag("collectable") ||
                tilemap.GetTile(new Vector3Int((int) _fireflyRb2d.position.x - 1, (int) _fireflyRb2d.position.y + 1, 
                    0)) != null || mauer.GetTile(new Vector3Int((int) _fireflyRb2d.position.x - 1, (int) _fireflyRb2d.position.y + 1, 0)) != null)
            {
                
                RaycastHit2D hitRight = Physics2D.Raycast(_fireflyRb2d.transform.position, new Vector3(1, 0, 0), 1);
                if(debug) Debug.Log(hitRight.collider + "top");
                
                    if (hitRight.collider == null)
                    {
                        _isRightStop = false;
                        StartCoroutine(WaitForNextMove());
                        if (_isDefyingLogic)
                        {
                            return;
                        }
                        _fireflyRb2d.MovePosition(_fireflyRb2d.position + _speedX);
                        return;
                    }


                    if (hitRight.collider != null)
                    {
                        if (hitRight.collider.CompareTag("Player"))
                        {
                            SceneManager.LoadScene("GameOver");
                            return;
                        }
                        
                        RaycastHit2D hitDown = Physics2D.Raycast(_fireflyRb2d.transform.position, new Vector3(0, -1, 0), 1);
                        if(debug) Debug.Log(hitDown.collider + "down");
                    
                    
                        if (hitDown.collider == null)
                        {
                            _isDownStop = false;
                            StartCoroutine(WaitForNextMove());
                            if (_isDefyingLogic)
                            {
                                return;
                            }
                            _fireflyRb2d.MovePosition(_fireflyRb2d.position - _speedY);
                            return;
                        }
                        if (hitDown.collider != null && hitDown.collider.CompareTag("Player"))
                        {
                            SceneManager.LoadScene("GameOver");
                            return;
                        }
                    }
                    
                    
            }
            
            
            if (hitTop.collider == null)
            {
                StartCoroutine(WaitForNextMove());
                if (_isDefyingLogic)
                {
                    return;
                }
                _fireflyRb2d.MovePosition(_fireflyRb2d.position + _speedY);
                return;
            }
            if (hitTop.collider != null && hitTop.collider.CompareTag("Player"))
            {
                SceneManager.LoadScene("GameOver");
                return;
            }
            
            
        }
        
        
        if (hitLeft.collider != null && hitLeft.collider.CompareTag("Player"))
        {
            SceneManager.LoadScene("GameOver");
            return;
        }

        if (hitLeft.collider != null && !hitLeft.collider.CompareTag("Player"))
        {
            _countTimesDenyLeftMove++;
        }

        if (_countTimesDenyLeftMove > 5)
        {
            RaycastHit2D hitRight = Physics2D.Raycast(_fireflyRb2d.transform.position, new Vector3(1, 0, 0), 1);
            if (hitRight.collider != null)
            {
                _isDefyingLogic = true;
            }
        }

        if (hitLeft.collider == null)
        {
            StartCoroutine(WaitForNextMove());
            _fireflyRb2d.MovePosition(_fireflyRb2d.position - _speedX);
            _countTimesDenyLeftMove = 0;
        }


        if (_isDefyingLogic)
        {
            _countTimesDenyLeftMove--;
            RaycastHit2D hitRight = Physics2D.Raycast(_fireflyRb2d.transform.position, new Vector3(1, 0, 0), 1); 

            if (hitRight.collider == null)
            {
                _isRightStop = false;
                StartCoroutine(WaitForNextMove());
                _fireflyRb2d.MovePosition(_fireflyRb2d.position + _speedX);
                return;
            }

            if (hitRight.collider != null)
            {
                if (hitRight.collider.CompareTag("Player"))
                {
                    SceneManager.LoadScene("GameOver");
                    return;
                }

                RaycastHit2D hitDown =
                    Physics2D.Raycast(_fireflyRb2d.transform.position, new Vector3(0, -1, 0), 1);
                if (debug) Debug.Log(hitDown.collider + "down");


                if (hitDown.collider == null)
                {
                    _isDownStop = false;
                    StartCoroutine(WaitForNextMove());
                    _fireflyRb2d.MovePosition(_fireflyRb2d.position - _speedY);
                    return;
                }

                if (hitDown.collider != null && hitDown.collider.CompareTag("Player"))
                {
                    SceneManager.LoadScene("GameOver");
                    return;
                }
            }

        }

        
        
    }
    
    IEnumerator WaitForNextMove()
    {
        _waiting = true;
        yield return new WaitForSeconds(0.2F);
        _waiting = false;
    }
}

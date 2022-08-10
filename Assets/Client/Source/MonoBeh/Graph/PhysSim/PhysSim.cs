using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PhysSim : MonoBehaviour
{
    public JumpAi jumpTest; 
    Transform testStartPos;
    public bool offLineRender = true;
    public bool testJumpSim = false;

    private PhysicsScene2D _physicsSim;
    private LineRenderer _line;

    [SerializeField]
    private GameObject _simulatedObject;
    private SimObj _simObj; 
    private GameObject _simObjGo;
    public string CollidableTag;
    Scene _simScene;

    [SerializeField]
    int _steps = 20;
    void Awake()
    {
        testStartPos = transform;
        CreateSceneParameters _param = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
        _simScene = SceneManager.CreateScene("Simulation", _param);
        _physicsSim = _simScene.GetPhysicsScene2D();
        CreateSimeObjects();
    }

    private void CreateSimeObjects()
    {
        _simObjGo = Instantiate(_simulatedObject, Vector3.zero, Quaternion.identity);
        _simObj = _simObjGo.GetComponent<SimObj>();
        SceneManager.MoveGameObjectToScene(_simObjGo, _simScene);

        GameObject goLine = new GameObject("lineRenderer");
        _line = goLine.AddComponent<LineRenderer>();
        _line.positionCount = _steps;
        _line.startWidth = 0.1f;
        _line.endWidth = 0.1f;

        // var _simGoLine = Instantiate(goLine, Vector3.zero, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(goLine, _simScene);
        
        GameObject[] _collidable = GameObject.FindGameObjectsWithTag(CollidableTag);
        foreach (var go in _collidable)
        {
            var newGo = Instantiate(go, go.transform.position, go.transform.rotation);    
            SceneManager.MoveGameObjectToScene(newGo, _simScene);
        }
        
    }
    private void FixedUpdate() {
        if(testJumpSim)
           SimulateLaunch(testStartPos.position);
    }
    public void SimulateLaunch(Vector2 StartPos)
    {
        _simObjGo.transform.position = StartPos;
        _simObjGo.transform.rotation = Quaternion.identity;
        
        var rb = _simObjGo.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;

        rb.velocity = new Vector2(jumpTest.RunSpeed, rb.velocity.y);
        rb.AddForce(jumpTest.JumpForce * Vector2.up, ForceMode2D.Impulse);

        for (int i = 0; i < _steps; i++)
        {
            // _simObj.CheckCollider();
            _physicsSim.Simulate(Time.fixedDeltaTime);            
            if(!offLineRender)
                _line.SetPosition(i, _simObjGo.transform.position);
        }

    }

    public bool SimulateJump(Vector2 StartPos, JumpAi jumpAi, ref Vector2 landingPos)
    {
        _simObj.transform.position = StartPos;
        _simObj.transform.rotation = Quaternion.identity;
        
        // var rb = _simObj.rb;
        var rb = _simObjGo.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;

        rb.velocity = new Vector2(jumpAi.RunSpeed, 0f);
        rb.AddForce(jumpAi.JumpForce * Vector2.up, ForceMode2D.Impulse);

        // rb.velocity = new Vector2(10f, rb.velocity.y);
        // rb.AddForce(50f * Vector2.up, ForceMode2D.Impulse);
        // Debug.DrawRay(_simObjGo.transform.position, Vector3.up, Color.cyan, 10f);

        for (int i = 0; i < _steps; i++)
        {
            _simObj.CheckCollider();
            bool isCollideInAir = _simObj.isLeftContact || _simObj.isRightContact || _simObj.isTopContact || _simObj.isBottomContact;
            bool isLanded = _simObj.isBottomRayContact; //&& _simObj.isBottomContact;
            bool stepsThreshold = i > 1;

            if(isCollideInAir && stepsThreshold)
            {
                // Debug.Log("false " + i);
                // Debug.DrawRay(_simObjGo.transform.position, Vector3.up, Color.red, 10f);
                return false;
            }
            if(isLanded && stepsThreshold)
            {
                // Debug.Log("true " + i);
                // Debug.DrawRay(_simObj.GroundCheck.position, Vector3.up, Color.green, 10f);
                landingPos = _simObj.GroundCheck.position;
                return true;
            }
            _physicsSim.Simulate(Time.fixedDeltaTime);            

            if(!offLineRender)
                _line.SetPosition(i, _simObjGo.transform.position);
        }
        return false;

    }

}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiderWebTest : MonoBehaviour
{
    //Rope
    [SerializeField] private int _numOfRopeSegments = 50;
    [SerializeField] private float _ropeSegmentLength = 0.225f;

    //Physics
    [SerializeField] private Vector2 _gravityForce = new Vector2(0f, -2f);
    [SerializeField] private float _dampingFactor = 0.98f;

    //Constraints
    [SerializeField] private int _numOfConstraintRuns = 50;
    
    //LineRenderer
    private LineRenderer _lineRenderer;
    private List<RopeSegment> _ropeSegments = new List<RopeSegment>();

    private Vector3 _ropeStartPoint;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _numOfRopeSegments;

        _ropeStartPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        for (int i = 0; i < _numOfRopeSegments; i++)
        {
            _ropeSegments.Add(new RopeSegment(_ropeStartPoint));
            _ropeStartPoint.y -= _ropeSegmentLength;
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        Vector3[] ropePositions = new Vector3[_numOfRopeSegments];
        for (int i = 0; i < _ropeSegments.Count; i++)
        {
            ropePositions[i] = _ropeSegments[i].CurrentPosition;
        }
        _lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector2 CurrentPosition;
        public Vector2 OldPosition;

        public RopeSegment(Vector2 pos)
        {
            CurrentPosition = pos;
            OldPosition = pos;
        }
    }
}

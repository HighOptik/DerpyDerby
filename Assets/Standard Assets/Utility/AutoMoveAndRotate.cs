using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class AutoMoveAndRotate : MonoBehaviour
    {
        public Vector3andSpace moveUnitsPerSecond;
        public Vector3andSpace rotateDegreesPerSecond;
        public bool ignoreTimescale;
        private float m_LastRealTime;
        public Vector3 startPos;
        public Vector3 currDestination;
        public Transform destination1;
        public Transform destination2;
        public float distance;
        public float speed;

        private void Start()
        {
            startPos = transform.position;
            m_LastRealTime = Time.realtimeSinceStartup;
            currDestination = destination1.position;

        }
        // Update is called once per frame
        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, currDestination, speed * Time.deltaTime);
            if (transform.position == destination1.position)
            {
                currDestination = destination2.position;
            }
            if (transform.position == destination2.position)
            {
                currDestination = destination1.position;
            }
            float deltaTime = Time.deltaTime;
            if (ignoreTimescale)
            {
                deltaTime = (Time.realtimeSinceStartup - m_LastRealTime);
                m_LastRealTime = Time.realtimeSinceStartup;
            }
            transform.Translate(moveUnitsPerSecond.value*deltaTime, moveUnitsPerSecond.space);
            transform.Rotate(rotateDegreesPerSecond.value*deltaTime, moveUnitsPerSecond.space);
        }

        [Serializable]
        public class Vector3andSpace
        {
            public Vector3 value;
            public Space space = Space.Self;
        }
    }
}

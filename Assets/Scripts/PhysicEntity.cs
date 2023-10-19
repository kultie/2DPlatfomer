using System;
using System.Collections;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent((typeof(BoxCollider2D)))]
    public class PhysicEntity : MonoBehaviour
    {
        private const float SKIN_WIDTH = 0.015f;

        [SerializeField] private LayerMask collisionMask;

        [Range(2, 64)] [SerializeField] private int horizontalRayCount = 4;
        [Range(2, 64)] [SerializeField] private int verticalRayCount = 4;

        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;

        private BoxCollider2D _collider2D;
        private RaycastOrigins _raycastOrigins;

        public CollisionInfo collisions;

        private float _currentVelocityX;
        private float _currentVelocityY;

        private void Awake()
        {
            _collider2D = GetComponent<BoxCollider2D>();
            CalculateRaySpacing();
        }

        private void Update()
        {
            UpdateRayCastOrigins();
            collisions.Reset();
            if (_currentVelocityX != 0)
            {
                HorizontalCollision(ref _currentVelocityX);
            }

            if (_currentVelocityY != 0)
            {
                VerticalCollision(ref _currentVelocityY);
            }
            transform.Translate(new Vector2(_currentVelocityX, _currentVelocityY));
        }
        
        public void SetVelocityX(float value)
        {
            _currentVelocityX = value;
        }

        public void SetVelocityY(float value)
        {
            _currentVelocityY = value;
        }


        public void AddVelocityX(float value)
        {
            _currentVelocityX += value;
        }

        public void AddVelocityY(float value)
        {
            _currentVelocityY += value;
        }


        #region Collision Handling

        void HorizontalCollision(ref float velocity)
        {
            float directionX = Mathf.Sign(velocity);
            float rayLength = Mathf.Abs(velocity) + SKIN_WIDTH;


            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * (directionX * rayLength), Color.red);

                if (hit)
                {
                    velocity = (hit.distance - SKIN_WIDTH) * directionX;
                    rayLength = hit.distance;
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }

        void VerticalCollision(ref float velocity)
        {
            float directionY = Mathf.Sign(velocity);
            float rayLength = Mathf.Abs(velocity) + SKIN_WIDTH;


            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * i + _currentVelocityX);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * (directionY * rayLength), Color.red);
                if (hit)
                {
                    velocity = (hit.distance - SKIN_WIDTH) * directionY;
                    rayLength = hit.distance;
                    
                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }
        }

        #endregion

        #region Internal Stuff

        void UpdateRayCastOrigins()
        {
            Bounds bounds = _collider2D.bounds;
            bounds.Expand(SKIN_WIDTH * -2);

            _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        void CalculateRaySpacing()
        {
            Bounds bounds = _collider2D.bounds;
            bounds.Expand(SKIN_WIDTH * -2);


            _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        #endregion

        struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

        public struct CollisionInfo
        {
            public bool above, below;
            public bool right, left;

            public void Reset()
            {
                above = below = false;
                right = left = false;
            }
        }
    }
}
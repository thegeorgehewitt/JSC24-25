using System.Collections.Generic;
using UnityEngine;

namespace Custom.Controller
{
    public class CharacterControlWallClimb : CharacterControlBase
    {
        [Header("WALL CHECK")]
        [SerializeField] private Collider2D wallCheck;
        [SerializeField] private LayerMask climbableWallLayers;

        private ContactFilter2D contactFilter;
        private List<Collider2D> contacts;

        private bool IsNearWall { get { return wallCheck.OverlapCollider(contactFilter, contacts) > 0; } }



        private void FixedUpdate()
        {
            ExecuteMovement();
        }



        #region Movement

        private void ExecuteMovement()
        {
            
        }

        #endregion
    }
}

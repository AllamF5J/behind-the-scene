﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Subterfuge.Remake.Core.GameEvents.EventPublishers;

namespace Subterfuge.Remake.Core.Entities.Components
{
    public class VisionManager : EntityComponent, IVisionEventPublisher
    {
        private float _visionRange;
        private PositionManager position;
        private List<IEntity> _lastCheckedEntitiesInRange = new List<IEntity>();
        
        public event EventHandler<OnVisionRangeChangeEventArgs> OnVisionRangeChange;
        public event EventHandler<OnEntityEnterVisionRangeEventArgs> OnEntityEnterVisionRange;
        public event EventHandler<OnEntityLeaveVisionRangeEventArgs> OnEntityLeaveVisionRange;
        
        public VisionManager(IEntity parent, float visionRange) : base(parent)
        {
            this._visionRange = visionRange;
            this.position = parent.GetComponent<PositionManager>();
        }

        /// <summary>
        /// Gets the range of vision for the current object
        /// </summary>
        /// <returns>The object's vision range</returns>
        public float GetVisionRange()
        {
            return this._visionRange;
        }

        /// <summary>
        /// Sets the vision range to the specified value.
        /// </summary>
        /// <param name="newVisionRange">The new vision range.</param>
        /// <param name="state">The game state</param>
        /// <param name="tick">The current tick</param>
        public void SetVisionRange(float newVisionRange, IGameState state)
        {
            var previousVisionRange = this._visionRange;
            this._visionRange = newVisionRange;
            
            OnVisionRangeChange?.Invoke(this, new OnVisionRangeChangeEventArgs()
            {
                NewVisionRange = newVisionRange,
                PreviousVisionRange = previousVisionRange,
                VisionManager = this,
            });
            
            // Update what the vision manager can see.
            GetEntitiesInVisionRange(state);
        }


        /// <summary>
        /// Determines if the position is in vision of this object.
        /// </summary>
        /// <param name="tick">The tick to check</param>
        /// <param name="positionManager">The position manager</param>
        /// <returns>If the object is in the vision range.</returns>
        public bool IsInVisionRange(PositionManager positionManager)
        {
            var pos = position.CurrentLocation;
            return Vector2.Distance(pos.ToVector2(), positionManager.CurrentLocation.ToVector2()) < _visionRange;
        }

        public List<IEntity> GetEntitiesInVisionRange(IGameState state)
        {
            var currentPosition = this.position.CurrentLocation;
            var currentEntitiesInRange = state.EntitesInRange(this._visionRange, currentPosition);
            
            // Check entities that have left the vision range (or that have died/etc.)
            foreach (IEntity e in _lastCheckedEntitiesInRange)
            {
                if (!currentEntitiesInRange.Contains(e))
                {
                    OnEntityLeaveVisionRange?.Invoke(this, new OnEntityLeaveVisionRangeEventArgs()
                    {
                        EntityLeavingVision = e,
                        VisionManager = this,
                    });
                }
            }
            
            // Check new entities in range
            foreach (IEntity e in currentEntitiesInRange)
            {
                if (!_lastCheckedEntitiesInRange.Contains(e))
                {
                    OnEntityEnterVisionRange?.Invoke(this, new OnEntityEnterVisionRangeEventArgs()
                    {
                        EntityInVision = e,
                        VisionManager = this,
                    });
                }
            }

            _lastCheckedEntitiesInRange = currentEntitiesInRange;
            return _lastCheckedEntitiesInRange;
        }
    }
}
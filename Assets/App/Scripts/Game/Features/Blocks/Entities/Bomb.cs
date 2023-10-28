﻿using App.Scripts.Game.Features.BlocksSplit.Components;
using App.Scripts.Game.Features.Cutting.Components;
using App.Scripts.Game.Features.Particles.Components;
using App.Scripts.Game.Features.Physics.Components;
using App.Scripts.Game.Features.Spawning.Components;
using App.Scripts.Game.Infrastructure.Ecs.Entities;
using UnityEngine;

namespace App.Scripts.Game.Features.Blocks.Entities {
    public class Bomb : Block {
        [SerializeField] private ParticleSystem[] _particles;

        protected override void AddComponentsToBlockEntity(Entity entity, ComponentBlockSpawnData blockSpawnData) {
            entity.AddComponent(new ComponentBlockCuttable());
            entity.AddComponent(new ComponentSpawnParticleOnCut {
                Particles = _particles
            });
            entity.AddComponent(new ComponentGravity {
                Acceleration = blockSpawnData.Acceleration,
                Speed = blockSpawnData.Speed,
                StartSpeed = blockSpawnData.Speed
            });
        }
    }
}
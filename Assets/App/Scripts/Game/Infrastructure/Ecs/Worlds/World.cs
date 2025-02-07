﻿using System.Collections.Generic;
using App.Scripts.Game.Infrastructure.Ecs.Components;
using App.Scripts.Game.Infrastructure.Ecs.Entities;
using App.Scripts.Game.Infrastructure.Ecs.Systems;

namespace App.Scripts.Game.Infrastructure.Ecs.Worlds {
    public class World {
        private readonly List<Entity> _frameCacheEntities;
        private readonly List<Entity> _entities;
        private readonly List<ISystem> _systems;
        
        public World() {
            _entities = new List<Entity>();
            _frameCacheEntities = new List<Entity>();
            _systems = new List<ISystem>();
        }

        public void Initialize(IEnumerable<ISystem> systems) {
            _systems.AddRange(systems);
        }

        public void Construct() {
            foreach (var system in _systems) {
                system.World = this;
                system.OnAwake();
            }
        }

        public IList<Entity> GetEntities() {
            return _entities;
        }

        public Entity CreateEntity() {
            var entity = new Entity();
            _entities.Add(entity);
            return entity;
        }

        public Entity AppendEntity() {
            var entity = new Entity();
            return AppendEntity(entity);
        }
        
        public Entity AppendEntity(Entity entity) {
            _frameCacheEntities.Add(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity) {
            entity.CleanUp();
            _entities.Remove(entity);
        }

        public void Update(float deltaTime) {
            UpdateSystems(deltaTime);
            CommitChanges();
        }
        
        public void FixedUpdate(float deltaTime) {
            foreach (var system in _systems) {
                system.OnFixedUpdate(deltaTime);
            }
        }

        public void Clear() {
            foreach (var entity in _entities) {
                entity.CleanUp();
            }
            
            _entities.Clear();
        }

        public void Dispose() {
            foreach (var system in _systems) {
                system.OnDispose();
            }
        }

        private void CommitChanges() {
            RemoveEntitiesEndOfFrame();
            AddFrameCachedEntities();
        }

        private void UpdateSystems(float deltaTime) {
            foreach (var system in _systems) {
                system.OnUpdate(deltaTime);
            }
        }

        private void AddFrameCachedEntities() {
            if (_frameCacheEntities.Count > 0) {
                _entities.AddRange(_frameCacheEntities);
                _frameCacheEntities.Clear();
            }
        }

        private void RemoveEntitiesEndOfFrame() {
            for (var i = _entities.Count - 1; i >= 0; i--) {
                var entity = _entities[i];

                if (entity.HasComponent<ComponentRemoveEntityEndOfFrame>()) {
                    RemoveEntity(entity);
                }
            }
        }
    }
}
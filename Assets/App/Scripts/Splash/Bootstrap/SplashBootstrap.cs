﻿using System.Threading.Tasks;
using App.Scripts.Common.Scenes.Base;
using App.Scripts.Common.ViewModels;
using App.Scripts.Splash.Features.Progress.Models;
using App.Scripts.Splash.Services.Initializer;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace App.Scripts.Splash.Bootstrap {
    public class SplashBootstrap : MonoBehaviour, IInitializable {
        [SerializeField] private ViewModelViewsBootstrap _modelViewsBootstrap;
        
        private IAppInitializer _appInitializer;
        private ISceneProvider _sceneProvider;
        private IProgressReporter _progressReporter;

        [Inject]
        private void Construct(IProgressReporter progressReporter, IAppInitializer appInitializer, ISceneProvider sceneProvider) {
            _progressReporter = progressReporter;
            _appInitializer = appInitializer;
            _sceneProvider = sceneProvider;
        }

        public async void Initialize() {
            await InitializeAsync();
        }

        public async Task InitializeAsync() {
            Application.targetFrameRate = 60;
            await _modelViewsBootstrap.InitializeAsync();
            await AppInitializedAndScreenLoaded();
            await _sceneProvider.LoadSceneAsync(SceneType.Menu);
            _modelViewsBootstrap.Dispose();
        }

        private Task AppInitializedAndScreenLoaded() {
            var cancellationToken = this.GetCancellationTokenOnDestroy();
            var initializeAppTask = _appInitializer.InitializeAsync(cancellationToken);
            var loadingScreenTask = _progressReporter.ProgressAsync(cancellationToken);
            return Task.WhenAll(initializeAppTask, loadingScreenTask);
        }
    }
}
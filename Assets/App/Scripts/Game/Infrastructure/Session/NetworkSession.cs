﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Telepathy;
using UnityEngine;
using ILogger = PhlegmaticOne.Logger.Base.ILogger;

namespace App.Scripts.Game.Infrastructure.Session {
    public class NetworkSession : INetworkSession {
        private readonly NetworkClientConfig _config;
        private readonly ILogger _logger;

        private TaskCompletionSource<bool> _completionSource;
        private CancellationTokenSource _cancellationTokenSource;
        private Client _client;

        private int _clientsCount;
        
        public NetworkSession(NetworkClientConfig clientConfig, ILogger logger) {
            _config = clientConfig;
            _logger = logger;
        }
        
        public event Action Disconnected;
        
        public event Action<ArraySegment<byte>> DataReceived;
        
        public Task StartAsync() {
            Application.runInBackground = true;
            _client = new Client(_config.MaxMessageSize);
            _client.OnConnected += OnConnected;
            _client.OnDisconnected += OnDisconnected;
            _client.OnData += OnData;
            _completionSource = new TaskCompletionSource<bool>();
            _cancellationTokenSource = new CancellationTokenSource();
            _client.Connect(_config.ServerAddress, _config.ServerPort);
            ListenConnections(_cancellationTokenSource.Token);
            return _config.TestNotConnect ? Task.CompletedTask : _completionSource.Task;
        }

        public void Tick() {
            _client.Tick(_config.ProcessLimit);
        }

        public void SendMessage(ArraySegment<byte> message) {
            _client.Send(message);
        }

        public void Dispose() {
            _client.OnConnected -= OnConnected;
            _client.OnDisconnected -= OnDisconnected;
            _client.OnData -= OnData;
            _client.Disconnect();
            _clientsCount = 0;
        }

        private void OnData(ArraySegment<byte> message) {
            DataReceived?.Invoke(message);
        }

        private void OnDisconnected() {
            _logger.LogMessage("Disconnected");
        }

        private void OnConnected() {
            _logger.LogMessage("Connected");
            ++_clientsCount;

            if (_clientsCount == _config.ClientsCount) {
                _cancellationTokenSource.Cancel();
                _completionSource.SetResult(true);
            }
        }

        private async Task ListenConnections(CancellationToken cancellationToken) {
            while (_cancellationTokenSource.IsCancellationRequested == false) {
                await Task.Delay(20, cancellationToken);
                Tick();
            }
            
            _logger.LogMessage("Exiting ListenConnections");
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class LanDiscoverer {
    public class Server : IDisposable {
        private readonly string _Keyword;
        private          byte[] _Message;

        private readonly IPEndPoint _EndPoint;
        private readonly UdpClient  _UdpClient;
        private readonly int        _SleepTime;

        private readonly CancellationTokenSource _CancelTokenSrc = new();

        public Server(string keyword, int port, int sleepTime, string data = "") {
            _Keyword = keyword;
            UpdateData(data);
            _EndPoint  = new IPEndPoint(IPAddress.Broadcast, port);
            _UdpClient = new UdpClient { EnableBroadcast = true };
            _SleepTime = sleepTime;

            Task.Run(BroadcastLoop);
        }

        public void UpdateData(string data) => Interlocked.Exchange(ref _Message, Encoding.UTF8.GetBytes($"{data}{_Keyword}"));

        private async Task BroadcastLoop() {
            try {
                while (true) {
                    await _UdpClient.SendAsync(_Message, _Message.Length, _EndPoint);
                    await Task.Delay(_SleepTime, _CancelTokenSrc.Token);
                }
            } catch (ObjectDisposedException) {
                // _UdpClient disposed
            } catch (TaskCanceledException) {
                // Task cancelled 
            } catch (Exception e) {
                // Unexpected exception
                Debug.LogException(e);
            }
        }

        public void Dispose() {
            _CancelTokenSrc.Cancel();
            _CancelTokenSrc.Dispose();
            _UdpClient.Dispose();
        }
    }

    public class Client : IDisposable {
        private readonly string                      _Keyword;
        private readonly UdpClient                   _UdpClient;
        private readonly ConcurrentQueue<ListenData> _DataPool;

        public Client(string keyword, int port, ConcurrentQueue<ListenData> dataPool) {
            _Keyword   = keyword;
            _UdpClient = new UdpClient(port);
            _DataPool  = dataPool;

            Task.Run(ListenLoop);
        }

        private async Task ListenLoop() {
            try {
                while (true) {
                    var result         = await _UdpClient.ReceiveAsync();
                    var labeledMessage = Encoding.UTF8.GetString(result.Buffer);
                    if (labeledMessage.EndsWith(_Keyword)) _DataPool.Enqueue(new ListenData(result.RemoteEndPoint, labeledMessage[..^_Keyword.Length]));
                }
            } catch (ObjectDisposedException) {
                // _UdpClient disposed
            } catch (Exception e) {
                // Unexpected exception
                Debug.LogException(e);
            }
        }

        public void Dispose() {
            _UdpClient.Dispose();
        }
    }
    
    public record ListenData(IPEndPoint EndPoint, in string Message) {
        public IPEndPoint EndPoint { get; } = EndPoint;
        public string     Message     { get; } = Message;
    }
}
using System;
using Microsoft.CSharp;
using UnityEngine;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



[Serializable]
class CreatedGameMessage
{
    [Serializable]
    public class CreatedGamePayload
    {
        public string playerId;
        public string gameId;
        public int playerIndex;
        public int turn;
    }
    public string type;
    public CreatedGamePayload payload;
}

class FireMessage
{
    [Serializable]
    public class FireMessagePayload
    {
        public string gameId;
        public string playerId;
        public string tile;
    }

    public string type;
    public FireMessagePayload payload;

    public FireMessage()
    {
        payload = new FireMessagePayload();
    }
}

[Serializable]
class FireResultMessage
{
    public string type;
    [Serializable]
    public class FireResultPayload
    {
        public string status;
        public int pos;
        public int turn;
    }
    public FireResultPayload payload;
}

[Serializable]
class SetShipMessage
{
    [Serializable]
    public class SetShipPayload
    {
        public int shipIndex;
        public string[] tiles;
        public string gameId;
        public string playerId;
    }
    public string type;
    public SetShipPayload payload;

    public SetShipMessage()
    {
        payload = new SetShipPayload();
    }
}

[Serializable]
class GameSocketMessage
{
    public string type;

    public string payload;
}

class GameServerConnection : MonoBehaviour
{
    private ClientWebSocket ws;

    private CancellationTokenSource
        socketCancelSource = new CancellationTokenSource();

    private string connectionStatus { get; set; }

    private string gameId { get; set; }

    public int turn { get; set; }

    public int playerIndex { get; set; }

    private string playerId { get; set; }

    private GameServerConnection()
    {
    }

    public GameServerConnection(Uri socketServerUri)
    {
        ws = new ClientWebSocket();
        Task connectionTask =
            this
                .ws
                .ConnectAsync(socketServerUri, socketCancelSource.Token);
        connectionTask.Wait();
        connectionStatus = "connected";
    }

    public void Send(string message)
    {
        var sendBuffer =
            new ArraySegment<Byte>(Encoding.UTF8.GetBytes(message));
        Task recieveTask =
            ws
                .SendAsync(sendBuffer,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        recieveTask.Wait();
    }

    public String Receive()
    {
        ArraySegment<byte> receivedBytes =
            new ArraySegment<byte>(new byte[1024]);
        Task recieveTask =
            ws.ReceiveAsync(receivedBytes, CancellationToken.None);
        recieveTask.Wait();
        string response = Encoding.UTF8.GetString(receivedBytes.Array);
        return response;
    }

    public async void Receive(Action<String> callback)
    {
        ArraySegment<byte> receivedBytes =
            new ArraySegment<byte>(new byte[1024]);
        Task recieveTask =
            ws.ReceiveAsync(receivedBytes, CancellationToken.None);
        await Task.WhenAll(recieveTask);
        string response = Encoding.UTF8.GetString(receivedBytes.Array);
        callback(response);
    }

    public void JoinGame(Action callback)
    {
        GameSocketMessage joinGameMessage = new GameSocketMessage();
        joinGameMessage.type = "join_game";
        Send(JsonUtility.ToJson(joinGameMessage));
        Receive((string response) =>
        {
            CreatedGameMessage createdGame = JsonUtility.FromJson<CreatedGameMessage>(response);
            gameId = createdGame.payload.gameId;
            playerId = createdGame.payload.playerId;
            turn = createdGame.payload.turn;
            playerIndex = createdGame.payload.playerIndex;
            Debug.Log(response);
            if (turn == playerIndex)
            {
                Debug.Log("It is my turn");
            }
            callback();
        });
    }

    public void setShip(int shipIndex, string[] tiles)
    {
        SetShipMessage message = new SetShipMessage();
        message.type = "set_ship";
        message.payload.shipIndex = shipIndex;
        message.payload.tiles = tiles;
        message.payload.gameId = gameId;
        message.payload.playerId = playerId;
        Send(JsonUtility.ToJson(message));
    }

    public string Fire(string tileName)
    {
        Debug.Log("fire");
        Debug.Log(tileName);
        FireMessage fireMessage = new FireMessage();
        fireMessage.type = "fire";
        fireMessage.payload.gameId = gameId;
        fireMessage.payload.playerId = playerId;
        fireMessage.payload.tile = tileName;
        Send(JsonUtility.ToJson(fireMessage));
        FireResultMessage resultMessage = JsonUtility.FromJson<FireResultMessage>(Receive());
        turn = resultMessage.payload.turn;
        return resultMessage.payload.status;
    }

    public int RecieveFire()
    {
        FireResultMessage resultMessage = JsonUtility.FromJson<FireResultMessage>(Receive());
        turn = resultMessage.payload.turn;
        return resultMessage.payload.pos;
    }

    public bool IsPlayerTurn()
    {
        return turn == playerIndex;
    }
}
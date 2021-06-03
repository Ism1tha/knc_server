using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;



namespace KnC_Server
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientCheck = _packet.ReadInt();
            int _clientStatus = _packet.ReadInt();

            if(_clientCheck != _fromClient)
            {
                Console.WriteLine($"El paquete recibido no coincide con el cliente: {_fromClient} - {_clientCheck}");
            }
            else
            {
                if(_clientStatus == 1)
                {
                    Console.WriteLine($"El cliente ha respondido a la bienvenida, ahora está conectado.");
                    ServerSend.ServerStatus(_fromClient, Constants.SERVER_STATUS);
                }
            }
        }
        public static void LoginAttemptReceived(int _fromClient, Packet _packet)
        {
            int _loginSender = _packet.ReadInt();
            string _loginUsuario = _packet.ReadString();
            Server.clients[_fromClient].username = _loginUsuario;
            Console.WriteLine($"Usuario con ID de instancia {_loginSender} se quiere conectar como {_loginUsuario}");
            Server.clients[_fromClient].username = _loginUsuario;
            Server.clients[_fromClient].loggedin = true;
            Console.WriteLine($"{_loginUsuario} se ha conectado en el juego y es enviado al lobby");
            ServerSend.LoginSuccess(_fromClient);
            EndPoint _ip = Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint;
            string _country = Country.CountryByIP("85.54.219.34");
            Console.WriteLine($"Country: {_country}");
            ServerSend.OnlineUsersUpdateForce(-1); //Update the player list to every single user that is in the lobby.
        }

        public static void LobbyMessageReceived(int _fromClient, Packet _packet)
        {
            string _username = Server.clients[_fromClient].username;
            string _message = _packet.ReadString();
            Console.WriteLine($"{_username}: {_message}");
            for(int i = 1; i < Server.MaxPlayers; i++)
            {
                if(Server.clients[i].gamestatus == Constants.PLAYER_STATUS_LOBBY)
                {
                    ServerSend.LobbyXatMessage(i, _username, _message);
                    Console.WriteLine("Mensaje de lobby recibido y enviado al cliente para actualizar el chat.");
                }
            }
        }

        public static void OnlineUsersRequest(int _fromClient, Packet _packet)
        {
            for(int i = 1; i < Server.MaxPlayers; i++)
            {
                if(Server.clients[i].loggedin == true)
                {
                    ServerSend.OnlineUsersAdd(_fromClient, Server.clients[i].username, "ES");
                    Console.WriteLine("New user to the list for the client-");
                }
                if (i == Server.MaxPlayers - 1)
                {
                    ServerSend.OnlineUsersUpdate(_fromClient);
                    Console.WriteLine("Update user list send");
                }
            }
        }

        public static void clientStatusReceived(int _fromClient, Packet _packet)
        {
            int status = _packet.ReadInt();
            Server.clients[_fromClient].gamestatus = status;
            Console.WriteLine($"New client status {status}");
        }
    }
}

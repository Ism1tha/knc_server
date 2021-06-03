using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnC_Server
{
    //En esta clase definiremos todos los metodos para crear los paquetes que enviaremos mediante el servidor.
    class ServerSend
    {
        
        //Enviar un paquete a un solo usuario.
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength(); //Inserta en el inicio del paquete el Length.
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        //Enviar un paquete a todos los jugadores.
        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        //Enviar un paquete a todos los jugadores a excepción de el indicado.
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }


        //Paquetes que el servidor puede enviar..
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);
                SendTCPData(_toClient, _packet);
            }
        }

        //Enviamos el estado del servidor de juego a un cliente.
        public static void ServerStatus(int _toClient, int _serverStatus)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverStatus))
            {
                _packet.Write(Constants.SERVER_STATUS);
                SendTCPData(_toClient, _packet);
            }
        }

        public static void LoginSuccess(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.loginSuccess))
            {
                SendTCPData(_toClient, _packet);
            }
        }

        public static void LobbyXatMessage(int _toClient, string _username, string _message)
        {
            using (Packet _packet = new Packet((int)ServerPackets.lobbyXatMessage))
            {
                _packet.Write(_username);
                _packet.Write(_message);
                SendTCPData(_toClient, _packet);
                Console.WriteLine("Has enviado un mensaje de lobby a un jugador.");
            }
        }

        public static void OnlineUsersAdd(int _toClient, string _username, string _country)
        {
            using (Packet _packet = new Packet((int)ServerPackets.onlineUsersAdd))
            {
                _packet.Write(_username);
                _packet.Write(_country);
                SendTCPData(_toClient, _packet);
            }
        }

        public static void OnlineUsersUpdate(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.onlineUsersUpdate))
            {
                SendTCPData(_toClient, _packet);
                Console.WriteLine($"Enviado actualización de lista de usuarios online en lobby al cliente {_toClient}");
            }
        }

       public static void OnlineUsersUpdateForce(int _toClient)
       {
            using (Packet _packet = new Packet((int)ServerPackets.onlineUsersUpdateForce))
            {
                for(int i = 1; i < Server.MaxPlayers; i++)
                {
                    if (Server.clients[i].loggedin == true)
                    {
                        if (Server.clients[i].gamestatus == Constants.PLAYER_STATUS_LOBBY)
                        {
                            SendTCPData(i, _packet);
                            Console.WriteLine("Sent force update user list");
                        }
                    }
                }
            }
       }
    }
}

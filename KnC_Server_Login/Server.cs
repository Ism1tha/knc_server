using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace KnC_Server
{
    class Server
    {
        //Empezamos a escuchar las conexiones TCP (servidor) y gestionamos las conexiones.
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>(); //Creamos un diccionario de clientes con la estructura de cada cliente (TcpClient y sus sockets)
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers; //Asignamos al listener el número máximo de jugadores.
            Port = _port; //Asignamos al listener el puerto en el que recibiremos las conexiones.

            Console.WriteLine("Inicializando el servidor de Login para Kart n' Crazy..");

            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port); //Iniciamos el Listener sobre la IP y el puerto al arrancar el servidor.
            tcpListener.Start(); //Iniciamos a recibir conexiones sobre el Listener.
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null); //Empezamos a aceptar conexiones de clientes en el Listener.
            Console.WriteLine($"Servidor inicializado sobre el puerto {Port}");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result); //Aceptamos definitivamente la conexión con el cliente tras recibir el intento de conexión.
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null); //Permitimos aceptar conexiones de clientes al Listener nuevamente.
            Console.WriteLine($"Conexión entrante desde {_client.Client.RemoteEndPoint}...");
            //Mirará si hay algún slot libre comprobando que el socket (enchufe) sea nulo, si no es que el servidor está lleno.
            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null) //Si el slot dentro del diccionario de Clientes no tiene sockets..
                {
                    try

                    {
                        clients[i].tcp.Connect(_client); //Slot libre, iniciará la conexión del cliente con su respectiva estructura dentro de clientes.
                        Console.WriteLine("El cliente se ha conectado satisfactoriamente con el servidor.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error con la conexión del cliente {_client.Client.RemoteEndPoint}: {ex}");
                    }
                }
                else
                {
                    //Socket ocupado, pasamos al siguiente.
                }
            }
            Console.WriteLine($"{_client.Client.RemoteEndPoint} no se ha podido conectar: Servidor lleno!"); //Si llega a este punto significa que todos los sockets posibles están ocupados.
        }

        private static void InitializeServerData() //Loop para crear cada cliente en base a la classe.
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i)); //Añadimos cliente dentro del diccionaro (array) de clientes con la estructura del objeto Client.
            }
            //Asociamos cada tipo de paquete con una respuesta y en ella enviamos la información _fromClient y el paquete para ser procesado.
            packetHandlers = new Dictionary<int, PacketHandler>
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.loginAttempt, ServerHandle.LoginAttemptReceived },
                { (int)ClientPackets.SendLobbyMessage, ServerHandle.LobbyMessageReceived },
                { (int)ClientPackets.onlineUsersRequest, ServerHandle.OnlineUsersRequest },
                { (int)ClientPackets.clientStatus, ServerHandle.clientStatusReceived }
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace KnC_Server
{
    class Client //Objeto client, con esto generaremos el diccionario (array) de cada cliente posible, dentro de MaxPlayers.
    {
   
        public static int dataBufferSize = 4096; //Asignamos la memoria máxima para los clientes (enviar/recibir)

        public int id; //Variable para guardar la ID de los clientes.
        public TCP tcp; //Almacenaremos la referencia a su clase de TCP.
        public string username; //Almacenaremos el nombre de cada cliente.
        public bool loggedin; //Almacenaremos el estado de acceso de cada cliente.
        public int gamestatus; //Almacenaremos en qué parte del juego está (Lobby, Sala, Partida, Garage..)
        public int Level;

        public Client(int _clientId) //Constructor para utilizar X variables de los clientes.
        {
            id = _clientId;
            tcp = new TCP(id);
            username = "NULL";
            loggedin = false;
            gamestatus = Constants.PLAYER_STATUS_DISCONNECTED;
            Level = 0;
        }

        public class TCP
        {
            public TcpClient socket; //Guardaremos la instancia del cliente que obtenemos en el callback de conexión al servidor por parte del cliente.


            private readonly int id;
            private NetworkStream stream; //Streaming que utilizará el socket del cliente.
            private byte[] receiveBuffer; //Byte donde guardaremos la información (paquetes) recibidos por parte del cliente.
            Packet receivedData;

            public TCP(int _id) //Constructor para utilizar X id de los clientes.
            {
                id = _id; //Asignamos en la estructura de cada cliente el ID por si es necesario realizar comprobaciones.
            }

            public void Connect(TcpClient _socket) //En una instancia de TCP como cliente para el cliente.
            {
                socket = _socket; //Definimos el socket que asignamos al llamar connect para realizar la conexión de un cliente.
                socket.ReceiveBufferSize = dataBufferSize; //Asignamos máximo para recibir datos sobre la instancia
                socket.SendBufferSize = dataBufferSize; //Asignamos minímo para recibir datos sobre la instancia;
          
                stream = socket.GetStream(); //Devuelve la network stream utilizada para enviar y recibir datos de la instancia.

                receiveBuffer = new byte[dataBufferSize]; //Generamos el byte antes de conectarnos con X cliente mediante TCP.
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); //Almacenamos la información en el byte receiveBuffer y asignamos ReceiveCallback como la función que se encargará de gestionar los paquetes recibidos.

                receivedData = new Packet();

                ServerSend.Welcome(id, "iBienvenido a Kart n' Crazy Remastered! Version: Alpha");
            }

            //Función para enviar paquetes a un cliente.
            public void SendData(Packet _packet)
            {
                try
                {
                    //int _packeteTipo = _packet.ReadInt();
                    //Console.WriteLine($"Packete de tipo {_packeteTipo} enviado satisfactoriamente.");
 
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error enviando un paquete al usuario {id}: {ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result) //Callback que gestionará los paquetes recibidos hacia el servidor.
            {
                try
                {
                    int _byteLength = stream.EndRead(_result); //Almacena el tamaño de un buffer recibido al servidor. 
                    if(_byteLength <= 0)
                    {
                        //TODO: Desconectar cliente.
                        return;
                    }

                    byte[] _data = new byte[_byteLength];  //Creamos un byte donde almacenaremos el buffer del último paquete recibido en base a su tamaño.
                    Array.Copy(receiveBuffer, _data, _byteLength); //Copiamos el buffer recibido en el byte creado llamado data con la longitud del resultado.

                    receivedData.Reset(HandleData(_data));

                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error recibiendo información TCP: {_ex}");
                    //TODO: Desconectar cliente.
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;
                receivedData.SetBytes(_data);
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id,_packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }
        }

    }
}

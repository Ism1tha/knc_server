using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnC_Server
{
    class Constants
    {
        public const int TICKS_PER_SECOND = 30;
        public const int MS_PER_TICK = 1000 / TICKS_PER_SECOND;
        public const int SERVER_STATUS_ONLINE = 1;
        public const int SERVER_STATUS_MANTEINANCE = 2;
        public const int SERVER_STATUS_OFFLINE = 3;

        //Estado actual del servidor
        public static int SERVER_STATUS;

        //Estado de los jugadores
        public const int PLAYER_STATUS_DISCONNECTED = 0;
        public const int PLAYER_STATUS_LOGIN = 1;
        public const int PLAYER_STATUS_LOBBY = 2;
        public const int PLAYER_STATUS_ROOM = 3;
    }
}

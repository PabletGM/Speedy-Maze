using System;
using System.Collections.Generic;
using System.Text;

namespace SpeedyMaze
{
    class Jugador
    {
        const int numeroVidas = 3;
        //para poder acceder a fil,col y la dirección del jugador
        private int filJug;
        private int colJug;
        private int dirJug; //posición del jugador y su dirección,dirección de la siguiente iteración, 0(izquierda), 2(derecha), 1(arriba) , 3(abajo)  
                            //numero de vidas del jugador
        public int nObjetosJugador;
        public int nVidas;
        //será true mientras el jugador tenga nvidas>0
        public bool Juego=true;

        #region Objetos
            //devuelve el numero de objetos quee tiene el jugador
            public int NumeroObjetosJugador()
            {
                return nObjetosJugador;
            }

            public void AumentarObjetosJugador()
            {
                nObjetosJugador++;

            }
        #endregion
        #region Vidas
            //pone al principio del nivel las vidas a 3
            public void VidasInicioNivel(int vidasNivel)
            {
                //pones las vidas a 3
                nVidas = numeroVidas;

            }

            //actualiza vidas del jugador
            public void CuentaVidas(int vidas)
            {
                nVidas -= vidas;
            }
        //devuelve fila de pos jugador
        #endregion
        #region PosicionJugador
            public int GetJugFilas()
            {

                return filJug;
            }
            //devuelve columna de pos jugador
            public int GetJugCols()
            {
                return colJug;
            }
            //pone fila y columna inicial del jugador
            public void IniPosJugador(int filas, int columnas)
            {
                //llamamos a variable publica con referencia a struct de clase Tablero
                //le asociamos a su variable dir el parametro pasado en consola
                filJug = filas;
                colJug = columnas;
            }
            //actualiza posicion del jugador sumandole las direcciones
            public void ActualizaPosJugador(int dirfilas, int dircolumnas)
            {
                //llamamos a variable publica con referencia a struct de clase Tablero
                //le asociamos a su variable dir el parametro pasado en consola
                filJug += dirfilas;
                colJug += dircolumnas;
            }


        #endregion
        #region Direccion Jugador
            //devuelve dir de pos jugador
            public int GetJugDir()
            {
                return dirJug;
            }
            //actualiza dirección del jugador
            public void ActualizaDirJugador(int direccion)
            {
                dirJug = direccion;
            }
            //dice dirección inicial del jugador
            public void IniDirJugador(int direccion)
            {
                dirJug = direccion;
            }
            //suma 5 segundos al tiempo del nivel

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;

namespace SpeedyMaze
{

    class Tablero
    {
        //para mantener la dirección antigua
        public char oldInstruction = ' ';
        //caracteres para definir tipos de objetos
        const string enemigo = "☻", bloque = "▓▓▓▓", powerUp = "♪", objeto = "○", trampa = "×",meta= "░░░░", jugador= "☺";
        
        //Tipo de casilla que se puede dar en el juego
        public enum Casilla { None,Camino, Muro, Trampa, Objeto, PowerUp, Meta, Start };

            //para posición en fila y columna y dirección de jugador y enemigos
            public struct Ubicacion
            {
               public int fil;
               public int col;
               public int dir;//dice la dirección de la siguiente iteración, 0(izquierda), 2(derecha), 1(arriba) , 3(abajo) 
            }
            public Casilla[,] tab; //matriz de casillas del nivel //para tipo de casilla según el enum

            //indica la ubicación y dirección del enemigo
            //public Ubicacion enemyUbi; //posición del enemigo y su dirección
            public int TiempoNivel;//tiempo de un nivel en concreto
            
            //nivel actual en el que estás
            public int nivelActual;

            //modo Prueba
            private bool Debug = false;
           

            //array para cantidad de enemigos
            Ubicacion[] enemy;

            // generador de aleatorios
            Random rnd=new Random();

            //variables locales
            private int nObjetos;//para saber en todo momento cuántos objetos tienes.
            private int nTrampas;//para saber en todo momento cuántas trampas tienes.
            private int nPowerUps;//para saber en todo momento cuantos PowerUps tienes
            private int nEnemigos; //para saber numero de enemigos en todo momento
            
            //booleano de acabar un nivel
            private bool nivelAcabado = false;

        

        #region Metodos Enemigo
        private bool MismaPosEnemigo(Jugador jug)
        {
                bool MismaPos = false;

                //añadimos a 2 variables la posicion del jugador en filas y columnas
                int filaJug = jug.GetJugFilas();
                int colJug = jug.GetJugCols();

                int i = 0;
                //mientras existan enemigos en ese nivel
                if (nEnemigos > 0)
                {
                    //mientras que ninguna posicion coincida o no se supere maximo numero de enemigos
                    while (!MismaPos && i < nEnemigos)
                    {
                        //si coincide misma fila y coolumna enemigo y jug
                        if (enemy[i].fil == filaJug && enemy[i].col == colJug)
                        {
                            ////restas 1 vida al jugador
                            //jug.CuentaVidas(1);
                            MismaPos = true;
                        }
                        i++;
                    }
                }
                return MismaPos;
        }
            //asocia info de ubicacion y dirección de enemigo
            private Ubicacion CreaEnemigo(int filaEnemigo, int columnaEnemigo, int DirAleatoria)
            {
                    Ubicacion enemigo;
                    enemigo.fil = filaEnemigo;
                    enemigo.col = columnaEnemigo;
                    enemigo.dir = DirAleatoria;

                    return enemigo;
            }
            //pone a todos los enemigos una dirección aleatoria
            public void ProcesarDirEnemy()
            {
                    //recorremos todos los enemigos
                    for (int i = 0; i < enemy.Length; i++)
                    {
                        //si existe el enemigo
                        if (enemy[i].fil != -1 && enemy[i].col != -1)
                        {

                            //si la dirección del enemigo es 0 =izquierda y se puede desplazar
                            if (enemy[i].dir == 0)
                            {
                                //posicion enemigo
                                int filaEnemigo = enemy[i].fil;
                                int columnaEnemigo = enemy[i].col - 1;

                                //comprobamos si puede moverse ahí , si lo hace , cambiamos la posicion enemiga
                                if (Siguiente(filaEnemigo, columnaEnemigo))
                                {
                                    //necesitamos saber que enemigo y que posicion , actuaaliza
                                    NuevaPosEnemy(i, filaEnemigo, columnaEnemigo);
                                }
                            }

                            //si la dirección del enemigo es 2 =derecha y se puede desplazar
                            if (enemy[i].dir == 2)
                            {
                                //posicion enemigo
                                int filaEnemigo = enemy[i].fil;
                                int columnaEnemigo = enemy[i].col + 1;

                                //comprobamos si puede moverse ahí , si lo hace , cambiamos la posicion enemiga
                                if (Siguiente(filaEnemigo, columnaEnemigo))
                                {
                                    //necesitamos saber que enemigo y que posicion , actuaaliza
                                    NuevaPosEnemy(i, filaEnemigo, columnaEnemigo);
                                }
                            }

                            //si la dirección del enemigo es 1=arriba y se puede desplazar
                            if (enemy[i].dir == 1)
                            {
                                //posicion enemigo
                                int filaEnemigo = enemy[i].fil - 1;
                                int columnaEnemigo = enemy[i].col;

                                //comprobamos si puede moverse ahí , si lo hace , cambiamos la posicion enemiga
                                if (Siguiente(filaEnemigo, columnaEnemigo))
                                {
                                    //necesitamos saber que enemigo y que posicion , actuaaliza
                                    NuevaPosEnemy(i, filaEnemigo, columnaEnemigo);
                                }
                            }

                            //si la dirección del enemigo es 3=abajo y se puede desplazar
                            if (enemy[i].dir == 3)
                            {
                                //posicion enemigo
                                int filaEnemigo = enemy[i].fil + 1;
                                int columnaEnemigo = enemy[i].col;

                                //comprobamos si puede moverse ahí , si lo hace , cambiamos la posicion enemiga
                                if (Siguiente(filaEnemigo, columnaEnemigo))
                                {
                                    //necesitamos saber que enemigo y que posicion , actuaaliza
                                    NuevaPosEnemy(i, filaEnemigo, columnaEnemigo);
                                }
                            }

                            //ponemos direccion aleatoria de todos los enemigos otra vez
                            DireccionAleatoriaEnemiga();
                        }

                    }

            }

            //direccion aleatoria a cada enemigo tras el movimiento
            private void DireccionAleatoriaEnemiga()
            {
                for (int i = 0; i < enemy.Length; i++)
                {
                    enemy[i].dir = rnd.Next(0, 4);
                }
            }

            //actualiza la posicion enemiga tras moverse
            private void NuevaPosEnemy(int i, int filaEnemigo, int columnaEnemigo)
            {
                enemy[i].fil = filaEnemigo;
                enemy[i].col = columnaEnemigo;
            }
            //dice si el enemigo puede ir a esa siguiente dirección comprobando que no haya un objeto ni un powerUp ni un muro ni una trampa
            private bool Siguiente(int filaEnemigo, int columnaEnemigo)
            {
                //miramos con el tab[,] si la fila y columna enemiga corresponde a una de las casillas que no puede pisar
                if(tab[filaEnemigo, columnaEnemigo] == Casilla.Muro || tab[filaEnemigo, columnaEnemigo] == Casilla.Trampa || tab[filaEnemigo, columnaEnemigo] == Casilla.Objeto || tab[filaEnemigo, columnaEnemigo] == Casilla.PowerUp)
                {
                    return false;
                }
                else return true;

                    
                
            }
        
        #endregion

        #region Metodos Tiempo
            //resta 1 a tiempoNivel cada segundo
            public void TimerCallback()
            {
                //quitas 1 en TiempoNivel
                TiempoNivel--;
            }
            private void TiempoPorNivel()
            {//establecemos tiempos por defecto segun nivelpara dificultad añadida
                if (nivelActual == 1)
                {
                    TiempoNivel = 60;
                }
                else if (nivelActual == 2)
                {

                    TiempoNivel = 40;
                }
                //numero mayor que 2
                else
                {
                    TiempoNivel = 30;
                }


            }
            private bool TiempoDisponible()
            {
                return (TiempoNivel > 0);
            }
        //devuelve true sin queda tiempo o si variable Juego=false

        #endregion

        #region Metodos PowerUp
            private void CogerPowerUpJugador()
            {
                //le sumas 5 segundos a el tiempo del nivel
                TiempoNivel += 5;

            }
            //quita el objeto si lo hay en el area de adyacencia del jugador
            private void QuitarPowerUpMapa(Jugador jug)
            {
                //le restas la cantidad de objetos en el nivel
                nPowerUps--;
                //llamamos a el metodo   que aumenta 5 segundos al jugador
                CogerPowerUpJugador();
                //si la casilla posee un objeto , convertimos esta en camino normal
                if (tab[jug.GetJugFilas(), jug.GetJugCols()] == Casilla.PowerUp)
                {
                    tab[jug.GetJugFilas(), jug.GetJugCols()] = Casilla.Camino;
                }
            }
        #endregion

        #region Metodos Objeto
            //quita el objeto si lo hay en el area de adyacencia del jugador
            private void QuitarObjetoMapa(int filas, int columnas, Jugador jug)
            {
                //le restas la cantidad de objetos en el nivel
                nObjetos--;
                //aumentamos en 1 el numero de objetos del jugador
                jug.AumentarObjetosJugador();
                //si la casilla posee un objeto , convertimos esta en camino normal
                if (tab[filas, columnas] == Casilla.Objeto)
                {
                    tab[filas, columnas] = Casilla.Camino;
                }

            }
            //devuelve true si en las casillas adyacentes(arriba ,abajo ,derecha o izquierda) respecto a la posicion del jugador hay un objeto
            private bool ObjetoCerca(Jugador jug)
            {
                //ya poseemos la posicion del jugador
                int n = jug.GetJugFilas();
                int m = jug.GetJugCols();


                //si hay un objeto en la casilla de arriba del jugador
                if (tab[n - 1, m] == Casilla.Objeto)
                {
                    //pasamos las coordenadas de la casilla con el objeto
                    QuitarObjetoMapa(n - 1, m, jug);
                    return true;
                }
                //si hay un objeto en la casilla de abajo del jugador
                else if (tab[n + 1, m] == Casilla.Objeto)
                {
                    //pasamos las coordenadas de la casilla con el objeto
                    QuitarObjetoMapa(n + 1, m, jug);
                    return true;
                }
                //si hay un objeto en la casilla de la derecha del jugador
                else if (tab[n, m + 1] == Casilla.Objeto)
                {
                    //pasamos las coordenadas de la casilla con el objeto
                    QuitarObjetoMapa(n, m + 1, jug);
                    return true;
                }
                //si hay un objeto en la casilla de la izquierda del jugador
                else if (tab[n, m - 1] == Casilla.Objeto)
                {
                    //pasamos las coordenadas de la casilla con el objeto
                    QuitarObjetoMapa(n, m - 1, jug);
                    return true;
                }
                else
                    return false;
            }
        #endregion

        #region Metodo Trampa
            //si la posicion del jugador coincide con una trampa , le quita una vida
            private bool TrampaMismaPosJugador(Jugador jug)
            {
                int n = jug.GetJugFilas();
                int m = jug.GetJugCols();
                return (tab[n, m] == Casilla.Trampa);
            }
        #endregion

        #region ProcesaInput
            public char LeeInput()
            {
                char d = ' ';


                if (Console.KeyAvailable)
                {
                    string tecla = Console.ReadKey(true).Key.ToString();
                    if (tecla == "A" || tecla == "LeftArrow") d = 'a';
                    else if (tecla == "D" || tecla == "RightArrow") d = 'd';
                    else if (tecla == "W" || tecla == "UpArrow") d = 'w';
                    else if (tecla == "S" || tecla == "DownArrow") d = 's';
                    else if (tecla == "E" || tecla == "Spacebar") d = 'e';//coger un objeto o PowerUp                  
                    else if (tecla == "P") d = 'p'; // pausa					
                    else if (tecla == "Q" || tecla == "Escape") d = 'q'; // salir
                    else if (tecla == "M") d = 'm'; //Mostrar item
                    else if (tecla == "X") d = 'x'; //modo debug
                    //modo Pausa
                    else if (tecla == "P") d = 'p';
                    while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
                }

                return d;
            }

            public void ProcesaInput(char d, Jugador jug, Program game,ListaEnlazada listMoves,string rutaObjeto, string rutaPowerUp , string rutaTrampa,string rutaVLC)
            {

                if (d == 'w' || d == 'a' || d == 's' || d == 'd' || d == ' ')
                {
                    MoverJugador(d, jug, ref oldInstruction, game,listMoves,rutaPowerUp,rutaTrampa,rutaVLC);
                }

                switch (d)
                {

                    //pulsar spaceBar para recoger item
                    case 'e':
                        ObjetoCerca(jug);
                        //ponemos la musica de recoger objeto durante 1 segundo
                        Process p = Process.Start(rutaVLC, rutaObjeto);
                        Thread.Sleep(1000);
                        break;
                    //pulsar q escape
                    case 'q':
                        jug.Juego = false;
                        break;
                    case 'x':
                        Debug = !Debug;
                        break;
                    //pausa
                    case 'p':
                        ModoPausa();
                        break;
                }
            }
            private void ModoPausa()
            {
                string tecla = " ";

                while (tecla != "p")
                {
                    Console.SetCursorPosition(35, 15);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.Write("Menu Pausa");
                    Console.SetCursorPosition(35, 16);
                    Console.Write("(pulsa p para salir): ");

                    tecla = Console.ReadLine();
                }
                Console.ResetColor();
                Console.Clear();

            }
            private void MoverJugador(char d, Jugador jug, ref char oldInstruction, Program game, ListaEnlazada listMoves,string rutaPowerUp , string rutaTrampa, string rutaVLC)
            {
                int n = jug.GetJugFilas();
                int m = jug.GetJugCols();

                //sino se pulsa nada , d == oldIstruction
                if (d == ' ')
                {
                    d = oldInstruction;
                    //la oldInstruction o repeticion de tecla hay que ponerla en la lista de movimientos
                    listMoves.InsertaFinal(oldInstruction);
                }


                //antes de comprobar y mover al jugador comprobamos si en su posicion hay un powerUp
                if (tab[jug.GetJugFilas(), jug.GetJugCols()] == Casilla.PowerUp)
                {
                    //ponemos sonido de recoger PowerUp
                    Process p = Process.Start(rutaVLC, rutaPowerUp);
                    QuitarPowerUpMapa(jug);
                    //esperamos un segundo y quitamos la musica
                    Thread.Sleep(1000);
                    p.Kill();
                }

                //comprobar que el jugador no se está en la meta
                if (tab[jug.GetJugFilas(), jug.GetJugCols()] == Casilla.Meta)
                {
                    Meta(game, jug);
                }


                //si se pulsa w, se intenta subir 1 posicion
                if (d == 'w')
                {
                    //si la posicion de encima del jugador!=Muro , PowerUp o Objeto
                    if (n > 0 && tab[n - 1, m] != Casilla.Muro && tab[n - 1, m] != Casilla.Objeto)
                    {
                        //comprueba si hay trampa o enemigo para quitar 1 vida antes de cambiar la posicion del jugador
                        if (TrampaMismaPosJugador(jug) || MismaPosEnemigo(jug))
                        {
                           //ponemos sonido de enemigo o trampa
                            Process p = Process.Start(rutaVLC, rutaTrampa);
                            jug.CuentaVidas(1);
                            //esperamos un segundo y quitamos la musica
                            Thread.Sleep(1000);
                            p.Kill();
                        }
                        //le restamos 1 posicion en filas para que suba
                        jug.ActualizaPosJugador(-1, 0);
                        //tiene direccion 1 = arriba
                        jug.ActualizaDirJugador(1);

                    }
                    //se iguala la instrucción a otro char
                    oldInstruction = d;
                }

                //si se pulsa s se intenta bajar 1 posicion
                else if (d == 's')
                {
                    if (n < tab.GetLength(0) - 1 && tab[n + 1, m] != Casilla.Muro && tab[n + 1, m] != Casilla.Objeto)
                    {
                        //comprueba si hay trampa o enemigo para quitar 1 vida antes de cambiar la posicion del jugador
                        if (TrampaMismaPosJugador(jug) || MismaPosEnemigo(jug))
                        {
                            //ponemos sonido de enemigo o trampa
                            Process p = Process.Start(rutaVLC, rutaTrampa);
                            jug.CuentaVidas(1);
                            //esperamos un segundo y quitamos la musica
                            Thread.Sleep(1000);
                            p.Kill();
                        }
                        //le sumamos 1 posicion en filas para que baje
                        jug.ActualizaPosJugador(1, 0);
                        //tiene direccion 3 = abajo
                        jug.ActualizaDirJugador(3);

                    }
                    //se iguala la instrucción a otro char
                    oldInstruction = d;
                }

                //si se pulsa d se intenta ir 1 posicion a la derecha
                else if (d == 'd')
                {
                    if (m < tab.GetLength(1) - 1 && tab[n, m + 1] != Casilla.Muro && tab[n, m + 1] != Casilla.Objeto)
                    {
                        //comprueba si hay trampa o enemigo para quitar 1 vida antes de cambiar la posicion del jugador
                        if (TrampaMismaPosJugador(jug) || MismaPosEnemigo(jug))
                        {
                            //ponemos sonido de enemigo o trampa
                            Process p = Process.Start(rutaVLC, rutaTrampa);
                            jug.CuentaVidas(1);
                            //esperamos un segundo y quitamos la musica
                            Thread.Sleep(1000);
                            p.Kill();
                        }
                        //le sumamos 1 posicion en columnas para que vaya a la derecha
                        jug.ActualizaPosJugador(0, 1);
                        //tiene direccion 2 = derecha
                        jug.ActualizaDirJugador(2);

                    }
                    //se iguala la instrucción a otro char
                    oldInstruction = d;
                }
                //si se pulsa d se intenta ir 1 posicion a la derecha
                else if (d == 'a')
                {
                    if (m > 0 && tab[n, m - 1] != Casilla.Muro && tab[n, m - 1] != Casilla.Objeto)
                    {
                        //comprueba si hay trampa o enemigo para quitar 1 vida antes de cambiar la posicion del jugador
                        if (TrampaMismaPosJugador(jug) || MismaPosEnemigo(jug))
                        {
                            //ponemos sonido de enemigo o trampa
                            Process p = Process.Start(rutaVLC, rutaTrampa);
                            jug.CuentaVidas(1);
                            //esperamos un segundo y quitamos la musica
                            Thread.Sleep(1000);
                            p.Kill();
                        }
                        //le restamos 1 posicion en columnas para que vaya a la izquierda
                        jug.ActualizaPosJugador(0, -1);
                        //tiene direccion 0=izquierda
                        jug.ActualizaDirJugador(0);

                    }
                    //se iguala la instrucción a otro char
                    oldInstruction = d;
                }



            }
        #endregion

        #region Metodos Render
            //En este render Inicial en la casilla de Start s colocamos al jugador por defecto
            public void RenderInicial(Jugador jug)
            {
                //limpias la pantalla por defecto
                Console.Clear();

                //imprimir numero de Nivel
                Console.WriteLine("___________Nivel " + nivelActual + "___________");

                //dibujas tablero sin enemigos y personaje
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    for (int j = 0; j < tab.GetLength(1); j++)
                    {
                        Console.SetCursorPosition(j, i + 1);
                        //según valor de casilla
                        if (tab[i, j] == Casilla.Muro)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(bloque);

                        }
                        else if (tab[i, j] == Casilla.PowerUp)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(powerUp);
                        }
                        else if (tab[i, j] == Casilla.Objeto)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(objeto);
                        }
                        else if (tab[i, j] == Casilla.Trampa)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(trampa);
                        }
                        else if (tab[i, j] == Casilla.Meta)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(meta);
                        }
                        //dibujas el jugador encima
                        else if (tab[i, j] == Casilla.Start)
                        {
                            //almacenamos filas y columnas en un metodo accesible desde cualquier clasee

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(jugador);
                        }
                        else//if (tab[i, j] == Casilla.Libre) o Casilla.Camino
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  ");
                        }

                        Console.ResetColor();
                    }
                }

                //dibujas enemigos
                for (int i = 0; i < enemy.Length; i++)
                {
                    //asocias fila y columna de enemigo a 2 variables

                    if (enemy[i].fil > -1 && enemy[i].col > -1)
                    {
                        int fila = enemy[i].fil;
                        int columna = enemy[i].col;

                        //imprimimos en pantalla el enemigo colocado en la posicion que estaba
                        Console.SetCursorPosition(columna, fila);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(enemigo);
                    }

                }

                //PANELES

                //vidas
                Console.SetCursorPosition(0, tab.GetLength(0) + 1);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Vidas Jugador:" + jug.nVidas);
                //TiempoNivel
                Console.SetCursorPosition(0, tab.GetLength(0) + 2);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Tiempo Restante:" + TiempoNivel);
                //numero de enemigos por nivel
                if (Debug)
                {
                    //numero de Enemigos
                    Console.SetCursorPosition(0, tab.GetLength(0) + 3);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Numero Enemigos:" + nEnemigos);

                    //numero de Objetos
                    Console.SetCursorPosition(0, tab.GetLength(0) + 4);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Numero Objetos:" + nObjetos);

                    //numero de PowerUps
                    Console.SetCursorPosition(0, tab.GetLength(0) + 5);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Numero PowerUps:" + nPowerUps);

                    //numero de Trampas
                    Console.SetCursorPosition(0, tab.GetLength(0) + 6);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Numero Trampas:" + nTrampas);

                    //Posicion del jugador
                    Console.SetCursorPosition(0, tab.GetLength(0) + 7);
                    Console.ForegroundColor = ConsoleColor.White;
                    int n = jug.GetJugFilas();
                    int m = jug.GetJugCols();
                    Console.Write("Posicion Jugador: " + n + "," + m);
                }

                //Pones el cursor en el jugador
                Console.SetCursorPosition(jug.GetJugCols(), jug.GetJugFilas() + 1);
                Console.ResetColor();

            }

            //en este render colocamos al jugador después segun su fila y columna
            public void Render(Jugador jug)
            {
                ////limpias la pantalla por defecto
                //Console.Clear();
                Console.SetCursorPosition(0, 0);

                //imprimir numero de Nivel
                Console.WriteLine("___________Nivel " + nivelActual + "___________");

                //dibujas tablero sin enemigos y personaje
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    for (int j = 0; j < tab.GetLength(1); j++)
                    {
                        Console.SetCursorPosition(j, i + 1);
                        //según valor de casilla
                        if (tab[i, j] == Casilla.Muro)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(bloque);

                        }
                        else if (tab[i, j] == Casilla.PowerUp)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(powerUp);
                        }
                        else if (tab[i, j] == Casilla.Objeto)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(objeto);
                        }
                        else if (tab[i, j] == Casilla.Trampa)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(trampa);
                        }
                        else if (tab[i, j] == Casilla.Meta)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(meta);
                        }
                        //dibujas el jugador encima
                        else if (tab[i, j] == Casilla.Start)
                        {
                            //almacenamos filas y columnas en un metodo accesible desde cualquier clasee

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(meta);
                        }
                        else//if (tab[i, j] == Casilla.Libre) o Casilla.Camino
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  ");
                        }

                        Console.ResetColor();
                    }
                }

                //dibujas enemigos
                for (int i = 0; i < enemy.Length; i++)
                {
                    //asocias fila y columna de enemigo a 2 variables

                    if (enemy[i].fil > -1 && enemy[i].col > -1)
                    {
                        int fila = enemy[i].fil;
                        int columna = enemy[i].col;

                        //imprimimos en pantalla el enemigo colocado en la posicion que estaba en fila+1 ya que al principio está el nombre del nivel
                        Console.SetCursorPosition(columna, fila + 1);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(enemigo);
                    }

                }

                //dibujas jugador calculando su posición y colocándolo
                int x = jug.GetJugFilas();
                int y = jug.GetJugCols();
                Console.SetCursorPosition(y, x + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(jugador);

                //PANELES

                //vidas
                Console.SetCursorPosition(0, tab.GetLength(0) + 1);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Vidas Jugador:" + jug.nVidas);
                //TiempoNivel
                Console.SetCursorPosition(0, tab.GetLength(0) + 2);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Tiempo Restante: " + TiempoNivel + " ");

                //numero de Objetos del jugador
                Console.SetCursorPosition(0, tab.GetLength(0) + 3);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Numero Objetos del Jugador:" + jug.nObjetosJugador);

                //numero de enemigos por nivel
                if (Debug)
                {
                    //numero de Objetos
                    Console.SetCursorPosition(0, tab.GetLength(0) + 4);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Numero Objetos del mapa:" + nObjetos);

                    //numero de Enemigos
                    Console.SetCursorPosition(0, tab.GetLength(0) + 5);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Numero Enemigos:" + nEnemigos);



                    //numero de PowerUps
                    Console.SetCursorPosition(0, tab.GetLength(0) + 6);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Numero PowerUps:" + nPowerUps);

                    //numero de Trampas
                    Console.SetCursorPosition(0, tab.GetLength(0) + 7);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Numero Trampas:" + nTrampas);

                    //Posicion del jugador
                    Console.SetCursorPosition(0, tab.GetLength(0) + 8);
                    Console.ForegroundColor = ConsoleColor.White;
                    int n = jug.GetJugFilas();
                    int m = jug.GetJugCols();
                    Console.Write("Posicion Jugador: " + n + "," + m);

                    //direccion jugador
                    Console.SetCursorPosition(0, tab.GetLength(0) + 9);
                    Console.ForegroundColor = ConsoleColor.White;
                    int direccion = jug.GetJugDir();
                    Console.Write("Direccion Jugador: " + NumeroDireccion(direccion)+"     ");
                }

                //Pones el cursor en el jugador
                Console.SetCursorPosition(jug.GetJugCols(), jug.GetJugFilas() + 1);
                Console.ResetColor();

            }
        
        #endregion

        #region MetodosDeNivel
            public int FilaMeta()
            {
                int filaMeta = 0;
                
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    for (int j = 0; j < tab.GetLength(1); j++)
                    {
                        if (tab[i, j] == Casilla.Start)
                        {
                             filaMeta = i;
                        }
                    }
                }
                return filaMeta;
            }
            public int ColumnaMeta()
            {
                int ColumnaMeta = 0;

                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    for (int j = 0; j < tab.GetLength(1); j++)
                    {
                        if (tab[i, j] == Casilla.Start)
                        {
                            ColumnaMeta = j;
                        }
                    }
                }
                return ColumnaMeta;
            }
            public int NumeroNiveles()
            {
                    //nombre de archivo que queremos que lea
                    string file = "Niveles.txt";
                    bool nivelEncontrado = false;
                    string[] s;
                    int numeroNiveles = 0;
                    //conexion con archivo
                    StreamReader f = new StreamReader(file);
                    string linea = "";
                    //leemos lineas hasta que acabe el txt
                    while (!f.EndOfStream)
                    {
                        //reinicias la búsqueda
                        nivelEncontrado = false;
                        //Dividimos cada linea  leida en palabras
                        linea = f.ReadLine();
                        s = linea.Split(' ');
                        //se busca cada palabra de la linea leida
                        for (int i = 0; i < s.Length && nivelEncontrado != true; i++)
                        {
                            if (s[i] == "Level")
                            {
                                //hemos encontrado un nivel se le suma 1 al numero de niveles
                                numeroNiveles++;
                                nivelEncontrado = true;
                            }
                        }
                    }
                    return numeroNiveles;
                }
                //cuando llegas a la meta
            private bool Meta(Program game, Jugador jug)
            {
                //si el numero de niveles superados es el total ponemos bool juego a false
                if (game.numNivelesSup >= NumeroNiveles())
                {
                    jug.Juego = false;
                }
                //el nivel se acaba
                NivelAcabado = true;
                return NivelAcabado;
            }
            public bool FinJuego(Jugador jug)
            {
                
                    return (!TiempoDisponible() || jug.nVidas <= 0 );
                
            }
            //lee el nivel del txt hasta encontrar el nivel numeroNivel
            public void LeeNivel(int numeroNivel, Jugador jug, string file)
            {
                //nombre de archivo que queremos que lea
                string txt = file;
                //conexion con archivo
                StreamReader f = new StreamReader(file);
                string[] s;
                bool nivelEncontrado = false;
                int NivelActual;
                string linea = "";
                //ponemos array de enemigos totales a 100 como máximo
                enemy = new Ubicacion[10];

                //por defecto numero de vidas del jugador a 3

                //leemos lineas hasta que no encuentre una con la palabra "Level"
                while (!nivelEncontrado)
                {
                    //Dividimos cada linea  leida en palabras
                    linea = f.ReadLine();
                    
                         s = linea.Split(' ');
                        //se busca cada palabra de la linea leida
                        for (int i = 0; i < s.Length && nivelEncontrado != true; i++)
                        {
                            if (s[i] == "Level")
                            {
                                //convertimos a int el numero leido después de Level " " 
                                NivelActual = int.Parse(s[i + 1]);
                                //ponemos variable publica de nivelActual con palabra leida
                                nivelActual = NivelActual;
                                //establecemos variable publica tiempo por Nivel según el nivel
                                TiempoPorNivel();
                                if (numeroNivel == NivelActual)
                                {
                                    nivelEncontrado = true;
                                }
                            }
                        }
                }
               
                //una vez que se encuentra el nivel asociamos  info de lineas hasta encontrar linea vacia
                while (linea != "")
                {
                    //primera linea con valor será numero de filas
                    int filas = int.Parse(f.ReadLine());
                    //segunda fila con valor será numero de columnas
                    int columnas = int.Parse(f.ReadLine());
                    //asociamos a tamaño matriz casilla cas 
                    tab = new Casilla[filas, columnas];
                    //leemos la dirección Derecha , Izquierda , Arriba o Abajo
                    string dir = f.ReadLine();
                    //pasando la dirección como argumento lo convertimos en un numero
                    int direccion = DireccionNumero(dir);
                    jug.IniDirJugador(direccion);
                    //pasamos como parámetro el numero que será la dirección inicial del Jugador
                    


                    //para que lea la siguiente linea , deberia ser en blanco
                    linea = f.ReadLine();
                }

                //por defecto ponemos todos los enemigos y sus posiciones a 0
                for (int i = 0; i < enemy.Length; i++)
                {
                    enemy[i].fil = -1;
                    enemy[i].col = -1;
                }

                //por defecto ponemos todo a null
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    for (int j = 0; j < tab.GetLength(0) && j < linea.Length; j++)
                    {
                        tab[i, j] = Casilla.None;
                    }
                }

                //ahora queda la lectura de Tablero
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    //leemos una linea
                    linea = f.ReadLine();
                    //la dividimos por palabras
                    //s = linea.Split(' ');

                    for (int j = 0; j < tab.GetLength(1) && j < linea.Length; j++)
                    {
                        //muro
                        if (linea[j] == '#')
                        {
                            tab[i, j] = Casilla.Muro;
                        }
                        //enemigo , por defecto todos enum Camino(0)
                        else if (linea[j] == 'e')
                        {
                            //dan numeros del 0 al 3,0(izq), 2(der), 1(arriba) , 3(abajo)
                            int DirAleatoria = rnd.Next(0, 4);
                            //en posición array nEnemigos se crea uno y se asocia su info
                            enemy[nEnemigos] = CreaEnemigo(i, j, DirAleatoria);
                            //se suma 1 al contador
                            nEnemigos++;
                        }
                        //powerUp
                        else if (linea[j] == '=')
                        {
                            tab[i, j] = Casilla.PowerUp;
                            nPowerUps++;
                        }
                        //objeto
                        else if (linea[j] == 'o')
                        {
                            tab[i, j] = Casilla.Objeto;
                            nObjetos++;
                        }
                        //trampa
                        else if (linea[j] == 'x')
                        {
                            tab[i, j] = Casilla.Trampa;
                            nTrampas++;
                        }
                        //entrada o start
                        else if (linea[j] == 's')
                        {
                            tab[i, j] = Casilla.Start;
                            jug.IniPosJugador(i, j);
                        }
                        //meta
                        else if (linea[j] == 'm')
                        {
                            tab[i, j] = Casilla.Meta;
                        }
                        //sino es ninguna de las demas significa que es s[j]== "-" , esto es camino Libre
                        else
                        {
                            tab[i, j] = Casilla.Camino;
                        }
                    }
                }

            }
            //para poder darle valor a bool nivelAcabado y que lo devuelva al llamar a la propiedad
            public bool NivelAcabado
            {
                get { return nivelAcabado; }
                set { nivelAcabado = value; }
            }
        #endregion

        #region MetodosExtraConversion
            //devuelve un valor numerico cogiendo un string de entrada
            private int DireccionNumero(string direccion)
            {
                int direccionNumerica = 0;

                switch (direccion)
                {
                    case "Arriba":
                        direccionNumerica = 1;
                        break;

                    case "Abajo":
                        direccionNumerica = 3;
                        break;

                    case "Derecha":
                        direccionNumerica = 2;
                        break;

                    case "Izquierda":
                        direccionNumerica = 0;
                        break;
                }

                return direccionNumerica;
            }

            //devuelve una direccion cogiendo un string de entrada
            private string NumeroDireccion(int direccion)
            {
                string direccionNumerica = "";

                switch (direccion)
                {
                    case 1:
                        direccionNumerica = "Arriba";
                        break;

                    case 3:
                        direccionNumerica = "Abajo";
                        break;

                    case 2:
                        direccionNumerica = "Derecha";
                        break;

                    case 0:
                        direccionNumerica = "Izquierda";
                        break;
                }

                return direccionNumerica;
            }
        #endregion
    }
}

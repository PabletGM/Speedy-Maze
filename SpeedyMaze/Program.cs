 using System;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace SpeedyMaze
{
    
    class Program
    {
        const int NumeroCiclosSec= 3;
        const int retardoBucle = 300;
        //Para llevar la cuenta de los niveles que has superado
        public int numNivelesSup = 1;

        static void Main(string[] args)
        {  
            //para tener disponibles todo tipo de caracteres
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //uso de clase Tablero
            Tablero tab = new Tablero();
            //uso de la clase Jugador
            Jugador jug = new Jugador();
            //uso de la clase Program
            Program game = new Program();
            #region MusicaJuego
                //Ubicación del programa VLC cancion
                string rutaVLC = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
                
                //ruta banda sonora
                string bandaSonora= @"--intf dummy  C:\hlocal\MusicaProyectoFinalFP2\banda_sonora.wav";

                //Ruta  DE la cancion PowerUp
                string rutaPowerUp = @"--intf dummy  C:\hlocal\MusicaProyectoFinalFP2\Power_Up.wav";

                //Ruta  DE la cancion Trampa
                string rutaTrampa = @"--intf dummy   C:\hlocal\MusicaProyectoFinalFP2\Trampa.wav";

                //Ruta  DE la cancion Objeto
                string rutaObjeto = @"--intf dummy   C:\hlocal\MusicaProyectoFinalFP2\Objeto.wav";

                //Ruta  DE la cancion Win
                string Win = @"--intf dummy  C:\hlocal\MusicaProyectoFinalFP2\Win.wav";

                //Ruta  DE la cancion Game_Over
                string GameOver = @"--intf dummy   C:\hlocal\MusicaProyectoFinalFP2\Game_over.wav";


            #endregion
            //menus
            #region Menus
            //menú principal con diferentes opciones , iniciamos musica si está activada
           
                Process p = Process.Start(rutaVLC, bandaSonora);
                MainMenu(rutaVLC, bandaSonora, p);
            
             //damos la opcion de cargar una partida ya existente debajo de los menus para poder jugar niveles creados
             Console.Clear();
            #endregion
            //partida existente
            #region PartidaExistente/Creada
                Console.WriteLine("Quieres cargar una partida?: [s/n]");
                string opcion = Console.ReadLine();

                if(opcion=="s")
                {
                    PartidaExistenteCreada(tab, jug, game);
                }
                else
                {
                    LecturaRenderInicial(jug, tab, ref game.numNivelesSup, "Niveles.txt");
                }
            
                
            #endregion
            //bucle general
            #region BucleGeneral
            //booleano para forzar salida con q en caso de querer guardar partida
            bool forzarSalida=false;
                //mientras que los niveles superados sea menor a el numero total de niveles
                while (game.numNivelesSup <= tab.NumeroNiveles() && !tab.FinJuego(jug) && !forzarSalida)
                {
                    //uso de lista para movimientos por cada nivel
                    ListaEnlazada listMoves = new ListaEnlazada();

                    //bucle general con cada`pista de musica
                    BucleGeneral(tab, jug, game,listMoves,GameOver,rutaVLC,rutaPowerUp,rutaTrampa,rutaObjeto,ref forzarSalida );
                   
                    //aumentamos el numero de niveles superados si  metodo finJuego=false y se ha acabado el nivel
                    if (!tab.FinJuego(jug) && tab.NivelAcabado)
                    {
                         //al salir del bucle general del nivel , opcion demo  si te has pasado el nivel
                        ModoDemo(tab, jug, game, listMoves, rutaObjeto, rutaPowerUp, rutaTrampa,rutaVLC );
                        game.numNivelesSup++;
                    }

                    //reinicias el booleano al ser un nivel nuevo, este no está acabado
                    tab.NivelAcabado = false;

                    //leemos el siguiente nivel si no hemos perdido y si hay siguiente nivel
                    if (!tab.FinJuego(jug) && game.numNivelesSup <= tab.NumeroNiveles() && !forzarSalida)
                        LecturaRenderInicial(jug, tab, ref game.numNivelesSup, "Niveles.txt");
                }
   
                //has ganado si finJuego=true y todos los niveles superados y aun tienes vidas
                if ( game.numNivelesSup >= tab.NumeroNiveles() && jug.nVidas > 0)
                {
                    Victoria(jug);
                    //ejecutamos cancion de victoria
                    Process p2 = Process.Start(rutaVLC, Win);
                    //dejamos la pantalla de victoria 2 segundos
                    Thread.Sleep(4000);
                    //paramos cancion de victoria
                    p2.Kill();    
                }

            #endregion
            //guardar partida
            #region GuardarPartida
            //opcion de guardar la partida
            Console.Write("¿Quieres guardar la partida?: [s/n]");
                string respuesta = Console.ReadLine();
                //Si el jugador desea seguir jugando desde el último nivel completado
                if (respuesta == "s")
                {
                    Console.Write("Nombre de la partida(añade .txt): ");
                    string file2 = Console.ReadLine();

                    //almacenamos los objetos recolectados del jugador 
                    int nObjetos = jug.NumeroObjetosJugador();
                    //leemos el nivel en el que estamos
                    int numeroNivel = game.numNivelesSup;
                    //el tiempo que queda de nivel
                    int TiempoNivel = tab.TiempoNivel;
                    //numero de vidas del jugador
                    int nVidas = jug.nVidas;
                    //posicion del jugador
                    int filas = jug.GetJugFilas();
                    int columnas = jug.GetJugCols();
                    //dirección del jugador
                    int dir = jug.GetJugDir();
                    //tanto si existe como sino guardamos el archivo
                    if (File.Exists(file2))
                    {
                        GuardarPartida(nObjetos, numeroNivel, TiempoNivel, nVidas, filas, columnas, dir, file2);
                    }
                    else
                    {
                        GuardarPartida(nObjetos, numeroNivel, TiempoNivel, nVidas, filas, columnas, dir, file2);
                    }

                }
            #endregion
        }
        #region Metodos GuardarPartida, Leerla
            private static void GuardarPartida(int nObjetos, int numeroNivel, int TiempoNivel, int nVidas, int filas, int columnas, int dir, string file2)
            {
                //creas la conexion entre el txt con nombre file2 y aquí
                StreamWriter f = new StreamWriter(file2);
                //escribes numero de nivel en la primera Linea
                f.WriteLine("Level " + numeroNivel);
                //escribes numero de objetos recolectados en la segunda Linea
                f.WriteLine("Objetos: " + nObjetos);
                //escribes Tiempo del nivel en la tercera Linea
                f.WriteLine("Tiempo: " + TiempoNivel);
                //escribes numero de vidas del jugador en la cuarta Linea
                f.WriteLine("Vidas: " + nVidas);
                //escribes la posicion del jugador
                f.WriteLine("Pos: " + filas + " " + columnas);
                //escribes direccion del jugador en la sexta Linea
                f.WriteLine("Dir: " + dir);

                f.Close();
            }

            //leemos el nivel en el que estamos, el numero de objetos que tenemos, el tiempo que queda y el numero de vidas y la pos del jugador y dirección
            private static void LeerPartidaGuardada(string opcion2, Tablero tab, Jugador jug, Program game)
            {
                //conectamos el txt opcion2 
                StreamReader read = new StreamReader(opcion2);
                //inicializamos variables , leemos la primer linea
                string linea;
                //mientras que no acabe el txt seguir leyendo lineas
                while (!read.EndOfStream)
                {
                    //leemos la linea
                    linea = read.ReadLine();
                    //la dividimos sabiendo que buscamos la primera palabra de cada linea
                    string[] palabras = linea.Split();

                    //encontramos el numero de nivel
                    if (palabras[0] == "Level ")
                    {
                        int numNivel = int.Parse(Convert.ToString(palabras[1]));
                        game.numNivelesSup = numNivel;
                    }
                    //encontramos el numero de objetos recolectados
                    else if (palabras[0] == "Objetos:")
                    {
                        int numObjetos = int.Parse(Convert.ToString(palabras[1]));
                        jug.nObjetosJugador = numObjetos;
                    }
                    //encontramos el tiempo por nivel
                    else if (palabras[0] == "Tiempo:")
                    {
                        int tiempoNivel = int.Parse(Convert.ToString(palabras[1]));
                        tab.TiempoNivel = tiempoNivel;
                    }
                    //encontramos el numero de vidas
                    else if (palabras[0] == "Vidas:")
                    {
                        int vidas = int.Parse(Convert.ToString(palabras[1]));
                        jug.nVidas = vidas;
                    }
                    //encontramos las filas posicion del jugador
                    else if (palabras[0] == "Pos:")
                    {
                        int filas = int.Parse(Convert.ToString(palabras[1]));
                        int cols = int.Parse(Convert.ToString(palabras[2]));
                        jug.IniPosJugador(filas, cols);
                    }
                    //encontramos la direccion del jugador
                    else if (palabras[0] == "Dir:")
                    {
                        int dir = int.Parse(Convert.ToString(palabras[1]));
                        jug.IniDirJugador(dir);
                    }


                }


            }
            private static void PartidaExistenteCreada(Tablero tab,Jugador jug, Program game)
            {
                Console.WriteLine("Quieres cargar una partida ya existente?: [s/n]");
                string opcion = Console.ReadLine();
                if (opcion == "s")
                {
                    Console.WriteLine("Dime el nombre del archivo añadiendo .txt");
                    string opcion2 = Console.ReadLine();
                    //si existe el archivo lo leemos
                    try
                    {
                        if (File.Exists(opcion2))
                        {
                            //lectura y renderizado inicial por defecto el txt que ya funciona
                            LecturaRenderInicial(jug, tab, ref game.numNivelesSup,"Niveles.txt");
                            //leemos el nivel en el que estamos, el numero de objetos que tenemos, el tiempo que queda y el numero de vidas.
                            LeerPartidaGuardada(opcion2, tab, jug, game);
                            Console.Clear();
                        }

                    }
                    catch
                    {
                        throw new Exception("No existe un archivo guardado con ese nombre");
                    }

                }
                else 
                {
                    //nivel creado
                    Console.WriteLine("¿Quieres jugar un nivel creado?: [s/n]");
                    string nivelCreado = Console.ReadLine();
                    try
                    {
                        if (nivelCreado == "s")
                        {
                            Console.WriteLine("Dime el nombre del archivo añadiendo .txt");
                            string file = Console.ReadLine();
                            int numeroNivelCreado = 10;
                            //lectura y renderizado inicial del nivel creado , que será el 10 por defecto
                            if (File.Exists(file))
                            { LecturaRenderInicial(jug, tab, ref numeroNivelCreado, file); }
                        }
                    }
                    catch
                    {
                        throw new Exception("No existe un archivo guardado con ese nombre");
                    }
                }
            }
        #endregion
        #region ModoDemo
            private static void ModoDemo(Tablero tab, Jugador jug, Program game, ListaEnlazada listMoves,string rutaObjeto,string rutaPowerUp,string rutaTrampa,string rutaVLC)
            {
                //limpias la pantalla anterior
                Console.Clear();

                Console.Write("Quieres ver el modo Demo del nivel completado?: [s/n] ");
                string respuesta = Console.ReadLine();

                //si dice que si activas bucle
                if (respuesta == "s")
                {
                    //contador de movimientos
                    int cMoves = 0;
                    int contadordeVueltas = 0;
                    //colocamos al jugador en la casilla de salida, la buscamos
                    int FilaStart = tab.FilaMeta(), ColumnaStart = tab.ColumnaMeta();
                    
                    //lectura y renderizado inicial por defecto el txt que ya funciona
                    LecturaRenderInicial(jug, tab, ref game.numNivelesSup, "Niveles.txt");
                    //bucle general modo Demo
                    while (cMoves < listMoves.NumElems())
                    {
                        contadordeVueltas++;
                        //movimiento aleatorio de enemigos
                        tab.ProcesarDirEnemy();
                        //la procesa y actua en consecuencia , debería de poner la posicion de la lista que quiere leer en cada turno e ir aumentando en 1 todo el rato
                        tab.ProcesaInput(listMoves.Nesimo(cMoves), jug, game,listMoves, rutaObjeto, rutaPowerUp, rutaTrampa, rutaVLC);
                        //lo renderiza
                        tab.Render(jug);
                        //cada 5 vueltas resta 1 segundo
                        if (contadordeVueltas == NumeroCiclosSec)
                        {
                            //reiniciamos el contador
                            contadordeVueltas = 0;
                            tab.TimerCallback();
                        }
                        //aumentas numero de movimientos hechos
                        cMoves++;
                        //retardo 
                        Thread.Sleep(retardoBucle);
                    }
                }
            }
        #endregion
        #region Metodos CreadorNiveles
            private static void CreadorNiveles()
            {
                //limpias lo anterior
                Console.Clear();

                Console.WriteLine("Dime que anchura tendrá tu juego(se recomienda minimo de 20): ");
                int columnaMaxima = int.Parse(Console.ReadLine());
                Console.WriteLine("Dime que altura tendrá tu juego(se recomienda minimo de 20): ");
                int filaMaxima = int.Parse(Console.ReadLine());

                //inicializas array donde guardar cada caracter del mapa
                char[,] tablero = new char[filaMaxima, columnaMaxima];
                
                //limpias lo anterior
                Console.Clear();


                //das las instrucciones
                Console.SetCursorPosition(0, filaMaxima + 2);
                Console.Write("e --> enemigo");

                Console.SetCursorPosition(25, filaMaxima + 2);
                Console.Write("p --> bloques");

                Console.SetCursorPosition(50, filaMaxima + 2);
                Console.Write("i --> powerUp");

                Console.SetCursorPosition(0, filaMaxima + 4);
                Console.Write("o --> objeto");

                Console.SetCursorPosition(25, filaMaxima + 4);
                Console.Write("× --> trampa");

                Console.SetCursorPosition(50, filaMaxima + 4);
                Console.Write("m --> meta");

                Console.SetCursorPosition(0, filaMaxima + 6);
                Console.Write("j -->  jugador");

                Console.SetCursorPosition(25, filaMaxima + 6);
                Console.Write("c  -->  camino Libre");


                Console.SetCursorPosition(0, filaMaxima + 10);
                Console.Write("Escribe caracteres en su posicion , si has acabado , pulsa q para guardar el nivel: ");

                Console.SetCursorPosition(0, filaMaxima + 12);
                Console.Write("La posicion del jugador lo ubicarás después: ");

                Console.SetCursorPosition(0, filaMaxima + 14);
                Console.Write("Para que un nivel sea valido debe tener 1 casilla de jugador, 1 casilla de meta  \n y ninguna parte del mapa sin caracteres ");
                
                

                BucleCreadorNiveles(columnaMaxima, filaMaxima, tablero);

            }
            private static char LeeInputCreadorNiveles()
            {
                char d = ' ';


                if (Console.KeyAvailable)
                {
                    string tecla = Console.ReadKey(true).Key.ToString();
                    //caracteres juego
                    if (tecla == "E") d = 'e';//enemigo
                    else if (tecla == "C") d = 'c';//camino libre
                    else if (tecla == "P") d = 'p';//muro
                    else if (tecla == "I") d = 'i';//powerUp
                    else if (tecla == "O") d = 'o';//objeto
                    else if (tecla == "X") d = 'x';//trampa
                    else if (tecla == "J") d = 'j';//jugador
                    else if (tecla == "M") d = 'm';//meta				
                    else if (tecla == "Q") d = 'q';//acabar construccion del nivel

                    //posicion
                    else if (tecla == "D") d = 'd';//derecha
                    else if (tecla == "A") d = 'a';//izquierda         
                    else if (tecla == "W") d = 'w';//arriba				
                    else if (tecla == "S") d = 's';//abajo


                    while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
                }

                return d;
            }
            private static void ProcesaInputCreadorNiveles(char d, ref int filaUsuario, ref int colUsuario, int columnaMaxima, int filaMaxima, ref char[,] tablero)
            {
                //si es una tecla que nos interesa
                //# = p= pared  = bloque
                //= -->powerUp = i
                // - --->camino libre = c
                if (d == 'p' || d == 'i' || d == 'x' || d == 'm' || d == 'o' || d == 'e' || d == 'w' || d == 's' || d == 'a' || d == 'd' || d == 'c' || d == 'j')
                {
                    switch (d)
                    {
                        //para mover al usuario por la pantalla
                        case 'w':
                            if (filaUsuario > 0) { filaUsuario--; }
                            break;

                        case 's':
                            //segun limite dimension
                            if (filaUsuario < filaMaxima - 1) filaUsuario++;
                            break;

                        case 'a':
                            if (colUsuario > 0) { colUsuario--; }
                            break;

                        case 'd':
                            //segun limite dimension
                            if (colUsuario < columnaMaxima - 1) colUsuario++;
                            break;

                        default:
                            RenderCreadorNiveles(filaUsuario, colUsuario, d);
                            break;

                    }

                    //lo desplazas , asi ves el cursor siempre
                    Console.SetCursorPosition(colUsuario, filaUsuario);
                    
                }
            }
            private static void RenderCreadorNiveles(int filaUsuario, int colUsuario, char d)
            {   //metodo de renderizado   
                    Console.Write(d);
            }
            private static void BucleCreadorNiveles(int columnaMaxima, int filaMaxima, char[,] tablero)
            {
                //te colocas en el 0,0 por defecto
                Console.SetCursorPosition(0, 0);
                //para cambiar pos de usuario
                int filaUsuario = 0;
                int columnaUsuario = 0;
                //caracter vacio
                char d = ' ';
                //mientras que jugador tenga vidas , no sea finJuego
                while (d != 'q')
                {
                    //leemos la tecla pulsada
                    d = LeeInputCreadorNiveles();
                    //almacenamos la tecla en su posicion del tablero si es un caracter del mapa
                    if (d == 'p' || d == 'i' || d == 'x' || d == 'm' || d == 'o' || d == 'e' || d == 'c' || d == 'j')
                    { tablero[filaUsuario, columnaUsuario] = d; }
                    //la procesa y actua en consecuencia
                    ProcesaInputCreadorNiveles(d, ref filaUsuario, ref columnaUsuario, columnaMaxima, filaMaxima, ref tablero);
                }

                if (NivelValido(filaMaxima, columnaMaxima, tablero))
                {
                    Console.Clear();
                    //crear nivel con .txt
                    Console.Write("Nombre del nivel creado(añade .txt): ");
                    string nivelCreado = Console.ReadLine();
                    //si es valido que puedas guardarlo
                    GuardarPartidaCreadorNiveles(nivelCreado, tablero, filaMaxima, columnaMaxima);
                }
            }

            //guardamos en un archivo la partida guardada
            private static void GuardarPartidaCreadorNiveles(string nivelCreado, char[,] tablero, int filaMaxima, int columnaMaxima)
            {
                //creas la conexion entre el txt con nombre file2 y aquí
                StreamWriter f = new StreamWriter(nivelCreado);
                //ponemos el  level 10 por defecto
                f.WriteLine("        Level 10        ");
                //fila maxima de nivel
                f.WriteLine(filaMaxima);
                //columna maxima de nivel
                f.WriteLine(columnaMaxima);
                //direccion del jugador por defecto a la izquierda
                f.WriteLine("Izquierda");

                //escribimos el tablero convirtiendolo a los caracteres originales esto es:
                // #--->p     bloque
                // =--->i     powerUp
                // - --->c    caminoLibre
                // x --->x    trampa
                // m --->m    meta
                // o --->o    objeto
                // e --->e    enemigo
                // s --->j    jugador

                for (int i = 0; i < tablero.GetLength(0); i++)
                {
                    //salto de linea
                    f.WriteLine();
                    for (int j = 0; j < tablero.GetLength(1); j++)
                    {
                        //condiciones para escribir en el archivo

                        //muro o bloque
                        if (tablero[i, j] == 'p')
                        {
                            f.Write("#");
                        }
                        //powerUp
                        else if (tablero[i, j] == 'i')
                        {
                            f.Write("=");
                        }
                        //caminoLibre
                        else if (tablero[i, j] == 'c')
                        {
                            f.Write("-");
                        }
                        //trampa
                        else if (tablero[i, j] == 'x')
                        {
                            f.Write("x");
                        }
                        //meta
                        else if (tablero[i, j] == 'm')
                        {
                            f.Write("m");
                        }
                        //objeto
                        else if (tablero[i, j] == 'o')
                        {
                            f.Write("o");
                        }
                        //enemigo
                        else if (tablero[i, j] == 'e')
                        {
                            f.Write("e");
                        }
                        //jugador
                        else if (tablero[i, j] == 'j')
                        {
                            f.Write("s");
                        }
                    }
                }

                f.Close();
            }
            private static bool NivelValido(int filaMaxima, int columnaMaxima, char[,] tablero)
            {
                bool CasillasVacias = false;
                bool CasillaJugador = false;
                bool CasillaMeta = false;
                //comprobamos si el nivel es valido = no tiene partes del mapa vacias , tiene casilla de jugador, tiene casilla de meta

                for (int i = 0; i < filaMaxima; i++)
                {
                    for (int j = 0; j < columnaMaxima; j++)
                    {
                        //si hay un caracter vacio
                        if (tablero[i, j] == ' ')
                        { CasillasVacias = true; }
                        //si hay casilla del jugador
                        if (tablero[i, j] == 'j')
                        { CasillaJugador = true; }
                        //si hay casilla de meta
                        if (tablero[i, j] == 'm')
                        { CasillaMeta = true; }
                    }
                }
                    //si el nivel es valido               
                    return (CasillasVacias == false && CasillaJugador == true && CasillaMeta == true);
            }
        #endregion
        #region MetodoMenu
                private static void MainMenu(string rutaVLC, string banda_Sonora, Process p)
                {
                    //renderizamos el menú
                    RenderMainMenu();
                    //elegimos opcion
                    int opcion = int.Parse(Console.ReadLine());
                    switch (opcion)
                    {
                        //comenzar aventura
                        case 1:
                            //si se comienza partida se acaba musica inicial
                            p.Kill();
                            break;
                        case 2:
                            Ajustes(rutaVLC, banda_Sonora, p);
                            break;
                        case 3:
                            CreadorNiveles();
                            break;
                    }

                }
                private static void RenderMainMenu()
                {
                    Console.Clear();
                    //diseño de interfaz
                    Console.ForegroundColor = ConsoleColor.Blue;
                    //fila
                    Console.Write("████████████████████████████████████████████████████████");
                    Console.SetCursorPosition(0, 8);
                    Console.Write("████████████████████████████████████████████████████████");
                    //columnas izquierda
                    Console.SetCursorPosition(55, 1);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 2);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 3);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 4);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 5);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 6);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 7);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 8);
                    Console.Write("██");

                    //columna derecha
                    Console.SetCursorPosition(0, 1);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 2);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 3);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 4);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 5);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 6);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 7);
                    Console.Write("██");



                    //opción Comenzar Aventura
                    Console.SetCursorPosition(10, 2);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("1-Comenzar Aventura:");

                    //opción ajustes
                    Console.SetCursorPosition(10, 4);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("2-Ajustes:");

                    //Creador de niveles
                    Console.SetCursorPosition(10, 6);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("3-Creador de Niveles:");
                    //posicion del cursor
                    Console.SetCursorPosition(30, 4);
                    
                }
                private static void Ajustes(string rutaVLC, string banda_Sonora, Process p)
                {
                    //renderizamos menu de ajustes
                    RenderMenuAjustes();
                    //elegimos opcion
                    int opcion = int.Parse(Console.ReadLine());

                    //según la opción que hayas escogido
                    switch (opcion)
                    {
                        //comenzar aventura
                        case 1:
                            Controles(rutaVLC, banda_Sonora,  p);
                            break;
                        case 2:
                            Instrucciones(rutaVLC, banda_Sonora,p);
                            break;
                    }
                }
                private static void RenderMenuAjustes()
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    //fila

                    Console.Write("████████████████████████████████████████████████████████");
                    Console.SetCursorPosition(0, 8);
                    Console.Write("████████████████████████████████████████████████████████");
                    //columnas izquierda
                    Console.SetCursorPosition(55, 1);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 2);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 3);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 4);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 5);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 6);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 7);
                    Console.Write("██");

                    Console.SetCursorPosition(55, 8);
                    Console.Write("██");

                    //columna derecha
                    Console.SetCursorPosition(0, 1);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 2);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 3);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 4);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 5);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 6);
                    Console.Write("██");

                    Console.SetCursorPosition(0, 7);
                    Console.Write("██");



                    //Controles 
                    Console.SetCursorPosition(15, 2);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("1-Controles:");

                    //Instrucciones 
                    Console.SetCursorPosition(15, 4);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("2-Instrucciones:");
                    Console.SetCursorPosition(15, 6);
                }
                private static void Controles(string rutaVLC, string banda_Sonora, Process p)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();

                    Console.WriteLine("    Controles de Juego:    ");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("+Boton de Pausa---> P    ");
                    Console.WriteLine("");
                    Console.WriteLine("+Movimiento Arriba Jugador---> W    ");
                    Console.WriteLine("+Movimiento Izquierda Jugador---> A   ");
                    Console.WriteLine("+Movimiento Abajo Jugador---> S  ");
                    Console.WriteLine("+Movimiento Derecha Jugador---> D  ");
                    Console.WriteLine("");
                    Console.WriteLine("+Boton de Coger Objetos---> SPACEBAR o E    ");
                    Console.WriteLine("+Boton Modo DEBUG---> X   ");


                    Console.SetCursorPosition(0, 15);
                    Console.Write("Pulsa Q para salir al menú principal");
                    string salir = Console.ReadLine();

                    if (salir == "q")
                    {
                        MainMenu(rutaVLC,banda_Sonora, p);
                    }

                }
                private static void Instrucciones(string rutaVLC, string banda_Sonora, Process p)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write("Bienvenido a Speedy Maze , el más emocionante juego contrarreloj \n . \n ¿Serás capaz de superar los niveles del laberinto y escapar antes de tiempo? \n Enfrentate a: \n enemigos=☻ \n trampas=× \n powerUps=♪ \n \n  Tu personaje es este:☺ , disfruta y sal con vida. Coge todos los objetos que quieras , te darán sorpresas");


                    Console.WriteLine(" \n \n Pulsa Q para salir al menú principal");
                    string salir = Console.ReadLine();

                    if (salir == "q")
                    {
                        MainMenu(rutaVLC, banda_Sonora,  p);
                    }
                }
        #endregion
        #region Metodos BucleGeneral
                private static void LecturaRenderInicial(Jugador jug, Tablero tab, ref int numNivelesSup, string file)
                {
                    //pedimos nivel para leer por consola        
                    tab.LeeNivel(numNivelesSup, jug, file);
                    //inicializas las vidas a 3 inicialmente, para que no se reinicien en cada nivel
                    jug.VidasInicioNivel(3);
                    //renderiza el nivel leido
                    tab.RenderInicial(jug);
                }
                private static void BucleGeneral(Tablero tab, Jugador jug, Program game, ListaEnlazada listMoves,string GameOver, string rutaVLC, string rutaPowerUp, string rutaTrampa, string rutaObjeto,ref bool forzarSalida)
                {
                    char d = ' ';
                    int contadordeVueltas = 0;
                    

                    //mientras que jugador tenga vidas , no sea finJuego y no hayas forzado la salida
                    while (!tab.FinJuego(jug) && !tab.NivelAcabado && !forzarSalida)
                    {
                        contadordeVueltas++;

                        //movimiento aleatorio de enemigos
                        tab.ProcesarDirEnemy();

                        //leemos la tecla 
                        d = tab.LeeInput();

                        //si hemos pulsado q
                        if(d=='q')forzarSalida=true;

                        //almacenamos la tecla en la lista de movimientos del nivel
                        if (d == 'w' || d == 'a' || d == 's' || d == 'd') listMoves.InsertaFinal(d);

                        //la procesa y actua en consecuencia
                        tab.ProcesaInput(d, jug, game,listMoves,rutaObjeto,rutaPowerUp,rutaTrampa, rutaVLC);

                        //lo renderiza
                        tab.Render(jug);

                        //cada 5 vueltas resta 1 segundo
                        if (contadordeVueltas == NumeroCiclosSec)
                        {
                            //reiniciamos el contador
                            contadordeVueltas = 0;
                            tab.TimerCallback();
                        }

                        //retardo 
                        Thread.Sleep(retardoBucle);
                    }

                    //si es el fin del juego y no se ha pulsado q
                    if (tab.FinJuego(jug))
                    {
                          //ejecutamos cancion de GameOver
                        Process p = Process.Start(rutaVLC, GameOver);  
                        Derrota();
                        Thread.Sleep(4000);
                        //la terminamos la cancion de GameOver
                        p.Kill();   
                    }
                }
                private static void Derrota()
                {
                    Console.Clear();
                    Console.SetCursorPosition(15, 4);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("   You lost...");
                    Console.Write("GAME OVER MY FRIEND!");
                    Console.ForegroundColor = ConsoleColor.Black;

                }
                private static void Victoria(Jugador jug)
                {
                    Console.Clear();

                    Console.SetCursorPosition(15, 4);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   YOU WIN !!!   ");

                    Console.SetCursorPosition(0, 6);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("             +Objetos cogidos: " + jug.nObjetosJugador);

                    Console.ForegroundColor = ConsoleColor.Black;

                }
                

        #endregion
    }
}

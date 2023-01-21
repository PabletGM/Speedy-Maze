using System;
using System.Collections.Generic;
using System.Text;

namespace SpeedyMaze
{
	class ListaEnlazada
	{

		// CLASE NODO (clase privada para los nodos de la lista), utilizar para hacer una lista de movimientos registrados con una lista de teclas pulsadas almacenadas en orden con elementos char  
		private class Nodo
		{
			public char movimiento;   // información del nodo (podría ser de cualquier tipo)
			public Nodo sig;   // referencia al siguiente

			// la constructora por defecto sería:
			// public Nodo() {} // por defecto

			// implementamos nuestra propia constructora para nodos
			public Nodo(char _movimiento = ' ', Nodo _sig = null)
			{  // valores por defecto dato=0; y sig=null
				movimiento  = _movimiento;
				sig = _sig;
			}
		}
		//he añadido otra clase para que la lista lea elementos string


		// FIN CLASE NODO

		// CAMPOS Y MÉTODOS DE LA CLASE Lista

		// campo pri: referencia al primer nodo de la lista
		Nodo pri;


		// constructora de la clase Lista
		public ListaEnlazada()
		{
			pri = null;   //  lista vacia

		}


		
		// añadir elto e al final de la lista
		public void InsertaFinal(char e)
		{
			// distinguimos dos casos

			// lista vacia
			if (pri == null)
			{
				//dices que pri tenga como campo de dato el nuevo elemento de la lista leido por consola y como avanzar sig = null
				pri = new Nodo(e, null); // creamos nodo en pri

				// lista no vacia				
			}
			else
			{
				Nodo aux = pri;   // recorremos la lista hasta el ultimo nodo



				//hasta que no llegue a el ultimo nodo (el cual tendrá aux.sig==null)
				while (aux.sig != null) aux = aux.sig;
				// aux apunta al último nodo
				aux.sig = new Nodo(e, null); // creamos el nuevo a continuación, osea que el contador o campo sig va a ser el siguiente nodo
			}
		}



		
		// devuelve el num de eltos de la lista
		public int NumElems()
		{
			int n = 0;
			Nodo aux = pri;
			while (aux != null)
			{
				aux = aux.sig;
				n++;
			}
			return n;
		}


		private Nodo NesimoNodo(int n)
		{
			//si no existe el elemento  o dato en la lista , es decir ==-1
			if (n < 0) throw new Exception($"Error: no existe {n}-esimo en la lista");
			//si existe
			else
			{
				//referencia al primer nodo
				Nodo aux = pri;
				//mientras exista el nodo y el elemento  a buscar >0
				while (aux != null && n > 0)
				{
					//se pasa al siguiente nodo
					aux = aux.sig;
					//del 3 va al nodo 2 , del 2 al 1...
					n--;
				}
				//
				if (aux == null) throw new Exception($"Error: no existe {n}-esimo en la lista");
				else return aux;
			}
		}

		public char Nesimo(int n)
		{
			try
			{
				//devuelve el dato del nodo(que devuelve el metodo NesimoNodo)
				return NesimoNodo(n).movimiento;
			}
			catch
			{
				throw new Exception($"Error: no existe {n}-ésimo de la lista");
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace recorridoMatrices2
{
    public partial class Inicio : System.Web.UI.Page
    {
        public struct camino
        {
            public Int32 fila, columna, valor;
            public String secuenciaNros;
            public Int32 longitudCamino;
            public Int32 inclinacionCamino;
        };

        public struct pos
        {
            public Int32 fila;
            public Int32 columna;
        }

        Int32 minVal = 0;
        Int32 nroFilas = 0;
        Int32 nroColumnas = 0;

        protected void btnCargar_Click(object sender, EventArgs e)
        {
            //
            lblResultado.Text = "";
            if (txtMinVal.Text == "0" || txtMinVal.Text == "1")
            {
                minVal = Convert.ToInt32(txtMinVal.Text);
            }
            else
            {
                lblMensaje.Text = "Valor de elevación mínima debe ser 1 o 0";
                return;
            }

            //archivo seleccionado?
            if (fupld.FileName != "")
            {
                //tipo archivo Txt?
                if (fupld.PostedFile.ContentType == "text/plain")
                {
                    lblMensaje.Text = "Procesando ..... Matriz: ";
                    procesarArchivo();
                    
                }
                else
                {
                    lblMensaje.Text = "Se requiere Archivo Texto";
                }
                
            }
            else
            {
                lblMensaje.Text = "No ha seleccinado un archivo";
            }
        }

        void procesarArchivo()
        {
            string dirArchivo = Server.MapPath("~/") + "Datos\\" + fupld.PostedFile.FileName;
            //leer archivo
            StreamReader sr = new StreamReader(dirArchivo);
            //leer dimensiones de la matriz
            String linea = sr.ReadLine();            
            lblMensaje.Text = lblMensaje.Text + " " + linea;
            Int32 i = 0;
            nroFilas = Convert.ToInt32(linea.Substring(0,linea.IndexOf(" ")));
            nroColumnas = Convert.ToInt32(linea.Substring(linea.IndexOf(" ") + 1));
            //leer contenido matriz
            Int32 [,] matriz = new int[nroFilas, nroColumnas];
            linea = sr.ReadLine();
            while(linea != null)
            {
                if (linea != String.Empty)
                {
                    String[] nros = linea.Split(' ');
                    for (Int32 j = 0; j < nroColumnas; j++)
                    {
                        matriz[i, j] = Convert.ToInt32(nros[j]);
                    }
                }
                linea = sr.ReadLine();
                i++;
            }
            lblMensaje.Text = "Matriz Leida";
            var caminosCandidatos = new List<camino>();
            
            //Recorrer matrix en busca de caminos
            for (int x = 0; x < nroFilas; x++)
            {
                for (int y = 0; y < nroColumnas; y++)
                {
                    revisarPunto(matriz, caminosCandidatos, x, y, matriz[x, y], "", 1, 0, x, y);
                }
            }
            
            if (caminosCandidatos.Count() > 0)
            {//Si se encontraron varios caminos buscar el de mayor salto.
                Int32 res = buscarMejorCamino(caminosCandidatos);
                lblResultado.Text = "Resultado:  Punto de Arranque (fila, columna):" + caminosCandidatos.ElementAt(res).fila.ToString() + ", " + caminosCandidatos.ElementAt(res).columna + ", Mejor Camino: " + caminosCandidatos.ElementAt(res).secuenciaNros.Substring(1);
                lblResultado2.Text = "Longitud del camino: " + caminosCandidatos.ElementAt(res).longitudCamino.ToString() + ", Altura Calculada: " + caminosCandidatos.ElementAt(res).inclinacionCamino.ToString();
            }
        }

        void revisarPunto(Int32[,] matriz, List<camino> caminosCandidatos, Int32 filaA, Int32 ColumnaA, Int32 valorA, String secuenciaNrosA, Int32 longitudCaminoA, Int32 inclinacionCaminoA, Int32 filaPInicial, Int32 columnaPInicial)
        {
            //Condiciones de salida de la recursión
            if ((filaA >= nroFilas || ColumnaA >= nroColumnas) || (filaA == -1 || ColumnaA == -1) || (valorA == minVal))
            {
                if ((filaA >= nroFilas || ColumnaA >= nroColumnas))
                {//si ya se recorrió toda la matriz
                    return;
                }
                if ((filaA == -1 || ColumnaA == -1))
                {//no hay camino a seguir desde filaA, columnaA, revisar siguiente punto
                    return;
                }
                if (valorA == minVal)
                {//el camino llego al final: elevación 0
                    //verificar si la longitud de este camino es mayor que la de los caminos en la lista caminosCandidatos
                    Int32 resVerifLong = verificarMayorLongitudCamino(caminosCandidatos, longitudCaminoA);
                    //Si la longitud del camino actual es igual (0) a la de los caminos en la lista se adiciona un nuevo nodo
                    //si la longitud del camino actual es mayor (1) a la de los caminos en la lista, se reinicia la lista y se deja un solo nodo con la información de este camino
                    //si la longitud del camino actual es menor a la de los caminos en la lista, no se hace nada. 
                    camino nodo = new camino() { fila = filaPInicial, columna = columnaPInicial, valor = valorA, secuenciaNros = secuenciaNrosA + "," + minVal.ToString(), longitudCamino = longitudCaminoA, inclinacionCamino = matriz[filaPInicial, columnaPInicial] - minVal};
                    switch (resVerifLong)
                    {
                        case 0:
                            caminosCandidatos.Add(nodo);
                            break;
                        case 1:
                            caminosCandidatos.Clear();
                            caminosCandidatos.Add(nodo);
                            break;
                    }
                    return;
                }
            }
            else
            {   //revisar si hay camino desde filaA,ColumnuaA. Se recibe una nueva Fila, nueva Columna. Si son -1 es porque no hay camino desde este punto
                pos puntoSiguiente1 = hayCamino(matriz, filaA, ColumnaA, 1);
                pos puntoSiguiente2 = hayCamino(matriz, filaA, ColumnaA, 2);
                pos puntoSiguiente3 = hayCamino(matriz, filaA, ColumnaA, 3);
                pos puntoSiguiente4 = hayCamino(matriz, filaA, ColumnaA, 4);
                Boolean puntoAdicionado = false;
                if (puntoSiguiente1.fila != -1 && puntoSiguiente1.columna != -1)
                {
                    if (!puntoAdicionado)
                    {
                        secuenciaNrosA = secuenciaNrosA + ", " + valorA.ToString();
                        longitudCaminoA = longitudCaminoA + 1;
                        puntoAdicionado = true;
                    }
                    revisarPunto(matriz, caminosCandidatos, puntoSiguiente1.fila, puntoSiguiente1.columna, matriz[puntoSiguiente1.fila, puntoSiguiente1.columna], secuenciaNrosA, longitudCaminoA, inclinacionCaminoA, filaPInicial, columnaPInicial);
                }
                if (puntoSiguiente2.fila != -1 && puntoSiguiente2.columna != -1)
                {
                    if (!puntoAdicionado)
                    {
                        secuenciaNrosA = secuenciaNrosA + ", " + valorA.ToString();
                        longitudCaminoA = longitudCaminoA + 1;
                        puntoAdicionado = true;
                    }
                    revisarPunto(matriz, caminosCandidatos, puntoSiguiente2.fila, puntoSiguiente2.columna, matriz[puntoSiguiente2.fila, puntoSiguiente2.columna], secuenciaNrosA, longitudCaminoA, inclinacionCaminoA, filaPInicial, columnaPInicial);
                }
                if (puntoSiguiente3.fila != -1 && puntoSiguiente3.columna != -1)
                {
                    if (!puntoAdicionado)
                    {
                        secuenciaNrosA = secuenciaNrosA + ", " + valorA.ToString();
                        longitudCaminoA = longitudCaminoA + 1;
                        puntoAdicionado = true;
                    }
                    revisarPunto(matriz, caminosCandidatos, puntoSiguiente3.fila, puntoSiguiente3.columna, matriz[puntoSiguiente3.fila, puntoSiguiente3.columna], secuenciaNrosA, longitudCaminoA, inclinacionCaminoA, filaPInicial, columnaPInicial);
                }
                if (puntoSiguiente4.fila != -1 && puntoSiguiente4.columna != -1)
                {
                    if (!puntoAdicionado)
                    {
                        secuenciaNrosA = secuenciaNrosA + ", " + valorA.ToString();
                        longitudCaminoA = longitudCaminoA + 1;
                        puntoAdicionado = true;
                    }
                    revisarPunto(matriz, caminosCandidatos, puntoSiguiente4.fila, puntoSiguiente4.columna, matriz[puntoSiguiente4.fila, puntoSiguiente4.columna], secuenciaNrosA, longitudCaminoA, inclinacionCaminoA, filaPInicial, columnaPInicial);
                }
            }
            return;
        }

        Int32 verificarMayorLongitudCamino(List<camino> caminosCandidatos, Int32 longitudCamino)
        {// retorna 0, si la longitud de los caminos en la lista es igual a la que se recibe, 1 si es mayor, -1 si es menor
            if (caminosCandidatos.Count > 0)
            {
                if (caminosCandidatos.ElementAt(caminosCandidatos.Count() - 1).longitudCamino == longitudCamino)
                    return 0;
                else if (caminosCandidatos.ElementAt(caminosCandidatos.Count() - 1).longitudCamino < longitudCamino)
                    return 1;
                else
                    return -1;
            }
            return 1;
        }

        pos hayCamino(Int32[,] matriz, Int32 fila, Int32 columna,  Int32 pc)
        {// Verifica si hay un valor menor que el de la fila, columna recibidas en las casillas alrededor. Si lo encuentra retorna la nueva fila, nueva columna como resultado.
            Int32 valorA = matriz[fila, columna];
            Int32 filaN = 0;
            Int32 columnaN = 0;
            if (pc == 1)
            {
                filaN = fila;
                columnaN = columna - 1;
                if (columnaN >= 0)
                {
                    Int32 valorN = matriz[filaN, columnaN];
                    if (valorN < valorA)
                    {
                        pos res = new pos() { fila = filaN, columna = columnaN};
                        return res;
                    }
                }
            }
            else if (pc == 2)
            {
                filaN = fila - 1;
                columnaN = columna;
                if (filaN >= 0)
                {
                    Int32 valorN = matriz[filaN, columnaN];
                    if (valorN < valorA)
                    {
                        pos res = new pos() { fila = filaN, columna = columnaN };
                        return res;
                    }
                }
            }
            else if (pc == 3)
            {
                filaN = fila;
                columnaN = columna + 1;
                if (columnaN < nroColumnas)
                {
                    Int32 valorN = matriz[filaN, columnaN];
                    if (valorN < valorA)
                    {
                        pos res = new pos() { fila = filaN, columna = columnaN };
                        return res;
                    }
                }
            }
            if (pc == 4)
            {
                filaN = fila + 1;
                columnaN = columna;
                if (filaN < nroFilas)
                {
                    Int32 valorN = matriz[filaN, columnaN];
                    if (valorN < valorA)
                    {
                        pos res = new pos() { fila = filaN, columna = columnaN };
                        return res;
                    }
                }
            }

            pos res1 = new pos() { fila = -1, columna = -1 };
            return res1;
        }

        Int32 buscarMejorCamino(List<camino> caminosCandidatos)
        {
            Int32 mayorSalto = caminosCandidatos.ElementAt(0).inclinacionCamino;
            Int32 mayorSaltoIndex = 0;
            Int32 x = 0; 
            foreach (var n in caminosCandidatos)
            {
                if (n.inclinacionCamino > mayorSalto)
                {
                    mayorSalto = n.inclinacionCamino;
                    mayorSaltoIndex = x;
                }
                x++;
            }
            return mayorSaltoIndex;
        }
    }
}
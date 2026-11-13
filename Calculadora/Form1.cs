using System;
using System.Globalization;
using System.Windows.Forms;

namespace Calculadora
{
    public partial class Form1 : Form
    {
        // Valores y estado de la operacion actual.
        private double primerNumero;
        private double segundoNumero;
        private double resultado;
        private string operacion = string.Empty;
        private bool resultadoMostrado;

        public Form1()
        {
            InitializeComponent();
            LimpiarCalculadora();
        }

        // Todos los botones del 0 al 9 utilizan este evento Click.
        private void BotonNumero_Click(object sender, EventArgs e)
        {
            if (sender is Button boton)
            {
                AgregarNumero(boton.Text);
            }
        }

        private void AgregarNumero(string numero)
        {
            // Cuando aparece un resultado, un nuevo digito inicia otro calculo.
            // Si se presiona un operador primero, se reutiliza el resultado.
            if (resultadoMostrado && string.IsNullOrEmpty(operacion))
            {
                txtPantalla.Clear();
                lblOperacion.Text = "Nueva operacion";
                resultadoMostrado = false;
            }

            // Evita ceros innecesarios al comienzo (ej.: 0007).
            if (txtPantalla.Text == "0")
            {
                txtPantalla.Text = numero;
            }
            else if (txtPantalla.Text.Length < 15)
            {
                txtPantalla.Text += numero;
            }
        }

        // Los cuatro botones de operaciones comparten el evento Click.
        private void BotonOperacion_Click(object sender, EventArgs e)
        {
            if (sender is Button boton && boton.Tag is string simbolo)
            {
                SeleccionarOperacion(simbolo);
            }
        }

        private void SeleccionarOperacion(string simbolo)
        {
            // No se puede reemplazar una operacion que aun esta sin resolver.
            if (!string.IsNullOrEmpty(operacion))
            {
                MostrarError("Primero complete la operacion actual con el boton =.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPantalla.Text))
            {
                MostrarError("Ingrese el primer numero antes de elegir una operacion.");
                return;
            }

            if (!TryLeerPantalla(out double numero))
            {
                MostrarError("El primer numero no es valido. Presione C e intente otra vez.");
                return;
            }

            primerNumero = numero;
            operacion = simbolo;
            lblOperacion.Text = FormatearNumero(primerNumero) + " " + SimboloVisible(operacion);
            txtPantalla.Clear();
            resultadoMostrado = false;
        }

        private void btnIgual_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(operacion))
            {
                MostrarError("Seleccione una operacion (+, -, x o /) antes de presionar =.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPantalla.Text))
            {
                MostrarError("Ingrese el segundo numero antes de presionar =.");
                return;
            }

            if (!TryLeerPantalla(out segundoNumero))
            {
                MostrarError("El segundo numero no es valido. Presione C e intente otra vez.");
                return;
            }

            switch (operacion)
            {
                case "+":
                    resultado = primerNumero + segundoNumero;
                    break;
                case "-":
                    resultado = primerNumero - segundoNumero;
                    break;
                case "*":
                    resultado = primerNumero * segundoNumero;
                    break;
                case "/":
                    if (segundoNumero == 0)
                    {
                        MostrarError("No es posible dividir para cero. Ingrese otro segundo numero.");
                        txtPantalla.Clear();
                        return;
                    }
                    resultado = primerNumero / segundoNumero;
                    break;
                default:
                    MostrarError("La operacion seleccionada no es valida.");
                    return;
            }

            if (double.IsNaN(resultado) || double.IsInfinity(resultado))
            {
                MostrarError("El resultado se encuentra fuera del rango permitido.");
                LimpiarCalculadora();
                return;
            }

            lblOperacion.Text = FormatearNumero(primerNumero) + " "
                + SimboloVisible(operacion) + " "
                + FormatearNumero(segundoNumero) + " =";
            txtPantalla.Text = FormatearNumero(resultado);

            // Permite continuar calculando con el resultado anterior.
            primerNumero = resultado;
            operacion = string.Empty;
            resultadoMostrado = true;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCalculadora();
        }

        private void LimpiarCalculadora()
        {
            primerNumero = 0;
            segundoNumero = 0;
            resultado = 0;
            operacion = string.Empty;
            resultadoMostrado = false;
            txtPantalla.Clear();
            lblOperacion.Text = "Ingrese un numero";
        }

        // TryParse evita excepciones cuando el texto no se puede convertir.
        private bool TryLeerPantalla(out double numero)
        {
            return double.TryParse(
                       txtPantalla.Text,
                       NumberStyles.Float,
                       CultureInfo.CurrentCulture,
                       out numero)
                   && !double.IsNaN(numero)
                   && !double.IsInfinity(numero);
        }

        private static string FormatearNumero(double numero)
        {
            return numero.ToString("G15", CultureInfo.CurrentCulture);
        }

        private static string SimboloVisible(string simbolo)
        {
            switch (simbolo)
            {
                case "*": return "×";
                case "/": return "÷";
                default: return simbolo;
            }
        }

        private void MostrarError(string mensaje)
        {
            MessageBox.Show(this, mensaje, "Calculadora - Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}

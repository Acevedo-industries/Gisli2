using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Element;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Events;
using iText.Kernel.Pdf.Canvas;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Layout.Borders;


namespace Gisli2
{

    public partial class cotizacion : Form
    {
        protected internal class MyEventHandler : IEventHandler
        {

            public virtual void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDoc = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();
                int pageNumber = pdfDoc.GetPageNumber(page);
                iText.Kernel.Geom.Rectangle pageSize = page.GetPageSize();
                PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                double ancho_encabezado = vendedor_nombre.Length * 3.5;
                //Add header and footer
                pdfCanvas
                    .BeginText()
                    .SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD), 13)
                            .MoveText(pageSize.GetWidth() / 2 - ancho_encabezado, pageSize.GetTop() - 60)
                    //.ShowText(vendedor_nombre)
                    //.MoveText(60, -pageSize.GetTop() + 30)
                    //.ShowText(pageNumber.ToString())
                            .EndText();

                if (nombre_imagen != "")
                {
                    iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(nombre_imagen));
                    img.Scale(0.50f, 0.50f);
                    img.SetFixedPosition(0, pageSize.GetTop() - (img.GetImageHeight() / 2));
                    iText.Kernel.Geom.Rectangle area = page.GetPageSize();
                    new Canvas(pdfCanvas, pdfDoc, area)
                            .Add(img);
                }


            }
            internal MyEventHandler(cotizacion _enclosing)
            {
                this._enclosing = _enclosing;
            }

            private readonly cotizacion _enclosing;
        }

        static string nombre_imagen = "";
        static string vendedor_nombre = "";
        static string vendedor_tipo = "";
        static string vendedor_domicilio = "";
        static string vendedor_telefono = "";
        static string vendedor_representantes = "";
        static string vendedor_representantes_puesto = "";
        static string vendedor_actividades = "";


        static string comprador_nombre = "";
        static string comprador_rfc = "";
        static string comprador_tipop = "";
        static string comprador_domicilio = "";


        static string cantidad = "";
        static string cantidad_conletra = "";
        static string fechalimite = "";

        static string lugar_compraventa = "";
        static string fecha_compraventa = "";

        static string subtotal = "";
        static string iva = "";

        static string servicios_vendedor = "";
        static string fecha_actual = DateTime.Today.ToString("dd/MM/yyyy");
        public cotizacion()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vendedor_nombre = textBox1.Text;
            vendedor_domicilio = textBox5.Text;
            vendedor_telefono = textBox4.Text;

            vendedor_representantes = textBox10.Text;
            vendedor_representantes_puesto = textBox11.Text;


            comprador_nombre = textBox7.Text;
          
            subtotal = textBox22.Text;
            iva = textBox13.Text;
            cantidad = textBox14.Text;


            servicios_vendedor = dataGridView1.Rows[0].Cells[0].Value + "";

            saveFileDialog1.ShowDialog();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void createfile(object sender, CancelEventArgs e)
        {
            try
            {
                string DEST = saveFileDialog1.FileName;
                FileInfo file = new FileInfo(DEST);
                file.Directory.Create();

                PdfWriter writer = new PdfWriter(DEST);
                PdfDocument pdf = new PdfDocument(writer);
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new cotizacion.MyEventHandler(this));

                Document document = new Document(pdf, PageSize.LETTER);
                try
                {

                    document.SetMargins(70.8f, 84.75f, 121.60f, 85.03f);
                    servicios(document);

                    document.Close();

                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                }
                catch (Exception eX)
                {
                    document.Close();
                    MessageBox.Show(eX.ToString(), "Ocurrio un error al guardar el Documento");
                    if (File.Exists(DEST))
                    {
                        File.Delete(DEST);
                    }
                }
            }
            catch (Exception except)
            {

            }
        }

        public virtual void Process(Table table, String line, PdfFont font, bool isHeader)
        {
            Process(table, line, font, isHeader, new SolidBorder(ColorConstants.BLACK, 0.5f));
        }
        public virtual void Process(Table table, String line, PdfFont font, bool isHeader, Border borde)
        {
            int columnNumber = 0;
            if (isHeader)
            {
                Cell cell = new Cell().Add(new Paragraph(line));
                cell.SetPadding(5).SetBorder(null);
                table.AddHeaderCell(cell);
            }
            else
            {
                columnNumber++;
                Cell cell = new Cell().Add(new Paragraph(line));
                cell.SetFont(font).SetFontSize(10);
                cell.SetBorder(borde);
                table.AddCell(cell);
            }
        }

        private void addtable(Document document, DataGridView tabla)
        {
            addtable(document, tabla, tabla.ColumnCount);

        }
        private void addtable(Document document, DataGridView tabla, int num_columns)
        {
            Table table = new Table(num_columns);
            table.SetWidth(UnitValue.CreatePercentValue(100));

            for (int i = 0; i < num_columns; i++)
            {
                Process(table, tabla.Columns[i].HeaderText + "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            }

            for (int i = 0; i < tabla.Rows.Count - 1; i++)
            {
                for (int j = 0; j < num_columns; j++)
                {
                    if (tabla.Columns[j].HeaderText + "" == "IMPORTE")
                    {
                        Process(table, "$" + tabla.Rows[i].Cells[j].Value + "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
                    }
                    else if (tabla.Columns[j].HeaderText + "" == "VALOR UNITARIO")
                    {
                        Process(table, "$" + tabla.Rows[i].Cells[j].Value + "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
                    }
                    else
                    {
                        Process(table, tabla.Rows[i].Cells[j].Value + "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
                    }

                }
            }


            if (num_columns == 3)
            {
                add_iva_subtotal(document, table);
            }
            else
            {
                document.Add(table);
            }
        }

        private void add_iva_subtotal(Document document, iText.Layout.Element.Table table)
        {

            Process(table, "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "SUBTOTAL", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "$" + subtotal, PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);


            Process(table, "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "IVA", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "$" + iva, PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);


            Process(table, "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "TOTAL", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "$" + cantidad, PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);

            document.Add(table);
        }


        private void addtable_puestos(Document document, DataGridView tabla)
        {
            if (tabla.Rows.Count > 1)
            {
                addtable_puestos(document, tabla, tabla.Rows.Count - 1);
            }

        }


        private void addtable_puestos(Document document, DataGridView tabla, int num_filas)
        {
            Table table = new Table(num_filas);
            table.SetWidth(UnitValue.CreatePercentValue(100));

            for (int i = 0; i < tabla.Rows.Count - 1; i++)
            {
                string texto = "";
                int n_caracteres = tabla.Rows[i].Cells[0].Value.ToString().Length + 6;
                for (int a = 0; a <= n_caracteres; a++)
                {
                    texto = texto + "_";
                }
                texto = texto + "\nC. ";
                for (int j = 0; j < tabla.ColumnCount; j++)
                {
                    texto = texto + tabla.Rows[i].Cells[j].Value + "\n";
                }
                Process(table, texto.ToUpper() + " DEL " + comprador_nombre, PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false, Border.NO_BORDER);
            }

            document.Add(table);
        }

        private void Createparagrah(Document document, String text)
        {
            Createparagrah(document, text, TextAlignment.JUSTIFIED);
        }

        private void Createparagrah(Document document, String text, TextAlignment estilo)
        {
            PdfFont fuente = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            int tamanio_fuente = 10;

            Paragraph p = new Paragraph().SetFont(fuente).SetFontSize(tamanio_fuente).Add(text);
            p.SetTextAlignment(estilo);
            document.Add(p);
        }

        private void servicios(Document document)
        {
            Createparagrah(document, "COTIZACIÓN", TextAlignment.CENTER);
            Createparagrah(document, vendedor_nombre, TextAlignment.CENTER);
            Createparagrah(document, vendedor_domicilio , TextAlignment.CENTER);
            Createparagrah(document, @" TELEFONO:" + vendedor_telefono, TextAlignment.CENTER);
            Createparagrah(document, @" FECHA:" + fecha_actual, TextAlignment.CENTER);
            Createparagrah(document, @"Atencion A : " + comprador_nombre + @" 	 


");
            addtable(document, dataGridView1);

            Createparagrah(document, @"

ATENTAMENTE

" + vendedor_representantes + "", TextAlignment.CENTER);
            Createparagrah(document, vendedor_representantes_puesto, TextAlignment.CENTER);

        }




        private void image_fondo(object sender, CancelEventArgs e)
        {
            nombre_imagen = openFileDialog1.FileName;
            label1.Text = nombre_imagen;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                openFileDialog1.ShowDialog();
            }
        }

        private void cerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        public int xClick = 0, yClick = 0;

        //PASO 2: en el evento MouseMove del Form
        private void formulario_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            { xClick = e.X; yClick = e.Y; }
            else
            { this.Left = this.Left + (e.X - xClick); this.Top = this.Top + (e.Y - yClick); }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                openFileDialog1.ShowDialog();
            }
        }

        private void cerrar_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void cerrar_Click_2(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void checkBox1_CheckedChanged_2(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                openFileDialog1.ShowDialog();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}


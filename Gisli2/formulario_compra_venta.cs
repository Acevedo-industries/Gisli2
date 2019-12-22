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

    public partial class formulario_compra_venta : Form
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
                double ancho_encabezado = vendedor_nombre.Length*3.5;
                    //Add header and footer
                    pdfCanvas
                        .BeginText()
                        .SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD), 13)
                                .MoveText(pageSize.GetWidth() / 2 - ancho_encabezado, pageSize.GetTop() - 60)
                                .ShowText(vendedor_nombre)
                        //.MoveText(60, -pageSize.GetTop() + 30)
                        //.ShowText(pageNumber.ToString())
                                .EndText();

                    if(nombre_imagen!=""){
                        iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(nombre_imagen));
                        img.Scale(0.50f, 0.50f);
                        img.SetFixedPosition(0, pageSize.GetTop() - (img.GetImageHeight() / 2));
                        iText.Kernel.Geom.Rectangle area = page.GetPageSize();
                        new Canvas(pdfCanvas, pdfDoc, area)
                                .Add(img);
                    }
               
               
            }
            internal MyEventHandler(formulario_compra_venta _enclosing)
            {
                this._enclosing = _enclosing;
            }

            private readonly formulario_compra_venta _enclosing;
        }

        static string nombre_imagen = "";
        static string vendedor_nombre = "";
        static string vendedor_tipo = "";
        static string vendedor_domicilio = "";
        static string vendedor_rfc = "";
        static string vendedor_representantes = "";

        static string comprador_nombre = "";
        static string comprador_ejercicio = "";
        static string comprador_domicilio = "";

        static string declaran = "";

        static string cantidad = "";
        static string cantidad_conletra = "";
        static string fechalimite = "";

        static string lugar_compraventa = "";
        static string fecha_compraventa = "";

        static string subtotal = "";
        static string iva = "";

        static string fecha_actual = DateTime.Today.ToString("dd/MM/yyyy");

        public formulario_compra_venta()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vendedor_nombre = textBox1.Text;
            vendedor_tipo = textBox3.Text;
            vendedor_domicilio = textBox5.Text;
            vendedor_rfc = textBox3.Text; 
            vendedor_representantes = textBox10.Text;

            comprador_nombre = textBox7.Text;
            comprador_ejercicio = textBox2.Text;
            comprador_domicilio = textBox8.Text;

            declaran = textBox9.Text;

            
            cantidad_conletra = textBox20.Text;
            fechalimite = textBox21.Text;

            lugar_compraventa = textBox12.Text;
            fecha_compraventa = textBox6.Text;

            subtotal = textBox22.Text;
            iva = textBox13.Text;
            cantidad = textBox14.Text;


            saveFileDialog1.ShowDialog();
        }

        private void createfile(object sender, CancelEventArgs e)
        {
            string DEST = saveFileDialog1.FileName;

            FileInfo file = new FileInfo(DEST);
            file.Directory.Create();
            PdfWriter writer = new PdfWriter(DEST);
            PdfDocument pdf = new PdfDocument(writer);
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new formulario_compra_venta.MyEventHandler(this));

            Document document = new Document(pdf, PageSize.LETTER);
            try
            {            
            
            document.SetMargins(70.8f, 84.75f, 121.60f, 85.03f);
            compra_venta(document);

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

        public virtual void Process(Table table, String line, PdfFont font, bool isHeader )
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
                Process(table, tabla.Columns[i].HeaderText+"", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            }

            for (int i = 0; i < tabla.Rows.Count - 1; i++)
            {
                for (int j = 0; j < num_columns; j++)
                {
                    if (tabla.Columns[j].HeaderText + "" == "IMPORTE")
                    {
                        Process(table, "$" + tabla.Rows[i].Cells[j].Value + "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
                    }
                    else if (tabla.Columns[j].HeaderText + "" ==  "VALOR UNITARIO")
                    {
                        Process(table, "$" + tabla.Rows[i].Cells[j].Value + "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
                    }
                    else
                    {
                        Process(table, tabla.Rows[i].Cells[j].Value + "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
                    }
                    
                }
            }
            
            
            if(num_columns == 4){
                add_iva_subtotal(document, table);
            }
            else
            {
                document.Add(table);
            }
        }

        private void add_iva_subtotal(Document document,iText.Layout.Element.Table table)
        {
            Process(table,"", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "SUBTOTAL", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "$"+subtotal, PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);

            Process(table, "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "IVA", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "$" + iva, PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);

            Process(table, "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "TOTAL", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            Process(table, "$" + cantidad, PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);

            document.Add(table);
        }


        private void addtable_puestos(Document document, DataGridView tabla)
        {
            Console.Write(tabla.Rows.Count);
            if (tabla.Rows.Count>1)
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
                int n_caracteres =tabla.Rows[i].Cells[0].Value.ToString().Length+6;
                for (int a = 0; a <= n_caracteres; a++ )
                {
                    texto = texto + "_";
                }
                texto = texto+"\nC. ";
                for (int j = 0; j < tabla.ColumnCount; j++)
                {
                    texto = texto+ tabla.Rows[i].Cells[j].Value + "\n";
                }
                Process(table, texto.ToUpper() +" DEL "+ comprador_nombre, PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false, Border.NO_BORDER);
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
        
        private void compra_venta(Document document)
        {
           

    string nombres ="";
    string puestos="";
    for(int b=0; b< dataGridView2.Rows.Count - 1;b++){
        if(b== dataGridView2.Rows.Count - 3){
             nombres = nombres + dataGridView2.Rows[b].Cells[0].Value+" y ";
             puestos = puestos + dataGridView2.Rows[b].Cells[1].Value+" y ";
        } else if(b== dataGridView2.Rows.Count - 2){
             nombres = nombres + dataGridView2.Rows[b].Cells[0].Value;
            puestos = puestos + dataGridView2.Rows[b].Cells[1].Value;
        }else{
            nombres = nombres + dataGridView2.Rows[b].Cells[0].Value+" , ";
            puestos = puestos + dataGridView2.Rows[b].Cells[1].Value+" , ";
        }
       
    }
    Createparagrah(document, @"EN EL " + lugar_compraventa + @", SE CELEBRA EL PRESENTE CONTRATO DE COMPRAVENTA POR UNA PARTE EL" + comprador_nombre + @", REPRESENTADO POR LOS CC. "+nombres.ToUpper()+" EN SU CARÁCTER DE "+puestos.ToUpper()+@" RESPECTIVAMENTE, QUE EN ADELANTE SE DENOMINARA ´´EL COMPRADOR´´ Y LA EMPRESA ´´ " + vendedor_nombre + @",   REPRESENTADA POR LOS CC. "+vendedor_representantes+@" EN SU CALIDAD DE APODERADO LEGAL, QUE EN LO SUCESIVO SE LE DENOMINARA ´´EL VENDEDOR´´ MISMA PARTE QUE EN SU CONJUNTO SE LES LLAMARÁ ´´LOS CONTRATANTES´´, AL TENOR DE LAS SIGUIENTES DECLARACIONES Y CLAUSULAS: 
 
DECLARACIONES 
 I.- ´´EL COMPRADOR´´ DECLARA 
 
i. QUE ES UNA ENTIDAD DE CARÁCTER PÚBLICO, CON PATRIMONIO PROPIO, AUTÓNOMO EN SU RÉGIMEN INTERIOR Y CON LIBRE ADMINISTRACIÓN DE SU HACIENDA, INVESTIDO DE PERSONALIDAD JURÍDICA PROPIA EN LOS TÉRMINOS DE LAS FRACCIONES II Y IV DEL ARTICULO 115 DE LA CONSTITUCIÓN POLITICA DE LOS ESTADOS UNIDOS MEXICANOS, ARTICULO 113 DE LA CONSTITUCIÓN POLITICA DEL ESTADO LIBRE Y SOBERANO DE OAXACA Y ARTICULO 68 DE LA LEY ORGANICA MUNICIPAL DE ESTADO DE OAXACA. 

ii. QUE SEÑALA COMO SU DOMICILIO PARA LOS FINES Y EFECTOS DE ESTE CONTRATO EL HUBICADO EN: " + comprador_domicilio + @". 

iii. QUE LA AUTORIDAD, REPRESENTANTE DEL MUNICIPIO, ESTÁ FACULTADA PARA CONTRATAR EN LOS TERMINOS DEL ARTICULO 43 FRACCIÓN V, DE LA LEY ORGANICA MUNICIPAL DEL  ESTADO DE OAXACA. 

iv. QUE PARA CUBRIR LAS EROGACIONES QUE SE DERIVEN DEL PRESENTE CONTRATO “EL MUNICIPIO” CUENTA CON LOS RECURSOS PRESUPUESTALES ASIGNADOS Y DISPONIBLES DEL EJERCICIO " + comprador_ejercicio + @", EL PAGO DE LA COMPRA, DEL OBJETO DE ESTE CONTRATO. 
 
II.- “EL VENDEDOR” DECLARA 
 
i. DECLARA " + vendedor_nombre + @" SER UNA " + vendedor_tipo + @" LEGALMENTE CONSTITUIDA CONFORME LAS LEYES DE LOS ESTADOS UNIDOS MEXICANOS, QUE TIENE SU DOMICILIO FISCAL UBICADO EN " + vendedor_domicilio + @". 
ii. QUE ES UNA PERSONA MORAL EN PLENO EJERCICIO DE SUS DERECHOS Y QUE SU REGISTRO FEDERAL DE CONTRIBUYENTES ES " + vendedor_rfc + @". 
iii. QUE CUENTA CON CAPACIDAD JURIDICA PARA CONTRATAR Y OBLIGARSE A LA COMPRA EN MATERIA DE ESTE CONTRATO. 
iv. QUE PARA EL CUMPLIMIENTO DEL OBJETO DEL CONTRATO CUENTA CON LOS RECURSOS SUFICIENTES PARA CUBRIR EL MONTO PACTADO Y ASÍ CUMPLIR CON LOS COMPROMISOS FIRMADOS EN ESTE CONTRATO. 
v. QUE SE ENCUENTRA AL CORRIENTE EN EL PAGO DE SUS OBLIGACIONES FISCALES. 
 
III.-  “LOS CONTRATANTES” DECLARAN  
 " + declaran +
@" 

CLÁUSULAS 
 
PRIMERA: OBJETO 
LAS PARTES, RECONOCEN QUE EL OBJETO DEL PRESENTE ES LA COMPRAVENTA DE: 
"); 
addtable(document, dataGridView1,2);
Createparagrah(document, @" SEGUNDA: MONTO  
 
“EL COMPRADOR” EXHIBIRÁ A “EL VENDEDOR” LA CANTIDAD DE $"+ cantidad +@" ("+cantidad_conletra+@") DICHA CANTIDAD QUE INCLUYE YA SU RESPECTIVO IMPUESTO AL VALOR AGREGADO.  LA EXHIBICIÓN DEL PAGO DEBERÁ REALIZARSE A MAS TARDAR EL "+fechalimite+@". 
  
TERCERA: ENTREGA DE LAS MERCANCIAS  
“EL VENDEDOR” SE COMPROMETE A ENTREGAR LAPS MERCANCIAS OBJETO DEL PRESENTE CONTRATO A MAS TARDAR EL DIA "+fechalimite+@". 
 
CUARTA: CONFIDENCIALIDAD 
LAS PARTES CONVIENEN EN QUE  “EL VENDEDOR” NO PODRÁ DIVULGAR POR MEDIO DE PUBLICACIONES, CONFERENCIAS, INFORMES O CUALQUIER OTRA FORMA, LOS RESULTADOS OBTENIDOS EN LOS TRABAJOS OBJETO DE ESTE CONTRATO, SIN LA AUTORIZACIÓN EXPRESA Y POR ESCRITO DE “EL COMPRADOR”,  PUES DICHOS DATOS Y RESULTADOS SON PROPIEDAD DE “EL COMPRADOR”, LA QUE DEBERÁ MANEJAR “EL VENDEDOR”, BAJO EL PRINCIPIO DE CONFIDENCIALIDAD Y RESERVA, POR LO QUE ESTA RESTRICCIÓN SE EXTIENDE A AQUELLAS PERSONAS QUE POR ALGUNA RAZÓN LLEGARAN A TENER CONOCIMIENTO DE LA MISMA, YA SEAN SUS SOCIOS, ASESORES, DEPENDIENTES O CUALQUIER OTRA PERSONA FÍSICA O MORAL QUE GUARDEN RELACIÓN CON “EL VENDEDOR”, AUN Y CUANDO LOS FINES DE LA EXPOSICIÓN, YA SEA CÁTEDRA, CONFERENCIA O CUALQUIER OTRO MEDIO, SEAN O NO REMUNERADOS. 
 
QUINTA: RESICION 
PARA EL CASO DE INCUMPLIMIENTO DE CUALQUIERA DE LAS CLÁUSULAS ANTES SEÑALADAS, LAS PARTES PODRÁN PROMOVER LA RECISIÓN DEL PRESENTE. PARA LA INTERPRETACIÓN Y CUMPLIMIENTO DEL PRESENTE CONTRATO, LAS PARTES SE SOMETEN A LA JURISDICCIÓN Y COMPETENCIA DE LOS TRIBUNALES DONDE SE SUSCRIBE EL PRESENTE CONTRATO, O EN SU DEFECTO EN LA CIUDAD DE OAXACA DE JUÁREZ, OAXACA. POR LO TANTO, RENUNCIAN AL FUERO QUE PUDIESE CORRESPONDERLE EN RAZÓN DE SU DOMICILIO PRESENTE O FUTURO O POR CUALQUIER OTRA CAUSA. 
 
EL PRESENTE CONTRATO SE FIRMA EN EL "+lugar_compraventa+@", EL DIA "+fecha_compraventa+"."); 
 
Createparagrah(document, @" 

                          POR “EL MUNICIPIO” 


                                                        ",TextAlignment.CENTER);

addtable_puestos(document, dataGridView2);

Createparagrah(document, @"

POR “EL VENDEDOR” 

                       _____________________________
"+vendedor_nombre, TextAlignment.CENTER);


document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

Createparagrah(document, @"COTIZACIÓN                                                                                                                       
"+fecha_actual+@"
DIRIGIDO A : "+comprador_nombre+@" 	 


");
addtable(document,dataGridView1);

Createparagrah(document, @"
EN ESPERA DE SU RESPUESTA
");

Createparagrah(document, @"

ATENTAMENTE

ENCARGADO DE LA EMPRESA
", TextAlignment.CENTER);
        }


        

        private void image_fondo(object sender, CancelEventArgs e)
        {
            nombre_imagen = openFileDialog1.FileName;
            label1.Text = nombre_imagen;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked){
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

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        
    }
}

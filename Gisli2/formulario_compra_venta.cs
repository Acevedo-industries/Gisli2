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
                
           
                    //Add header and footer
                    pdfCanvas
                        .BeginText()
                        .SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD), 14)
                                .MoveText(pageSize.GetWidth() / 2 - 60, pageSize.GetTop() - 40)
                                .ShowText("Encabezado")
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

        public formulario_compra_venta()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void createfile(object sender, CancelEventArgs e)
        {
            string DEST = saveFileDialog1.FileName;
            FileInfo file = new FileInfo(DEST);
            file.Directory.Create();
            CreatePdf(DEST);
   
            //this.Close();
            System.Diagnostics.Process.Start(saveFileDialog1.FileName);
        }

        private void CreatePdf(String dest)
        {
            
            PageSize ps = PageSize.LETTER;
            PdfWriter writer = new PdfWriter(dest);
            PdfDocument pdf = new PdfDocument(writer);
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new formulario_compra_venta.MyEventHandler(this));
            Document document = new Document(pdf, ps);
            document.SetMargins(70.8f, 84.75f, 121.60f, 85.03f);
            

            compra_venta(document);

            document.Close();
        }
        public virtual void Process(Table table, String line, PdfFont font, bool isHeader)
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
                    cell.SetFont(font);
                     cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f));
                    table.AddCell(cell);
                }
        }
        private void addtable(Document document, DataGridView tabla)
        {
            Table table = new Table(tabla.ColumnCount);
            table.SetWidth(UnitValue.CreatePercentValue(100));

            for (int i = 0; i < tabla.ColumnCount; i++)
            {
                Process(table, tabla.Columns[i].HeaderText+"", PdfFontFactory.CreateFont(StandardFonts.HELVETICA), false);
            }

            for (int i = 0; i < tabla.Rows.Count - 1; i++)
            {
                for (int j = 0; j < tabla.ColumnCount; j++)
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
            
            document.Add(table);
        }

        private void Createparagrah(Document document,String text)
        {
            PdfFont fuente = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            int tamanio_fuente = 10;
            
            Paragraph p = new Paragraph().SetFont(fuente).SetFontSize(tamanio_fuente).Add(text);
            document.Add(p);
        }
        
        private void compra_venta(Document document)
        { 
            Createparagrah(document, @"EN EL MUNICIPIO DE SAN JUAN LALANA, CHOAPAM, OAXACA, SE CELEBRA EL PRESENTE CONTRATO DE COMPRAVENTA POR UNA PARTE EL MUNICIPIO DE SAN 
JUAN LALANA, CHOAPAM, OAXACA, REPRESENTADO POR LOS CC. MIGUEL PEREZ CORREA Y DELFINO BAUTISTA OJEDA EN SU CARÁCTER DE PRESIDENTE MUNICIPAL CONSTITUCIONAL Y SÍNDICO PROCURADOR RESPECTIVAMENTE, QUE EN ADELANTE SE DENOMINARA ´´EL COMPRADOR´´ Y LA EMPRESA ´´ PAPELERIA COMERCIAL DEL ITSMO S.A DE C.V ,   REPRESENTADA POR LOS CC. ANGEL IVÁN CANELA FABIÁN Y VERÓNICA LETICIA MARTÍNEZ HERNÁNDEZ EN SU CALIDAD DE APODERADO LEGAL, QUE EN LO SUCESIVO SE LE DENOMINARA ´´EL VENDEDOR´´ MISMA PARTE QUE EN SU CONJUNTO SE LES LLAMARÁ ´´LOS CONTRATANTES´´, AL TENOR DE LAS SIGUIENTES DECLARACIONES Y CLAUSULAS: 
 
DECLARACIONES 
 I.- ´´EL COMPRADOR´´ DECLARA 
 
i.   QUE ES UNA ENTIDAD DE CARÁCTER PÚBLICO, CON PATRIMONIO PROPIO, AUTÓNOMO EN SU RÉGIMEN INTERIOR Y CON LIBRE ADMINISTRACIÓN DE SU HACIENDA, INVESTIDO DE PERSONALIDAD JURÍDICA PROPIA EN LOS TÉRMINOS 
DE LAS FRACCIONES II Y IV DEL ARTICULO 115 DE LA CONSTITUCIÓN POLITICA DE  
DE LOS ESTADOS UNIDOS MEXICANOS, ARTICULO 113 DE LA CONSTITUCIÓN POLITICA DEL ESTADO LIBRE Y SOBERANO DE OAXACA Y ARTICULO 68 DE LA LEY ORGANICA MUNICIPAL DE ESTADO DE OAXACA. ii. QUE SEÑALA COMO SU DOMICILIO PARA LOS FINES Y EFECTOS DE ESTE CONTRATO EL HUBICADO EN: DOMICILIO CONOCIDO SIN NUMERO SAN JUAN LALANA, CHOAPAM, OAXACA. 

iii.	QUE LA AUTORIDAD, REPRESENTANTE DEL MUNICIPIO, ESTÁ FACULTADA PARA CONTRATAR EN LOS TERMINOS DEL ARTICULO 43 FRACCIÓN V, DE LA LEY ORGANICA MUNICIPAL DEL  ESTADO DE OAXACA. 

iv.	QUE PARA CUBRIR LAS EROGACIONES QUE SE DERIVEN DEL PRESENTE CONTRATO “EL MUNICIPIO” CUENTA CON LOS RECURSOS PRESUPUESTALES ASIGNADOS Y DISPONIBLES DEL EJERCICIO 2019, EL PAGO DE LA COMPRA, DEL OBJETO DE ESTE CONTRATO. 
 
II.- “EL VENDEDOR” DECLARA 
 
i. DECLARA PAPELERIA COMERCIAL DEL ITSMO S.A DE C.V SER UNA SOCIEDAD ANÓNIMA DE CAPITAL VARIABLE LEGALMENTE CONSTITUIDA CONFORME LAS 
LEYES DE LOS ESTADOS UNIDOS MEXICANOS, QUE TIENE SU DOMICILIO FISCAL 
UBICADO EN ORIENTE 5 100 VICTOR BRAVO AHUJA SANTA LUCIA DEL CAMINO, OAXACA. Mexico. C.P. 71244 
ii. QUE ES UNA PERSONA MORAL EN PLENO EJERCICIO DE SUS DERECHOS Y QUE SU REGISTRO FEDERAL DE CONTRIBUYENTES ES PCI1602189R4. 
iii. QUE CUENTA CON CAPACIDAD JURIDICA PARA CONTRATAR Y OBLIGARSE A LA COMPRA EN MATERIA DE ESTE CONTRATO. iv. QUE PARA EL CUMPLIMIENTO DEL OBJETO DEL CONTRATO CUENTA CON LOS RECURSOS SUFICIENTES PARA CUBRIR EL MONTO PACTADO Y ASÍ CUMPLIR CON LOS COMPROMISOS FIRMADOS EN ESTE CONTRATO. 
v. QUE SE ENCUENTRA AL CORRIENTE EN EL PAGO DE SUS OBLIGACIONES FISCALES. 
 
III.-  “LOS CONTRATANTES” DECLARAN  
 
I.- QUE SE RECONOCEN MUTUAMENTE LA PERSONALIDAD CON QUE COMPARECEN CONTRATAR. 
II.-QUE NO TIENEN IMPEDIMENTO LEGAL ALGUNO PARA OBLIGARSE, QUE RECONOCEN Y ADEMÁS ACEPTAN LAS DECLARACIONES ANTERIORES, POR LO QUE ES SU VOLUNTAD SUSCRIBIR  EL PRESENTE CONTRATO Y OBLIGARSE AL TENOR DE LAS SIGUIENTES: 
 
CLÁUSULAS 
 
PRIMERA: OBJETO 
LAS PARTES, RECONOCEN QUE EL OBJETO DEL PRESENTE ES LA COMPRAVENTA DE: 
 
CANTIDAD 	DESCRIPCIÓN 
25 	PAPEL COPY PAPER CARTA 5000H 
40 	LAPIZ ADHESIVO PRITT (11 GR, 3 PZS.)| 
20 	FOLDER CARTA (MANILA, 100 PZS.) 
25 	FOLDER OFICIO (MANILA, 100 PZS.) 
60 	GOMAS M-20 (2 PZS.) 
60 	SACAPUNTAS DE METAL (PLATA, 2 PZS.) 
7 	CARTUCHO DE TINTA (NEGRO) 
7 	CARTUCHO DE TINTA (TRICOLOR) 
12 	PAQ. DE HOJAS OPALINA C/100 PZAS 
1 	PAQUETE DE CARPETAS DE COLORES T/CARTA C/100 PZAS 
 
SEGUNDA: MONTO  
 
“EL COMPRADOR” EXHIBIRÁ A “EL VENDEDOR” LA CANTIDAD DE $38,538.00 (TREINTA Y OCHO MIL QUINIENTOS TREINTA Y OCHO PESOS 00/100 M.N) DICHA CANTIDAD QUE INCLUYE YA SU RESPECTIVO IMPUESTO AL VALOR AGREGADO.  LA EXHIBICIÓN DEL PAGO DEBERÁ REALIZARSE A MAS TARDAR EL DIA TREINTA Y UN DEL MES DE AGOSTO DEL AÑO EN CURSO. 
 
 
 
 
TERCERA: ENTREGA DE LAS MERCANCIAS  
“EL VENDEDOR” SE COMPROMETE A ENTREGAR LAPS MERCANCIAS OBJETO DEL PRESENTE CONTRATO A MAS TARDAR EL DIA 31 DE AGOSTO DEL 2019. 
 
CUARTA: CONFIDENCIALIDAD 
LAS PARTES CONVIENEN EN QUE  “EL VENDEDOR” NO PODRÁ DIVULGAR POR MEDIO DE PUBLICACIONES, CONFERENCIAS, INFORMES O CUALQUIER OTRA FORMA, LOS RESULTADOS OBTENIDOS EN LOS TRABAJOS OBJETO DE ESTE CONTRATO, SIN LA AUTORIZACIÓN EXPRESA Y POR ESCRITO DE “EL COMPRADOR”,  PUES DICHOS DATOS Y RESULTADOS SON PROPIEDAD DE 
“EL COMPRADOR”, LA QUE DEBERÁ MANEJAR “EL VENDEDOR”, BAJO EL PRINCIPIO DE CONFIDENCIALIDAD Y RESERVA, POR LO QUE ESTA RESTRICCIÓN   
SE EXTIENDE A AQUELLAS PERSONAS QUE POR ALGUNA RAZÓN LLEGARAN A TENER CONOCIMIENTO DE LA MISMA, YA SEAN SUS SOCIOS, ASESORES, DEPENDIENTES O CUALQUIER OTRA PERSONA FÍSICA O MORAL QUE GUARDEN 
RELACIÓN CON “EL VENDEDOR”, AUN Y CUANDO LOS FINES DE LA EXPOSICIÓN, YA SEA CÁTEDRA, CONFERENCIA O CUALQUIER OTRO MEDIO, SEAN O NO REMUNERADOS. 
 
QUINTA: RESICION 
PARA EL CASO DE INCUMPLIMIENTO DE CUALQUIERA DE LAS CLÁUSULAS ANTES SEÑALADAS, LAS PARTES PODRÁN PROMOVER LA RECISIÓN DEL PRESENTE.  
PARA LA INTERPRETACIÓN Y CUMPLIMIENTO DEL PRESENTE CONTRATO, LAS PARTES SE SOMETEN A LA JURISDICCIÓN Y COMPETENCIA DE LOS TRIBUNALES DONDE SE SUSCRIBE EL PRESENTE CONTRATO, O EN SU DEFECTO EN LA CIUDAD DE OAXACA DE JUÁREZ, OAXACA. POR LO TANTO, RENUNCIAN AL FUERO QUE PUDIESE CORRESPONDERLE EN RAZÓN DE SU DOMICILIO PRESENTE O FUTURO O POR CUALQUIER OTRA CAUSA. 
 
EL PRESENTE CONTRATO SE FIRMA EN EL MUNICIPIO DE SAN JUAN LALANA, CHOAPAM, OAXACA, EL DIA 01 DE AGOSTO DE 2019. 
 
 
POR “EL MUNICIPIO” 
            	 	        
 
____________________________        	____________________________       
 
 	C. MIGUEL PEREZ CORREA 	C. DELFINO BAUTISTA OJEDA 
 	PRESIDENTE MUNICIPAL DEL 	SINDICO PROCURADOR DEL 
 
MUNICIPIO DE SAN JUAN LALANA, 	MUNICIPIO DE SAN JUAN LALANA,  	CHOAPAM, OAXACA. 	CHOAPAM, OAXACA. 
 	 
 	           POR “EL VENDEDOR” 
____________________________       
PAPELERIA COMERCIAL DEL ITSMO 
S.A DE C.V 
 ");

            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            Createparagrah(document, @"COTIZACIÓN                                                                                                                       
29/07/2019 
DIRIGIDO A : MUNICIPIO DE SAN JUAN LALANA 	 


");
            addtable(document,dataGridView1);
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

        
    }
}

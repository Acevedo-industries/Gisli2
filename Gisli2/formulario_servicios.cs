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
    public partial class formulario_servicios : Form
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
            internal MyEventHandler(formulario_servicios _enclosing)
            {
                this._enclosing = _enclosing;
            }

            private readonly formulario_servicios _enclosing;
        }
        static string nombre_imagen = "";
        static string vendedor_nombre = "";
        static string vendedor_tipo = "";
        static string vendedor_domicilio = "";
        static string vendedor_rfc = "";
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
        public formulario_servicios()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vendedor_nombre = textBox1.Text;
            vendedor_tipo = textBox3.Text;
            vendedor_domicilio = textBox5.Text;
            vendedor_rfc = textBox4.Text;

            vendedor_representantes = textBox10.Text;
            vendedor_representantes_puesto = textBox11.Text;
            

            comprador_nombre = textBox7.Text;
            comprador_rfc = textBox2.Text;
            comprador_tipop = textBox16.Text;
            comprador_domicilio = textBox8.Text;

           
             vendedor_actividades = textBox9.Text;

            cantidad_conletra = textBox20.Text;
            fechalimite = textBox21.Text;

            lugar_compraventa = textBox12.Text;
            fecha_compraventa = textBox6.Text;

            subtotal = textBox22.Text;
            iva = textBox13.Text;
            cantidad = textBox14.Text;


            servicios_vendedor = dataGridView1.Rows[0].Cells[0].Value+"";

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
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new formulario_servicios.MyEventHandler(this));

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


            string nombres = "";
            string puestos = "";
            for (int b = 0; b < dataGridView2.Rows.Count - 1; b++)
            {
                if (b == dataGridView2.Rows.Count - 3)
                {
                    nombres = nombres + dataGridView2.Rows[b].Cells[0].Value + " y ";
                    puestos = puestos + dataGridView2.Rows[b].Cells[1].Value + " y ";
                }
                else if (b == dataGridView2.Rows.Count - 2)
                {
                    nombres = nombres + dataGridView2.Rows[b].Cells[0].Value;
                    puestos = puestos + dataGridView2.Rows[b].Cells[1].Value;
                }
                else
                {
                    nombres = nombres + dataGridView2.Rows[b].Cells[0].Value + " , ";
                    puestos = puestos + dataGridView2.Rows[b].Cells[1].Value + " , ";
                }

            }
            Createparagrah(document, @"CONTRATO DE PRESTACIÓN DE SERVICIOS", TextAlignment.CENTER);
            Createparagrah(document, @"QUE CELEBRAN POR UNA PARTE EL" + comprador_nombre + @", REPRESENTADA EN ESTE ACTO POR SU REPRESENTANTE LEGAL EL C. " + nombres.ToUpper() + " EN SU CARÁCTER DE " + puestos.ToUpper() + @" EN LO SUCESIVO EL “PRESTATARIO”,Y POR LA OTRA PARTE, ´´ " + vendedor_nombre + @", EN LO SUCESIVO EL “PRESTADOR”, EN CONJUNTO A EL “PRESTADOR” Y A EL “PRESTATARIO” SE LES DENOMINARÁ COMO LAS “PARTES”, QUIENES SE SUJETAN AL TENOR DE LAS SIGUIENTES: ");
 
Createparagrah(document, @" D E C L A R A C I O N E S", TextAlignment.CENTER);

Createparagrah(document, @"
I.- Declara el “PRESTADOR”:

a) I.1.-Ser una persona moral debidamente constituida y válidamente existente conforme a las leyes de los Estados Unidos Mexicanos, según acredita en este acto con el Testimonio del Instrumento Público número quince mil ciento dos (15102), del protocolo a cargo de la Licenciada Lilian Alejandra Bustamante García, Notario Público número 87, de Oaxaca de Juárez, Oaxaca, otorgada con fecha catorce de febrero del año 2017, y que quedó debidamente inscrito en el Registro Público de la Propiedad y de Comercio del Estado de Oaxaca.

b) I.2.- Que la C. " + vendedor_representantes + @" cuenta con las facultades necesarias y suficientes para celebrar el presente contrato y obligar a su representada en términos del mismo, en virtud de ostentar el cargo de " + vendedor_representantes_puesto + @" de la misma y que dichas facultades no le han sido revocadas, modificadas o limitadas en forma alguna.

c) I.3.Que tal y como consta en el instrumento notarial citado en la declaración 
   I.1, su objeto social consiste, entre otras actividades, en: " + vendedor_actividades + @"

d) I.4.- Que su Registro Federal de Contribuyentes es " + vendedor_rfc + @", y su domicilio fiscal el ubicado en " + vendedor_domicilio + @", mismo que señala para todos los efectos del presente contrato. 

II.- Declara el “PRESTATARIO”, que:
e) Es una persona " + comprador_tipop + @" que cuenta con clave del registro federal de contribuyentes: " + comprador_rfc + @"
f) Que cuenta con capacidad jurídica para contratar, obligarse y que está al corriente en el pago de sus obligaciones fiscales.
g) Manifiesta tener su domicilio fiscal en " + comprador_domicilio + @".
h) Que es su deseo de contratar los servicios profesionales de el “PRESTADOR” para que los ejecute de acuerdo a los intereses de el “PRESTATARIO”, en los términos del presente “CONTRATO”.

De conformidad con las declaraciones anteriores, habiendo las “PARTES” reconocido mutuamente la personalidad y capacidad con las cuales comparecen al presente instrumento debidamente representadas en este acto, sometiéndose a lo establecido en las siguientes:");
 
Createparagrah(document, @"C L Á U S U L A S", TextAlignment.CENTER);

Createparagrah(document,@"
PRIMERA.- OBJETO DEL CONTRATO: 

Por medio del presente “CONTRATO” el “PRESTADOR” se obliga a brindar al
“PRESTATARIO”. "+servicios_vendedor+@".

SEGUNDA.- LUGAR DE LA PRESTACIÓN DE LOS SERVICIOS.
Acuerdan las “PARTES” que la prestación de los servicios se sujetará las siguientes prevenciones:

  a) El “SERVICIO” será desarrollado por el “PRESTADOR” en "+comprador_domicilio+@", en el domicilio o domicilios que sea procedente conforme a las actividades propias del “SERVICIO”.

TERCERA. CONTRAPRESTACIÓN.

Tomando en consideración que las contraprestaciones relativas a los servicios contemplados en el presente “CONTRATO” son de devengo periódico, por lo que el “PRESTADOR” realizará la facturación de los servicios que desarrolle en periodos quecomprendan de manera mensual del mes del calendario. En todo caso, la factura y/o recibo que el “PRESTADOR” emita deberá reunir los requisitos fiscales y contables así como contener expresos efectos liberatorios en cuanto al pago realizado por el “PRESTATARIO”.
 
 El monto pactado como contraprestación será de "+cantidad+" ("+cantidad_conletra+@") entregándose en una sola exhibición deberá de ser depositado por el “PRESTATARIO”, mediante Transferencia Electrónica o Cheque el “PRESTADOR”, por cada periodo que se devenguen los servicios.

CUARTA. VIGENCIA DEL CONTRATO.
Acuerdan las “PARTES” que el presente “CONTRATO” tendrá una vigencia del "+fechalimite+@".Cualquiera de las “PARTES” podrá dar por terminado el presente “CONTRATO” mediante previo aviso por escrito de alguna de las “PARTES” por lo menos con 30 días de anticipación.



QUINTA. RESPONSABILIDAD LABORAL.

El “PRESTADOR” es el único y directo responsable de las obligaciones laborales que surjan con el personal que intervenga en los servicios, por consiguiente, “no existe subordinación como elemento esencial de trabajo entre el personal del “PRESTADOR” y el “PRESTATARIO”, por lo cual, toda responsabilidad civil, laboral, etc., derivada por la ejecución del presente instrumento y que se relacione con el personal de el “PRESTADOR”, es responsabilidad total de este último. El “PRESTADOR” no deberá responsabilizar al “PRESTATARIO” con respecto a cualquier obligación que pudiese ser reclamado a este último derivado de las actividades de dicho personal. En todo caso, El “PRESTADOR” deberá reembolsar y no considerar responsable a el “PRESTATARIO” con respecto a ninguna retribución de carácter laboral y/o cualquier otra suma por diverso concepto que se requiera erogar y ser abonada a dicho personal, agentes o terceros, pudiendo estar tal cantidad en conexión directa y/o indirecta con el desarrollo de los Servicios. El “PRESTADOR” responderá de los gastos y costas de naturaleza jurisdiccional en que pudiese incurrir el “PRESTATARIO” por alguno de los conceptos antes referidos.El “PRESTADOR” estará obligado a celebrar los convenios de trabajo que resulten necesarios con su personal tanto dependiente como independiente, personal a través del cual llegue a proporcionar los servicios al “PRESTATARIO”. El “PRESTADOR” no tendrá facultad alguna para contratar servicios y/o persona en representación del “PRESTATARIO” cualquiera que sea la finalidad o el propósito del contrato o acuerdo sin el previo consentimiento expreso por escrito que esta última otorgue. Por tanto, se deja asentado que la persona o personas que auxilien al “PRESTADOR” en el desempeño de sus actividades no tendrán relación de hecho o de derecho alguno con el “PRESTATARIO”, en razón de que el “PRESTADOR” actúa con la organización y estructura propia, además de los recursos suficientes y adecuados para responder de sus obligaciones. En virtud de lo anterior el “PRESTADOR” liberará y/o relevará al “PRESTATARIO” de toda responsabilidad civil, laboral o de cualquier otra naturaleza proveniente de alguna obligación o acción que cualquiera de los miembros del Personal del “PRESTADOR” pudiera reclamar al “PRESTATARIO”.

Aun y cuando el “PRESTADOR” adquiera servicios, equipo y/o personal de otros proveedores, El “PRESTADOR” será la única y exclusiva responsable ante el “PRESTATARIO” de la eficiencia y adecuada prestación de todas las obligaciones contraídas bajo el presente “CONTRATO” y en ningún caso y bajo ninguna circunstancia el “PRESTATARIO” asumirá responsabilidades de carácter laboral o de cualquier índole ante terceros o ante el “PRESTADOR” por dicho concepto.

SEXTA. CONFIDENCIALIDAD DE LA INFORMACIÓN.

Toda información técnica, administrativa, de sistemas, clientes, proyectos, estrategias, y en general toda la información que las “PARTES” se proporcionen entre sí, bajo los términos del presente contrato, sea forma tangible o intangible, deberá ser considerada como propiedad de la parte divulgante de la misma, y por lo que la misma será de carácter restringido y confidencial (en lo sucesivo denominada “INFORMACIÓN CONFIDENCIAL”, por lo que únicamente será utilizada por la parte receptora conforme a lo que se establece en este “CONTRATO”, y no deberá ser entregada o revelada a cualquier persona que de manera expresa la parte divulgante no haya autorizado. La parte receptora se obliga a manejar la “INFORMACIÓN CONFIDENCIAL” propiedad de la otra parte divulgante, de igual o mejor manera con que manejen su propia “INFORMACIÓN CONFIDENCIAL”, pero siempre con las especificaciones mínimas de esta cláusula. Las partes se obligan a utilizar la “INFORMACIÓN CONFIDENCIAL” únicamente para la realización y cumplimiento del objeto del presente contrato, quedándoles estrictamente prohibido divulgarla por cualquier medio a terceros, copiarla, reproducirla o darle cualquier uso diverso al establecido. A la terminación o rescisión del presente contrato, la parte receptora se obliga a devolver a la parte divulgante, toda la información obtenida para el cumplimiento del objeto de este contrato, así como los productos derivados de su ejecución, ya que todos estos son propiedad exclusiva de la parte divulgante. La obligación de confidencialidad a cargo de las partes, a que se refiere la presente clausula, permanecerá vigente en forma indefinida posteriormente a la rescisión o terminación natural o anticipada del presente instrumento, por cualquiera de las partes de cualquiera de las obligaciones emanadas del presente “CONTRATO”, liberara a la otra de la presente obligación de confidencialidad.En caso de violación a lo dispuesto en la presente cláusula, la parte receptora se obliga a pagar a la parte divulgante los daños y perjuicios que le haya causado, mediante declaración judicial. Así mismo, la parte receptora será responsable de la comisión de infracciones y/o delitos previstos en las leyes correspondientes.

SÉPTIMA. RESCISIÓN DEL CONTRATO.

El presente “CONTRATO” podrá ser dado por terminado por cualquiera de las “PARTES” sin causa alguna por medio de notificación por escrito dada a la otra parte con una anticipación de 30 días.Cualquiera de las partes tendrá la facultad de dar por terminado el presente “CONTRATO” de manera inmediata mediando causa justificada que enunciativa más no limitativamente incluye:

	a) Cuando el Personal o agentes de el “PRESTADOR” por cualquier causa no imputable a el “PRESTATARIO” dejen de prestar los servicios objeto del presente “CONTRATO” o los desarrollen en contravención a las especificaciones contempladas en el mismo, esto transcurridos 15 días siguientes contados a partir de la fecha en que el “PRESTATARIO” haga del conocimiento de el “PRESTADOR” sobre la existencia de tal irregularidad y la misma no sea subsanada en el plazo antes referido.
	b) Por la falta de pago del “PRESTATARIO” de 3 facturas emitidas por concepto de los servicios llevados a cabo por el “PRESTADOR”, pudiendo ser tales faltas de pago sucesivo o discontinuo.

	c) Si alguna de las partes solicitara su declaración en concurso mercantil o sus acreedores hubiesen demandado la declaración de concurso mercantil.
Para el caso de que se dé por terminado el presente contrato, las “PARTES” observarán lo
siguiente:
	d) A solicitud del “PRESTATARIO”, el “PRESTADOR” deberá completar todos los servicios en proceso o pendientes de cumplimiento al momento de la rescisión o terminación del presente “CONTRATO”.
	e) Llevar adelante las acciones necesarias conforme a los acuerdos de confidencialidad y/o compromisos de secreto integrantes de este acuerdo.
	f) El “PRESTATARIO” deberá de pagar dentro de los plazos señalados al efecto y previa presentación de la factura y/o recibo correspondiente, las sumas devengadas y no pagadas por concepto de los servicios ya prestados al momento de la terminación de la vigencia de este “CONTRATO”, así como los demás gastos en los que incurra el “PRESTADOR” con posterioridad al término de este acuerdo y que resulten estrictamente necesarios para la conclusión de los Servicios en desarrollo al tiempo de la terminación o rescisión del presente instrumento.

Si la rescisión del presente “CONTRATO” se da por virtud de causa justificada atribuible al “PRESTADOR”, ésta deberá responder de los daños y perjuicios ocasionados al “PRESTATARIO”. Igualmente, el “PRESTADOR” pagará al “PRESTATARIO” los daños y perjuicios que se le ocasionen o pudieran ocasionar, en relación a los siguientes acontecimientos:
	
	a) Cualquier acto ilícito o indebido ya sea doloso o negligente que realice el “PRESTADOR” con motivo del presente “CONTRATO”. 
	b) Cualquier reclamación o exigencia laboral que efectuara cualquier miembro del Personal o agente del “PRESTADOR” en contra del “PRESTATARIO” de conformidad con lo pactado en el presente instrumento.

OCTAVA. EXCLUSIVIDAD.

Queda expresamente establecido en el presente “CONTRATO” que el “PRESTADOR” no cuenta con exclusividad alguna para el otorgamiento de sus servicios. Sin embargo el “PRESTADOR” no podrá desarrollar actividad alguna que directa o indirectamente compita o dañe o pueda competir o dañar a las actividades ordinarias de el “PRESTATARIO” que constituyen su propio objeto social, y/o a sus Compañías Filiales, sucursales, clientes y subsidiarias de sus clientes.

NOVENA. TÍTULOS.

Las “PARTES” del presente acuerdo de voluntades están conformes en admitir que los encabezados o títulos que se citan en todas y cada una de las cláusulas de este “CONTRATO” son sólo utilizados para la identificación de las mismas cláusulas, y bajo ninguna circunstancia alteran lo pactado por las partes en dichas cláusulas ni se consideran parte integrante de las mismas.

DECIMA. DISPOSICIONES GENERALES.

Este “CONTRATO” conforma el acuerdo total de las “PARTES” y deja sin efectos cualquier convenio o acuerdo anterior en relación con el objeto de este acuerdo. No se realizará cambio o modificación alguna a este instrumento con excepción de convenio por escrito, que sea suscrito y formalizado por todas las partes contratantes. El derecho de las partes a demandar el exacto cumplimiento y observancia de las obligaciones, términos, condiciones o convenciones con motivo de este contrato, no se verá afectado por negociaciones, dispensas o transacciones ocurridas con anterioridad a la fecha del presente acuerdo en relación con la misma u otra obligación, término o condición.Cada una de las partes declara y reconoce mutuamente para todos los efectos legales que en la celebración del presente contrato no ha mediado vicio alguno de voluntad, esto en virtud de que el objeto, términos y condiciones de este acuerdo han sido establecidos de común acuerdo por las partes signantes.

DECIMA PRIMERA. INVALIDEZ PARCIAL.

Si alguna o algunas de las disposiciones o pactos contenidos en este contrato se declaran ilegales, nulas, inválidas o no ejecutables, por orden o resolución de autoridad judicial competente, dicha declaración no afectará la validez de los demás términos y condiciones, mismos que permanecerán en vigor y serán cumplidos. Las partes de este acuerdo se obligan a modificar las disposiciones, pactos o convenciones afectadas, en la forma que resulte más congruente con el espíritu de tal disposición, pacto o convención y del contrato en su integridad.

DECIMA SEGUNDA. CASO FORTUITO O FUERZA MAYOR.

Para el caso de que cualquiera de las “PARTES” quedase inhabilitada total o parcialmente para cumplir sus obligaciones de conformidad con este “CONTRATO” en virtud de un caso de Fuerza Mayor, dichas obligaciones serán suspendidas en tanto dure tal impedimento. La parte inhabilitada deberá notificar por escrito a la otra parte en relación con tales circunstancias, estableciendo su naturaleza y explicando las razones por las cuales se deberán suspender temporalmente sus obligaciones. Solamente las obligaciones afectadas por una causa de Fuerza Mayor serán consideradas suspendidas, y tal caso de Fuerza Mayor deberá ser remediado o solucionado en un período de tiempo razonable. Una vez remediada o solucionada la circunstancia de Fuerza Mayor, las obligaciones que hubiesen sido afectadas por el caso de Fuerza Mayor deberán ser restablecidas en su vigencia de manera inmediata.

DECIMA TERCERA. LEY APLICABLE.

Para el cumplimiento e interpretación, este contrato se regirá por la Legislación vigente en los Estados Unidos Mexicanos.

LEÍDO QUE FUE EN VOZ ALTA POR AMBAS PARTES ESTE CONTRATO, E IMPUESTAS Y ENTERADAS DE SU CONTENIDO, EFECTOS Y ALCANCES LEGALES CORRESPONDIENTES, LO RATIFICAN EN TODO SU CLAUSULADO Y LO FIRMAN AL CALCE Y MARGEN PARA DEBIDA CONSTANCIA Y NO MEDIANDO ENTRE ELLAS INCAPACIDAD LEGAL O VICIO DE CONSENTIMIENTO ALGUNO, EN "+lugar_compraventa+", "+fecha_compraventa+".");


Createparagrah(document, @"

 “El PRESTADOR” 

                       _____________________________
" + vendedor_nombre, TextAlignment.CENTER);

            Createparagrah(document, @" 

                          “El PRESTATARIO” 


                                                        ", TextAlignment.CENTER);

            addtable_puestos(document, dataGridView2);

            


            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            Createparagrah(document, @" FECHA " + fecha_actual,TextAlignment.RIGHT); 
         	                                                                                                                    
Createparagrah(document, @" COTIZACIÓN  SOLICITADA  ",TextAlignment.LEFT); 
Createparagrah(document,@"DIRIGIDO A : " + comprador_nombre + @" 	 


");
            addtable(document, dataGridView1);

            Createparagrah(document, @"
EN ESPERA DE SU RESPUESTA
");

            Createparagrah(document, @"

ATENTAMENTE

"+vendedor_nombre+"", TextAlignment.CENTER);

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.IO;

namespace vxml
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var xmlFile = string.Empty;
            var xsdFile = string.Empty;

            // Выбор xml файла:
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Title = "Выберете xml файл";
                ofd.Filter = "Xml File (*.xml)|*.xml|All files (*.*)|*.*";
                ofd.FilterIndex = 1;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                
                    Console.WriteLine("Xml файл не выбран!");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
                xmlFile = ofd.FileName;
            }

            // Выбор xsd схемы:
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Title = "Выберете XSD схему";
                ofd.Filter = "XML Schema (*.xsd)|*.xsd|All files (*.*)|*.*";
                ofd.FilterIndex = 1;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    
                    Console.WriteLine("Xsd схема не выбрана!");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
                xsdFile = ofd.FileName;
            }

            var xmlReaderSettings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings
            };

            var log = new StringBuilder();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            xmlReaderSettings.ValidationEventHandler += (obj, arg) =>
            {
                log.AppendLine($"Обнаружен {arg.Severity:G}: {arg.Message} {arg.Exception.LineNumber} ({arg.Exception.LinePosition})");
            };
            
            xmlReaderSettings.Schemas.Add(null, xsdFile);
            var xmlReader = XmlReader.Create(xmlFile, xmlReaderSettings);

            bool ExcTrue = false;

            try
            {
                while (xmlReader.Read()) { }
            }
            catch (System.Xml.XmlException Exc)
            {
                MessageBox.Show("Возникли критические ошибки при открытии xml файла!!!");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Exc.Message);
                Console.WriteLine("LineNumber:" + Exc.LineNumber);
                Console.WriteLine("LinePosition:" + Exc.LinePosition);
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.WriteLine(log.ToString());
                ExcTrue = !ExcTrue;

            }
            if (!ExcTrue)
            {
                if (log.ToString().Length > 1)
                {
                    if (MessageBox.Show("Сохранить в отчет в файл?", "Отчет о проверке", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Выбор файла для сохранения отчета
                        using (SaveFileDialog sfd = new SaveFileDialog())
                        {
                            sfd.InitialDirectory = "c:\\";
                            sfd.Title = "Сохранение отчета";
                            sfd.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
                            sfd.FilterIndex = 1;
                            sfd.RestoreDirectory = true;

                            while(sfd.ShowDialog() != DialogResult.OK)
                            {

                            }

                            File.WriteAllText("WriteLines.txt", log.ToString());
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(log.ToString());
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Ошибок не найдено!");
                }
                
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(String.Format("Затрачено: {0} времени", elapsedTime));
            Console.ReadKey();
        }
    }
}

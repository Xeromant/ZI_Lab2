using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Lab2
{
    public partial class Form1 : Form
    {
        private string beginingPath;
        private string endingPath = "C:\\Users\\inet\\Desktop\\ВУЗ\\7 семестр\\ЗИ\\Lab2\\TestData.txt";
        public Form1()
        {
            InitializeComponent();
        }
        // Функция выбора файла. Используется для просмотра файлов и при выборе файла для
        // Шифрования/Дешифрования
        public void ChooseFile()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\Users\\inet\\Desktop\\ВУЗ\\7 семестр\\ЗИ\\Lab2\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                beginingPath = openFileDialog.FileName;
                textBox1.Text = OpenTxt(beginingPath);

            }
        }
        // Функция преобразует файл в строку и позволяет его использовать
        // По идее, можно и без этой функции, но я затупил
        public string OpenTxt(string folder)
        {
            string textFromFile;
            using (FileStream fstream = File.OpenRead(folder))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                textFromFile = Encoding.UTF8.GetString(array);
                fstream.Close();

            }
            
            return textFromFile;
        }
        // Функция преобразует пароль пользователя в 256-битовый хэш 
        public byte[] ComputeHash(string UserPassword, HashAlgorithm algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(UserPassword);

            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            
            //MessageBox.Show(algorithm.OutputBlockSize.ToString());
            return hashedBytes;
        }

        public void Enycrypt(string folder, string UserPassword)
        {
            
            FileStream myStream = new FileStream(endingPath, FileMode.OpenOrCreate);
            Aes aes = Aes.Create();

            byte[] key = ComputeHash(UserPassword, new SHA256CryptoServiceProvider());
            byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            CryptoStream cryptStream = new CryptoStream(
                            myStream,
                            aes.CreateEncryptor(key, iv),
                            CryptoStreamMode.Write);
            StreamWriter sWriter = new StreamWriter(cryptStream);

            //Write to the stream.  
            sWriter.WriteLine(OpenTxt(folder));
            sWriter.Close();
            cryptStream.Close();
            myStream.Close();
        }

        public string Decrypt(string folder, string UserPassword)
        {
            byte[] key = ComputeHash(UserPassword, new SHA256CryptoServiceProvider());
            byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            FileStream myStream = new FileStream(folder, FileMode.Open);
            Aes aes = Aes.Create();
            CryptoStream cryptStream = new CryptoStream(
               myStream,
               aes.CreateDecryptor(key, iv),
               CryptoStreamMode.Read);
            StreamReader sReader = new StreamReader(cryptStream);
            string result = sReader.ReadToEnd();
            sReader.Close();
            myStream.Close();
            return result;
        }

        public void CreateDecryptionFile(string folder, string UserPassword)
        {
            
            FileStream myStream = new FileStream("C:\\Users\\inet\\Desktop\\ВУЗ\\7 семестр\\ЗИ\\Lab2\\DecryptionFile.txt", FileMode.Create);
            StreamWriter sWriter = new StreamWriter(myStream);
            sWriter.WriteLine(Decrypt(folder, UserPassword));
            sWriter.Close();
            myStream.Close();
        }

        
        private void Button1_Click(object sender, EventArgs e)
        {
            ChooseFile();
            string password = textBox2.Text;
            Enycrypt(beginingPath, password);
            textBox1.Text = OpenTxt(endingPath);
            textBox3.Text = "TestData.txt";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ChooseFile();
            string password = textBox2.Text;
            textBox1.Text = Decrypt(endingPath, password);
            CreateDecryptionFile(endingPath, password);
            textBox3.Text = "DecryptionFile.txt";

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ChooseFile();
        }
    }
}

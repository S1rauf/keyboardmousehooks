using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml;
using System.Security.Cryptography;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace KeyboardMouseHooks
{
    using HWND = IntPtr;
    public partial class mainForm : Form
    {
        
        int i2;
        int w;
        int h;
        string[] splitsw;
        public mainForm()
        {
            InitializeComponent();
            Rectangle screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            w = screenSize.Size.Width;
            h = screenSize.Size.Height;
            openFileDialog1.Filter = "XML files(*.xml)|*.xml|All files(*.*)|*.*";
            saveFileDialog1.Filter = "XML files(*.xml)|*.xml|All files(*.*)|*.*";
            keyboardHook.Install();
            mouseHook.Install();
            mouseHook.MouseMove += new RamGecTools.MouseHook.MouseHookCallback(mouseHook_MouseMove);
            keyboardHook.KeyDown += new RamGecTools.KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new RamGecTools.KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);
        }
        private void mainForm_Load(object sender, EventArgs e)
        {
            

        }
      
        delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        static extern IntPtr GetShellWindow();
        // конец

        [DllImport("User32.dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(ref POINT lpPoint);

        [DllImport("User32.dll")]
        public static extern bool ClientToScreen(IntPtr xWnd, ref POINT point);

        [System.Runtime.InteropServices.DllImport("user32.dll",
         CharSet = System.Runtime.InteropServices.CharSet.Auto,
         CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        //Нормированные абсолютные координаты
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        //Нажатие на левую кнопку мыши
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;

        //Поднятие левой кнопки мыши
        private const int MOUSEEVENTF_LEFTUP = 0x0004;

        //Нажатие на левую кнопку мыши
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;

        //Поднятие левой кнопки мыши
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;

        //перемещение указателя мыши
        private const int MOUSEEVENTF_MOVE = 0x0001;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, UInt32 dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Флаг нажатия кнопки
        public const int KEYEVENTF_KEYUP = 0x0002; //Флаг отпускания кнопки

        // Нажатие Cntrl+V
        private static void CtrlV()
        {
            SendKeys.SendWait("^{v}");
        }
        // Нажатие Cntrl+C
        private static void CtrlC()
        {
            SendKeys.SendWait("^{c}");
        }
        // Нажатие Enter
        private static void Ent_V()
        {
            keybd_event(Keys.Enter, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(Keys.Enter, 0, KEYEVENTF_KEYUP, 0);

        }
        int mX;
        int mY;
        // Движение курсора
        private void DoMouseMove(int x, int y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, 0);
        }
        // Левый клик
        private void DoMouseLeftClick(int x, int y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }
        // Правый клик
        private void DoMouseRightClick(int x, int y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
        }
        // Двойной клик
        private void DoMouseDoubleLeftClick(int x, int y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP, x, y, 0, 0);

            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        RamGecTools.MouseHook mouseHook = new RamGecTools.MouseHook();
        RamGecTools.KeyboardHook keyboardHook = new RamGecTools.KeyboardHook();

        // координаты мышки
        void mouseHook_MouseMove(RamGecTools.MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            mX = Convert.ToInt32(mouseStruct.pt.x.ToString());
            mY = Convert.ToInt32(mouseStruct.pt.y.ToString());
        }


        // при нажатии горячей клавиши z сохраняем координаты мышки
        void keyboardHook_KeyDown(RamGecTools.KeyboardHook.VKeys key) 
        {
            if (key.ToString() == "KEY_Z")
            {
                clki2(mX, mY);
            }
        }
        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // удаляем установленные хуки
            mouseHook.Uninstall();
            keyboardHook.Uninstall();      
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox3 = (CheckBox)sender; // Выставляем CheckBox соглачно выбранной схеме работы
            if (checkBox3.Checked )
            {
                radioZam.Enabled = true;
                radioPast.Enabled = true;
                radioConnection.Enabled = true;
                radioZam.Checked = true;
                radioPast.Checked = false;
                radioConnection.Checked = false;
            }
            else
            {
                radioZam.Checked = false;
                radioPast.Checked = false;
                radioConnection.Checked = false;
                radioZam.Enabled = false;
                radioPast.Enabled = false;
                radioConnection.Enabled = false;
            }
        }
        // Заполнение таблицы координат
        public void clki2(int m_X, int m_Y)
        {
            int pogrX = 20;
            int pogrY = 3;
            if ( dataGridView1.RowCount == 1)
            {
                dataGridView1.Rows.Add((m_X + pogrX) + " x " + (m_Y + pogrY), true);
            }
            else
            {
                if (checkBox3.Checked)
                {
                    if (radioZam.Checked)
                    {
                        dataGridView1.Rows.Add((m_X + pogrX) + " x " + (m_Y + pogrY), false, false, true, true, null, true);
                    }
                    if (radioPast.Checked)
                    {
                        dataGridView1.Rows.Add((m_X + pogrX) + " x " + (m_Y + pogrY), false, false, true, false, null, true);
                    }
                    if (radioConnection.Checked)
                    {
                        dataGridView1.Rows.Add((m_X + pogrX) + " x " + (m_Y + pogrY), true, false, false, false, null, false);
                    }
                }
                else
                {
                    dataGridView1.Rows.Add((m_X + pogrX) + " x " + (m_Y + pogrY));
                }
            }           
        }
        //Запуск работы по координатам
        private void button6_Click(object sender, EventArgs e)
        {
            avt();
        }
        //Работа в автомате
        public void avt()
        {
            bool bf;
            string text;
            string[] words;
            bool lm;
            bool rm;
            bool dlm;
            bool azm;
          
            int count = dataGridView1.Rows.Count - 1;
            if (count > 0)
            {
                for (int ind = 0; ind < count; ind++)
                {
                    text = dataGridView1[0, ind].Value.ToString();
                    words = text.Split(new char[] { ' ' });
                    lm = Convert.ToBoolean(dataGridView1[1, ind].Value);
                    rm = Convert.ToBoolean(dataGridView1[2, ind].Value);
                    dlm = Convert.ToBoolean(dataGridView1[3, ind].Value);
                    azm = Convert.ToBoolean(dataGridView1[4, ind].Value);

                    if (dataGridView1[5, ind].Value == null)
                    {
                        bf = false;
                    }
                    else
                    {
                        bf = true;
                        string buf = dataGridView1[5, ind].Value.ToString();
                        Clipboard.Clear();
                        Clipboard.SetText(buf);
                    }

                    bool Ent = Convert.ToBoolean(dataGridView1[6, ind].Value);

                    int lX = Convert.ToInt32(words[0]);
                    int lY = Convert.ToInt32(words[2]);
                    int X = (65535 / w) * lX;
                    int Y = (65535 / h) * lY;
                    // Окрашивание сделанной работы
                    dataGridView1.Rows[ind].DefaultCellStyle.BackColor = Color.Green;

                    DoMouseMove(X, Y);

                    Thread.Sleep(100);
                    if (lm)
                    {
                        Thread.Sleep(200);
                        DoMouseLeftClick(X, Y);
                        Thread.Sleep(100);
                    }
                    if (rm)
                    {
                        Thread.Sleep(200);
                        DoMouseRightClick(X, Y);
                        Thread.Sleep(100);
                    }
                    if (dlm)
                    {
                        Thread.Sleep(200);
                        DoMouseDoubleLeftClick(X, Y);
                        Thread.Sleep(100);
                        int w = 0;

                        while (w == 0)
                        {
                            bool hg = GetOpenWindows();
                            if (hg )
                            {

                                w = 1;

                            }
                            else
                            {
                                Thread.Sleep(100);
                            }

                        }
                    }

                    if (bf)
                    {
                        Thread.Sleep(500);
                        CtrlV();
                        Thread.Sleep(200);
                    }
                    if(azm)
                    {
                        
                            Thread.Sleep(200);
                            CtrlC();
                            Thread.Sleep(200);
                            string b_uf = null;
                            bool containsText = false;
                            bool exception = false;
                            do
                            {
                                exception = false;
                                try
                                {
                                    containsText = Clipboard.ContainsText();
                                }
                                catch(Exception)
                                {
                                    exception = true;
                                }
                            } while(exception);
                            if (containsText)
                            {
                                do
                                {
                                    exception = false;

                                    try
                                    {
                                        b_uf = Clipboard.GetText();
                                    }
                                    catch (Exception)
                                    {
                                        exception = true;
                                    }
                                } while (exception);
                            }                     
                            Thread.Sleep(200);
                            try
                            {
                                string b_uf2 = (b_uf.Replace(textBox1.Text, textBox2.Text));
                                Clipboard.SetText(b_uf2);
                            }
                            catch (Exception)
                            { }
                            
                            CtrlV();                      
                            Thread.Sleep(100);
                            Clipboard.Clear();                                               
                    }
                    if (Ent)
                    {
                        Ent_V();
                    }
                }
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7)
            {
                int a = dataGridView1.CurrentRow.Index;
                dataGridView1.Rows.RemoveAt(a);
            }
        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
        private void очиститьТаблицуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clear_datagrid();
        }
        public void clear_datagrid()
         {
            int nr;
            nr = dataGridView1.RowCount-2;
            if (nr > 1)
            {
                for (int rind = nr; rind >= 1; rind--)    
                {
                    dataGridView1.Rows.RemoveAt(rind); // удалить первую строку  
                }
            } 
         }
        private void вставитьИзБуфераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = dataGridView1.Rows.Count - 1;
            IDataObject dataInClipboard = Clipboard.GetDataObject();

            string stringInClipboard = (string)dataInClipboard.GetData(DataFormats.Text);
            splitsw = stringInClipboard.Split(new Char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int rowindex = dataGridView1.CurrentRow.Index;

            int i5;
            if (count > splitsw.Length)
            {
                i5 = splitsw.Length;
            }
            else
            {
                i5 = count;

            }
            for (int i = 0; i < i5; i++)
            {
                this.dataGridView1[5, rowindex].Value = splitsw[i];
                rowindex++;
            }
        }
        //Заполнение таблицы координат относительно первой точки по шаблону
        private void координатыПоШаблонуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filepath = openFileDialog1.FileName;

            XmlDocument doc = new XmlDocument();
            doc.Load(@filepath); // загружается XML-файл
            XmlNodeList nodes = doc.ChildNodes;

            string text;
            string[] words;
            text = dataGridView1[1, 0].Value.ToString();
            words = text.Split(new char[] { ' ' });
            int x_dgv = Convert.ToInt32(words[0]);
            int y_dgv = Convert.ToInt32(words[2]);

            // Обходим значения
            foreach (XmlNode n in nodes)
            {
                // Обрабатываем в цикле 
                if ("parameters".Equals(n.Name))
                {
                    // Читаем в цикле 
                    for (XmlNode d = n.FirstChild; d != null; d = d.NextSibling)
                    {
               
                        if ("data".Equals(d.Name))
                        {
                            int y = 0;
                            int x = 0;

                            bool l = false;
                            bool r = false;
                            bool dl = false;
                            bool azm = false;
                            bool en = false;
                            
                            for (XmlNode k = d.FirstChild; k != null; k = k.NextSibling)
                            {
                                //} + Convert.ToInt32(dataGridView1[0, 1].Value)
                                if ("X".Equals(k.Name) && k.FirstChild != null) // + проверка на Null
                                {
                                    x = Convert.ToInt32(k.FirstChild.Value) + x_dgv; // Присваиваем значения переменной
                                }
                                if ("Y".Equals(k.Name) && k.FirstChild != null) 
                                {
                                    y = Convert.ToInt32(k.FirstChild.Value) + y_dgv; 
                                }
                                if ("L".Equals(k.Name) && k.FirstChild != null) 
                                {
                                    l = Convert.ToBoolean(k.FirstChild.Value); 
                                }
                                if ("R".Equals(k.Name) && k.FirstChild != null) 
                                {
                                    r = Convert.ToBoolean(k.FirstChild.Value); 
                                }
                                if ("DL".Equals(k.Name) && k.FirstChild != null) 
                                {
                                    dl = Convert.ToBoolean(k.FirstChild.Value);
                                }
                                if ("Azm".Equals(k.Name) && k.FirstChild != null) 
                                {
                                    azm = Convert.ToBoolean(k.FirstChild.Value);
                                }
                                if ("En".Equals(k.Name) && k.FirstChild != null) 
                                {
                                    en = Convert.ToBoolean(k.FirstChild.Value); 
                                }

                            }
                            // Присваиваем значение 
                            dataGridView1.Rows.Add(x + " x " + y, l, r, dl, azm, "", en);
                        }
                    }
                }
            }
        }
        private void сохранитьКакШаблонToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filepath = saveFileDialog1.FileName;

            int count = dataGridView1.Rows.Count - 1;
            // string filepath = "C:\\xml_2.xml";
            XmlTextWriter writer = new XmlTextWriter(filepath, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("parameters");

            string text;
            string[] words;
            text = dataGridView1[0, 1].Value.ToString();
            words = text.Split(new char[] { ' ' });
            int x_dgv = Convert.ToInt32(words[0]);
            int y_dgv = Convert.ToInt32(words[2]);
            // Запись в цикле
            for (int xc = 2; xc < count; xc++)
            {
                string text2;
                string[] words2;
                text2 = dataGridView1[0, xc].Value.ToString();
                words2 = text2.Split(new char[] { ' ' });
                int x2_dgv = Convert.ToInt32(words2[0]);
                int y2_dgv = Convert.ToInt32(words2[2]);

                bool lm = Convert.ToBoolean(dataGridView1[1, xc].Value);
                bool rm = Convert.ToBoolean(dataGridView1[2, xc].Value);
                bool dlm = Convert.ToBoolean(dataGridView1[3, xc].Value);
                bool Azm = Convert.ToBoolean(dataGridView1[4, xc].Value);
                bool En = Convert.ToBoolean(dataGridView1[6, xc].Value);

                // Начинаем записывать значение
                writer.WriteStartElement("data");
                // Записываем атрибут
                writer.WriteAttributeString("point", xc.ToString());
 
                writer.WriteStartElement("X");
                writer.WriteValue(x2_dgv - x_dgv);
                writer.WriteEndElement();

                writer.WriteStartElement("Y");
                writer.WriteValue(y2_dgv - y_dgv);
                writer.WriteEndElement();

                writer.WriteStartElement("L");
                writer.WriteValue(lm);
                writer.WriteEndElement();

                writer.WriteStartElement("R");
                writer.WriteValue(rm);
                writer.WriteEndElement();

                writer.WriteStartElement("DL");
                writer.WriteValue(dlm);
                writer.WriteEndElement();

                writer.WriteStartElement("Azm");
                writer.WriteValue(Azm);
                writer.WriteEndElement();

                writer.WriteStartElement("En");
                writer.WriteValue(En);
                writer.WriteEndElement();

            }
            writer.WriteEndElement(); 
            writer.WriteEndDocument(); 
            writer.Flush();
            writer.Close();
        }
        private void очититьСтолбецToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow myRow in dataGridView1.Rows)
            {
                myRow.Cells[5].Value = null;
            }
        }
        private void удалитьСтрокуToolStripMenuItem_Click(object sender, EventArgs e)
        {
                int a = dataGridView1.CurrentRow.Index;
                dataGridView1.Rows.RemoveAt(a);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            add();
        }
        public void add()
        {
            if (textBox3.Text != String.Empty)
            {
                int step = 16;
                int count = dataGridView1.Rows.Count - 2;
                string text = dataGridView1[0, count].Value.ToString();
                string[] words = text.Split(new char[] { ' ' });

                int l_x = Convert.ToInt32(words[0]) - 20;
                int l_y = Convert.ToInt32(words[2]);

                int stroki = Convert.ToInt32(textBox3.Text);
                int iter = 0;

                do
                {
                    iter++;
                    l_y = l_y + step;
                    clki2(l_x, l_y);

                } while (iter < stroki);
            }
            else 
            {
                MessageBox.Show("Введите какое кол-во строк необходимо добавить!!!");
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e) // В текстбокс должны быть только цифры
        {
            char number = e.KeyChar;
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && number != 8 && number != 44)
            {
                e.Handled = true;
            } 
        }

        private void radioZam_CheckedChanged(object sender, EventArgs e)
        {
          
            if (radioZam.Checked)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
        }

        public static Boolean GetOpenWindows()
        {
            Boolean wind = false;
            HWND lShellWindow = GetShellWindow();
       
            EnumWindows(delegate(HWND hWnd, int lParam)
            {
                if (hWnd == lShellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int lLength = GetWindowTextLength(hWnd);
                if (lLength == 0) return true;

                StringBuilder lBuilder = new StringBuilder(lLength);
                GetWindowText(hWnd, lBuilder, lLength + 1);
                
                if (lBuilder.ToString() == "Выбрать переменную")
                {
                    wind = true;
                }
               
                return true;
                
            }, 0);    
            return wind;
        }

    }
}

﻿using System;
using System.IO;
using System.Text;  // for Encoding
using System.Timers;




namespace WebChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wellcom to my home !!");
            WebPage webPage = new WebPage();
            webPage.outputHTMLfile();
            webPage.dispURL();

            TimerProgram tm = new TimerProgram();
            tm.loopstart();
            webPage.dispURL();

        }
    }
    #region WebPage_class
    class WebPage
    {
        String PageURL = "";
        const String URLfilename = "TargetURL.txt";
        #region Constracter
        /* コンストラクタ */
        public WebPage()
        {
            this.getPageURL();
        }
        public WebPage(String sPageURL)
        {
            this.PageURL = sPageURL;
        }
        #endregion
        #region method
        /* メソッド */
        void getPageURL()
        {
            /* ファイルからURLを読み込む */
            if (File.Exists(URLfilename))
            {
                StreamReader reader = new StreamReader(URLfilename,
                                           Encoding.GetEncoding("UTF-8"));
                string A;
                while ((A = reader.ReadLine()) != null)
                {
                    /* 読み込んだ１行分のテキスト A の処理 */
                    if (PageURL.Length == 0)
                    {
                        PageURL = A;
                    }
                }
                reader.Close();
            }
            else
            {  /* ファイルがなければ標準入力 */
                Console.WriteLine("Can't find file ! enter url");
                PageURL = Console.ReadLine();
            }
        }
        /* Debug用の標準出力 */
        public void dispURL()
        {
            Console.WriteLine(PageURL);
        }


        /* 取得したファイル内容を書きだす */
        public void outputHTMLfile(){
            String outputFilePath;
            String filePath;
            /* 出力Pathを作る */
            filePath = "Log/";
            filePath += midDomain();

            /* 出力ファイル名を作る */
            String fileName;
            fileName = createFileName();

            /* 出力Pathを完成させる */
            outputFilePath = System.IO.Path.Combine(filePath, fileName);

            //Console.WriteLine(outputFilePath);

            /* 出力処理 */
            getWebPage(outputFilePath);


        }

        /* 出力するファイル名を返す */
        String createFileName(){
            String filename = "defaultfilename.html";

            DateTime now = DateTime.Now;
            filename = now.ToString("yyyyMMdd_HHmmss") + ".txt";

            return filename;
        }

        /* TargetURLからドメイン部分を切り出す */
        String midDomain(){
            String DomainName = "DomainName";
            String splitterStrt = "//";
            String splitterEnd = "/";
            int start;
            int end;

            start = PageURL.IndexOf(splitterStrt, StringComparison.Ordinal);
            if(start < 1){
                /* URLの中に"//"が無い場合はデフォルトの名前 */
                return DomainName;
            }
            end = PageURL.IndexOf(splitterEnd, start + splitterStrt.Length, StringComparison.Ordinal);

            if(end - start <= 0){
                return DomainName;
            }
            if(end - start > 30){
                /* 長い時は端折る */
                end = start + 30;
            }

            /* 切りだす */
            DomainName = PageURL.Substring(start+splitterStrt.Length, (end - (start + splitterStrt.Length)) + 1);

            return DomainName;
        }

        void getWebPage(string opfpath)
        {
            //opfpath = "test.html";
            if (Directory.Exists(System.IO.Path.GetDirectoryName(opfpath) + "/"))
            {
                ;
            }
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(opfpath) + "/");

            StreamWriter writer = new StreamWriter(opfpath,
                                       false,  // 上書き （ true = 追加 ）
                                       Encoding.GetEncoding("UTF-8"));

            //writer.WriteLine("testdata");
            //writer.Close();

            System.Net.WebClient client = new System.Net.WebClient();
            //client.Encoding = System.Text.Encoding.shift_jis;
            string str = client.DownloadString(PageURL);
            client.Dispose();

            writer.Write(str);
            writer.Close();

        }


        #endregion

    }

    class TimerProgram{
        Timer timer;

        public void loopstart(){
            // 開始時に間隔を指定する
            timer = new Timer(1000/*msec*/);

            // Elapsedイベントにタイマー発生時の処理を設定する
            timer.Elapsed += (sender, e) =>
            {
                try
                {
                    timer.Stop(); // もしくは timer.Enabled = false;

                    // 何らかの処理
                    Console.WriteLine("Ticks = " + DateTime.Now.Ticks);
                    this.setInterval();

                }
                finally
                {
                    timer.Start(); // もしくは timer.Enabled = true;
                }
            };

            // タイマーを開始する
            timer.Start();

            Console.ReadLine();

            // タイマーを停止する
            timer.Stop();

            // 資源の解放
            using (timer) { }

        }

        /* 次の起動までの時間をセットする */
        public void setInterval(){
            timer.Interval += 1000;
        }
    }



    class DifferenceDitector
    {
        /* 指定されたPathのファイルを読み取って違いがあるか調べるクラス */
        public String oldFilePath { get; set; }
        public String newestFilePath { get; set; }

        /* ファイルが存在するか確認する */
        private bool CheckFileExist(){
            bool bRtn = false;
            if(System.IO.File.Exists(oldFilePath) && System.IO.File.Exists(newestFilePath)){
                bRtn = true;
            }else{
                bRtn = false;
                Console.WriteLine("File not found !!");
            }
            return bRtn;
        }
        /* テキスト内容の単純比較 */
        public bool CompareRawText(){
            bool bRtn = false;
            /* ファイルの存在チェック */
            if(bRtn = CheckFileExist()){
                return bRtn;
            }
            /* ファイルの比較 */
            bRtn = FileCompare(oldFilePath, newestFilePath);
            return bRtn;
        }

        /* https://support.microsoft.com/ja-jp/help/320348/how-to-create-a-file-compare-function-in-visual-c */
        // This method accepts two strings the represent two files to // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

    }
    #endregion
}

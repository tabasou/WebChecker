using System;
using System.IO;
using System.Text;  // for Encoding
using System.Timers;
using System.Net.Mail;



namespace WebChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wellcom to my home !!");


            TimerProgram tm = new TimerProgram();
            tm.initLoop();
            tm.loopstart();

        }
    }
    #region WebPage_class

    class LogFile{
        public String prevFilePath { get; set; }
        public String newestFilePath { get; set; }

        public void upDate(String newFilePath){
            prevFilePath = newestFilePath;
            newestFilePath = newFilePath;
        }
        public void dispPath(){
            Console.WriteLine("Pre:" + prevFilePath);
            Console.WriteLine("Now:" + newestFilePath);
        }
        public String prevFileName(){
            return Path.GetFileName(prevFilePath);
        }
        public String newestFileName(){
            return Path.GetFileName(newestFilePath);
        }
    }
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
        public String outputHTMLfile(){
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

            return outputFilePath;
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
        LogFile logFile = new LogFile();
        IntervalBatch ib = new IntervalBatch();
        int LoopCounter;
        Random cRandom;

        public void initLoop(){
            logFile.newestFilePath = "Default";
            logFile.upDate("Default");

            ib.initIntervalBatch(ref logFile);
            LoopCounter = 0;
            cRandom = new System.Random();

            Console.WriteLine("StartProgram : " + DateTime.Now);
        }
        public void loopstart(){
            //http://takachan.hatenablog.com/entry/2017/09/09/225342
            // 開始時に間隔を指定する
            timer = new Timer(1000/*msec*/);

            // Elapsedイベントにタイマー発生時の処理を設定する
            timer.Elapsed += (sender, e) =>
            {
                try
                {
                    timer.Stop(); // もしくは timer.Enabled = false;

                    // 何らかの処理
                    //Console.WriteLine("Ticks = " + DateTime.Now);
                    //logFile.upDate(timer.Interval.ToString());
                    //ib.testmethod();
                    ib.IntervalMain();

                    this.setInterval();
                    LoopCounter++;

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
            timer.Interval = disideNextInterval();

            DateTime dt1 = DateTime.Now;

            TimeSpan ts1 = new TimeSpan(0, 0, 0, (int)(timer.Interval / 1000));
            //TimeSpan ts1 = new TimeSpan(0, 0, 0, 500);
            //TimeSpan ts1 = new TimeSpan(1, 2, 45, 15);

            DateTime dt2 = dt1 + ts1;
            Console.WriteLine("now is        " + dt1.ToString());
            Console.WriteLine("next check is " + dt2.ToString());



        }
        private int disideNextInterval(){
            const int Base_Interval_minuts = 15;    //基本インターバル（分）
            const int Base_Minus_minuts = 5;        //基本マイナス幅（分）
            const int Base_Plus_minuts = 5;         //基本プラス幅（分）

            int Base_Interval_msec = Base_Interval_minuts * 60 * 1000;          //基本インターバル（ミリ秒）
            int Base_Minus_msec = Base_Minus_minuts * 60 * 1000;
            int Base_Plus_msec = Base_Plus_minuts * 60 * 1000;

            int tmpNextInterval = disideNextInterval_msec(Base_Interval_msec, Base_Minus_msec, Base_Plus_msec);

            //除外スケジュール
            DateTime dt1 = DateTime.Now;
            DateTime dt2;
            TimeSpan ts1;
            bool bFlag = true;
            int wcounter = 0;

            while(bFlag){
                ts1 = new TimeSpan(0, 0, 0, (int)(tmpNextInterval / 1000));
                dt2 = dt1 + ts1;

                int nextHour = dt2.Hour;
                //夜中0時から5時台は起動しない
                if((0 <= nextHour) && (nextHour <= 5)){
                    tmpNextInterval += Base_Interval_msec;
                    wcounter++;
                }else{
                    bFlag = false;
                }

                if(wcounter >= (24*60 / Base_Interval_minuts)){
                    bFlag = false;
                }

            }


            return tmpNextInterval;
        }
        private int disideNextInterval_msec(int base_msec, int minus_msec, int plus_msec){
            //http://jeanne.wankuma.com/tips/csharp/random/next.html
            int iResult3 = cRandom.Next(base_msec - minus_msec, base_msec + plus_msec);

            return iResult3;
        }
    }

    class IntervalBatch{
        LogFile rLf;
        WebPage webPage;
        DifferenceDitector differenceDitector;
        GMailSender gMailSender;


        public void initIntervalBatch(ref LogFile lf){
            this.rLf = lf;
            webPage = new WebPage();
            differenceDitector = new DifferenceDitector();
            gMailSender = new GMailSender();
        }

        public void testmethod(){
            Console.WriteLine("[internalBatch]--->");
            rLf.dispPath();
            Console.WriteLine("[internalBatch]<---");
        }
        public void IntervalMain(){
            /* 周期的に実行する一連の処理をここに書く */

            // Webページをチェックしてファイルに出力する
            String opfp = webPage.outputHTMLfile();

            //比較する２つのファイルのPathをセットする
            rLf.upDate(opfp);
            differenceDitector.setCompareFilePath(rLf.prevFilePath, rLf.newestFilePath);

            //比較して結果に応じて処理を振り分ける
            if(differenceDitector.CompareRawText()){
                Console.WriteLine("same !!");
            }else{
                Console.WriteLine("Different !!");
                Console.WriteLine("opendiff " + rLf.prevFileName() + " " + rLf.newestFileName());
                gMailSender.setBody("new Item !?");
                gMailSender.setSubject("testTitle");
                gMailSender.sendExe();
            }

        }
        
    }

    class DifferenceDitector
    {
        /* 指定されたPathのファイルを読み取って違いがあるか調べるクラス */
        private String oldFilePath { get; set; }
        private String newestFilePath { get; set; }

        public void setCompareFilePath(String oldpath, String newpath){
            oldFilePath = oldpath;
            newestFilePath = newpath;
        }
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
            if((bRtn = CheckFileExist()) != true){
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


    class GMailSender{
        //https://qiita.com/rrryutaro/items/d746e0e8197ce36a14fe

        const string mailsettingFilePath = "../../setting.txt";
        private string mailID;
        private string mailPassword;
        private string mailFrom;
        private string mailTo;
        private string mailSubject;
        private string mailBody;

        public void setBody(String body)
        {
            mailBody = body;
        }
        public void setSubject(String Sbjct)
        {
            mailSubject = Sbjct;
        }
        public void sendExe(){
            this.setMailInfo();
            this.SendMail();
        }

        private void setMailInfo(){
            int lineNo = 0;
            /* ファイルからメール送信のための情報を取得する */
            if (File.Exists(mailsettingFilePath))
            {
                StreamReader reader = new StreamReader(mailsettingFilePath,
                                           Encoding.GetEncoding("UTF-8"));
                string A;
                while ((A = reader.ReadLine()) != null)
                {
                    /* 読み込んだ１行分のテキスト A の処理 */
                    String label = A.Substring(0, A.IndexOf(":"));
                    switch(label){
                        case "mailID":
                            mailID = A.Substring(A.IndexOf(":")+1, A.Length - (A.IndexOf(":") + 1));
                            lineNo += 1;
                            break;
                        case "mailPassword":
                            mailPassword = A.Substring(A.IndexOf(":") + 1, A.Length - (A.IndexOf(":") + 1));
                            lineNo += 2;
                            break;
                        case "mailFrom":
                            mailFrom = A.Substring(A.IndexOf(":") + 1, A.Length - (A.IndexOf(":") + 1));
                            lineNo += 4;
                            break;
                        case "mailTo":
                            mailTo = A.Substring(A.IndexOf(":") + 1, A.Length - (A.IndexOf(":") + 1));
                            lineNo += 8;
                            break;
                        default:
                            break;
                    }
                }
                reader.Close();
            }
            else
            {  /* ファイルがなければ標準入力 */
                Console.WriteLine("Can't find file ! enter url");

            }
        }

        private void SendMail()
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;

                smtp.Credentials = new System.Net.NetworkCredential(mailID, mailPassword);
                smtp.EnableSsl = true;
                MailMessage msg = new MailMessage(mailFrom, mailTo, mailSubject, mailBody);
                smtp.Send(msg);

                Console.WriteLine($"{mailTo} にメール送信しました。");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
 
    }



    #endregion
}
